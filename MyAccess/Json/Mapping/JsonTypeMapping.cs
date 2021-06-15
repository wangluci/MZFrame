using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyAccess.Json.Mapping
{
    abstract public class JsonTypeMappingBase : ICloneable
    {
        internal bool UsesReferencing { get; set; }
        internal Dictionary<string, JsonFieldMappingBase> FieldMappings { get; private set; }

        internal JsonTypeMappingBase()
        {
            this.FieldMappings = new Dictionary<string, JsonFieldMappingBase>();
        }

        abstract public object Clone();
        abstract internal void AutoGenerate();
    }

    public class JsonTypeMapping<T> : JsonTypeMappingBase
    {
        private List<MemberInfo> _exludes;

        public JsonTypeMapping()
        {
            _exludes = new List<MemberInfo>();
        }

        #region ICloneable Members
        public override object Clone()
        {
            JsonTypeMapping<T> clone = new JsonTypeMapping<T>();
            clone.UsesReferencing = this.UsesReferencing;

            Dictionary<string, JsonFieldMappingBase>.Enumerator enumerator = this.FieldMappings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                clone.FieldMappings.Add(enumerator.Current.Key, (JsonFieldMappingBase)enumerator.Current.Value.Clone());
            }

            foreach(MemberInfo exclude in _exludes)
            {
                clone._exludes.Add(exclude);
            }

            return clone;
        }
        #endregion

        /// <summary>
        /// Enables or disables support for referencing multiple occurences to an instance of this type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JsonTypeMapping<T> UseReferencing(bool value)
        {
            this.UsesReferencing = value;
            return this;
        }

        /// <summary>
        /// Maps all fields automatically, except any excluded fields.
        /// </summary>
        /// <returns></returns>
        public JsonTypeMapping<T> AllFields()
        {
            List<MemberInfo> members = new List<MemberInfo>();
            members.AddRange(typeof(T).GetFields());
            members.AddRange(typeof(T).GetProperties());

            foreach (MemberInfo memberInfo in members)
            {
                _addFieldMapping(new JsonFieldMapping<object>(memberInfo));
            }

            return this;
        }



        internal override void AutoGenerate()
        {
            this.AllFields();
        }

        private void _addFieldMapping(JsonFieldMappingBase fieldMapping)
        {
            if (!_exludes.Contains(fieldMapping.ReflectedField))
            {
                if (this.FieldMappings.ContainsKey(fieldMapping.JsonField) && this.FieldMappings[fieldMapping.JsonField].ReflectedField.Name != fieldMapping.ReflectedField.Name)
                {
                    throw new Exception();
                }

                Dictionary<string, JsonFieldMappingBase>.Enumerator enumerator = this.FieldMappings.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.Value.ReflectedField.Name == fieldMapping.ReflectedField.Name)
                    {
                        this.FieldMappings.Remove(enumerator.Current.Key);
                        break;
                    }
                }

                this.FieldMappings.Add(fieldMapping.JsonField, fieldMapping);
            }
        }
    }
}