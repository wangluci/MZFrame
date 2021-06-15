using System;
using System.Threading;

namespace SysWeb.TemplateAction
{
    public class ErrAsyncResult : IAsyncResult
    {
        public object AsyncState
        {
            get
            {
               return null;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get
            {
                return true;
            }
        }
    }
}
