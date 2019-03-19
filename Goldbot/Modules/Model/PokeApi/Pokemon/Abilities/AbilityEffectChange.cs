using Goldbot.Modules.Model.PokeApi.Games;
using Goldbot.Modules.Model.PokeApi.Utility;
using System.Collections.Generic;

namespace Goldbot.Modules.Model.PokeApi.Pokemon.Abilities {
    public class AbilityEffectChange {
        public List<Effect> effect_entries { get; set; }
        public VersionGroup version_group { get; set; }
    }
}
