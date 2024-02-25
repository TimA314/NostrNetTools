using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Nostr.Connections
{
    public class Pool : IPool
    {
        private readonly List<Uri> _relays;
        private readonly List<INostrClient> _clients = new();
        private readonly HashSet<string> _seenEventIds = new();
        private readonly Dictionary<string, object> _subscriptions = new();
        private readonly INostrClientFactory _clientFactory;
        public List<Uri> Relays => _relays;
        public int ConnectedClientsCount => _clients.Count(client => client.IsConnected);

        public event EventHandler<string>? NoticeReceived;
        public event EventHandler<(string eventId, bool success, string message)>? OkReceived;
        public event EventHandler<string>? EoseReceived;
        public event EventHandler<(string subscriptionId, string message)>? ClosedReceived;
        public event EventHandler<(string subscriptionId, NostrEvent[] events)>? EventsReceived;

        public Pool(List<Uri> relays, INostrClientFactory clientFactory)
        {
            if (relays is null) throw new ArgumentNullException(nameof(relays));
            if (relays.Count <= 0) throw new ArgumentException("At least one relay is required.", nameof(relays));
            if (clientFactory is null) throw new ArgumentNullException(nameof(clientFactory));

            _relays = relays;
            _clientFactory = clientFactory;
            InitializeClients();
        }

        public async Task ConnectAsync()
        {
            var connectTasks = _clients.Select(client => client.ConnectAsync());
            await Task.WhenAll(connectTasks);
        }

        public async Task SubscribeAsync(string subscriptionId, object filter)
        {
            _subscriptions[subscriptionId] = filter;

            var subscriptionTasks = _clients.Select(client => client.SendSubscriptionRequestAsync(subscriptionId, filter));
            await Task.WhenAll(subscriptionTasks);
        }

        public async Task PublishEventAsync(NostrEvent nostrEvent)
        {
            var publishTasks = _clients.Select(client => client.PublishEventAsync(nostrEvent));
            await Task.WhenAll(publishTasks);
        }
            
        public async Task DisconnectAsync()
        {
            if (!_clients.Any(client => client.IsConnected))
            {
                return;
            }
          
            var disconnectTasks = _clients.Where(client => client.IsConnected).Select(client => client.DisconnectAsync());
            await Task.WhenAll(disconnectTasks);
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                if (client is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        private void InitializeClients()
        {
            foreach (var relay in _relays)
            {
                var client = _clientFactory.CreateClient(relay);
                AttachEventHandlers(client);
                _clients.Add(client);
            }
        }

        private void AttachEventHandlers(INostrClient client)
        {
            client.EventsReceived += OnEventsReceived;
            client.NoticeReceived += OnNoticeReceived;
            client.OkReceived += OnOkReceived;
            client.EoseReceived += OnEoseReceived;
            client.ClosedReceived += OnClosedReceived;
        }

        private void DetachEventHandlers(INostrClient client)
        {
            client.EventsReceived -= OnEventsReceived;
            client.NoticeReceived -= OnNoticeReceived;
            client.OkReceived -= OnOkReceived;
            client.EoseReceived -= OnEoseReceived;
            client.ClosedReceived -= OnClosedReceived;
        }

        private void OnNoticeReceived(object sender, string message)
        {
            NoticeReceived?.Invoke(this, message);
        }

        private void OnOkReceived(object sender, (string eventId, bool success, string message) e)
        {
            OkReceived?.Invoke(this, e);
        }

        private void OnEoseReceived(object sender, string subscriptionId)
        {
            EoseReceived?.Invoke(this, subscriptionId);
        }

        private void OnClosedReceived(object sender, (string subscriptionId, string message) e)
        {
            ClosedReceived?.Invoke(this, e);
        }

        private void OnEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) e)
        {
            if (_subscriptions.TryGetValue(e.subscriptionId, out var filter))
            {
                var uniqueEvents = e.events.Where(evt => _seenEventIds.Add(evt.Id)).ToArray();
                if (uniqueEvents.Any())
                {
                    EventsReceived?.Invoke(this, (e.subscriptionId, uniqueEvents));
                }
            }
        }
    }
}
