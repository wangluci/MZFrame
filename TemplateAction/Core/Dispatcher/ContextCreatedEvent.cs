using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Core.Dispatcher
{
    public class ContextCreatedEvent
    {
        private ITAContext _context;
        public ContextCreatedEvent(ITAContext context)
        {
            _context = context;
        }
        public ITAContext Context
        {
            get { return _context; }
        }
    }
}
