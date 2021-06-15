using System;
namespace MyNet.Buffer
{
    public class PoolBufferAllocator : IStreamAllocator
    {
        public readonly static PoolBufferAllocator Default = new PoolBufferAllocator();
        private int _maxbuffsize = BUFF_SIZE;
        private GroupChunk _cacheGroup;
        /// <summary>
        /// 每个chunkgroup的缓存数
        /// </summary>
        public const int COUNT_PERGROUOP = 2000;
        /// <summary>
        /// 默认缓存大小
        /// </summary>
        public const int BUFF_SIZE = 8192;
        /// <summary>
        /// 缓存的最大倍数
        /// </summary>
        public const int MAX_BEI = 8;


        public PoolBufferAllocator()
        {
            _maxbuffsize = BUFF_SIZE * MAX_BEI;
            _cacheGroup = new GroupChunk(COUNT_PERGROUOP, BUFF_SIZE);
        }

        public int MaxFrameBufferSize
        {
            get
            {
                return _maxbuffsize;
            }
            set
            {
                _maxbuffsize = value;
            }
        }
        public void Release(ArraySegment<byte> chunk)
        {
            if (chunk.Array == _cacheGroup.Buffer)
            {
                _cacheGroup.Release(chunk.Offset);
            }
        }
        public ArraySegment<byte> Alloc(int allocsize)
        {
            if (allocsize >= MaxFrameBufferSize)
            {
                return new ArraySegment<byte>(new byte[allocsize], 0, allocsize);
            }
            else
            {
                int needsize = allocsize;
                if (allocsize % BUFF_SIZE != 0)
                {
                    needsize = (allocsize / BUFF_SIZE + 1) * BUFF_SIZE;
                }
                if (needsize <= BUFF_SIZE)
                {
                    ArraySegment<byte> chunk = _cacheGroup.AllocChunk();
                    if (chunk.Count > 0)
                    {
                        return chunk;
                    }
                }
                return new ArraySegment<byte>(new byte[needsize], 0, needsize);
            }
        }

        public IByteStream AllocStream(int allocsize)
        {
            return new PoolIOStream(this, Alloc(allocsize), 0);
        }
        public IByteStream AllocStream()
        {
            return AllocStream(BUFF_SIZE);
        }
    }
}
