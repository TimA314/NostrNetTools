using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.NIP11;

namespace NostrNetTools.Nostr.Connections
{
    public class NostrClient : IDisposable
    {
        private readonly Uri _relay;
        private ClientWebSocket _websocket;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly Channel<string> _pendingIncomingMessages = Channel.CreateUnbounded<string>();
        private readonly Channel<string> _pendingOutgoingMessages = Channel.CreateUnbounded<string>();
        public event EventHandler<string>? MessageReceived;
        public event EventHandler<string>? NoticeReceived;
        public event EventHandler<(string subscriptionId, NostrEvent[] events)>? EventsReceived;
        public event EventHandler<(string eventId, bool success, string message)>? OkReceived;
        public event EventHandler<string>? EoseReceived;
        public bool IsConnected => _websocket.State == WebSocketState.Open;

        public NostrClient(Uri relay)
        {
            _relay = relay ?? throw new ArgumentNullException(nameof(relay));
            _websocket = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            StartMessageProcessing();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default)
        {
            if (_websocket.State == WebSocketState.Open)
                return;

            _websocket.Dispose();
            _websocket = new ClientWebSocket();
            _websocket.Options.HttpVersion = HttpVersion.Version11;

            try
            {
                await _websocket.ConnectAsync(_relay, cancellationToken);
                _ = ListenForMessagesAsync(cancellationToken);
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"WebSocketException: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
                throw new NostrClientException("Failed to connect to the relay.", ex);
            }
        }


        public async Task DisconnectAsync()
        {
            if (_websocket.State != WebSocketState.Open)
                return;

            _cancellationTokenSource.Cancel();
            await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            _websocket.Dispose();
        }

        public async Task SendSubscriptionRequestAsync(string subscriptionId, object filters, CancellationToken cancellationToken = default)
        {
            var subscriptionMessage = JsonSerializer.Serialize(new object[] { "REQ", subscriptionId, filters });
            await HandleOutgoingMessageAsync(subscriptionMessage, cancellationToken);
        }


        private async Task ListenForMessagesAsync(CancellationToken cancellationToken)
        {
            var buffer = new ArraySegment<byte>(new byte[2048]);

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    using var memoryStream = new MemoryStream();
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _websocket.ReceiveAsync(buffer, cancellationToken);
                        memoryStream.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);

                    var message = Encoding.UTF8.GetString(memoryStream.ToArray());
                    await _pendingIncomingMessages.Writer.WriteAsync(message, cancellationToken);
                    MessageReceived?.Invoke(this, message);
                }
            }
            catch (WebSocketException ex)
            {
                // Handle WebSocket exceptions
                // Optionally, raise an event or log the exception
            }
            finally
            {
                _websocket.Abort();
            }
        }

        private async void StartMessageProcessing()
        {
            try
            {
                await Task.WhenAll(
                    ProcessChannel(_pendingIncomingMessages, HandleIncomingMessageAsync, _cancellationTokenSource.Token),
                    ProcessChannel(_pendingOutgoingMessages, HandleOutgoingMessageAsync, _cancellationTokenSource.Token));
            }
            catch (OperationCanceledException)
            {
                // Handle cancellation
            }
        }

        private async Task<bool> HandleIncomingMessageAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                var json = JsonDocument.Parse(message).RootElement;
                // Handle different message types
                // ...
                return true;
            }
            catch (JsonException ex)
            {
                // Handle JSON parsing errors
                return false;
            }
        }

        private async Task<bool> HandleOutgoingMessageAsync(string message, CancellationToken cancellationToken)
        {
            try
            {
                if (_websocket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await _websocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, cancellationToken);
                    return true;
                }
                return false;
            }
            catch (WebSocketException ex)
            {
                // Handle WebSocket exceptions
                return false;
            }
        }

        private async Task ProcessChannel<T>(Channel<T> channel, Func<T, CancellationToken, Task<bool>> processor, CancellationToken cancellationToken)
        {
            while (await channel.Reader.WaitToReadAsync(cancellationToken))
            {
                if (channel.Reader.TryRead(out var item))
                {
                    if (!await processor(item, cancellationToken))
                    {
                        // Handle failed processing
                    }
                }
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _websocket.Dispose();
        }

        // Add additional methods for subscription management, event publishing, etc.
    }

    public class NostrClientException : Exception
    {
        public NostrClientException(string message, Exception innerException) : base(message, innerException) { }
    }
}
