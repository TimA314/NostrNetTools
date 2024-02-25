using NBitcoin.Secp256k1;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Keys;
using System.Text;
using System.Text.Json;

namespace NostrNetTools.Utils
{
    public class NostrEventSigner
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        private readonly NostrKeyService _nostrKeyService;

        public NostrEventSigner()
        {
            _nostrKeyService = new NostrKeyService();
        }

        public NostrEvent SignNostrEvent(NostrEvent nostrEvent, string nsec)
        {
            NostrKeySet nostrKeySet = _nostrKeyService.GenerateNostrKeySetFromNSec(nsec);
            if (nostrEvent.PublicKey != nostrKeySet.PublicKey.Hex)
            {
                throw new Exception("Public key does not provided nsec");
            }

            string serializedEvent = SerializeEvent(nostrEvent);
            Span<byte> buf = stackalloc byte[64];
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            sha256.TryComputeHash(Encoding.UTF8.GetBytes(serializedEvent), buf, out _);
            nostrKeySet.PrivateKey.EC.SignBIP340(buf[..32]).WriteToSpan(buf);

            nostrEvent.Signature = ToHex(buf);

            return nostrEvent;
        }

        private string SerializeEvent(NostrEvent nostrEvent)
        {
            string serializedEvent = JsonSerializer.Serialize(new object[]
            {
                0,
                nostrEvent.PublicKey,
                nostrEvent.CreatedAt,
                nostrEvent.Kind,
                nostrEvent.Tags,
                nostrEvent.Content
            }, _jsonSerializerOptions);

            return serializedEvent;
        }

        private string ToHex(Span<byte> bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
