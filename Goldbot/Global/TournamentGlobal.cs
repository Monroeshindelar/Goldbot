using RestSharp;

namespace Goldbot.Global {
    internal static class TournamentGlobal {
        internal static string api_key = "PCiEIPAU2dILrHvizllGRaEEnQ4Y0IyFfMW6N3UL";
        internal static string api_base_url = "https://api.challonge.com/v1/";
        internal static RestClient client { get; set; }
    }
}
