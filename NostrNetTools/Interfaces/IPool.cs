using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;

namespace NostrNetTools.Interfaces
{
    public interface IPool : IDisposable
    {
        event EventHandler<string> NoticeReceived;
        event EventHandler<(string eventId, bool success, string message)> OkReceived;
        event EventHandler<string> EoseReceived;
        event EventHandler<(string subscriptionId, string message)> ClosedReceived;
        event EventHandler<(string subscriptionId, NostrEvent[] events)> EventsReceived;

        Task ConnectAsync();
        Task SubscribeAsync(string subscriptionId, object filter);
        Task PublishEventAsync(NostrEvent nostrEvent);
        Task DisconnectAsync();
    }
}
