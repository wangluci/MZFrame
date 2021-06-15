
using TemplateAction.Core;

namespace SysWeb.TemplateAction
{
    public class MySessionProxy
    {
        private string _sessionid;
        private object _lock = new object();
        private MySession _session;
        public MySessionProxy(string sessionid)
        {
            _sessionid = sessionid;
        }
        public MySession Session()
        {
            if (_session == null)
            {
                lock (_lock)
                {
                    if (_session == null)
                    {
                        _session = new MySession();
                        _session.SessionID = _sessionid;
                        TAEventDispatcher.Instance.Dispatch(new StartSessionEvent(_session));
                    }
                }
            }
            return _session;
        }
    }
}
