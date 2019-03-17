using PokeAPI;
using System.Threading.Tasks;

namespace Goldbot.Modules.CommandHelpers {
    public class PokemonHelper {

        public static async Task<Pokemon> GetPokemonByName(string pokemonName) {
            return await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName);
        }
    }
}
