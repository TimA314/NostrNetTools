using NostrNetTools.Nostr.Events;

namespace NostrNetTools.Nostr.Connections
{
    public class Pool : IDisposable
    {
        private readonly List<Uri> _relays;
        private readonly string _subscriptionId;
        private readonly object _filter;
        private readonly List<NostrClient> _clients = [];
        private readonly HashSet<string> _seenEventIds = [];

        public event EventHandler<string>? NoticeReceived;
        public event EventHandler<(string eventId, bool success, string message)>? OkReceived;
        public event EventHandler<string>? EoseReceived;
        public event EventHandler<(string subscriptionId, string message)>? ClosedReceived;


        public event EventHandler<(string subscriptionId, NostrEvent[] events)>? EventsReceived;

        public Pool(List<Uri> relays, string subscriptionId, object filter)
        {
            _relays = relays ?? throw new ArgumentNullException(nameof(relays));
            _subscriptionId = subscriptionId;
            _filter = filter;

            InitializeClients();
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

        public async Task ConnectAndSubscribeAsync()
        {
            foreach (var client in _clients)
            {
                await client.ConnectAsync();
                await Task.Delay(3000);
                await client.SendSubscriptionRequestAsync(_subscriptionId, _filter);
            }
        }

        private void OnEventsReceived(object? sender, (string subscriptionId, NostrEvent[] events) e)
        {
            var uniqueEvents = e.events.Where(evt => _seenEventIds.Add(evt.Id)).ToArray();
            if (uniqueEvents.Any())
            {
                EventsReceived?.Invoke(this, (_subscriptionId, uniqueEvents));
            }
        }

        public async Task DisconnectAsync()
        {
            foreach (var client in _clients)
            {
                await client.DisconnectAsync();
            }
        }

        public void Dispose()
        {
            foreach (var client in _clients)
            {
                client.Dispose();
            }
        }
    }
}
