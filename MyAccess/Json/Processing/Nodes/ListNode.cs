
using System;
using System.Collections;
using System.Collections.Generic;

using MyAccess.Json.Helpers;

namespace MyAccess.Json.Processing.Nodes
{
    class ListNode : IEncodingNode, IDecodingNode
    {
        private Type _listType;
        private Type _genericArgument;

        internal ListNode()
        {

        }

        internal ListNode(Type listType, Type genericArgument)
        {
            _listType = listType;
            _genericArgument = genericArgument;
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {
            IList list = (IList)context.Value;
            bool parallel = false;

            // See if this list would benefit from parallel processing
            if (context.Process.IsParallel && _genericArgument != null && !context.Process.RequiresReferencing && TypeHelper.IsParallelBeneficial(_genericArgument))
            {
                foreach (object el in list)
                {
                    // Do parallel
                    yield return new DoParallelEncode(el, _genericArgument);
                }

                parallel = true;
            }

            context.Output += JsonToken.BeginArray;

            for (int index = 0; index < list.Count; index++)
            {
                object element = list[index];

                if (index > 0)
                {
                    context.Output += JsonToken.ValueSeperator;
                }
                else if (parallel)
                {
                    // Wait untill all parallel tasks are finished.
                    yield return new SyncParallelEncode();
                }

                DoEncode instruction = new DoEncode(element, _genericArgument);
                yield return instruction;

                context.Output += instruction.Output;

            }

            context.Output += JsonToken.EndArray;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            context.Input -= JsonToken.BeginArray;

            IList list;

            if (_listType == null || _listType.IsArray || _listType.IsGenericType)
            {
                if (_genericArgument == null)
                {
                    list = new List<object>();
                }
                else
                {
                    list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(_genericArgument));
                }
            }
            else
            {
                // Create the desired instance directly
                list = (IList)Activator.CreateInstance(_listType);
            }

            for (int index = 0; context.Input.Peek() != JsonToken.EndArray; index++)
            {
                if (index > 0)
                {
                    context.Input -= JsonToken.ValueSeperator;
                }

                DoDecode instruction = new DoDecode(context.Input, _genericArgument);
                yield return instruction;

                list.Add(instruction.Value);
            }

            context.Input -= JsonToken.EndArray;

            if (_listType == null || !_listType.IsArray || _listType.IsGenericType)
            {
                context.Value = list;
            }
            else
            {
                Array array = Array.CreateInstance(_listType.GetElementType(), list.Count);
                list.CopyTo(array, 0);

                context.Value = array;
            }
        }
    }
}
