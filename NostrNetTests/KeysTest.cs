using MioApp.Nostr;
using NBitcoin.Secp256k1;
using Xunit;

namespace NostrNetTests
{
    public class KeysTest
    {


        [Fact]
        public async Task CanHandleNIP04()
        {
            var user1 = CreateKeys("7f4c11a9742721d66e40e321ca50b682c27f7422190c14a187525e69e604836a");
            var user2 = CreateKeys("203b892f1d671fec43a04b36c452de631c9cf55b7a93b75d97ff1e41d217f038");
            var evtFromUser1ToUser2 = new NostrEvent()
            {
                Content = "test",
                Kind = 4,
                Tags = new List<NostrEventTag>()
            {
                new()
                {
                    TagIdentifier = "p",
                    Data = new List<string>()
                    {
                        user2.publicKeyHex
                    }
                }
            }
            };

            await NIP04.EncryptNip04Event(evtFromUser1ToUser2, user1.privateKey);
            Assert.Equal("test", await NIP04.DecryptNip04Event(evtFromUser1ToUser2, user2.privateKey));
            Assert.Equal("test", await NIP04.DecryptNip04Event(evtFromUser1ToUser2, user1.privateKey));

        }
    }
}