
using System.Collections.Generic;

namespace MyAccess.Json.Processing.Nodes
{
    /// <summary>
    /// 未知类型处理
    /// </summary>
    class ResolveNextNode : IDecodingNode
    {
        internal ResolveNextNode()
        {
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            JsonToken token = context.Input.Peek();

            if (context.KnownType == null || context.KnownType == typeof(object))
            {
                if (token == JsonToken.BeginArray)
                {
                    yield return new ProvideNextNode(new ListNode());
                }
                else if (token == JsonToken.BeginObject)
                {
                    yield return new ProvideNextNode(new DictNode());
                }
                else
                {
                    yield return new ProvideNextNode(new ValudeNode());
                }
            }
            else if (token == JsonToken.Null)
            {
                yield return new ProvideNextNode(new ValudeNode());
            }

            yield break;
        }
    }
}
