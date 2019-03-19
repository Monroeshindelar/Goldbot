using PokeAPI;
using System.Threading.Tasks;

namespace Goldbot.Modules.CommandHelpers {
    public static class PokemonHelper {
        public static async Task<Pokemon> GetPokemon(string pokemonName) {
            return await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName);
        }
    }
}
