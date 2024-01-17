using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Events;

namespace NostrNetTools.Nostr.Connections
{
    public class Pool : IPool
    {
        private readonly List<Uri> _relays;
        private readonly List<NostrClient> _clients = [];
        private readonly HashSet<string> _seenEventIds = [];
        private readonly Dictionary<string, object> _subscriptions = [];

        public event EventHandler<string>? NoticeReceived;
        public event EventHandler<(string eventId, bool success, string message)>? OkReceived;
        public event EventHandler<string>? EoseReceived;
        public event EventHandler<(string subscriptionId, string message)>? ClosedReceived;
        public event EventHandler<(string subscriptionId, NostrEvent[] events)>? EventsReceived;

        public Pool(List<Uri> relays)
        {
            _relays = relays ?? throw new ArgumentNullException(nameof(relays));
            InitializeClients();
        }

        public async Task ConnectAsync()
        {
            foreach (var client in _clients)
            {
                await client.ConnectAsync();
            }
        }

        public async Task SubscribeAsync(string subscriptionId, object filter)
        {
            _subscriptions[subscriptionId] = filter;

            foreach (var client in _clients)
            {
                await client.SendSubscriptionRequestAsync(subscriptionId, filter);
            }
        }

        public async Task PublishEventAsync(NostrEvent nostrEvent)
        {
            foreach (var client in _clients)
            {
                await client.PublishEventAsync(nostrEvent);
            }
        }
            
        public async Task DisconnectAsync()
        {
            foreach (var client in _clients)
            {
                await client.DisconnectAsync();
            }

            Dispose();
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }

        private void InitializeClients()
        {
            foreach (var relay in _relays)
            {
                var client = new NostrClient(relay);
                client.EventsReceived += OnEventsReceived;
                client.NoticeReceived += OnNoticeReceived;
                client.OkReceived += OnOkReceived;
                client.EoseReceived += OnEoseReceived;
                client.ClosedReceived += OnClosedReceived;
                _clients.Add(client);
            }
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
