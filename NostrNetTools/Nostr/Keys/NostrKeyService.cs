using NBitcoin.Secp256k1;
using NostrNetTools.Utils;
using System.Security.Cryptography;

namespace NostrNetTools.Nostr.Keys
{
    public class NostrKeyService
    {
        public NostrKeySet GenerateNewKeySet()
        {
            var privateKeySet = GenerateNewPrivateKeySet();
            var publicKeySet = GetPublicKeySetFromEcPrivKey(privateKeySet.EC);

            return new NostrKeySet
            {
                PrivateKey = privateKeySet,
                PublicKey = publicKeySet
            };
        }

        public NostrKeySet GenerateKeySetFromNSec(string nsec)
        {
            Bech32.Decode(nsec, out string hrp, out byte[] data);

            if (hrp != "nsec")
            {
                throw new Exception("Invalid nsec");
            }

            var hex = data.ToHex();
            var ec = ECPrivKey.Create(data);

            return new NostrKeySet()
            {
                PrivateKey = new()
                {
                    Bech32 = nsec,
                    Hex = hex,
                    EC = ec
                },
                PublicKey = GetPublicKeySetFromEcPrivKey(ec)
            };
        }

        public NostrKeySet GenerateKeySetFromPrivateKeyHex(string privateKeyHex)
        {
            var privateKeyByte = KeyUtils.ToByteArray(privateKeyHex);
            var ec = ECPrivKey.Create(privateKeyByte);
            var privateKeySet = new PrivateKeySet
            {
                Hex = privateKeyHex,
                EC = ec,
                Bech32 = Bech32.Encode("nsec", privateKeyByte)
            };

            return new NostrKeySet
            {
                PrivateKey = privateKeySet,
                PublicKey = GetPublicKeySetFromEcPrivKey(ec)
            };
        }   


        public PrivateKeySet GenerateNewPrivateKeySet()
        {
            PrivateKeySet privateKeySet = new();
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] array = new byte[32];
            randomNumberGenerator.GetBytes(array);
            privateKeySet.Hex = array.ToHex();
            byte[] privateKeyByte = KeyUtils.ToByteArray(privateKeySet.Hex);
            privateKeySet.EC = ECPrivKey.Create(privateKeyByte);
            privateKeySet.Bech32 = Bech32.Encode("nsec", privateKeyByte);
            return privateKeySet;
        }


        public PublicKeySet GetPublicKeySetFromEcPrivKey(ECPrivKey eCPrivKey)
        {
            PublicKeySet pubKeySet = new();
            pubKeySet.ECXOnly = eCPrivKey.CreateXOnlyPubKey();
            pubKeySet.Hex = pubKeySet.ECXOnly.ToBytes().ToHex();

            if (string.IsNullOrWhiteSpace(pubKeySet.Hex))
            {
                throw new Exception("Public Key Hex is null or empty");
            }

            pubKeySet.Bech32 = Bech32.Encode("npub", KeyUtils.ToByteArray(pubKeySet.Hex));

            return pubKeySet;
        }

        public string ConvertBech32ToNpub(string publicKeyHex)
        {
            if (string.IsNullOrEmpty(publicKeyHex))
            {
                throw new ArgumentException("Public key hex cannot be null or empty", nameof(publicKeyHex));
            }

            var publicKeyBytes = KeyUtils.ToByteArray(publicKeyHex);
            return Bech32.Encode("npub", publicKeyBytes);
        }
    }
}
