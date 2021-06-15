using System;
namespace TemplateAction.Core
{
    public class StartSessionEvent
    {
        private ITASession _session;
        public StartSessionEvent(ITASession session)
        {
            _session = session;
        }
        public ITASession Session
        {
            get { return _session; }
        }
    }
}
