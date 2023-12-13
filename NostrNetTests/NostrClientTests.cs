using NostrNetTools.Nostr.Connections;
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
            await Task.Delay(3000);
            Assert.True(nostrClient.IsConnected);

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
            Thread.Sleep(3000);
            await nostrClient.DisconnectAsync();

            Assert.True(messageReceived);
        }
    }
}
