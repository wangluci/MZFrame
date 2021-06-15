using MyNet.Buffer;
using System;
using System.Collections.Generic;

namespace MyNet.SocketIO
{
    public class SocketIOPacket
    {
        protected PacketType _type;
        protected SubPacketType _subtype = SubPacketType.UNKNOWN;
        protected object _data;
        protected string _name;
        protected string _nsp;
        protected long _ackId;
        protected int _attachmentsCount;
        protected List<IByteStream> _attachments;

        public SocketIOPacket(PacketType t)
        {
            _type = t;
            _ackId = -1;
            _nsp = SocketIO.DEFAULT_NAME;
        }
        public void SetName(string name)
        {
            _name = name;
        }
        public void SetAckId(long id)
        {
            _ackId = id;
        }
        public void SetSubType(SubPacketType t)
        {
            _subtype = t;
        }
        public void SetData(object data)
        {
            _data = data;
        }
        public void SetNsp(string nsp)
        {
            _nsp = nsp;
        }
        public bool IsAttachmentsLoaded()
        {
            return this._attachments.Count == _attachmentsCount;
        }
        public bool HasAttachments()
        {
            return _attachmentsCount != 0;
        }
        public void InitAttachments(int attachmentsCount)
        {
            this._attachmentsCount = attachmentsCount;
            this._attachments = new List<IByteStream>(attachmentsCount);
        }
        public void AddAttachment(IByteStream attachment)
        {
            if (_attachments.Count < _attachmentsCount)
            {
                this._attachments.Add(attachment);
            }
        }
        public PacketType GetResponseType()
        {
            return _type;
        }
        public SubPacketType GetSubType()
        {
            return _subtype;
        }
        public object GetData()
        {
            return _data;
        }
        public string GetName()
        {
            return _name;
        }
        public string GetNsp()
        {
            return _nsp;
        }
        public long GetAckId()
        {
            return _ackId;
        }
    }
    public enum PacketType : byte
    {
        OPEN = 0,
        CLOSE = 1,
        PING = 2,
        PONG = 3,
        MESSAGE = 4,
        UPGRADE = 5,
        NOOP = 6,
    }
    public enum SubPacketType : byte
    {
        CONNECT = 0,
        DISCONNECT = 1,
        EVENT = 2,
        ACK = 3,
        ERROR = 4,
        BINARY_EVENT = 5,
        UNKNOWN = 11
    }
}
