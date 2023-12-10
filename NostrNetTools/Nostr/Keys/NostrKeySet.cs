using NBitcoin.Secp256k1;

namespace NostrNetTools.Nostr.Keys
{
    public class NostrKeySet
    {
        public PrivateKeySet PrivateKey { get; set; } = new PrivateKeySet();
        public PublicKeySet PublicKey { get; set; } = new PublicKeySet();
    }

    public class PrivateKeySet
    {
        public string Hex { get; set; }
        public string Bech32 { get; set; }
        public ECPrivKey EC { get; set; }
    }

    public class PublicKeySet
    {
        public string Hex { get; set; }
        public string Bech32 { get; set; }
        public ECXOnlyPubKey ECXOnly { get; set; }
    }
}
