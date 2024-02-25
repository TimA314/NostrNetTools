using Microsoft.Extensions.DependencyInjection;
using NBitcoin;
using NBitcoin.Secp256k1;
using NostrNetTools.Interfaces;
using Xunit;

namespace NostrNetTests
{
    public class NotesTest : IClassFixture<DependencyInjectionFixture>
    {
        private readonly INoteService _noteService;

        private ECPrivKey key = ECPrivKey.Create(RandomUtils.GetBytes(32)); // Generate a random private key

        public NotesTest(DependencyInjectionFixture fixture)
        {
            _noteService = fixture.ServiceProvider.GetService<INoteService>();
        }

        [Fact]
        public async Task GetNotes_ShouldReturnNotes()
        {
            // Arrange
            var eventIdsToGet = new HashSet<string> { "551176e56798776d03d7ef9c5bd27b3108be57a19282bb12365ff114f5dc7e64" };

            // Act
            var retrievedNotes = await _noteService.GetNotesById(eventIdsToGet);

            // Assert
            Assert.True(retrievedNotes.Count > 0);
        }
    }
}
