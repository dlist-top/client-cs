using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DlistTop.Types.Events
{
    public class RateData
    {
        public string AuthorID { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType EntityType { get; set; }
        public string EntityID { get; set; }
        
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime Date { get; set; }
        
        public int Rating { get; set; }
        public string Details { get; set; }
    }
}
