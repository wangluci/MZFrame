
using System;

namespace MyAccess.Json.Exceptions
{
    public class JsonException : Exception
    {
        private const int DISPLAY_SIZE = 10;
        public string MalformedJson { get; private set; }

        internal JsonException(string message, string relatedJson, int relatedPosition) : base(message)
        {
            int begin = relatedPosition - DISPLAY_SIZE;
            int end = relatedPosition + DISPLAY_SIZE;

            begin = Math.Max(0, begin);
            end = Math.Min(relatedJson.Length - 1, end);

            this.MalformedJson = relatedJson.Substring(begin, end - begin);
            this.MalformedJson += "\n";
            this.MalformedJson += "^".PadLeft(relatedPosition < DISPLAY_SIZE ? relatedPosition : DISPLAY_SIZE);
        }
    }
}
