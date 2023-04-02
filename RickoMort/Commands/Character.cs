using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RickoMort.Commands;

public abstract class Character : ApplicationCommandModule
{
    [SlashCommand("character", "Get a character from Rick and Morty")]
    public async Task CharacterCommand(InteractionContext ctx,
        [Option("name", "The name of the character")]
        string name)
    {
        var result = await Program.Client?.GetCharacter.ExecuteAsync(name.ToLower())!;
        var character = result.Data?.Characters?.Results?[0];

        var embed = new DiscordEmbedBuilder
        {
            Title = $"{character?.Name} ({character?.Origin?.Dimension ?? "unknown"})",
            Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail
            {
                Url = character?.Image
            },
        };

        embed.AddField("Info",
            $"Status: {character?.Status}\nGender: {character?.Gender}\nSpecies: {character?.Species}\nType: {character?.Type ?? "unknown"}\nOrigin: {character?.Origin?.Name ?? "unknown"}");
        embed.AddField("Last Location", character?.Location?.Name ?? "unknown");

        await ctx.CreateResponseAsync(embed.Build());
    }
}