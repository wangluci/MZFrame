using System;
using System.Collections;
using System.Collections.Generic;
using MyAccess.Json.Configuration;
using MyAccess.Json.Helpers;
using MyAccess.Json.Processing.Nodes;
using MyAccess.Json.Mapping;
using System.Data;
namespace MyAccess.Json.Processing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class DecodingProcess : Process
    {
        internal JsonDecodingConfiguration Configuration { get; private set; }

        internal DecodingProcess(JsonDecodingConfiguration configuration) : base(configuration.Mappings.Values, configuration.UsesParallelProcessing)
        {
            this.Configuration = configuration;
        }

        internal object Decode(string json,Type ktype)
        {
            DecodingContext context = new DecodingContext(this, new JsonTokenSequence(json), ktype);
            _decodeContext(context);

            return context.Value;
        }
        private DecodingContext _decodeContext(DecodingContext context)
        {
            Queue<DecodingContext> locals = new Queue<DecodingContext>();
            Queue<IDecodingNode> executions = _buildExecutionQueue(context);

            while (executions.Count > 0)
            {
                // Execute next node
                IDecodingNode node = executions.Dequeue();

                foreach (DecodingInstruction instruction in node.ExecuteDecode(context))
                {
                    if (instruction is DoDecode)
                    {
                        DoDecode casted = (DoDecode)instruction;

                        if (locals.Count == 0)
                        {
                            // Recursive processing
                            locals.Enqueue(_decodeContext(new DecodingContext(this, casted.Input, casted.KnownType)));
                        }

                        casted.Value = locals.Dequeue().Value;
                    }
                    else if (instruction is ProvideNextNode)
                    {
                        ProvideNextNode casted = (ProvideNextNode)instruction;
                        executions = new Queue<IDecodingNode>();
                        executions.Enqueue(casted.NextNode);


                    }
                }
            }

            return context;
        }

        private Queue<IDecodingNode> _buildExecutionQueue(DecodingContext context)
        {
            Queue<IDecodingNode> executions = new Queue<IDecodingNode>();
            executions.Enqueue(new ResolveNextNode());

            if (context.KnownType != null && context.KnownType != typeof(object))
            {
                if (context.KnownType == typeof(DateTime))
                {
                    executions.Enqueue(new DateTimeNode());
                }
                else if (context.KnownType == typeof(DataTable))
                {
                    executions.Enqueue(new DataTableNode());
                }
                else if (context.KnownType == typeof(decimal))
                {
                    executions.Enqueue(new DecimalNode());
                }
                // > Mapping
                else if (Configuration.Mappings.ContainsKey(context.KnownType))
                {
                    executions.Enqueue(new MappingNode(Configuration.Mappings[context.KnownType]));
                }
                // > IList<T>
                else if (context.KnownType.Name == "List`1" || context.KnownType.Name == "IList`1")
                {
                    executions.Enqueue(new ListNode(context.KnownType, context.KnownType.GetGenericArguments()[0]));
                }
                // [] Array
                else if (context.KnownType.IsArray)
                {
                    executions.Enqueue(new ListNode(context.KnownType, context.KnownType.GetElementType()));
                }
                // > IList
                else if (TypeHelper.IsThreatableAs(context.KnownType, typeof(IList)))
                {
                    executions.Enqueue(new ListNode(context.KnownType, null));
                }
                // > IDictionary<T>
                else if (TypeHelper.IsThreatableAs(context.KnownType, typeof(IDictionary<,>)))
                {
                    executions.Enqueue(new DictNode(context.KnownType, context.KnownType.GetGenericArguments()[1]));
                }
                // > IDictionary
                else if (TypeHelper.IsThreatableAs(context.KnownType, typeof(IDictionary)))
                {
                    executions.Enqueue(new DictNode(context.KnownType, null));
                }
                // > Value ?
                else
                {
                    executions.Enqueue(new ValudeNode());
                }
            }


            return executions;
        }
    }
}
