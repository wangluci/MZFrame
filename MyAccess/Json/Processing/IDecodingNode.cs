
using System;
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// Forms part of an decoding chain.
    /// </summary>
    internal interface IDecodingNode
    {
        /// <summary>
        /// Implementations can perform decoding routines upon the given context here.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context);
    }
}
