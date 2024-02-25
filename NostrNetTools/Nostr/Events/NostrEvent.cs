using NostrNetTools.Nostr.JsonConverters;
using System.Text.Json.Serialization;

namespace NostrNetTools.Nostr.Events
{
    namespace NostrNetTools.Nostr.Events
    {
        public class NostrEvent : IEqualityComparer<NostrEvent>
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("pubkey")]
            public required string PublicKey { get; set; }

            [JsonPropertyName("created_at")]
            public required long CreatedAt { get; set; }

            [JsonPropertyName("kind")]
            public required int Kind { get; set; }

            [JsonPropertyName("content")]
            [JsonConverter(typeof(StringEscaperJsonConverter))]
            public required string Content { get; set; }

            [JsonPropertyName("tags")]
            public required List<NostrEventTag> Tags { get; set; }

            [JsonPropertyName("sig")]
            public string? Signature { get; set; }

            public bool Equals(NostrEvent? x, NostrEvent? y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x?.GetType() != y?.GetType()) return false;
                return x?.Id == y?.Id;
            }

            public int GetHashCode(NostrEvent obj)
            {
                return obj.Id?.GetHashCode() ?? 0;
            }
        }
    }
}