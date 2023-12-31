using NostrNetTools.Nostr.Keys;
using Xunit;

namespace NostrNetTests
{
    public class KeysTests
    {

        [Fact]
        public void GenerateNewKeys()
        {
            Keys keys = new();            
            Assert.NotNull(keys.KeySet);
            Assert.NotNull(keys.KeySet.PrivateKey);
            Assert.NotNull(keys.KeySet.PublicKey);
            Assert.NotNull(keys.KeySet.PrivateKey.Hex);
            Assert.NotNull(keys.KeySet.PrivateKey.Bech32);
            Assert.NotNull(keys.KeySet.PrivateKey.EC);
            Assert.NotNull(keys.KeySet.PublicKey.Hex);
            Assert.NotNull(keys.KeySet.PublicKey.Bech32);
        }

        [Fact]
        public void GetNpubFromHex()
        {
            Keys keys = new();
            var npub = Keys.ConvertBech32ToNpub(keys.KeySet.PublicKey.Hex);

            Assert.NotNull(npub);
            Assert.Equal(keys.KeySet.PublicKey.Bech32, npub);
        }
    }
}
