using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Nostr.Tools
{
    public class NoteService : INoteService
    {
        private readonly IPool _pool;
        private readonly INostrEventService _nostrEventService;


        public NoteService(IPool pool, INostrEventService nostrEventService)
        {
            _pool = pool ?? throw new ArgumentNullException(nameof(pool));
            _nostrEventService = nostrEventService;
        }

        public async Task SignAndPublishNote(string privateKey, string publicKey, string content, List<NostrEventTag> tags)
        {
            await _pool.ConnectAsync();

            var createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var nostrEvent = new NostrEvent
            {
                CreatedAt = createdAt,
                Kind = 1,
                Content = content,
                Tags = tags,
                PublicKey = publicKey,
            };

            var signedNostrEvent = _nostrEventService.SignNostrEvent(nostrEvent, privateKey);

            await _pool.PublishEventAsync(signedNostrEvent);
            await _pool.DisconnectAsync();
        }

        public async Task<HashSet<NostrEvent>> GetNotesById(HashSet<string> eventIdsToGet)
        {
            var filter = new
            {
                ids = eventIdsToGet.ToArray()
            };

            var eventsReceived = new HashSet<NostrEvent>();

            _pool.EventsReceived += (sender, e) =>
            {
                _ = e.events.Select(e => eventsReceived.Add(e));
            };

            await _pool.ConnectAsync();
            await _pool.SubscribeAsync("notes", filter);

            await Task.Delay(10000);

            await _pool.DisconnectAsync();

            return eventsReceived;
        }

        private void SignEvent(string privateKey, ref NostrEvent nostrEvent)
        {
            // Placeholder for event signing logic
            // You would typically use the private key to sign the event's content and set the event's Id and Signature properties.

            // Example:
            // nostrEvent.Signature = SignWithPrivateKey(privateKey, nostrEvent.GetSigningString());
            // nostrEvent.Id = ComputeEventId(nostrEvent.Signature);
        }
    }
}
