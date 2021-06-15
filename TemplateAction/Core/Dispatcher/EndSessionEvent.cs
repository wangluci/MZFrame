using System;

namespace TemplateAction.Core
{
    public class EndSessionEvent
    {
        private ITASession _session;
        public EndSessionEvent(ITASession session)
        {
            _session = session;
        }
        public ITASession Session
        {
            get { return _session; }
        }
    }
}
