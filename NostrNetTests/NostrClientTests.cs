using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Events;
using Xunit;


namespace NostrNetTests
{
    public class NostrClientTests
    {
        private readonly Uri _testRelayUri = new Uri("wss://nos.lol");

        [Fact]
        public async Task ConnectAndSubscribeToRelay_ShouldReceiveMessages()
        {
            // Arrange
            var nostrClient = new NostrClient(_testRelayUri);
            var subscriptionId = "testSubscription";
            var filter = new { kinds = new[] { 1 }, limit = 10 };

            var messageReceived = false;
            var noticeReceived = false;
            var okReceived = false;
            var closedReceived = false;
            var eoseReceived = false;

            nostrClient.EventsReceived += (_, _) => messageReceived = true;
            nostrClient.NoticeReceived += (_, _) => noticeReceived = true;
            nostrClient.OkReceived += (_, _) => okReceived = true;
            nostrClient.ClosedReceived += (_, _) => closedReceived = true;
            nostrClient.EoseReceived += (_, _) => eoseReceived = true;

            // Act
            await nostrClient.ConnectAsync();
            Assert.True(nostrClient.IsConnected);

            await nostrClient.SendSubscriptionRequestAsync(subscriptionId, filter);

            int timeout = 0;
            while (eoseReceived == false && timeout < 30)
            {
                timeout++;
                await Task.Delay(1000);
            }
            await nostrClient.DisconnectAsync();

            // Assert
            Assert.True(messageReceived);
        }

        [Fact]
        public async Task PoolConnectAndSubscribeToMultipleRelays_ShouldReceiveMessages()
        {
            // Arrange
            var relays = new List<Uri> {
                new Uri("wss://nos.lol"),
                new Uri("wss://nostr.kungfu-g.rip")
            };

            var subscriptionId = "testSubscription";
            var filter = new
            {
                kinds = new[] { 1 },
                limit = 10
            };

            int eoseReceivedCount = 0;
            var eventsReceived = new List<NostrEvent>();

            var pool = new Pool(relays);

            pool.EventsReceived += (sender, e) =>
            {
                eventsReceived.AddRange(e.events);
            };

            pool.EoseReceived += (_, _) => eoseReceivedCount++;

            // Act
            await pool.ConnectAsync();
            await pool.SubscribeAsync(subscriptionId, filter);

            // Allow time for events to be received
            int timeout = 0;
            while (eoseReceivedCount < relays.Count && timeout <= 10)
            {
                timeout++;
                await Task.Delay(1000);
            }
            await pool.DisconnectAsync();

            // Assert
            Assert.True(eventsReceived.Count > 0, "No events were received.");
        }
    }
}
