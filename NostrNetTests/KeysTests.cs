using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Keys;
using Xunit;

namespace NostrNetTests
{
    public class KeysTests
    {
        private readonly INostrKeyService _nostrKeyService;

        public KeysTests()
        {
            _nostrKeyService = new NostrKeyService();
        }

        [Fact]
        public void GenerateNewKeySet_ShouldCreateValidKeys()
        {
            NostrKeySet keySet = _nostrKeyService.GenerateNewKeySet();
            Assert.NotNull(keySet);
            Assert.NotNull(keySet.PrivateKey);
            Assert.NotNull(keySet.PublicKey);
            Assert.NotNull(keySet.PrivateKey.Hex);
            Assert.NotNull(keySet.PrivateKey.Nsec);
            Assert.NotNull(keySet.PrivateKey.EC);
            Assert.NotNull(keySet.PublicKey.Hex);
            Assert.NotNull(keySet.PublicKey.Npub);
        }

        [Fact]
        public void ConvertPubkeyHexToNpub_ShouldReturnValidNpub()
        {
            NostrKeySet keySet = _nostrKeyService.GenerateNewKeySet();
            var npub = _nostrKeyService.ConvertPubkeyHexToNpub(keySet.PublicKey.Hex);

            Assert.NotNull(npub);
            Assert.Equal(keySet.PublicKey.Npub, npub);
        }

        [Fact]
        public void GenerateNostrKeySetFromNSec_ShouldCreateValidKeySet()
        {
            NostrKeySet keySet = _nostrKeyService.GenerateNewKeySet();
            var newKeySet = _nostrKeyService.GenerateNostrKeySetFromNSec(keySet.PrivateKey.Nsec);

            Assert.Equal(keySet.PrivateKey.Nsec, newKeySet.PrivateKey.Nsec);
            Assert.Equal(keySet.PrivateKey.Hex, newKeySet.PrivateKey.Hex);
            Assert.Equal(keySet.PrivateKey.EC, newKeySet.PrivateKey.EC);
        }

        [Fact]
        public void ConvertNsecToPrivateKeyHex_WithValidNsec_ShouldReturnHex()
        {
            var keySet = _nostrKeyService.GenerateNewKeySet();
            var convertedHex = _nostrKeyService.ConvertNsecToPrivateKeyHex(keySet.PrivateKey.Nsec);

            Assert.Equal(keySet.PrivateKey.Hex, convertedHex);
        }

        [Fact]
        public void ConvertPrivateKeyHexToNsec_WithValidPrivateKeyHex_ShouldReturnNsec()
        {
            var keySet = _nostrKeyService.GenerateNewKeySet();
            var nsec = _nostrKeyService.ConvertPrivateKeyHexToNsec(keySet.PrivateKey.Hex);

            Assert.Equal(keySet.PrivateKey.Nsec, nsec);
        }

        [Fact]
        public void ConvertNpubToPubkeyHex_WithValidNpub_ShouldReturnPubkeyHex()
        {
            var keySet = _nostrKeyService.GenerateNewKeySet();
            var convertedHex = _nostrKeyService.ConvertNpubToPubkeyHex(keySet.PublicKey.Npub);

            Assert.Equal(keySet.PublicKey.Hex, convertedHex);
        }

        [Fact]
        public void ConvertNpubToECXOnly_WithValidNpub_ShouldReturnECXOnlyPubKey()
        {
            var keySet = _nostrKeyService.GenerateNewKeySet();
            var ecxOnlyPubKey = _nostrKeyService.ConvertNpubToECXOnly(keySet.PublicKey.Npub);

            Assert.NotNull(ecxOnlyPubKey);
            Assert.Equal(keySet.PublicKey.ECXOnly, ecxOnlyPubKey);
        }

        [Fact]
        public void GenerateKeySetFromPrivateKeyHex_WithValidPrivateKeyHex_ShouldCreateValidKeySet()
        {
            var keySet = _nostrKeyService.GenerateNewKeySet();
            var newKeySet = _nostrKeyService.GenerateKeySetFromPrivateKeyHex(keySet.PrivateKey.Hex);

            Assert.Equal(keySet.PrivateKey.Hex, newKeySet.PrivateKey.Hex);
            Assert.Equal(keySet.PublicKey.Hex, newKeySet.PublicKey.Hex);
        }

        [Fact]
        public void GetPublicKeySetFromEcPrivKey_WithValidECPrivKey_ShouldCreateValidPublicKeySet()
        {
            var privateKeySet = _nostrKeyService.GenerateNewPrivateKeySet();
            var publicKeySet = _nostrKeyService.GetPublicKeySetFromEcPrivKey(privateKeySet.EC);

            Assert.NotNull(publicKeySet);
            Assert.NotNull(publicKeySet.ECXOnly);
            Assert.NotNull(publicKeySet.Hex);
            Assert.NotNull(publicKeySet.Npub);
        }
    }
}
