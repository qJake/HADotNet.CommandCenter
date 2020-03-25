using Newtonsoft.Json;
using System;
using System.Dynamic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HADotNet.CommandCenter.Utils
{
    public static class LlatHelper
    {
        public static async Task<string> ProvisionAccessToken(string url, string supervisorToken)
        {
            var cancel = new CancellationToken();

            var token = await Task.Run(async () =>
            {
                var client = new ClientWebSocket();

                client.Options.RemoteCertificateValidationCallback = (_, __, ___, ____) => true;

                await client.ConnectAsync(new Uri(url), cancel);

                dynamic authMsgToCheck;

                var authMsg = await Receive(client, cancel);
                dynamic auth = JsonConvert.DeserializeObject<ExpandoObject>(authMsg);

                authMsgToCheck = auth;

                if (auth.type == "auth_required")
                {
                    await Send(client, $@"{{""type"": ""auth"", ""access_token"": ""{supervisorToken}""}}", cancel);
                    var authRespMsg = await Receive(client, cancel);
                    dynamic authResp = JsonConvert.DeserializeObject<ExpandoObject>(authRespMsg);

                    authMsgToCheck = authResp;
                }

                if (authMsgToCheck.type == "auth_ok")
                {
                    await Send(client, $@"{{""id"": 1, ""type"": ""auth/long_lived_access_token"", ""client_name"": ""Home Assistant Command Center (HACC) ({Guid.NewGuid().ToString().Substring(0, 8)})"", ""client_icon"": ""mdi:tablet-dashboard"", ""lifespan"": 3650}}", cancel);

                    var llatMsg = await Receive(client, cancel);
                    dynamic llat = JsonConvert.DeserializeObject<ExpandoObject>(llatMsg);

                    if (llat.type == "result" && (bool)llat.success && JwtHelper.IsTokenValid(llat.result))
                    {
                        return (string)llat.result;
                    }
                    else
                    {
                        return "ERROR: " + llatMsg;
                    }
                }
                else
                {
                    return "ERROR: " + JsonConvert.SerializeObject(authMsgToCheck);
                }
            });

            return token;
        }

        private static async Task Send(ClientWebSocket ws, string message, CancellationToken c)
        {
            if (ws.State != WebSocketState.Open)
            {
                return;
            }

            var msgBytes = Encoding.UTF8.GetBytes(message);

            await ws.SendAsync(new ArraySegment<byte>(msgBytes, 0, msgBytes.Length), WebSocketMessageType.Text, true, c);
        }

        private static async Task<string> Receive(ClientWebSocket ws, CancellationToken c)
        {
            var buffer = new byte[2048];
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), c);

            switch (result.MessageType)
            {
                case WebSocketMessageType.Text:
                    return Encoding.UTF8.GetString(buffer, 0, result.Count);

                case WebSocketMessageType.Close:
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
                    return null;

                // Unsupported, and HA should never send this.
                case WebSocketMessageType.Binary:
                default:
                    return null;
            }
        }
    }
}
