namespace NostrNetTools.Interfaces
{
    public interface INostrClientFactory
    {
        INostrClient CreateClient(Uri relayUri);
    }
}
