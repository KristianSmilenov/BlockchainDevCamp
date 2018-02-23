using BlockchainCore.Utils;
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
            string stringDate = DateTimeUtils.GetISO8601DateFormat((DateTime)value);
            writer.WriteValue(stringDate);
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