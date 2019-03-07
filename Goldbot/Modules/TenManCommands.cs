using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSGSI;
using CSGSI.Nodes;


namespace Goldbot.Modules {
    public class TenManCommands : ModuleBase<SocketCommandContext> {
        [Command("tm")]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task TenMan([Remainder]string args) {
            string[] tokens = args.Split(' ');

            if (tokens.Length == 0) return;
            string command = tokens[0].ToLower();

            SocketRole master = Context.Guild.Roles.First(x => x.Name == "Monroe"),
            capA = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team A Captain"),
            capB = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team B Captain"),
            teamA = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team A"),
            teamB = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Team B");

            SocketGuildUser caller = Context.User as SocketGuildUser;

            bool a = true;
            if (caller.Roles.Contains(capB)) a = false;

            EmbedBuilder embed = null;

            switch (command) {
                case "init":
                    if(Context.Message.MentionedUsers.Count() < 10) {
                        await Context.Channel.SendMessageAsync("You have no listed the correct amount of players to initialize a ten man.\nIt's in the name.");
                        return;
                    }
                    
                    Global.TenManGlobal.participants = new List<SocketGuildUser>();
                    Global.TenManGlobal.mapThumbnails = new Dictionary<string, string>();
                    Global.TenManGlobal.playersRemaining = new List<string>();
                    Global.TenManGlobal.mapsRemaining = new List<string>();

                    Global.TenManGlobal.mapsRemaining.Add("mirage"); Global.TenManGlobal.mapsRemaining.Add("cache"); Global.TenManGlobal.mapsRemaining.Add("inferno");
                    Global.TenManGlobal.mapsRemaining.Add("overpass"); Global.TenManGlobal.mapsRemaining.Add("train"); Global.TenManGlobal.mapsRemaining.Add("nuke");
                    Global.TenManGlobal.mapsRemaining.Add("dust2");

                    Global.TenManGlobal.mapThumbnails.Add("mirage", "https://vignette.wikia.nocookie.net/cswikia/images/a/a7/CSGO_de_Mirage.jpg/revision/latest?cb=20140316221852");
                    Global.TenManGlobal.mapThumbnails.Add("cache", "https://www.killping.com/blog/wp-content/uploads/2018/04/Cache.jpg");
                    Global.TenManGlobal.mapThumbnails.Add("inferno", "https://vignette.wikia.nocookie.net/cswikia/images/f/f0/Inferno.jpg/revision/latest?cb=20161014013320");
                    Global.TenManGlobal.mapThumbnails.Add("overpass", "https://vignette.wikia.nocookie.net/cswikia/images/6/6e/Csgo-de-overpass.png/revision/latest?cb=20140820130544");
                    Global.TenManGlobal.mapThumbnails.Add("train", "https://liquipedia.net/commons/images/thumb/5/56/Train_csgo.jpg/600px-Train_csgo.jpg");
                    Global.TenManGlobal.mapThumbnails.Add("nuke", "https://vignette.wikia.nocookie.net/cswikia/images/5/51/De_nuke_thumbnail.jpg/revision/latest?cb=20180209112248");
                    Global.TenManGlobal.mapThumbnails.Add("dust2", "https://cdn.vox-cdn.com/thumbor/5668HDDEe6lGfyp96HMJGa7IV6I=/0x0:2560x1600/1200x800/filters:focal(1076x596:1484x1004)/cdn.vox-cdn.com/uploads/chorus_image/image/57107595/csgo_dust2.0.jpg");

                    foreach (SocketUser participant in Context.Message.MentionedUsers) {
                        Global.TenManGlobal.participants.Add(participant as SocketGuildUser);
                        Global.TenManGlobal.playersRemaining.Add(participant.Username);
                    }
                    string rp = "";
                    foreach (string playername in Global.TenManGlobal.playersRemaining) rp += playername + "\n";
                    string rm = "";
                    foreach (string mapname in Global.TenManGlobal.mapsRemaining) rm += mapname + "\n";

                    RestUserMessage remainingPlayers = await Context.Channel.SendMessageAsync("```css\nRemaining Players:\n" + rp + "```");
                    RestUserMessage teamACompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam A: ```");
                    RestUserMessage teamBCompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam B: ```");
                    RestUserMessage remainingMapMessage = await (Context.Channel.SendMessageAsync("```css\nMaps Remaining:\n" + rm + "```"));
                    RestUserMessage mapPickBanMessage = await Context.Channel.SendMessageAsync("```css\nMap Pick/Ban: ```");
                    Global.TenManGlobal.TeamAMessageID = teamACompositionMessage.Id;
                    Global.TenManGlobal.TeamBMessageID = teamBCompositionMessage.Id;
                    Global.TenManGlobal.MapPickMessaegID = mapPickBanMessage.Id;
                    Global.TenManGlobal.RemainingPlayersMessageID = remainingPlayers.Id;
                    Global.TenManGlobal.RemainingMapsMessageID = remainingMapMessage.Id;
                    Global.TenManGlobal.IsBanPhase = true;
                    Global.TenManGlobal.TeamATurn = true;
                    SocketRole[] roles = { teamA, teamB, capA, capB };
                    break;
                case "free":
                    if (Global.TenManGlobal.participants.Count == 0) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. There is nothing to free.\nPlease initialize a game using the init command before calling free.");
                        return;
                    }
                    await Context.Channel.SendMessageAsync("Beginning deconstruction.");
                    SocketRole[] rolesToRemove = { teamA, teamB, capA, capB };
                    foreach (SocketGuildUser participant in Global.TenManGlobal.participants) {
                        await participant.RemoveRolesAsync(rolesToRemove);
                    }
                    Global.TenManGlobal.participants = null;
                    await Context.Channel.SendMessageAsync("Players freed.");
                    break;
                case "pickcaptains":
                    if (Global.TenManGlobal.participants.Count < 10) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. \nCaptains cannot be selected. \nPlease initialize a game using the init command before trying to pick captains");
                        return;
                    }
                    Random r = new Random();
                    int capAIndex = r.Next(0, Global.TenManGlobal.participants.Count);
                    SocketGuildUser cap = Global.TenManGlobal.participants[0];
                    embed = EmbedHelper("Team A Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(255, 0, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capA);
                    var msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamAMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + "```");

