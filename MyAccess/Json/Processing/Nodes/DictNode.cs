
using System;
using System.Collections;
using System.Collections.Generic;

using MyAccess.Json.Helpers;

namespace MyAccess.Json.Processing.Nodes
{
    class DictNode : IEncodingNode, IDecodingNode
    {
        private Type _dictType;
        private Type _genericArgument;

        internal DictNode()
        {

        }

        internal DictNode(Type dictType, Type genericArgument)
        {
            _dictType = dictType;
            _genericArgument = genericArgument;
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {

            IDictionary dict = (IDictionary)context.Value;



            
            bool parallel = false;

            // See if this list would benefit from parallel processing
            if (context.Process.IsParallel && _genericArgument != null && !context.Process.RequiresReferencing && TypeHelper.IsParallelBeneficial(_genericArgument))
            {
                #if NET40
                foreach (object el in (dict != null ? (ICollection)dict.Values : (ICollection)expando.Values))
                #else
                foreach (object el in dict.Values)
                #endif
                {
                    // Do parallel
                    yield return new DoParallelEncode(el, _genericArgument);
                }

                parallel = true;
            }

            context.Output += JsonToken.BeginObject;

            object key;
            object value;


            IDictionaryEnumerator enumerator = dict.GetEnumerator();

            for (int index = 0; enumerator.MoveNext(); index++)
            {
   
                key = enumerator.Key;
                value = enumerator.Value;

                if (index > 0)
                {
                    context.Output += JsonToken.ValueSeperator;
                }
                else if (parallel)
                {
                    // Wait untill all parallel tasks are finished.
                    yield return new SyncParallelEncode();
                }

                context.Output += JsonToken.Parse(key, JsonTokenType.String, context);
                context.Output += JsonToken.NameSeperator;

                DoEncode instruction = new DoEncode(value, _genericArgument);
                yield return instruction;

                context.Output += instruction.Output;
            }

            context.Output += JsonToken.EndObject;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            context.Input -= JsonToken.BeginObject;

            IDictionary dict;

            #if NET40
            IDictionary<string, object> expando = null;
            #endif

            if (_dictType == null)
            {
                if (_genericArgument != null)
                {
                    dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), _genericArgument));
                }
                else
                {
                    dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<string, object>));
                }
      
            }
            else if(_dictType.IsInterface)
            {
                if (_genericArgument != null)
                    dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(typeof(string), _genericArgument));
                else
                    dict = (IDictionary)Activator.CreateInstance(typeof(Dictionary<string, object>));

            }
            else
            {
                // Create the desired instance directly
                dict = (IDictionary)Activator.CreateInstance(_dictType);
            }

            for (int index = 0; context.Input.Peek() != JsonToken.EndObject; index++)
            {
                if (index > 0)
                {
                    context.Input -= JsonToken.ValueSeperator;
                }

                string key = context.Input.Pop().Value<string>();

                context.Input -= JsonToken.NameSeperator;

                DoDecode instruction = new DoDecode(context.Input, _genericArgument);
                yield return instruction;


                dict.Add(key, instruction.Value);

            }

            context.Input -= JsonToken.EndObject;

 
            context.Value = dict;

        }
    }
}
