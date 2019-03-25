using PokeAPI;
using System.Threading.Tasks;

namespace Goldbot.Modules.CommandHelpers {
    public static class PokemonHelper {
        private static Pokemon currentPokemon;

        public static Pokemon GetPokemon(string pokemonName) {
            GetPokemonHelper(pokemonName);
            return currentPokemon;
        }

        private static async void GetPokemonHelper(string pokemonName) {
            currentPokemon = await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName);
        }

    }
}
