
using System;
using System.Collections.Generic;

using MyAccess.Json.Mapping;

namespace MyAccess.Json.Processing.Nodes
{
    class MappingNode : IEncodingNode, IDecodingNode
    {
        JsonTypeMappingBase _mapping;
        internal MappingNode(JsonTypeMappingBase mapping)
        {
            _mapping = mapping;
        }

        public IEnumerable<EncodingInstruction> ExecuteEncode(EncodingContext context)
        {
            if (_mapping.UsesReferencing)
            {
                if (context.Process.References.HasReferenceTo(context.Value))
                {
                    double reference = context.Process.References.GetReferenceTo(context.Value);
                    context.Output += JsonToken.Parse(reference, context);

                    yield break;
                }
                else
                {
                    context.Process.References.Reference(context.Value);
                }
            }

            context.Output += JsonToken.BeginObject;

            Dictionary<string, JsonFieldMappingBase>.Enumerator fields = _mapping.FieldMappings.GetEnumerator();
            for (int index = 0; fields.MoveNext(); index++)
            {
                JsonFieldMappingBase fieldMapping = fields.Current.Value;

                if (index > 0)
                {
                    context.Output += JsonToken.ValueSeperator;
                }

                object value = fieldMapping.Get(context.Value);

                context.Output += JsonToken.Parse(fieldMapping.JsonField, JsonTokenType.String, context);
                context.Output += JsonToken.NameSeperator;

                DoEncode instruction = new DoEncode(fieldMapping.Encode(value), fieldMapping.DesiredType);
                yield return instruction;

                context.Output += instruction.Output;
            }

            context.Output += JsonToken.EndObject;
        }

        public IEnumerable<DecodingInstruction> ExecuteDecode(DecodingContext context)
        {
            if (_mapping.UsesReferencing && context.Input.Peek().Type == JsonTokenType.Number)
            {
                double reference = context.Input.Pop().Value<double>();
                context.Value = context.Process.References.FollowReference(reference);

                yield break;
            }

            context.Input -= JsonToken.BeginObject;

            object reflected = Activator.CreateInstance(context.KnownType);

            if (_mapping.UsesReferencing)
            {
                context.Process.References.Reference(reflected);
            }

            for (int index = 0; context.Input.Peek() != JsonToken.EndObject; index++)
            {
                if (index > 0)
                {
                    context.Input -= JsonToken.ValueSeperator;
                }

                string key = context.Input.Pop().Value<string>();
                JsonFieldMappingBase fieldMapping = _mapping.FieldMappings[key];
                if (fieldMapping == null)
                {
                    throw new MissingFieldException("class miss field " + key);
                }
                context.Input -= JsonToken.NameSeperator;

                DoDecode instruction = new DoDecode(context.Input, fieldMapping.DesiredType);
                yield return instruction;

                object value = fieldMapping.Decode(instruction.Value);
                fieldMapping.Set(reflected, value);
            }

            context.Input -= JsonToken.EndObject;
            context.Value = reflected;
        }
    }
}
