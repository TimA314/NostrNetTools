using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Nostr.Events
{
    public class NostrEventMessage
    {
        public string? EventType { get; set; }
        public string? SubscriptionId { get; set; }
        public NostrEvent? Event { get; set; }
    }
}
