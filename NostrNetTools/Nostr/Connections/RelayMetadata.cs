using NostrNetTools.Nostr.NIP11;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NostrNetTools.Nostr.Connections
{
    public class RelayMetadata
    {
        public async Task<NostrRelayMetadata> GetRelayMetadataAsync(Uri relayUri, CancellationToken cancellationToken = default)
        {
            using HttpClient client = new();
            client.Timeout = TimeSpan.FromSeconds(7);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/nostr+json"));

            var response = await client.GetAsync(relayUri);
            response.EnsureSuccessStatusCode();

            var metadataJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var nostrRelayMetadata = JsonSerializer.Deserialize<NostrRelayMetadata>(metadataJson);
            return nostrRelayMetadata;
        }
    }
}
