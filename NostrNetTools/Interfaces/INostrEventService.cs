using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Interfaces
{
    public interface INostrEventService
    {
        NostrEvent FinishNostrEvent(NostrEvent nostrEvent, string nsec);
        NostrEvent GenerateNostrEventId(NostrEvent nostrEvent);
        NostrEvent SignNostrEvent(NostrEvent nostrEvent, string nsec);
        NostrEvent AddReferencedPublickKeyToNostrEvent(NostrEvent nostrEvent, string referencedNpub);
        NostrEvent AddReferencedEvent(NostrEvent nostrEvent, string referencedEventId);
    }
}
