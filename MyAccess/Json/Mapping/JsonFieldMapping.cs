using MyAccess.Json.Attributes;
using System;
using System.Reflection;

namespace MyAccess.Json.Mapping
{
    abstract public class JsonFieldMappingBase : ICloneable
    {
        internal string JsonField { get; set; }
        internal Type DesiredType { get; set; }

        internal MemberInfo ReflectedField { get; set; }

        abstract internal object Encode(object value);
        abstract internal object Decode(object value);

        abstract internal object Get(object target);
        abstract internal void Set(object target, object value);

        abstract public object Clone();
    }

    public class JsonFieldMapping<T> : JsonFieldMappingBase
    {
        private Delegate _decodeAs;
        private Delegate _encodeAs;

        internal JsonFieldMapping(MemberInfo reflectedField)
        {
            this.ReflectedField = reflectedField;
            this.JsonField = this.ReflectedField.Name;

            if (typeof(T) == typeof(object))
            {
                if (this.ReflectedField is PropertyInfo)
                {
                    this.DesiredType = (this.ReflectedField as PropertyInfo).PropertyType;
                }
                else if (this.ReflectedField is FieldInfo)
                {
                    this.DesiredType = (this.ReflectedField as FieldInfo).FieldType;
                }
            }
            else
            {
                this.DesiredType = typeof(T);
            }
        }

        internal JsonFieldMapping(MemberInfo memberInfo, string jsonField) : this(memberInfo)
        {
            this.JsonField = jsonField;
        }

        #region ICloneable Members

        public override object Clone()
        {
            JsonFieldMapping<T> clone = new JsonFieldMapping<T>(this.ReflectedField);

            clone._decodeAs = _decodeAs;
            clone._encodeAs = _encodeAs;

            clone.JsonField = this.JsonField;
            clone.DesiredType = this.DesiredType;

            return clone;
        }

        #endregion

        /// <summary>
        /// Maps this field to the specified json field.
        /// </summary>
        /// <param name="jsonField"></param>
        /// <returns></returns>
        public JsonFieldMapping<T> To(string jsonField)
        {
            this.JsonField = jsonField;
            return this;
        }


        override internal object Encode(object value)
        {
            if (_encodeAs != null)
            {
                try
                {
                    value = _encodeAs.DynamicInvoke(value);
                }
                catch (Exception exception)
                {
                    throw new Exception("EncodeAs expression caused an exception while attempting to encode '" + value + "'.", exception);
                }
            }

            return value;
        }

        override internal object Decode(object value)
        {
            if (_decodeAs != null)
            {
                try
                {
                    value = _decodeAs.DynamicInvoke(value);
                }
                catch (Exception exception)
                {
                    throw new Exception("DecodeAs expression caused an exception while attempting to decode '" + value + "'.", exception);
                }
            }

            return value;
        }

        override internal void Set(object target, object value)
        {
            if (this.ReflectedField is PropertyInfo)
            {
                PropertyInfo propertyInfo = (PropertyInfo)this.ReflectedField;
                if (propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(target, value, null);
                    return;
                }
            }
            else if (this.ReflectedField is FieldInfo)
            {
                FieldInfo fieldInfo = (FieldInfo)this.ReflectedField;
                fieldInfo.SetValue(target, value);
                return;
            }

            throw new Exception("Not a field nor a property.");
        }

        override internal object Get(object target)
        {
            if (this.ReflectedField is PropertyInfo)
            {
                PropertyInfo propertyInfo = (PropertyInfo)this.ReflectedField;
                if (propertyInfo.CanWrite)
                {
                    return propertyInfo.GetValue(target, null);
                }
            }
            else if (this.ReflectedField is FieldInfo)
            {
                FieldInfo fieldInfo = (FieldInfo)this.ReflectedField;
                return fieldInfo.GetValue(target);
            }

            throw new Exception("Not a field nor a property.");
        }
    }
}