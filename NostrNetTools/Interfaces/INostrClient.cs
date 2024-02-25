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

        Uri Relay { get; }
        bool IsConnected { get; }

        event EventHandler<string> NoticeReceived;
        event EventHandler<(string subscriptionId, NostrEvent[] events)> EventsReceived;
        event EventHandler<(string eventId, bool success, string message)> OkReceived;
        event EventHandler<string> EoseReceived;
        event EventHandler<(string subscriptionId, string message)> ClosedReceived;
    }
}
