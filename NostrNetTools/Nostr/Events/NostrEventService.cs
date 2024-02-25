using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Keys;
using System.Text;

namespace NostrNetTools.Nostr.Events
{
    public class NostrEventService : INostrEventService
    {
        private readonly INostrKeyService _nostrKeyService;

        public NostrEventService(INostrKeyService nostrKeyService)
        {
            _nostrKeyService = nostrKeyService;
        }

        public NostrEvent FinishNostrEvent(NostrEvent nostrEvent, string nsec)
        {
            nostrEvent = GenerateNostrEventId(nostrEvent);
            nostrEvent = SignNostrEvent(nostrEvent, nsec);
            return nostrEvent;
        }

        public NostrEvent GenerateNostrEventId(NostrEvent nostrEvent)
        {
            var eventJson = GetUnsignedNostrEventJson(nostrEvent);
            var hash = GetSha256Hash(eventJson);
            var span = hash.AsSpan();
            var hex = ToHex(span);
            nostrEvent.Id = hex;
            return nostrEvent;
        }

        public NostrEvent SignNostrEvent(NostrEvent nostrEvent, string nsec)
        {
            NostrKeySet nostrKeySet = _nostrKeyService.GenerateNostrKeySetFromNSec(nsec);
            if (nostrEvent.PublicKey != nostrKeySet.PublicKey.Hex)
            {
                throw new Exception("Public key does not provided nsec");
            }

            var eventJson = GetUnsignedNostrEventJson(nostrEvent);
            Span<byte> buf = stackalloc byte[64];
            using var sha256 = System.Security.Cryptography.SHA256.Create();

            sha256.TryComputeHash(Encoding.UTF8.GetBytes(eventJson), buf, out _);
            nostrKeySet.PrivateKey.EC.SignBIP340(buf[..32]).WriteToSpan(buf);

            nostrEvent.Signature = ToHex(buf);
            return nostrEvent;
        }

        public NostrEvent AddReferencedPublickKeyToNostrEvent(NostrEvent nostrEvent, string referencedNpub)
        {
            var publicKeyHex = _nostrKeyService.ConvertNpubToPubkeyHex(referencedNpub);
            var eventWithNewTag = AddTagToNostrEvent(nostrEvent, "p", publicKeyHex);
            return eventWithNewTag;
        }

        public NostrEvent AddReferencedEvent(NostrEvent nostrEvent, string referencedEventId)
        {
            var eventWithNewTag = AddTagToNostrEvent(nostrEvent, "e", referencedEventId);
            return eventWithNewTag;
        }

        private NostrEvent AddTagToNostrEvent(NostrEvent nostrEvent, string identifier, params string[] data)
        {
            nostrEvent.Tags.Add(new NostrEventTag()
            {
                TagIdentifier = identifier,
                Data = data.ToList()
            });
            return nostrEvent;
        }

        private string GetUnsignedNostrEventJson(NostrEvent nostrEvent)
        {
            string eventJson = $"[{$"\"{nostrEvent.Id}\""},\"{nostrEvent.PublicKey}\",{nostrEvent.CreatedAt},{nostrEvent.Kind},[{string.Join(',', nostrEvent.Tags.Select(tag => tag))}],\"{nostrEvent.Content}\"]";
            return eventJson;
        }

        private byte[] GetSha256Hash(string data)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
        }

        private string ToHex(Span<byte> bytes)
        {
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