                    Global.TenManGlobal.playersRemaining.Remove(cap.Username);

                    int capBIndex = capAIndex;
                    while (capBIndex == capAIndex) capBIndex = r.Next(0, Global.TenManGlobal.participants.Count);
                    cap = Global.TenManGlobal.participants[capBIndex];
                    embed = EmbedHelper("Team B Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(0, 0, 255));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capB);

                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamBMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + " - Captain" + "```");
                    Global.TenManGlobal.playersRemaining.Remove(cap.Username);

                    rp = "";
                    foreach (string playername in Global.TenManGlobal.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");
                    break;
                case "pickplayer":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    if (tokens.Length == 1) return;

                    if(a && !Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a player.\nPlease wait your turn.");
                        return;
                    } else if(!a && Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a player.\nPlease wait your turn.");
                        return;
                    }
                    
                    if(Global.TenManGlobal.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All the players have been picked.\nIt's time to start the pick/ban phase.\nTo start banning and picking maps use the banmap and pickmap commands.\nThe pick ban order is B-B-P-P-B-B");
                        return;
                    }

                    var pick = Context.Message.MentionedUsers.First();

                    var user = pick as SocketGuildUser;
                    if (user.Roles.Contains(teamB) || user.Roles.Contains(capB)
                        || user.Roles.Contains(teamA) || user.Roles.Contains(capA) || 
                        !Global.TenManGlobal.participants.Contains(user)) 
                    {
                        await Context.Channel.SendMessageAsync(pick.Username + " is unavailable.");
                        return;
                    }

                    if (a) await (pick as IGuildUser).AddRoleAsync(teamA);
                    else await (pick as IGuildUser).AddRoleAsync(teamB);

                    Global.TenManGlobal.playersRemaining.Remove(pick.Username);

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked:"), pick.Username, pick.GetAvatarUrl(), (a ? new Color(255, 0, 0) : new Color(0, 0, 255)));
                    await Context.Channel.SendMessageAsync("", false, embed);

                    RestUserMessage message;
                    if (a) message = await Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamAMessageID) as RestUserMessage;
                    else message = await Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamBMessageID) as RestUserMessage;
                    await message.ModifyAsync(x => x.Content = "```" + message.Content.ToString().Trim(new char[] { '`' }) + "\n" + pick.Username + "```");

