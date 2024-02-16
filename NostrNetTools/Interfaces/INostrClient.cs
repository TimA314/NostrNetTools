using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Interfaces
{
    public interface INostrClient : IDisposable
    {
        Task ConnectAsync(CancellationToken cancellationToken = default);
        Task DisconnectAsync();
        Task PublishEventAsync(NostrEvent nostrEvent, CancellationToken cancellationToken = default);
        Task SendSubscriptionRequestAsync(string subscriptionId, object filter, CancellationToken cancellationToken = default);
    }
}
