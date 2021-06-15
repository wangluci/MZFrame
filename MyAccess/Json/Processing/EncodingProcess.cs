using System;
using System.Collections;
using System.Collections.Generic;
using MyAccess.Json.Helpers;
using MyAccess.Json.Configuration;
using MyAccess.Json.Processing.Nodes;
using System.Data;
namespace MyAccess.Json.Processing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class EncodingProcess : Process
    {
        internal JsonEncodingConfiguration Configuration { get; private set; }

        internal EncodingProcess(JsonEncodingConfiguration configuration) : base(configuration.Mappings.Values, configuration.UsesParallelProcessing)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Encodes the given object to a json string.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        internal string Encode<T>(T value)
        {
            EncodingContext root = new EncodingContext(this, value, typeof(T));
            _encodeContext(root);

            if (Configuration.UsesTidy)
                return root.Output.ToTidyString();
            else
                return root.Output.ToString();
        }

        private EncodingContext _encodeContext(EncodingContext context)
        {
            context.UseNullable = Configuration.UsesNullable;
            context.UseUnicode = Configuration.UseUnicode;
            context.DateTimeFormat = Configuration.DateTimeFormat;
            Queue<EncodingContext> locals = new Queue<EncodingContext>();


            Queue<IEncodingNode> executions = _buildExecutionQueue(context);

            while (executions.Count > 0)
            {
                // Execute next node
                IEncodingNode node = executions.Dequeue();

                foreach (EncodingInstruction instruction in node.ExecuteEncode(context))
                {
                    if (instruction.GetType() == typeof(DoEncode))
                    {
                        DoEncode casted = (DoEncode)instruction;

                        if (locals.Count == 0)
                        {
                            // Recursive processing
                            locals.Enqueue(_encodeContext(new EncodingContext(this, casted.Value, casted.KnownType)));
                        }

                        casted.Output = locals.Dequeue().Output;
                    }

                    else if (instruction.GetType() == typeof(ContextInvalidated))
                    {
                        executions = _buildExecutionQueue(context);
                        executions.Dequeue(); // Removes plugin point
                    }
                }
            }

            return context;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private Queue<IEncodingNode> _buildExecutionQueue(EncodingContext context)
        {
            Queue<IEncodingNode> executions = new Queue<IEncodingNode>();

        
            if (context.KnownType != null && context.KnownType != typeof(object) && !TypeHelper.IsBasic(context.KnownType) && context.Value != null)
            {
                if (context.KnownType == typeof(DateTime))
                {
                    executions.Enqueue(new DateTimeNode());
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
                    executions.Enqueue(new ListNode(null, context.KnownType.GetGenericArguments()[0]));
                }
                // > IDictionary<T>
                else if (TypeHelper.IsThreatableAs(context.KnownType, typeof(IDictionary<,>)))
                {
                    executions.Enqueue(new DictNode(null, context.KnownType.GetGenericArguments()[1]));
                }
                // [] Array
                else if (context.KnownType.IsArray)
                {
                    executions.Enqueue(new ListNode(context.KnownType, context.KnownType.GetElementType()));
                }
            }

            if (executions.Count == 0)
            {
                if (context.Value is IList)
                {
                    executions.Enqueue(new ListNode());
                }
                else if (context.Value is DataTable)
                {
                    executions.Enqueue(new DataTableNode());
                }
                else if (context.Value is IDictionary)
                {
                    executions.Enqueue(new DictNode());
                }
                else
                {
                    executions.Enqueue(new ValudeNode());
                }
            }

            return executions;
        }
    }
}
