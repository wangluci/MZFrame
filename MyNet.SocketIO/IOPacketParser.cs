using MyAccess.Json;
using MyNet.Buffer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyNet.SocketIO
{
    internal class IOPacketParser
    {
        static int[] sInputCodesUtf8 = initInputCodes();
        static int[] initInputCodes()
        {
            int[] sInputCodes;
            int[] table = new int[256];
            for (int i = 0; i < 32; ++i)
            {
                table[i] = -1;
            }
            table['"'] = 1;
            table['\\'] = 1;
            sInputCodes = table;

            int[] xtable = new int[sInputCodes.Length];
            System.Buffer.BlockCopy(sInputCodes, 0, xtable, 0, sInputCodes.Length);
            for (int c = 128; c < 256; ++c)
            {
                int code;
                if ((c & 0xE0) == 0xC0)
                {
                    code = 2;
                }
                else if ((c & 0xF0) == 0xE0)
                {
                    code = 3;
                }
                else if ((c & 0xF8) == 0xF0)
                {
                    code = 4;
                }
                else
                {
                    code = -1;
                }
                xtable[c] = code;
            }
            return xtable;
        }
        private static long ReadLong(IByteStream stream, int length)
        {
            long result = 0;
            for (int i = stream.ReaderIndex; i < stream.ReaderIndex + length; i++)
            {
                int digit = stream.GetByte(i) & 0xF;
                for (int j = 0; j < stream.ReaderIndex + length - 1 - i; j++)
                {
                    digit *= 10;
                }
                result += digit;
            }
            stream.SetReaderIndex(stream.ReaderIndex + length);
            return result;
        }
        private static string ReadString(IByteStream stream, int size)
        {
            if (size <= 0) return string.Empty;
            byte[] bytes = stream.ReadBytes(size);
            return Encoding.UTF8.GetString(bytes);
        }
        private static SocketIOPacket ParseBinary(IOClient client, IByteStream stream, int len)
        {
            if (stream.GetByte(stream.ReaderIndex) == 1)
            {
                stream.ReadSkip(1);
                int headEndIndex = stream.BytesBefore(stream.ReaderIndex, 0xf);
                len = (int)ReadLong(stream, headEndIndex);
                IByteStream oldFrame = stream;
                stream = stream.Slice(oldFrame.ReaderIndex + 1, len);
                if (stream == null)
                {
                    return null;
                }
                oldFrame.SetReaderIndex(oldFrame.ReaderIndex + 1 + len);
            }

            if (stream.GetByte(0) == 'b' && stream.GetByte(1) == '4')
            {
                stream.ReadSkip(2);
            }
            else if (stream.GetByte(0) == 4)
            {
                stream.ReadSkip(1);
            }

            SocketIOPacket binaryPacket = client.GetLastBinaryPacket();
            if (binaryPacket != null)
            {
                IByteStream attachBuf;
                if (stream.GetByte(0) == 'b' && stream.GetByte(1) == '4')
                {
                    attachBuf = stream;
                }
                else
                {
                    attachBuf = stream.ToBase64();
                }
                binaryPacket.AddAttachment(attachBuf);
                stream.SetReaderIndex(stream.Length);

                if (binaryPacket.IsAttachmentsLoaded())
                {
                    LinkedList<IByteStream> slices = new LinkedList<IByteStream>();

                    return binaryPacket;
                }
            }
            return new SocketIOPacket(PacketType.MESSAGE);
        }
        private static void ParseHeader(IByteStream frame, SocketIOPacket packet, SubPacketType innerType)
        {
            int endIndex = frame.BytesBefore(frame.WriterIndex, (byte)'[');
            if (endIndex <= 0)
            {
                return;
            }

            int attachmentsDividerIndex = frame.BytesBefore(endIndex, (byte)'-');
            bool hasAttachments = attachmentsDividerIndex != -1;
            if (hasAttachments && innerType == SubPacketType.BINARY_EVENT)
            {
                int attachments = (int)ReadLong(frame, attachmentsDividerIndex);
                packet.InitAttachments(attachments);
                frame.SetReaderIndex(frame.ReaderIndex + 1);
                endIndex -= attachmentsDividerIndex + 1;
            }
            if (endIndex == 0)
            {
                return;
            }

            bool hasNsp = frame.BytesBefore(endIndex, (byte)',') != -1;
            if (hasNsp)
            {
                string nspAckId = ReadString(frame, endIndex);
                string[] parts = nspAckId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                string nsp = parts[0];
                packet.SetNsp(nsp);
                if (parts.Length > 1)
                {
                    string ackId = parts[1];
                    packet.SetAckId(long.Parse(ackId));
                }
            }
            else
            {
                long ackId = ReadLong(frame, endIndex);
                packet.SetAckId(ackId);
            }
        }
        private static void ParseBody(IOClient client, IByteStream frame, SocketIOPacket packet)
        {
            if (packet.GetResponseType() == PacketType.MESSAGE)
            {
                if (packet.GetSubType() == SubPacketType.CONNECT
                        || packet.GetSubType() == SubPacketType.DISCONNECT)
                {
                    packet.SetNsp(ReadString(frame, frame.WriterIndex - frame.ReaderIndex));
                }

                if (packet.GetSubType() == SubPacketType.ACK)
                {
                    string json = ReadString(frame, frame.WriterIndex - frame.ReaderIndex);
                    List<object> tlist = Json.Decode(json) as List<object>;
                    if (tlist != null && tlist.Count > 0)
                    {
                        packet.SetData(tlist[0]);
                    }
                }

                if (packet.GetSubType() == SubPacketType.EVENT
                        || packet.GetSubType() == SubPacketType.BINARY_EVENT)
                {
                    if (packet.HasAttachments() && !packet.IsAttachmentsLoaded())
                    {
                        frame.SetReaderIndex(frame.WriterIndex);
                        client.SetLastBinaryPacket(packet);
                    }
                    else
                    {
                        string json = ReadString(frame, frame.WriterIndex - frame.ReaderIndex);
                        List<object> tlist = Json.Decode(json) as List<object>;
                        if (tlist != null && tlist.Count >= 2)
                        {
                            string tname = tlist[0] as string;
                            if (tname != null)
                            {
                                packet.SetName(tname);
                                packet.SetData(tlist[1]);
                            }
                        }
                    }
                }
            }
        }
        private static SocketIOPacket PareseFrame(IOClient client, IByteStream stream)
        {
            if (stream == null) return null;
            if (stream.ReaderIndex == stream.WriterIndex)
            {
                return null;
            }

            if ((stream.GetByte(0) == 'b' && stream.GetByte(1) == '4')
                || stream.GetByte(0) == 4 || stream.GetByte(0) == 1)
            {
                return ParseBinary(client, stream, stream.Length);
            }
            string typeint = Encoding.UTF8.GetString(stream.ReadBytes(1));
            PacketType type = (PacketType)int.Parse(typeint);
            SocketIOPacket packet = new SocketIOPacket(type);

            if (type == PacketType.PING)
            {
                packet.SetData(ReadString(stream, stream.WriterIndex - stream.ReaderIndex));
                return packet;
            }

            if (!stream.IsReadable())
            {
                return packet;
            }
            string subtypeint = Encoding.UTF8.GetString(stream.ReadBytes(1));
            SubPacketType innerType = (SubPacketType)Common.Converter.Cast<int>(subtypeint);
            packet.SetSubType(innerType);

            ParseHeader(stream, packet, innerType);
            ParseBody(client, stream, packet);
            return packet;
        }
        private static bool HasLengthHeader(IByteStream buffer)
        {
            for (int i = 0; i < Math.Min(buffer.WriterIndex - buffer.ReaderIndex, 10); i++)
            {
                byte b = buffer.GetByte(buffer.ReaderIndex + i);
                if (b == (byte)':' && i > 0)
                {
                    return true;
                }
                if (b > 57 || b < 48)
                {
                    return false;
                }
            }
            return false;
        }
        private static int getCharTailIndex(IByteStream inputBuffer, int i)
        {
            int c = inputBuffer.GetByte(i) & 0xFF;
            switch (sInputCodesUtf8[c])
            {
                case 2: // 2-byte UTF
                    i += 2;
                    break;
                case 3: // 3-byte UTF
                    i += 3;
                    break;
                case 4: // 4-byte UTF
                    i += 4;
                    break;
                default:
                    i++;
                    break;
            }
            return i;
        }
        private static int GetActualLength(IByteStream inputBuffer, int length)
        {
            int len = 0;
            int start = inputBuffer.ReaderIndex;
            for (int i = inputBuffer.ReaderIndex; i < inputBuffer.Length;)
            {
                i = getCharTailIndex(inputBuffer, i);
                len++;
                if (length == len)
                {
                    return i - start;
                }
            }
            return 0;
        }
        /// <summary>
        /// 解码接收包
        /// </summary>
        /// <param name="client"></param>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static SocketIOPacket Parse(IOClient client, IByteStream stream)
        {
            if (HasLengthHeader(stream))
            {
                int lengthEndIndex = stream.BytesBefore((byte)':');
                int lenHeader = (int)ReadLong(stream, lengthEndIndex);
                int len = GetActualLength(stream, lenHeader);
                IByteStream frame = stream.Slice(stream.ReaderIndex + 1, len);
                stream.SetReaderIndex(stream.ReaderIndex + 1 + len);
                return PareseFrame(client, frame);
            }
            return PareseFrame(client, stream);
        }
    }
}
