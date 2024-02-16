using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using System.Text;
using System.Text.Json;

namespace NostrNetTools.Utils
{
    public static class NostrEventIdGenerator
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string GenerateEventId(NostrEvent nostrEvent)
        {
            var serializedEvent = SerializeEvent(nostrEvent);

            var sha256Hash = CalculateSha256(serializedEvent);

            // Convert the hash to a lowercase hex string
            var eventId = ByteArrayToHex(sha256Hash);
            return eventId;
        }

        private static string SerializeEvent(NostrEvent nostrEvent)
        {
            string serializedEvent = JsonSerializer.Serialize(new object[]
            {
                0,
                nostrEvent.PublicKey,
                nostrEvent.CreatedAt.ToUnixTimeSeconds(),
                nostrEvent.Kind,
                nostrEvent.Tags,
                nostrEvent.Content
            }, _jsonSerializerOptions);

            return serializedEvent;
        }

        private static byte[] CalculateSha256(string serializedEvent)
        {
            using var sha256 = new NBitcoin.Secp256k1.SHA256();
            sha256.Initialize();
            var dataBytes = Encoding.UTF8.GetBytes(serializedEvent);
            sha256.Write(dataBytes);
            return sha256.GetHash();
        }

        private static string ByteArrayToHex(byte[] bytes)
        {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }
    }
}
