using System;

namespace Goldbot.Modules.Model.Challonge {
    public class Participant {
        public int id { get; set; }
        public int tourmanent_id { get; set; }
        public string name { get; set; }
        public int seed { get; set; }
        public bool active { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string invite_email { get; set; }
        public string final_rank { get; set; }
        public string misc { get; set; }
        public string icon { get; set; }
        public bool on_waiting_list { get; set; }
        public string invitation_id { get; set; }
        public string group_id { get; set; }
        public Nullable<DateTime> checked_in_at { get; set; }
        public Nullable<int> ranked_member_id { get; set; }
        public string challonge_username { get; set; }
        public Nullable<bool> challonge_email_address_verified { get; set; }
        public bool removeable { get; set; }
        public bool participatable_or_invitation_attached { get; set; }
        public bool confirm_remove { get; set; }
        public bool invitation_pending { get; set; }
        public string display_name_with_invitation_email_address { get; set; }
        public string email_hash { get; set; }
        public string username { get; set; }
        public string display_name { get; set; }
        public string attached_participateable_portrait_url { get; set; }
        public bool can_check_in { get; set; }
        public bool checked_in { get; set; }
        public bool reactivateable { get; set; }
        public bool check_in_open { get; set; }
        public int[] group_player_ids { get; set; }
        public bool has_irrelevant_seed { get; set; }
    }
}
