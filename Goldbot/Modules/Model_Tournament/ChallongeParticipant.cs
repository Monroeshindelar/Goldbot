using Newtonsoft.Json;
using System;

namespace Goldbot.Modules.Model_Tournament {
    public class ChallongeParticipant {
        [JsonProperty(PropertyName = "id")]
        public int id { get; set; }
        [JsonProperty(PropertyName = "tournament_id")]
        public int tourmanent_id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }
        [JsonProperty(PropertyName = "seed")]
        public int seed { get; set; }
        [JsonProperty(PropertyName = "active")]
        public bool active { get; set; }
        [JsonProperty(PropertyName = "created_at")]
        public DateTime created_at { get; set; }
        [JsonProperty(PropertyName = "updated_at")]
        public DateTime updated_at { get; set; }
        [JsonProperty(PropertyName = "invite_email")]
        public string invite_email { get; set; }
        [JsonProperty(PropertyName = "final_rank")]
        public string final_rank { get; set; }
        [JsonProperty(PropertyName = "misc")]
        public string misc { get; set; }
        [JsonProperty(PropertyName = "icon")]
        public string icon { get; set; }
        [JsonProperty(PropertyName = "on_waiting_list")]
        public bool on_waiting_list { get; set; }
        [JsonProperty(PropertyName = "invitation_id")]
        public string invitation_id { get; set; }
        [JsonProperty(PropertyName = "group_id")]
        public string group_id { get; set; }
        [JsonProperty(PropertyName = "checked_in_at")]
        public Nullable<DateTime> checked_in_at { get; set; }
        [JsonProperty(PropertyName = "ranked_member_id")]
        public Nullable<int> ranked_member_id { get; set; }
        [JsonProperty(PropertyName = "challonge_username")]
        public string challonge_username { get; set; }
        [JsonProperty(PropertyName = "challonge_email_address_verified")]
        public Nullable<bool> challonge_email_address_verified { get; set; }
        [JsonProperty(PropertyName = "removeable")]
        public bool removeable { get; set; }
        [JsonProperty(PropertyName = "participatable_or_invitation_attached")]
        public bool participatable_or_invitation_attached { get; set; }
        [JsonProperty(PropertyName = "confirm_remove")]
        public bool confirm_remove { get; set; }
        [JsonProperty(PropertyName = "invitation_pending")]
        public bool invitation_pending { get; set; }
        [JsonProperty(PropertyName = "display_name_with_invitation_email_address")]
        public string display_name_with_invitation_email_address { get; set; }
        [JsonProperty(PropertyName = "email_hash")]
        public string email_hash { get; set; }
        [JsonProperty(PropertyName = "username")]
        public string username { get; set; }
        [JsonProperty(PropertyName = "display_name")]
        public string display_name { get; set; }
        [JsonProperty(PropertyName = "attached_participateable_portrait_url")]
        public string attached_participateable_portrait_url { get; set; }
        [JsonProperty(PropertyName = "can_check_in")]
        public bool can_check_in { get; set; }
        [JsonProperty(PropertyName = "check_in")]
        public bool checked_in { get; set; }
        [JsonProperty(PropertyName = "reactivateable")]
        public bool reactivateable { get; set; }
        [JsonProperty(PropertyName = "check_in_open")]
        public bool check_in_open { get; set; }
        [JsonProperty(PropertyName = "group_player_ids")]
        public int[] group_player_ids { get; set; }
        [JsonProperty(PropertyName = "has_irrelevant_seed")]
        public bool has_irrelevant_seed { get; set; }
    }
}
