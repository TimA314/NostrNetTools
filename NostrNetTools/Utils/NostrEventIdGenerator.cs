using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using System.Text;
using System.Text.Json;

namespace NostrNetTools.Utils
{
    public class NostrEventIdGenerator
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public string GenerateEventId(NostrEvent nostrEvent)
        {
            var serializedEvent = SerializeEvent(nostrEvent);

            var sha256Hash = CalculateSha256(serializedEvent);

            // Convert the hash to a lowercase hex string
            var eventId = ByteArrayToHex(sha256Hash);
            return eventId;
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

        private byte[] CalculateSha256(string serializedEvent)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(serializedEvent));
        }

        private string ByteArrayToHex(byte[] bytes)
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
