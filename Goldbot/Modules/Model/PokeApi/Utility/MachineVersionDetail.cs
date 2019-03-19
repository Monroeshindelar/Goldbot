using Goldbot.Modules.Model.PokeApi.Games;
using Goldbot.Modules.Model.PokeApi.Machines;

namespace Goldbot.Modules.Model.PokeApi.Utility {
    public class MachineVersionDetail {
        public Machine machine { get; set; }
        public VersionGroup version_group { get; set; }
    }
}
