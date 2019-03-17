using Discord;
using Discord.Commands;
using Goldbot.Modules.CommandHelpers;
using PokeAPI;
using System.Threading.Tasks;

namespace Goldbot.Modules {
    public class PokemonCommands : ModuleBase<SocketCommandContext> {
        [Command("get_pokemon")]
        public async Task GetPokemon([Remainder]string args) {
            EmbedBuilder embed = null;
            Task<Pokemon> t = PokemonHelper.GetPokemonByName(args);
            Pokemon p = t.Result;

            string descrip = $"{p.Types.ToString()}\n";

            Utilities.EmbedHelper(p.Name, descrip, p.Sprites.FrontMale, new Color(0, 0, 0));
            await Context.Channel.SendMessageAsync("", false, embed);
        }

    }
}
