using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Events;
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
            var filter = new
            {
                kinds = new[] { 1 },
                limit = 1
            };

            // Act
            await nostrClient.ConnectAsync();
            await Task.Delay(3000);
            Assert.True(nostrClient.IsConnected);

            await nostrClient.SendSubscriptionRequestAsync(subscriptionId, filter);

            // Assert
            bool messageReceived = false;
            nostrClient.MessageReceived += (sender, message) =>
            {
                if (!string.IsNullOrEmpty(message))
                {
                    messageReceived = true;
                    var nostrEventMessage = JsonSerializer.Deserialize<NostrEventMessage>(message);
                }
            };

            // Use a delay to allow time for messages to be received
            Thread.Sleep(9000);
            await nostrClient.DisconnectAsync();

            Assert.True(messageReceived);
        }
    }
}
