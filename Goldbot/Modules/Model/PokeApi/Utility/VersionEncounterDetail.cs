using System.Collections.Generic;

namespace Goldbot.Modules.Model.PokeApi.Utility {
    public class VersionEncounterDetail {
        public Version version { get; set; }
        public int max_chance { get; set; }
        public List<Encounter> encounter_details { get; set; }
    }
}
