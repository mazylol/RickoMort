using DotNetEnv;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RickoMort.Commands;
using RickoMort.GraphQL;

namespace RickoMort
{
    internal abstract class Program
    {
        public static RickandMortyClient? Client;

        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            Env.TraversePath().Load();
            var discordToken = Env.GetString("DEV_TOKEN");
            var guildId = Convert.ToUInt64(Env.GetString("GUILD_ID"));

            var serviceCollection = new ServiceCollection();

            serviceCollection
                .AddRickandMortyClient()
                .ConfigureHttpClient(client =>
                    client.BaseAddress = new Uri("https://rickandmortyapi.com/graphql"));

            IServiceProvider services = serviceCollection.BuildServiceProvider();

            Client = services.GetRequiredService<RickandMortyClient>();

            var discord = new DiscordClient(new DiscordConfiguration
            {
                Token = discordToken,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged,
                MinimumLogLevel = LogLevel.Debug
            });

            discord.Ready += DiscordReady;

            var slash = discord.UseSlashCommands();

            slash.RegisterCommands<Character>(guildId);

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task DiscordReady(DiscordClient client, ReadyEventArgs e)
        {
            await client.UpdateStatusAsync(new DiscordActivity("Rick and Morty", ActivityType.Watching));
        }
    }
}