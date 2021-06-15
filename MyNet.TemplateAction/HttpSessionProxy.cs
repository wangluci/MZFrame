using TemplateAction.Core;

namespace MyNet.TemplateAction
{
    public class HttpSessionProxy
    {
        private string _sessionid;
        private object _lock = new object();
        private HttpSession _session;
        public HttpSessionProxy(string sessionid)
        {
            _sessionid = sessionid;
        }
        public HttpSession Session()
        {
            if (_session == null)
            {
                lock (_lock)
                {
                    if (_session == null)
                    {
                        _session = new HttpSession();
                        _session.SessionID = _sessionid;
                        TAEventDispatcher.Instance.Dispatch(new StartSessionEvent(_session));
                    }
                }
            }
            return _session;
        }
    }
}
