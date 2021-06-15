using System;
using System.IO;

namespace MyNet.Buffer
{
    public class PoolIOStream : AbstractByteStream
    {
        protected bool _released;
        protected ArraySegment<byte> _chunk;
        protected PoolBufferAllocator _allocator;
        public override int Capacity
        {
            get { return _chunk.Count; }
        }

        public override ArraySegment<byte> BufferChunk
        {
            get
            {
                return _chunk;
            }
        }

        public PoolIOStream(PoolBufferAllocator allocator, ArraySegment<byte> chunk) :
            this(allocator, chunk, 0)
        {
        }
        public PoolIOStream(PoolBufferAllocator allocator, ArraySegment<byte> chunk, int initlen)
        {
            _readerindex = 0;
            _writerindex = initlen;
            _allocator = allocator;
            _released = false;
            _chunk = chunk;
        }

        public override IByteStream AdjustCapacity(int newCapacity)
        {
            ArraySegment<byte> chunk = _allocator.Alloc(newCapacity);
            if (_writerindex > 0)
            {
                System.Buffer.BlockCopy(_chunk.Array, _chunk.Offset, chunk.Array, chunk.Offset, _writerindex);
            }
            _allocator.Release(_chunk);
            _chunk = chunk;
            return this;
        }
        public override IByteStream ToBase64()
        {
            string base64str = Convert.ToBase64String(this.ToArray());
            byte[] base64arr = Convert.FromBase64String(base64str);
            ArraySegment<byte> chunk = _allocator.Alloc(base64arr.Length);
            System.Buffer.BlockCopy(base64arr, 0, chunk.Array, chunk.Offset, base64arr.Length);
            return new PoolIOStream(_allocator, chunk, base64arr.Length);
        }
        public override IByteStream Slice(int start, int len)
        {
            ArraySegment<byte> chunk = _allocator.Alloc(len);
            if ((start + len) > Length)
            {
                return null;
            }
            System.Buffer.BlockCopy(_chunk.Array, _chunk.Offset + start, chunk.Array, chunk.Offset, len);
            return new PoolIOStream(_allocator, chunk, len);
        }
        public override byte GetByte(int index)
        {
            return _chunk.Array[_chunk.Offset + index];
        }
        public override byte[] GetBytes(int start, int len)
        {
            byte[] newBytes = new byte[len];
            System.Buffer.BlockCopy(_chunk.Array, _chunk.Offset + start, newBytes, 0, len);
            return newBytes;
        }
        public override void SetBytes(int start, byte[] bs, int offset, int len)
        {
            System.Buffer.BlockCopy(bs, offset, _chunk.Array, _chunk.Offset + start, len);
        }
        public override void SetByte(int start, byte b)
        {
            _chunk.Array[_chunk.Offset + start] = b;
        }

        public override int Read2Array(Array dst, int dstOffset, int len)
        {
            int readableBytes = _writerindex - _readerindex;
            int readlen = Math.Min(readableBytes, len);
            System.Buffer.BlockCopy(_chunk.Array, (_chunk.Offset + _readerindex), dst, dstOffset, readlen);
            _readerindex += readlen;
            return readlen;
        }
        public override void Clear()
        {
            Array.Clear(_chunk.Array, _chunk.Offset, _chunk.Count);
        }

        protected override void OnUnManDisposed()
        {
            if (!_released)
            {
                _readerindex = 0;
                _writerindex = 0;
                _allocator.Release(_chunk);
                _released = true;
            }
        }

    }
}
