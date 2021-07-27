using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DlistTop.Types
{
    public enum EntityType
    {
        [EnumMember(Value = "bots")]
        Bot,
        [EnumMember(Value = "servers")]
        Server
    }
    
    public class Entity
    {
        public string ID { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public EntityType Type { get; set; }
    }
}
