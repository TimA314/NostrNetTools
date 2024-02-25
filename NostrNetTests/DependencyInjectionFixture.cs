using Microsoft.Extensions.DependencyInjection;
using NostrNetTools.Interfaces;
using NostrNetTools.Nostr.Connections;
using NostrNetTools.Nostr.Tools;

namespace NostrNetTests
{
    public class DependencyInjectionFixture : IDisposable
    {
        public IServiceProvider ServiceProvider { get; private set; }
        private readonly Uri _relay= new("wss://eden.nostr.land");
        private readonly List<Uri> _relays = new()
        {
            new Uri("wss://nos.lol"),
            new Uri("wss://nostr.kungfu-g.rip"),
            new Uri("wss://nostr.foundrydigital.com"),
            new Uri("wss://eden.nostr.land")
        };

        public DependencyInjectionFixture()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddSingleton<INostrClient, NostrClient>();
            serviceCollection.AddSingleton<INostrClientFactory, NostrClientFactory>();
            serviceCollection.AddSingleton<IPool>(provider => new Pool(_relays, provider.GetRequiredService<INostrClientFactory>()));
            serviceCollection.AddScoped<INoteService, NoteService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public void Dispose()
        {
            if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
