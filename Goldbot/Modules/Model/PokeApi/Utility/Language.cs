using System.Collections.Generic;

namespace Goldbot.Modules.Model.PokeApi.Utility {
    public class Language : NamedApiResource {
        public int id { get; set; }
        public bool official { get; set; }
        public string iso639 { get; set; }
        public string iso3166 { get; set; }
        //public List<Name> names { get; set; } 
    }
}
