using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Connections;

namespace NostrNetTools.Nostr.Tools
{
    public class NostrClientFactory : INostrClientFactory
    {
        public INostrClient CreateClient(Uri relayUri)
        {
            return new NostrClient(relayUri);
        }
    }
}
