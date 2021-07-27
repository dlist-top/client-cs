using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace DlistTop.Types
{
    public enum GatewayOp
    {
        Hello = 1,
        Identify,
        Ready,
        Disconnect,
        Event
    }

    public class Payload
    {
        [JsonProperty("op")]
        public GatewayOp Op { get; set; }
        public JToken Data { get; set; }
        public string Event { get; set; } = "";
    }
}
