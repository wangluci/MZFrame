
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
                bool ignore = false;
                for (int index = 0; index < properties.Length; index++)
                {
                    System.Reflection.PropertyInfo item = properties[index];
                    key = item.Name;
                    value = item.GetValue(context.Value, null);

                    ignore = false;
                    JsonAttr[] jsarr = (JsonAttr[])item.GetCustomAttributes(typeof(JsonAttr), false);
                    foreach(JsonAttr ja in jsarr)
                    {

                        JsonIgnore ign = ja as JsonIgnore;
                        if (ign != null)
                        {
                            ignore = true;
                            break;

                        }

                        JsonHide hide = ja as JsonHide;
                        if (hide != null)
                        {
                            if (hide.Fun(value))
                            {
                                ignore = true;
                                break;
                            }
                        }

                        JsonDefault defobj = ja as JsonDefault;
                        if (defobj != null)
                        {
                            value = defobj.Fun(value);
                        }

                        JsonName jsname = ja as JsonName;
                        if (jsname != null)
                        {
                            key = jsname.Name;
                        }
                    }

                    if (ignore) continue;

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
