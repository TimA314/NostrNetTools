using NBitcoin.Secp256k1;

namespace NostrNetTools.Nostr.Keys
{
    public class NostrKeySet
    {
        public PrivateKeySet PrivateKey { get; set; }
        public PublicKeySet PublicKey { get; set; }
    }

    public class PrivateKeySet
    {
        public string Hex { get; set; }
        public string Nsec { get; set; }
        public ECPrivKey EC { get; set; }
    }

    public class PublicKeySet
    {
        public string Hex { get; set; }
        public string Npub { get; set; }
        public ECXOnlyPubKey ECXOnly { get; set; }
    }
}