                    rp = "";
                    foreach (string playername in Global.TenManGlobal.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");

                    if (Global.TenManGlobal.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All player have been pick.\nBegin the map pick/ban phase");
                        Global.TenManGlobal.IsBanPhase = true;
                    }
                    Global.TenManGlobal.TeamATurn = !Global.TenManGlobal.TeamATurn; 
                    break;
                case "pickmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    string mappick = tokens[1].ToLower();
                    if (!Global.TenManGlobal.mapThumbnails.ContainsKey(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " does not exist in the competitive pool.");
                        return;
                    }

                    if(Global.TenManGlobal.mapsRemaining.Count <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (Global.TenManGlobal.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                    }

                    if(Global.TenManGlobal.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the ban phase.\nPlease wait for both teams to ban their maps before picking a map.");
                        return;
                    }

                    if(!Global.TenManGlobal.mapsRemaining.Contains(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    if (a && !Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }
                    else if (!a && Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked: "), mappick, Global.TenManGlobal.mapThumbnails[mappick], new Color(0, 255, 0)); ;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    Global.TenManGlobal.mapsRemaining.Remove(mappick);

                    RestUserMessage mapPickMessage = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                    await mapPickMessage.ModifyAsync(x => x.Content = "```" + mapPickMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mappick +" - Picked by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in Global.TenManGlobal.mapsRemaining) rm += mapname + "\n";
                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) Global.TenManGlobal.IsBanPhase = true;
                    Global.TenManGlobal.TeamATurn = !Global.TenManGlobal.TeamATurn;
                    break;
                case "banmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }

                    if(Global.TenManGlobal.mapsRemaining.Count() <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (Global.TenManGlobal.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                        return;
                    }

                    if (!Global.TenManGlobal.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the pick phase.\nPlease wait for both teams to pick their maps before banning a map.");
                        return;
                    }

                    if(a && !Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    } else if (!a && Global.TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    }

                    string mapban = tokens[1].ToLower();
                    if (!Global.TenManGlobal.mapThumbnails.ContainsKey(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " does not exist in the competitive pool.");
                        return;
                    }

                    if(!Global.TenManGlobal.mapsRemaining.Contains(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Banned: " : "Team B Banned: "), mapban, Global.TenManGlobal.mapThumbnails[mapban], new Color(255, 0, 0));;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    Global.TenManGlobal.mapsRemaining.Remove(mapban);

                    RestUserMessage mapBanMessage = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                    await mapBanMessage.ModifyAsync(x => x.Content = "```" + mapBanMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mapban + " - Banned by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in Global.TenManGlobal.mapsRemaining) rm += mapname + "\n";

                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) Global.TenManGlobal.IsBanPhase = false;
                    Global.TenManGlobal.TeamATurn = !Global.TenManGlobal.TeamATurn;

                    if(Global.TenManGlobal.mapsRemaining.Count == 1) {
                        string name = Global.TenManGlobal.mapsRemaining.First();
                        embed = EmbedHelper("Decider: ", name, Global.TenManGlobal.mapThumbnails[name], new Color(0, 255, 0));
                        await Context.Channel.SendMessageAsync("", false, embed);
                        RestUserMessage decider = (await Context.Channel.GetMessageAsync(Global.TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                        await decider.ModifyAsync(x => x.Content = "```" + decider.Content.ToString().Trim(new char[] { '`' }) + "\n" + name + " - Decider Map ```");
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is completed.\nJoin your respective voice channels and begin the game!");
                    }
                    break;
                case "startgsl":
                case "startgamestatelistener":
                    Global.TenManGlobal.gsl = new GameStateListener("http://localhost:3000");
                    Global.TenManGlobal.gsl.NewGameState += new NewGameStateHandler(OnNewGameState);
                    if (!Global.TenManGlobal.gsl.Start()) {
                        Console.WriteLine("Failed to listen to the game");
                        return;
                    }
                    Console.WriteLine("Listening...");
                    break;
                default:
                    await Context.Channel.SendMessageAsync("Command is not recognized.");
                    break;
            }
        }

        void OnNewGameState(GameState gs) {
            if(gs.Map.TeamCT.Score == 16 || gs.Map.TeamT.Score == 16 ||
                (gs.Map.TeamCT.Score == 15 && gs.Map.TeamT.Score == 15)) {
                foreach (var player in gs.AllPlayers) {
                    Console.WriteLine(player.Name);
                    Console.WriteLine(player.SteamID);
                    Console.WriteLine(player.MatchStats.Kills);
                    Console.WriteLine(player.MatchStats.Deaths);
                }
            }
        }

        //Pull this out into its own helper class
        private EmbedBuilder EmbedHelper(string title, string desc, string url, Color color) {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle(title);
            embed.WithDescription(desc);
            embed.WithColor(color);
            embed.WithThumbnailUrl(url);
            return embed;
        }
    }
}
