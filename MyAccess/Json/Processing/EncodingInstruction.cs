
using System;
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// Abstract class for an encoding instruction. Implementations can direct the process.
    /// </summary>
    internal abstract class EncodingInstruction { }

    /// <summary>
    /// Tells the process to encode the given value.
    /// </summary>
    internal class DoEncode : EncodingInstruction
    {
        internal object Value { get; private set; }
        internal Type KnownType { get; private set; }

        internal JsonTokenSequence Output { get; set; }

        internal DoEncode(object value, Type knownType)
        {
            this.Value = value;
            this.KnownType = knownType;
        }
    }

    /// <summary>
    /// Tells the process to do a async encode for the given value.
    /// </summary>
    internal class DoParallelEncode : DoEncode
    {
        internal DoParallelEncode(object value, Type knownType)
            : base(value, knownType)
        {

        }
    }


    /// <summary>
    /// Tells the process to synchronize all currently async encoding taksks.
    /// </summary>
    internal class SyncParallelEncode : EncodingInstruction { }

    /// <summary>
    /// Tells the process that the current encoding context is invalid.
    /// </summary>
    internal class ContextInvalidated : EncodingInstruction { }
}
