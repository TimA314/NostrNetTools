using NBitcoin.Secp256k1;
using NostrNetTools.Nostr.Keys;

namespace NostrNetTools.Interfaces
{
    public interface INostrKeyService
    {
        NostrKeySet GenerateNewKeySet();
        NostrKeySet GenerateNostrKeySetFromNSec(string nsec);
        NostrKeySet GenerateKeySetFromPrivateKeyHex(string privateKeyHex);
        PrivateKeySet GenerateNewPrivateKeySet();
        PublicKeySet GetPublicKeySetFromEcPrivKey(ECPrivKey eCPrivKey);
        string ConvertPrivateKeyHexToNsec(string privateKeyHex);
        string ConvertPubkeyHexToNpub(string publicKeyHex);
        string ConvertNpubToPubkeyHex(string npub);
        ECXOnlyPubKey ConvertNpubToECXOnly(string npub);
        string ConvertNsecToPrivateKeyHex(string nsec);
    }
}
