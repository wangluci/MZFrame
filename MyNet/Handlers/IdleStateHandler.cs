using MyNet.Channel;
using MyNet.Common;
using MyNet.Loop;
using MyNet.Loop.Scheduler;
using System;

namespace MyNet.Handlers
{
    /// <summary>
    /// 空闲事件触发器，心跳处理用
    /// </summary>
    public class IdleStateHandler : AbstractChannelHandler
    {
        public const string CHANNEL_READ_IDLE = "my:readidle";
        public const string CHANNEL_WRITE_IDLE = "my:writeidle";
        public const string CHANNEL_ALL_IDLE = "my:allidle";
        protected ITriggerRunnable _readIdle;
        protected ITriggerRunnable _writeIdle;
        protected ITriggerRunnable _allIdle;
        protected int _readerIdleTimeMilliSeconds;
        protected int _writerIdleTimeMilliSeconds;
        protected int _allIdleTimeMilliSeconds;
        protected long _readNextTime;
        protected long _writeNextTime;
        protected long _allNextTime;
        public IdleStateHandler(int readerIdleTimeSeconds, int writerIdleTimeSeconds, int allIdleTimeSeconds)
        {
            ResetReaderIdleTime(readerIdleTimeSeconds);
            ResetWriterIdleTime(writerIdleTimeSeconds);
            ResetAllIdleTime(allIdleTimeSeconds);
        }
        public void ResetReaderIdleTime(int readerIdleTimeSeconds)
        {
            _readerIdleTimeMilliSeconds = readerIdleTimeSeconds * 1000;
            _readNextTime = Converter.Cast<long>(DateTime.Now) + _readerIdleTimeMilliSeconds;
        }
        public void ResetWriterIdleTime(int writerIdleTimeSeconds)
        {
            _writerIdleTimeMilliSeconds = writerIdleTimeSeconds * 1000;
            _writeNextTime = Converter.Cast<long>(DateTime.Now) + _writerIdleTimeMilliSeconds;
        }
        public void ResetAllIdleTime(int allIdleTimeSeconds)
        {
            _allIdleTimeMilliSeconds = allIdleTimeSeconds * 1000;
            _allNextTime = Converter.Cast<long>(DateTime.Now) + _allIdleTimeMilliSeconds;
        }
        protected void ReadIdle(IContext context)
        {
            if (!context.Channel.Active) return;
            long curtime = Converter.Cast<long>(DateTime.Now);
            if (_readNextTime <= curtime)
            {
                context.Channel.Emit(CHANNEL_READ_IDLE, EventArgs.Empty);
                _readNextTime = curtime + _readerIdleTimeMilliSeconds;
            }
            int deadTime = (int)(_readNextTime - curtime);
            context.Loop.Schedule(c =>
            {
                ReadIdle(c);
            }, context, deadTime);
        }
        protected void WriterIdle(IContext context)
        {
            if (!context.Channel.Active) return;
            long curtime = Converter.Cast<long>(DateTime.Now);
            if (_writeNextTime <= curtime)
            {
                context.Channel.Emit(CHANNEL_WRITE_IDLE, EventArgs.Empty);
                _writeNextTime = curtime + _writerIdleTimeMilliSeconds;
            }
            int deadTime = (int)(_writeNextTime - curtime);
            context.Loop.Schedule(c =>
            {
                WriterIdle(c);
            }, context, deadTime);
        }
        protected void AllIdle(IContext context)
        {
            if (!context.Channel.Active) return;
            long curtime = Converter.Cast<long>(DateTime.Now);
            if (_allNextTime <= curtime)
            {
                context.Channel.Emit(CHANNEL_ALL_IDLE, EventArgs.Empty);
                _allNextTime = curtime + _allIdleTimeMilliSeconds;
            }
            int deadTime = (int)(_allNextTime - curtime);
            context.Loop.Schedule(c =>
            {
                AllIdle(c);
            }, context, deadTime);
        }
        public void RestartIdleTimeOut(IContext context)
        {
            ChannelBase channel = context.Channel;
            if (_readerIdleTimeMilliSeconds > 0)
            {
                if (_readIdle == null)
                {
                    _readIdle = channel.Loop.Schedule(c =>
                    {
                        ReadIdle(c);
                    }, context, _readerIdleTimeMilliSeconds);
                }

            }

            if (_writerIdleTimeMilliSeconds > 0)
            {
                if (_writeIdle == null)
                {
                    _writeIdle = channel.Loop.Schedule(c =>
                    {
                        WriterIdle(c);
                    }, context, _writerIdleTimeMilliSeconds);
                }

            }

            if (_allIdleTimeMilliSeconds > 0)
            {
                if (_allIdle == null)
                {
                    _allIdle = channel.Loop.Schedule(c =>
                    {
                        AllIdle(c);
                    }, context, _allIdleTimeMilliSeconds);
                }
            }
        }
        public override void ChannelActive(IContext context)
        {
            RestartIdleTimeOut(context);
            ChildActive(context);
        }

        public override void ChannelRead(IContext context, object msg)
        {
            if (_readerIdleTimeMilliSeconds > 0)
            {
                _readNextTime = Converter.Cast<long>(DateTime.Now) + _readerIdleTimeMilliSeconds;
            }
            if (_allIdleTimeMilliSeconds > 0)
            {
                _allNextTime = Converter.Cast<long>(DateTime.Now) + _allIdleTimeMilliSeconds;
            }

            ChildRead(context, msg);
        }
        public override void ChannelWriteFinish(IContext context, object msg)
        {
            if (_writerIdleTimeMilliSeconds > 0)
            {
                _writeNextTime = Converter.Cast<long>(DateTime.Now) + _writerIdleTimeMilliSeconds;
            }

            if (_allIdleTimeMilliSeconds > 0)
            {
                _allNextTime = Converter.Cast<long>(DateTime.Now) + _allIdleTimeMilliSeconds;
            }
            ChildWriteFinish(context, msg);
        }

        public virtual void ChildActive(IContext context)
        {
            context.FireNextActive();
        }
        public virtual void ChildRead(IContext context, object msg)
        {
            context.FireNextRead(msg);
        }
        public virtual void ChildWriteFinish(IContext context, object msg)
        {
            context.FireNextWriteFinish(msg);
        }
        public override void HandlerInstalled(IContext context) { }

        public override void HandlerUninstalled(IContext context) { }
    }
}
