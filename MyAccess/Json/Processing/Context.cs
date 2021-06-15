using System.Collections.Generic;
using MyAccess.Json.Mapping;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// 
    /// </summary>
    internal class Context
    {
        /// <summary>
        /// The process providing this context.
        /// </summary>
        internal Process Process { get; private set; }

        internal Context(Process process)
        {
            this.Process = process;
        }
    }
}
