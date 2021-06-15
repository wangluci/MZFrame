using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyAccess.Json.Processing.Nodes
{
    class DecimalNode : IEncodingNode, IDecodingNode
    {
        internal DecimalNode()
        {
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {
            context.Output += JsonToken.Parse(context.Value, context);
            yield break;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            context.Value = context.Input.Pop().Value(context.KnownType == null ? typeof(object) : context.KnownType);
            yield break;
        }
    }
}
