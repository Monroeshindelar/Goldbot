using Goldbot.Modules.Model.PokeApi.Games;
using Goldbot.Modules.Model.PokeApi.Utility;

namespace Goldbot.Modules.Model.PokeApi.Pokemon.Abilities {
    public class AbilityFlavorText {
        public string flavor_text { get; set; }
        public Language language { get; set; }
        public VersionGroup version_group { get; set; }
    }
}
