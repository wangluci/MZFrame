using System;
using System.Collections.Generic;
using System.Threading;

namespace MyNet.Buffer
{
    public class GroupChunk
    {
        public byte[] Buffer
        {
            get
            {
                return _bufferBlock;
            }
        }

        public int BufferSize
        {
            get
            {
                return _totalCapacity;
            }
        }

        public int Offset
        {
            get
            {
                return _curOffset;
            }
        }

        private int _curOffset;
        private byte[] _bufferBlock;
        private int _buffSize;
        private int _buffNum;
        private int _totalCapacity;
        private Queue<int> _freeQuee;
        private SpinLock _spinlock = new SpinLock();
        public GroupChunk(int buffcount, int buffsize)
        {
            _curOffset = 0;
            _buffSize = buffsize;
            _totalCapacity = buffcount * buffsize;
            _bufferBlock = new byte[_totalCapacity];
            _buffNum = buffcount;
            _freeQuee = new Queue<int>(buffcount);
        }
        public ArraySegment<byte> AllocChunk()
        {
            bool lockToken = false;
            try
            {
                _spinlock.Enter(ref lockToken);//加锁
                if (_totalCapacity <= this._curOffset)
                {
                    if (_freeQuee.Count > 0)
                    {
                        return new ArraySegment<byte>(_bufferBlock, _freeQuee.Dequeue(), _buffSize);
                    }
                    else
                    {
                        return new ArraySegment<byte>(_bufferBlock, 0, 0);
                    }
                }
                int offset = this._curOffset;
                this._curOffset += this._buffSize;
                return new ArraySegment<byte>(_bufferBlock, offset, _buffSize);
            }
            finally
            {
                if (lockToken) _spinlock.Exit();//解锁
            }
        }
        public void Release(int offset)
        {
            bool lockToken = false;
            try
            {
                _spinlock.Enter(ref lockToken);//加锁
                _freeQuee.Enqueue(offset);
            }
            finally
            {
                if (lockToken) _spinlock.Exit();//解锁
            }
        }
    }
}
