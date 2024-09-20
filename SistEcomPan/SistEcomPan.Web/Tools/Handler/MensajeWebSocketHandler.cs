using System.Net.WebSockets;
using System.Text;

namespace SistEcomPan.Web.Tools.Handler
{
    public static class MensajeWebSocketHandler
    {
        private static readonly List<WebSocket> _webSockets = new List<WebSocket>();

        public static async Task HandleWebSocketAsync(WebSocket webSocket)
        {
            _webSockets.Add(webSocket);
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await BroadcastMessageAsync(message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    _webSockets.Remove(webSocket);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by the WebSocket client", CancellationToken.None);
                }
            }
        }

        private static async Task BroadcastMessageAsync(string message)
        {
            foreach (var socket in _webSockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public static async Task SendMessageAsync(string message)
        {
            await BroadcastMessageAsync(message);
        }
    }
}