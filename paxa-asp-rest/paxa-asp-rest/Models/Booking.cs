using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace paxa.Models
{
    public class Booking
    {
        public Booking() { }

        public Booking(long id, Resource resource, string userName, String email, DateTime startTime, DateTime endTime)
        {
            this.Id = id;
            this.Resource = resource;
            this.UserName = userName;
            this.Email = email;
            this.StartTime = startTime;
            this.EndTime = endTime;
        }

        public long Id { get; set; }
        public Resource Resource { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        //[JsonProperty(ItemConverterType = typeof(MillisToDateTimeConverter))]
        [JsonConverter(typeof(MillisToDateTimeConverter))]
        public DateTime StartTime { get; set; }

        //[JsonProperty(ItemConverterType = typeof(MillisToDateTimeConverter))]
        [JsonConverter(typeof(MillisToDateTimeConverter))]
        public DateTime EndTime { get; set; }
    }


    public class MillisToDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if(value is DateTime)
            {
                DateTime date = (DateTime)value;
                double millis = date.ToUniversalTime().Subtract(epoch).TotalMilliseconds;
                writer.WriteRawValue("" + millis);
            }
            else
            {
                throw new Exception("Expected DateTime type.");
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) { return null; }
            double d = Convert.ToDouble(reader.Value);

            DateTime clientDate = epoch.AddMilliseconds(d);
            TimeZoneInfo wEuropeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            return TimeZoneInfo.ConvertTime(clientDate, wEuropeZone);
        }
    }
}
