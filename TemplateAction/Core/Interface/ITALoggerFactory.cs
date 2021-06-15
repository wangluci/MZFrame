using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateAction.Core
{
    /// <summary>
    /// 日志工厂
    /// </summary>
    public interface ITALoggerFactory
    {
        ITALogger CreateLogger(string categoryName);
    }
}
