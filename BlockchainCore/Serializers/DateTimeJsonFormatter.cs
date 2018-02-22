using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlockchainCore.Serializers
{
    public class DateTimeJsonFormatter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((DateTime)value).ToUniversalTime().ToString("o"));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return reader.Value;
        }
        
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}