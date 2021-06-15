using System;
using System.Threading.Tasks;

namespace MyNet.SocketIO
{
    public class EmitWorker
    {
        private bool _async;
        private Action<IOEventArgs> _action;
        public EmitWorker(bool async, Action<IOEventArgs> action)
        {
            _async = async;
            _action = action;
        }
        public void AppendAction(Action<IOEventArgs> action)
        {
            _action += action;
        }
        public void Run(IOEventArgs e)
        {
            //业务逻辑另开线程
            if (_async)
            {
                Task.Run(() =>
                {
                    _action(e);
                });
            }
            else
            {
                _action(e);
            }
        }
    }
}
