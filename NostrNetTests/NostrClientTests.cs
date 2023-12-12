using NostrNetTools.Nostr.Connections;
using System.Net.WebSockets;
using System.Text.Json;
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
            var filters = new
            {
                kinds = new[] { 1 },
                limit = 1
            };

            // Act
            await nostrClient.ConnectAsync();
            Assert.True(nostrClient.IsConnected, "Client should be connected");

            await nostrClient.SendSubscriptionRequestAsync(subscriptionId, filters);

            // Assert
            bool messageReceived = false;
            nostrClient.MessageReceived += (sender, message) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageReceived = true;
                }
            };

            // Use a delay to allow time for messages to be received
            await Task.Delay(10000);

            await nostrClient.DisconnectAsync();

            Assert.True(messageReceived, "Should have received at least one message from the relay");
        }
    }
}
