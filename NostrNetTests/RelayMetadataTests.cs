using NostrNetTools.Nostr.Connections;
using Xunit;

namespace NostrNetTests
{
    public class RelayMetadataTests
    {
        private readonly Uri _testRelayUri = new("wss://nos.lol/");

        [Fact]
        public async Task GetRelayMetadataAsync_ShouldReturnRelayMetadata()
        {
            // Arrange
            var relayMetadata = new RelayMetadata();

            // Act
            var nostrRelayMetadata = await relayMetadata.GetRelayMetadataAsync(_testRelayUri);

            // Assert
            Assert.NotNull(nostrRelayMetadata);
        }   
    }
}
