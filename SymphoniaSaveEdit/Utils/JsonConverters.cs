using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SymphoniaSaveEdit.Utils
{
    public class BoolArrayConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var formatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            var bools = (bool[])value;
            writer.WriteStartArray();
            foreach (var b in bools)
                writer.WriteValue(b ? 1 : 0);
            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return array.ToObject<bool[]>();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool[]);
        }
    }

    public class ByteHexArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(byte[]).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var formatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            var bytes = (byte[])value;
            writer.WriteStartArray();
            foreach (var b in bytes)
                writer.WriteRawValue($"0x{b:X2}");
            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return array.ToObject<byte[]>();
        }
    }

    public class UShortArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(ushort[]).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var formatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            var nums = (ushort[])value;
            writer.WriteStartArray();
            foreach (var n in nums)
                writer.WriteValue(n);
            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return array.ToObject<ushort[]>();
        }
    }

    public class ByteArrayConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(byte[]).Equals(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var formatting = writer.Formatting;
            writer.Formatting = Formatting.None;
            var nums = (byte[])value;
            writer.WriteStartArray();
            foreach (var n in nums)
                writer.WriteValue(n);
            writer.WriteEndArray();
            writer.Formatting = formatting;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JArray array = JArray.Load(reader);
            return array.ToObject<byte[]>();
        }
    }
}
