using MyAccess.Json.Configuration;
using MyAccess.Json.Processing;
using System;
using System.Text.RegularExpressions;

namespace MyAccess.Json
{
    /// <summary>
    /// 简单易用json编解码
    /// </summary>
    public class Json
    {
        /// <summary>
        /// 任意对象转json
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <returns>A json string.</returns>
        static public string Encode(object value)
        {
            return Encode(value, null);
        }
        /// <summary>
        /// 任意对象转json
        /// </summary>
        /// <param name="value"></param>
        /// <param name="unicode">是否使用Unicode编码json中文字符</param>
        /// <returns></returns>
        static public string Encode(object value, bool unicode)
        {
            return Encode(value, null, unicode);
        }
        static public string Encode(object value, string datetimeformat, bool unicode)
        {
            return Encode(value, new DefaultDateTimeFormat(datetimeformat), unicode);
        }
        static public string Encode(object value, string datetimeformat)
        {
            return Encode(value, new DefaultDateTimeFormat(datetimeformat));
        }

        /// <summary>
        /// 任意对象转json
        /// </summary>
        /// <param name="value">要转的对象</param>
        /// <param name="unicode">是否使用Unicode编码json中文字符</param>
        /// <param name="datetimeformat">日期格式化模板</param>
        /// <returns></returns>
        static public string Encode(object value, IDateTimeFormat datetimeformat, bool unicode = false, bool nullable = true)
        {
            return new JsonEncoder(new JsonEncodingConfiguration().Unicode2Chinese(unicode).UseNullable(nullable).UseDateTimeFormat(datetimeformat)).Encode(value);
        }
        /// <summary>
        /// Encodes a value of type T to a json string. Type T will be automatically mapped as best as possible.
        /// </summary>
        /// <param name="value">Value to encode.</param>
        /// <returns>A json string.</returns>
        static public string EncodeType<T>(T value)
        {
            return EncoderFor<T>().Encode(value);
        }
        static public string EncodeType<T>(T value, bool unicode)
        {
            return EncoderFor<T>(unicode).Encode(value);
        }


        /// <summary>
        /// Returns a json encoder for the specified type. Type T will be automatically mapped as best as possible.
        /// </summary>
        /// <typeparam name="T">Type to encode.</typeparam>
        /// <returns>An encoder for type T.</returns>
        static public JsonEncoder EncoderFor<T>()
        {
            JsonEncodingConfiguration configuration = new JsonEncodingConfiguration();
            configuration.AutoGenerate(typeof(T));
            return new JsonEncoder(configuration);
        }

        static public JsonEncoder EncoderFor<T>(bool unicode)
        {
            JsonEncodingConfiguration configuration = new JsonEncodingConfiguration();
            configuration.Unicode2Chinese(unicode);
            configuration.AutoGenerate(typeof(T));
            return new JsonEncoder(configuration);
        }

        /// <summary>
        /// json转任意对象
        /// </summary>
        /// <param name="json">Json string to decode.</param>
        /// <returns>The decoded value.</returns>
        static public object Decode(string json)
        {
            return new JsonDecoder(new JsonDecodingConfiguration()).Decode(json, typeof(object));
        }
        static public object Decode(string json, Type t)
        {
            JsonDecodingConfiguration configuration = new JsonDecodingConfiguration();
            configuration.AutoGenerate(t);
            return new JsonDecoder(configuration).Decode(json, t);
        }
        /// <summary>
        /// Decodes a json string to type T. Type T will be automatically mapped as best as possible.
        /// </summary>
        /// <param name="json">Json string to decode.</param>
        /// <returns>The decoded value.</returns>
        static public T DecodeType<T>(string json)
        {
            return DecoderFor<T>().Decode<T>(json);
        }



        /// <summary>
        ///  Returns a json decoder for the specified configuration. Type T will be automatically mapped as best as possible.
        /// </summary>
        /// <typeparam name="T">Type to decode.</typeparam>
        /// <returns>A decoder for type T.</returns>
        static public JsonDecoder DecoderFor<T>()
        {
            JsonDecodingConfiguration configuration = new JsonDecodingConfiguration();
            configuration.AutoGenerate(typeof(T));
            return new JsonDecoder(configuration);
        }

        /// <summary>
        /// Returns an empty base configuration for the specified type T.
        /// </summary>
        /// <typeparam name="T">Type to create configuration for.</typeparam>
        /// <returns>A configuration for type T.</returns>
        static public JsonConfiguration ConfigurationFor()
        {
            JsonConfiguration configuration = new JsonConfiguration();
            return configuration;
        }

        /// <summary>
        /// Returns an empty encoding configuration for the specified type T.
        /// </summary>
        /// <typeparam name="T">Type to create configuration for.</typeparam>
        /// <returns>A configuration for type T.</returns>
        static public JsonEncodingConfiguration EncodingConfigurationFor()
        {
            JsonEncodingConfiguration configuration = new JsonEncodingConfiguration();
            return configuration;
        }

        /// <summary>
        /// Returns an empty decoding configuration for the specified type T.
        /// </summary>
        /// <typeparam name="T">Type to create configuration for.</typeparam>
        /// <returns>A configuration for type T.</returns>
        static public JsonDecodingConfiguration DecodingConfigurationFor()
        {
            JsonDecodingConfiguration configuration = new JsonDecodingConfiguration();
            return configuration;
        }
    }

    /// <summary>
    /// Defines a json encoder for type T.
    /// </summary>
    /// <typeparam name="T">Type to encode.</typeparam>
    public class JsonEncoder
    {
        private JsonEncodingConfiguration _configuration;

        internal JsonEncoder(JsonEncodingConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Encodes a value to a json string.
        /// </summary>
        /// <param name="value">Value of type T to encode.</param>
        /// <returns>A json string.</returns>
        public string Encode<T>(T value)
        {
            return new EncodingProcess(_configuration).Encode<T>(value);
        }
    }

    /// <summary>
    /// Defines a json decoder for type T.
    /// </summary>
    /// <typeparam name="T">Type to decode.</typeparam>
    public class JsonDecoder
    {
        private JsonDecodingConfiguration _configuration;

        internal JsonDecoder(JsonDecodingConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Decodes a json string.
        /// </summary>
        /// <param name="json">Json string to decode.</param>
        /// <returns>The decoded value of type T.</returns>
        public T Decode<T>(string json)
        {
            return (T)Decode(json, typeof(T));
        }
        public object Decode(string json, Type ktype)
        {
            string input = Regex.Replace(json, @"\/\*[\w\W]*?\*\/", "");
            return new DecodingProcess(_configuration).Decode(input, ktype);
        }
    }
}

