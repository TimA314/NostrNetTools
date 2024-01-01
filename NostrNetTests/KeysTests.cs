using NostrNetTools.Nostr.Keys;
using Xunit;

namespace NostrNetTests
{
    public class KeysTests
    {

        [Fact]
        public void GenerateNewKeys()
        {
            NostrKeyService keyService = new();
            NostrKeySet keySet = keyService.GenerateNewKeySet();
            Assert.NotNull(keySet);
            Assert.NotNull(keySet.PrivateKey);
            Assert.NotNull(keySet.PublicKey);
            Assert.NotNull(keySet.PrivateKey.Hex);
            Assert.NotNull(keySet.PrivateKey.Bech32);
            Assert.NotNull(keySet.PrivateKey.EC);
            Assert.NotNull(keySet.PublicKey.Hex);
            Assert.NotNull(keySet.PublicKey.Bech32);
        }

        [Fact]
        public void GetNpubFromHex()
        {
            NostrKeyService keyService = new();
            NostrKeySet keySet = keyService.GenerateNewKeySet();
            var npub = keyService.ConvertBech32ToNpub(keySet.PublicKey.Hex);

            Assert.NotNull(npub);
            Assert.Equal(keySet.PublicKey.Bech32, npub);
        }

        [Fact]
        public void GetKeySetFromNsec()
        {
            NostrKeyService keyService = new();
            NostrKeySet keySet = keyService.GenerateNewKeySet();

            var newKeySet = keyService.GenerateKeySetFromNSec(keySet.PrivateKey.Bech32);

            Assert.Equal(keySet.PrivateKey.Bech32, newKeySet.PrivateKey.Bech32);
            Assert.Equal(keySet.PrivateKey.Hex, newKeySet.PrivateKey.Hex);
            Assert.Equal(keySet.PrivateKey.EC, newKeySet.PrivateKey.EC);
        }
    }
}
