using NBitcoin.Secp256k1;
using NostrNetTools.Interfaces;
using NostrNetTools.Utils;
using System.Security.Cryptography;

namespace NostrNetTools.Nostr.Keys
{
    public class NostrKeyService : INostrKeyService
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

        public NostrKeySet GenerateNostrKeySetFromNSec(string nsec)
        {
            Bech32.Decode(nsec, out string? hrp, out byte[]? data);

            if (hrp != "nsec" || data is null)
            {
                throw new Exception("Invalid nsec");
            }

            var hex = data.ToHex();
            var ec = ECPrivKey.Create(data);

            return new NostrKeySet()
            {
                PrivateKey = new PrivateKeySet()
                {
                    EC = ec,
                    Hex = hex,
                    Nsec = nsec
                },
                PublicKey = GetPublicKeySetFromEcPrivKey(ec)
            };
        }

        public NostrKeySet GenerateKeySetFromPrivateKeyHex(string privateKeyHex)
        {
            var privateKeyByte = KeyUtils.ToByteArray(privateKeyHex);
            var ec = ECPrivKey.Create(privateKeyByte);
            var nsec = Bech32.Encode("nsec", privateKeyByte);
            if (nsec is null)
            {
                throw new Exception("Invalid private key");
            }
            var privateKeySet = new PrivateKeySet()
            {
                EC = ec,
                Hex = privateKeyHex,
                Nsec = nsec
            };

            return new NostrKeySet
            {
                PrivateKey = privateKeySet,
                PublicKey = GetPublicKeySetFromEcPrivKey(ec)
            };
        }   

        public PrivateKeySet GenerateNewPrivateKeySet()
        {
            using RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create();
            byte[] array = new byte[32];
            randomNumberGenerator.GetBytes(array);
            var privateKeySetHex = array.ToHex();
            byte[] privateKeyByte = KeyUtils.ToByteArray(privateKeySetHex);
            var privateKeySetEC = ECPrivKey.Create(privateKeyByte);

            var nsec = Bech32.Encode("nsec", privateKeyByte);
            if (nsec is null)
            {
                throw new Exception("Invalid nsec");
            }

            return new PrivateKeySet()
            {
                EC = privateKeySetEC,
                Hex = privateKeySetHex,
                Nsec = nsec
            };
        }

        public PublicKeySet GetPublicKeySetFromEcPrivKey(ECPrivKey eCPrivKey)
        {
            var pubkeyECXOnly = eCPrivKey.CreateXOnlyPubKey();
            var pubkeyHex = pubkeyECXOnly.ToBytes().ToHex();
            var npub = ConvertPubkeyHexToNpub(pubkeyHex);

            return new PublicKeySet()
            {
                ECXOnly = pubkeyECXOnly,
                Hex = pubkeyHex,
                Npub = npub
            };
        }

        public string ConvertPrivateKeyHexToNsec(string privateKeyHex)
        {
            if (string.IsNullOrEmpty(privateKeyHex))
            {
                throw new ArgumentException("Private key hex cannot be null or empty", nameof(privateKeyHex));
            }

            var privateKeyBytes = KeyUtils.ToByteArray(privateKeyHex);
            var nsec = Bech32.Encode("nsec", privateKeyBytes);

            if (nsec is null)
            {
                throw new Exception("Invalid Private Key");
            }

            return nsec;
        }   

        public string ConvertPubkeyHexToNpub(string publicKeyHex)
        {
            if (string.IsNullOrEmpty(publicKeyHex))
            {
                throw new ArgumentException("Public key hex cannot be null or empty", nameof(publicKeyHex));
            }

            var publicKeyBytes = KeyUtils.ToByteArray(publicKeyHex);
            var npub = Bech32.Encode("npub", publicKeyBytes);
            if (npub is null)
            {
                throw new Exception("Invalid pubkey");
            }
            return npub;
        }

        public string ConvertNpubToPubkeyHex(string npub)
        {
            Bech32.Decode(npub, out string? hrp, out byte[]? data);

            if (hrp != "npub" || data is null)
            {
                throw new Exception("Invalid npub");
            }

            return data.ToHex();
        }

        public ECXOnlyPubKey ConvertNpubToECXOnly(string npub)
        {
            Bech32.Decode(npub, out string? hrp, out byte[]? data);

            if (hrp != "npub" || data is null)
            {
                throw new Exception("Invalid npub");
            }

            return ECXOnlyPubKey.Create(data);
        } 

        public string ConvertNsecToPrivateKeyHex(string nsec)
        {
            Bech32.Decode(nsec, out string? hrp, out byte[]? data);

            if (hrp != "nsec" || data is null)
            {
                throw new Exception("Invalid nsec");
            }

            return data.ToHex();
        }
    }
}
