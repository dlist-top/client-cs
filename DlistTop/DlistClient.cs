using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DlistTop.Types;
using DlistTop.Types.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Websocket.Client;

namespace DlistTop
{
    public class DlistClient
    {
        private readonly Uri _gatewayURL = new("wss://gateway.dlist.top");

        private string _token;
        private ILogger _logger;

        private WebsocketClient _ws;


        public Entity Entity { get; private set; }

        public event EventHandler<GatewayEventArgs<Entity>> OnReady;

        public event EventHandler<GatewayEventArgs<VoteData>> OnVote;
        public event EventHandler<GatewayEventArgs<RateData>> OnRate;


        public DlistClient(string token, LogLevel logLevel = LogLevel.Information)
        {
            _token = token;

            _ws = new WebsocketClient(_gatewayURL)
            {
                ReconnectTimeout = TimeSpan.FromSeconds(30),
                MessageEncoding = Encoding.UTF8
            };


            using var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(logLevel);
            });

            _logger = loggerFactory.CreateLogger<DlistClient>();

            _ws.ReconnectionHappened.Subscribe(info =>
            {
                if (info.Type == ReconnectionType.Initial) return;
                _logger.LogWarning("Reconnection happened, type: {Type}", info.Type);
            });

            _ws.MessageReceived.Subscribe(msg =>
            {
                try
                {
                    OnMessage(msg.Text);
                }
                catch (Exception e)
                {
                    _logger.LogError("{Error}", e.ToString());
                }
            });
        }

        public async Task Connect()
        {
            _logger.LogDebug("Connecting to {GatewayUrl}", _gatewayURL);
            await _ws.Start();
        }

        private void SendJSON<T>(T data)
        {
            _ws.Send(JsonConvert.SerializeObject(data));
        }

        private void EmitEvent<T>(EventHandler<GatewayEventArgs<T>> handler, Payload payload)
        {
            handler.Invoke(this, new GatewayEventArgs<T>(payload.Data.ToObject<T>()));
        }

        private void OnMessage(string data)
        {
            var payload = JsonConvert.DeserializeObject<Payload>(data);

            if (payload == null)
            {
                _logger.LogWarning("Received empty payload");
                return;
            }

            switch (payload.Op)
            {
                case GatewayOp.Hello:
                    _logger.LogDebug("Connected to gateway with message: {Data}", payload.Data.ToString());
                    SendJSON(new Payload
                    {
                        Op = GatewayOp.Identify,
                        Data = JToken.FromObject(new
                        {
                            token = _token
                        })
                    });
                    _logger.LogDebug("Identify packet sent");
                    break;
                case GatewayOp.Ready:
                    Entity = payload.Data.ToObject<Entity>();
                    Trace.Assert(Entity != null);
                    _logger.LogDebug(
                        "Dlist.top gateway ready. Connected to entity (type={Type}, id={ID}, name={Name})",
                        Entity.Type.ToString(), Entity.ID, Entity.Name);
                    OnReady.Invoke(this, new GatewayEventArgs<Entity>(Entity));
                    break;
                case GatewayOp.Disconnect:
                    _logger.LogInformation("Disconnected from Dlist.top gateway with reason: {Data}",
                        payload.Data.ToString());
                    break;
                case GatewayOp.Event:

                    switch (payload.Event.ToLower())
                    {
                        case "vote":
                            EmitEvent(OnVote, payload);
                            break;
                        case "rate":
                            EmitEvent(OnRate, payload);
                            break;
                        default:
                            _logger.LogDebug("Unsupported event type: {Event}", payload.Event);
                            break;
                    }

                    break;
            }
        }
    }
}
