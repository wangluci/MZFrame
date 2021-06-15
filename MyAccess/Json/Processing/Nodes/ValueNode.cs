
using MyAccess.Json.Attributes;
using System.Collections;
using System.Collections.Generic;

namespace MyAccess.Json.Processing.Nodes
{
    class ValudeNode : IEncodingNode, IDecodingNode
    {
        internal ValudeNode()
        {
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {
            JsonToken jt = JsonToken.Parse(context.Value, context);
            //判断是否是未知类对象
            if (jt.Type == JsonTokenType.Undefined)
            {
                System.Reflection.PropertyInfo[] properties = context.Value.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                context.Output += JsonToken.BeginObject;
                object key;
                object value;
                bool isparsed = false;
                for (int index = 0; index < properties.Length; index++)
                {
                    System.Reflection.PropertyInfo item = properties[index];
                    key = item.Name;
                    value = item.GetValue(context.Value, null);
                    object[] defattrs = item.GetCustomAttributes(false);
                    if (defattrs.Length > 0)
                    {
                        bool encode = true;
                        foreach (object obj in defattrs)
                        {
                            if (obj is JsonDefault)
                            {
                                if (value == null)
                                {
                                    value = obj as JsonDefault;
                                }
                            }
                            else
                            {
                                JsonIgnore ign = obj as JsonIgnore;
                                if (ign != null)
                                {
                                    if (ign.Flag == 0)
                                    {
                                        encode = false;
                                    }
                                    else if ((ign.Flag & HideFlag.NullHide) != 0 && value == null)
                                    {
                                        encode = false;
                                    }
                                }
                       
                            }
                        }
                        if (!encode)
                        {
                            continue;
                        }
                    }
                    if (!context.UseNullable && value == null)
                    {
                        continue;
                    }
                    if (isparsed)
                    {
                        context.Output += JsonToken.ValueSeperator;
                    }
                    else
                    {
                        isparsed = true;
                    }


                    context.Output += JsonToken.Parse(key, JsonTokenType.String, context);
                    context.Output += JsonToken.NameSeperator;

                    DoEncode instruction = new DoEncode(value, null);
                    yield return instruction;

                    context.Output += instruction.Output;
                }

                context.Output += JsonToken.EndObject;
            }
            else
            {
                context.Output += jt;
            }

            yield break;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            context.Value = context.Input.Pop().Value(context.KnownType == null ? typeof(object) : context.KnownType);
            yield break;
        }
    }
}
