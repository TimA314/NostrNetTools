using Microsoft.Extensions.DependencyInjection;
using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using Xunit;

namespace NostrNetTests
{
    public class NostrClientTests : IClassFixture<DependencyInjectionFixture>
    {
        private readonly IPool _pool;

        public NostrClientTests(DependencyInjectionFixture fixture)
        {
            _pool = fixture.ServiceProvider.GetService<IPool>();
        }

        [Fact]
        public async Task PoolConnectAndSubscribeToMultipleRelays_ShouldReceiveMessages()
        {
            // Arrange
            var subscriptionId = "testSubscription";
            var filter = new { kinds = new[] { 1 }, limit = 10 };

            int eoseReceivedCount = 0;
            var eventsReceived = new List<NostrEvent>();
           
            _pool.EventsReceived += (sender, e) => eventsReceived.AddRange(e.events);
            _pool.EoseReceived += (_, _) => eoseReceivedCount++;

            // Act
            await _pool.ConnectAsync();
            var relays = _pool.Relays;
            var connectedClientsCount = _pool.ConnectedClientsCount;
            await _pool.SubscribeAsync(subscriptionId, filter);

            // Allow time for events to be received
            int timeout = 0;
            while (eoseReceivedCount < 4 /* Assuming 4 relays */ && timeout <= 10)
            {
                timeout++;
                await Task.Delay(1000);
            }
            await _pool.DisconnectAsync();

            // Assert
            Assert.True(eventsReceived.Count > 0, "No events were received.");
        }
    }
}
