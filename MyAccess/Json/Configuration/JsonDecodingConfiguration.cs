using System;
namespace MyAccess.Json.Configuration
{
    public class JsonDecodingConfiguration : JsonConfiguration
    {

        /// <summary>
        /// Derives this configuration from an existing configuration.
        /// </summary>
        /// <param name="configuration">The configuration to derive from.</param>
        /// <returns>The configuration.</returns>
        new public JsonDecodingConfiguration DeriveFrom(JsonConfiguration configuration)
        {
            return (JsonDecodingConfiguration)base.DeriveFrom(configuration);
        }

        /// <summary>
        /// Automatically generates a configuration for the current type.
        /// </summary>
        /// <returns>The configuration.</returns>
        new public JsonDecodingConfiguration AutoGenerate(Type ktype)
        {
            return (JsonDecodingConfiguration)base.AutoGenerate(ktype);
        }
    }
}