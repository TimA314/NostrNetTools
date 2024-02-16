using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Events;
using NostrNetTools.Nostr.Events.NostrNetTools.Nostr.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace NostrNetTools.Nostr.Tools
{
    public class Note
    {
        private Pool _pool { get; set; }
        private List<Uri> _relays { get; set; }
        public string _publicKey { get; set; }
        public string _content { get; set; }
        public List<List<string>> _tags { get; set; } = [];

        public Note(Pool pool, List<Uri> relays, string publicKey, string content, List<List<string>> tags)
        {
            _pool = pool;
            _relays = relays;
            _publicKey = publicKey;
            _content = content;
            _tags = tags;
        }

        public async Task SignAndPublishNote(string privateKey)
        {
            if (_pool is null)
            {
                _pool = new Pool(_relays);
            }

            await _pool.ConnectAsync();

            var createdAt = DateTimeOffset.UtcNow;

            var nostrEvent = new NostrEvent
            {
                CreatedAt = createdAt,
                Kind = 1,
                Content = _content,
                Tags = _tags,
            };

            // Add Id to event
            // Sign the event


            await _pool.PublishEventAsync(nostrEvent);
            await _pool.DisconnectAsync();
        }

    }
}
