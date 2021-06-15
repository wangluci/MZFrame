using System;
using System.Collections.Generic;

namespace MyAccess.Json.Processing
{
    /// <summary>
    /// Abstract class for an decoding instruction. Implementations can direct the process.
    /// </summary>
    internal abstract class DecodingInstruction { }

    /// <summary>
    /// Tells the process to decode the given sequence.
    /// </summary>
    internal class DoDecode : DecodingInstruction
    {
        internal object Value { get; set; }
        internal Type KnownType { get; private set; }

        internal JsonTokenSequence Input { get; set; }

        internal DoDecode(JsonTokenSequence input, Type knownType)
        {
            this.Input = input;
            this.KnownType = knownType;
        }
    }

    /// <summary>
    /// Tells the process to use the given Node as the next in chain.
    /// </summary>
    internal class ProvideNextNode : DecodingInstruction
    {
        internal IDecodingNode NextNode { get; private set; }

        internal ProvideNextNode(IDecodingNode nextNode)
        {
            this.NextNode = nextNode;
        }
    }
}
