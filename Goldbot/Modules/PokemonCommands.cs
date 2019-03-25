using Discord;
using Discord.Commands;
using PokeAPI;
using System.Threading.Tasks;

namespace Goldbot.Modules {
    public class PokemonCommands : ModuleBase<SocketCommandContext> {
        [Command("get_pokemon")]
        public async Task GetPokemon([Remainder]string args) {
            Pokemon p = await DataFetcher.GetNamedApiObject<Pokemon>(args);
            EmbedBuilder embed = Utilities.EmbedHelper(p.Name, "", p.Sprites.FrontMale, new Discord.Color(0, 0, 0));
            await Context.Channel.SendMessageAsync("", false, embed);
        }
    }
}
