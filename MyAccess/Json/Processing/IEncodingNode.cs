
using System;
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// Forms part of on an encoding chain.
    /// </summary>
    internal interface IEncodingNode
    {
        /// <summary>
        /// Implementations can perform encoding routines upon the given context here.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context);
    }
}
