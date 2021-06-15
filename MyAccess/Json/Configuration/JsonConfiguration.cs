using System;
using System.Collections;
using System.Collections.Generic;
using MyAccess.Json.Helpers;
using MyAccess.Json.Mapping;

namespace MyAccess.Json.Configuration
{
    public class JsonConfiguration
    {
        internal Dictionary<Type, JsonTypeMappingBase> Mappings { get; private set; }
        internal bool UsesParallelProcessing { get; private set; }


        internal JsonConfiguration()
        {
            this.Mappings = new Dictionary<Type, JsonTypeMappingBase>();
        }

        /// <summary>
        /// Automatically generates a configuration for the current type.
        /// </summary>
        /// <returns>The configuration.</returns>
        public JsonConfiguration AutoGenerate(Type ktype)
        {
            // Reset
            this.Mappings = new Dictionary<Type, JsonTypeMappingBase>();

            Stack<Type> mapped = new Stack<Type>();
            Stack<Type> unmapped = new Stack<Type>();

            Type root = _resolveMapType(ktype);
            if (root != null)
            {
                unmapped.Push(root);
            }

            while (unmapped.Count > 0)
            {
                Type type = unmapped.Pop();
                mapped.Push(type);

                JsonTypeMappingBase mapping = (JsonTypeMappingBase)Activator.CreateInstance(typeof(JsonTypeMapping<>).MakeGenericType(type));
                _addMapping(mapping);

                mapping.AutoGenerate();

                foreach (JsonFieldMappingBase field in mapping.FieldMappings.Values)
                {
                    Type nested = _resolveMapType(field.DesiredType);
                    if (nested != null && !mapped.Contains(nested) && !unmapped.Contains(nested))
                    {
                        unmapped.Push(nested);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Derives this configuration from an existing configuration.
        /// </summary>
        /// <param name="configuration">The configuration to derive from.</param>
        /// <returns>The configuration.</returns>
        public JsonTypeMapping<TType> GetMapping<TType>()
        {
            return (JsonTypeMapping<TType>)this.Mappings[typeof(TType)];
        }

        /// <summary>
        /// Derives this configuration from an existing configuration.
        /// </summary>
        /// <param name="configuration">The configuration to derive from.</param>
        /// <returns>The configuration.</returns>
        public JsonConfiguration DeriveFrom(JsonConfiguration configuration)
        {
            this.Mappings = new Dictionary<Type, JsonTypeMappingBase>();

            Dictionary<Type, JsonTypeMappingBase>.Enumerator enumerator = configuration.Mappings.GetEnumerator();
            while (enumerator.MoveNext())
            {
                _addMapping((JsonTypeMappingBase)enumerator.Current.Value.Clone());
            }

            return this;
        }

        /// <summary>
        /// Returns a mapping expression for the root type.
        /// </summary>
        /// <param name="expression">The object mapping expression.</param>
        /// <returns>The configuration.</returns>
        public JsonConfiguration WithMapping(JsonTypeMappingBase mapping)
        {
            _addMapping(mapping);
            return this;
        }


        private void _addMapping(JsonTypeMappingBase mapping)
        {
            Type type = mapping.GetType().GetGenericArguments()[0];
            if (type.IsValueType)
            {
                return;
            }
            if (!type.IsInterface)
            {
                if (!Mappings.ContainsKey(type))
                {
                    this.Mappings.Add(type, mapping);
                }
                else
                {
                    throw new Exception("A mapping for type '" + type.Name + "' already exists.");
                }
            }
            else
            {
                throw new Exception("Interfaces cannot be mapped.");
            }
        }

        private Type _resolveMapType(Type type)
        {
            Type result = type;

            if (type.IsGenericType)
            {
                if ((type.Name == "List`1" || type.Name == "IList`1"))
                {
                    result = type.GetGenericArguments()[0];
                }
                else if (TypeHelper.IsThreatableAs(type, typeof(IDictionary<,>)))
                {
                    result = type.GetGenericArguments()[1];
                }
            }
            else if (type.IsArray)
            {
                result = type.GetElementType();
            }

            if (_shouldMapType(result))
            {
                return result;
            }

            return null;
        }

        private bool _shouldMapType(Type type)
        {
            return !TypeHelper.IsBasic(type) && !TypeHelper.IsDictionary(type) && !TypeHelper.IsList(type) && type != typeof(object);
        }
    }
}
