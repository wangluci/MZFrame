using System;
using System.Collections.Generic;
using System.Data;

namespace MyAccess.Json.Processing.Nodes
{
    class DataTableNode : IEncodingNode, IDecodingNode
    {
        internal DataTableNode()
        {
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {
            DataTable dt = (DataTable)context.Value;
            context.Output += JsonToken.BeginArray;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i > 0)
                {
                    context.Output += JsonToken.ValueSeperator;
                }
                context.Output += JsonToken.BeginObject;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j > 0)
                    {
                        context.Output += JsonToken.ValueSeperator;
                    }
                    context.Output += JsonToken.Parse(dt.Columns[j].ColumnName, JsonTokenType.String, context);
                    context.Output += JsonToken.NameSeperator;
                    context.Output += JsonToken.Parse(dt.Rows[i][j], JsonTokenType.String, context);
                }
                context.Output += JsonToken.EndObject;
            }
            context.Output += JsonToken.EndArray;
            yield break;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            context.Input -= JsonToken.BeginArray;

            DataTable dt = new DataTable();

            for (int index = 0; context.Input.Peek() != JsonToken.EndArray; index++)
            {
                if (index > 0)
                {
                    context.Input -= JsonToken.ValueSeperator;
                }

                context.Input -= JsonToken.BeginObject;

                if (index > 0)
                {
                    DataRow dr = dt.NewRow();
                    for (int j = 0; context.Input.Peek() != JsonToken.EndObject; j++)
                    {
                        if (j > 0)
                        {
                            context.Input -= JsonToken.ValueSeperator;
                        }
                        string key = context.Input.Pop().Value<string>();
                        context.Input -= JsonToken.NameSeperator;
                        string value = context.Input.Pop().Value<string>();
                        dr[key] = value;
                    }
                }
                else
                {
                    Dictionary<string, string> tvalueList = new Dictionary<string, string>();
                    for (int j = 0; context.Input.Peek() != JsonToken.EndObject; j++)
                    {
                        if (j > 0)
                        {
                            context.Input -= JsonToken.ValueSeperator;
                        }
                        string key = context.Input.Pop().Value<string>();
                        dt.Columns.Add(key);
                        context.Input -= JsonToken.NameSeperator;
                        string value = context.Input.Pop().Value<string>();
                        tvalueList.Add(key, value);
                    }
                    DataRow dr = dt.NewRow();
                    foreach (KeyValuePair<string, string> pair in tvalueList)
                    {
                        dr[pair.Key] = pair.Value;
                    }
                }
                context.Input -= JsonToken.EndObject;
            }

            context.Input -= JsonToken.EndArray;
            context.Value = dt;
            yield break;
        }
    }
}
