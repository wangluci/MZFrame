
using System;

namespace MyAccess.Json.Configuration
{
    public class JsonEncodingConfiguration : JsonConfiguration
    {
        /// <summary>
        /// 配置可以为NULL
        /// </summary>
        internal bool UsesNullable { get; set; }
        internal bool UsesTidy { get; private set; }
        /// <summary>
        /// 配置中文字符串是否转成Unicode
        /// </summary>
        internal bool UseUnicode { get; private set; }
        /// <summary>
        /// 配置日期格式化模板
        /// </summary>
        internal IDateTimeFormat DateTimeFormat { get; private set; }
        internal JsonEncodingConfiguration()
        {
            UseUnicode = false;
            UsesNullable = true;
        }
        public JsonEncodingConfiguration UseNullable(bool used)
        {
            this.UsesNullable = used;
            return this;
        }
        public JsonEncodingConfiguration Unicode2Chinese(bool used)
        {
            this.UseUnicode = used;
            return this;
        }
        public JsonEncodingConfiguration UseDateTimeFormat(IDateTimeFormat format)
        {
            this.DateTimeFormat = format;
            return this;
        }
        /// <summary>
        /// Enables or disables tidy encoding.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public JsonEncodingConfiguration UseTidy(bool value)
        {
            this.UsesTidy = value;
            return this;
        }


        /// <summary>
        /// Derives this configuration from an existing configuration.
        /// </summary>
        /// <param name="configuration">The configuration to derive from.</param>
        /// <returns>The configuration.</returns>
        new public JsonEncodingConfiguration DeriveFrom(JsonConfiguration configuration)
        {
            return (JsonEncodingConfiguration)base.DeriveFrom(configuration);
        }

        /// <summary>
        /// Automatically generates a configuration for the current type.
        /// </summary>
        /// <returns>The configuration.</returns>
        new public JsonEncodingConfiguration AutoGenerate(Type ktype)
        {
            return (JsonEncodingConfiguration)base.AutoGenerate(ktype);
        }


    }
}