using Goldbot.Modules.Model.PokeApi.Games;
using Goldbot.Modules.Model.PokeApi.Utility;
using System.Collections.Generic;

namespace Goldbot.Modules.Model.PokeApi.Pokemon.Abilities{
    public class Ability{
        public int id { get; set; }
        public string name { get; set; }
        public bool is_main_Series { get; set; }
        public Generation generation { get; set; }
        public List<Name> names { get; set; }
        //public List<names> effect_entries { get; set; }
        public AbilityEffectChange effect_changes { get; set; }
        public AbilityFlavorText flavor_text_entries { get; set; }
        public AbilityPokemon pokemon { get; set; }
    }
}
