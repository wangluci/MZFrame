using System;
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    internal class DecodingContext : Context
    {
        /// <summary>
        /// Decoded value.
        /// </summary>
        internal object Value { get; set; }

        /// <summary>
        /// Available strong type, otherwise null
        /// </summary>
        internal Type KnownType { get; private set; }

        /// <summary>
        /// Json token sequence to read from.
        /// </summary>
        internal JsonTokenSequence Input { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="json"></param>
        /// <param name="value"></param>
        /// <param name="knownType"></param>
        internal DecodingContext(Process process, JsonTokenSequence input, Type knownType) : base(process)
        {
            this.Input = input;
            this.KnownType = knownType;
        }
    }
}
