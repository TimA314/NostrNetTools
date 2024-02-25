using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Interfaces
{
    public interface INoteService
    { 
        Task SignAndPublishNote(string privateKey, string publicKey, string content, List<NostrEventTag> tags);
        Task<List<NostrEvent>> GetNotesById(List<string> eventIdsToGet);
    }
}
