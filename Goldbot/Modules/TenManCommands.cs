using Discord;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Goldbot.Global;

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

            SocketGuildChannel aChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == "Team A"),
                bChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == "Team B"),
                mainChannel = Context.Guild.Channels.FirstOrDefault(c => c.Name == "TM General");
                


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
                    
                    TenManGlobal.participants = new List<SocketGuildUser>();
                    TenManGlobal.mapThumbnails = new Dictionary<string, string>();
                    TenManGlobal.playersRemaining = new List<string>();
                    TenManGlobal.mapsRemaining = new List<string>();

                    TenManGlobal.mapsRemaining.Add("mirage"); TenManGlobal.mapsRemaining.Add("cache"); TenManGlobal.mapsRemaining.Add("inferno");
                    TenManGlobal.mapsRemaining.Add("overpass"); Global.TenManGlobal.mapsRemaining.Add("train"); Global.TenManGlobal.mapsRemaining.Add("nuke");
                    TenManGlobal.mapsRemaining.Add("dust2");

                    TenManGlobal.mapThumbnails.Add("mirage", "https://vignette.wikia.nocookie.net/cswikia/images/a/a7/CSGO_de_Mirage.jpg/revision/latest?cb=20140316221852");
                    TenManGlobal.mapThumbnails.Add("cache", "https://www.killping.com/blog/wp-content/uploads/2018/04/Cache.jpg");
                    TenManGlobal.mapThumbnails.Add("inferno", "https://vignette.wikia.nocookie.net/cswikia/images/f/f0/Inferno.jpg/revision/latest?cb=20161014013320");
                    TenManGlobal.mapThumbnails.Add("overpass", "https://vignette.wikia.nocookie.net/cswikia/images/6/6e/Csgo-de-overpass.png/revision/latest?cb=20140820130544");
                    TenManGlobal.mapThumbnails.Add("train", "https://liquipedia.net/commons/images/thumb/5/56/Train_csgo.jpg/600px-Train_csgo.jpg");
                    TenManGlobal.mapThumbnails.Add("nuke", "https://vignette.wikia.nocookie.net/cswikia/images/5/51/De_nuke_thumbnail.jpg/revision/latest?cb=20180209112248");
                    TenManGlobal.mapThumbnails.Add("dust2", "https://cdn.vox-cdn.com/thumbor/5668HDDEe6lGfyp96HMJGa7IV6I=/0x0:2560x1600/1200x800/filters:focal(1076x596:1484x1004)/cdn.vox-cdn.com/uploads/chorus_image/image/57107595/csgo_dust2.0.jpg");

                    foreach (SocketUser participant in Context.Message.MentionedUsers) {
                        TenManGlobal.participants.Add(participant as SocketGuildUser);
                        TenManGlobal.playersRemaining.Add(participant.Username);
                    }
                    string rp = "";
                    foreach (string playername in TenManGlobal.playersRemaining) rp += playername + "\n";
                    string rm = "";
                    foreach (string mapname in TenManGlobal.mapsRemaining) rm += mapname + "\n";

                    RestUserMessage remainingPlayers = await Context.Channel.SendMessageAsync("```css\nRemaining Players:\n" + rp + "```");
                    RestUserMessage teamACompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam A: ```");
                    RestUserMessage teamBCompositionMessage = await Context.Channel.SendMessageAsync("```css\nTeam B: ```");
                    RestUserMessage remainingMapMessage = await (Context.Channel.SendMessageAsync("```css\nMaps Remaining:\n" + rm + "```"));
                    RestUserMessage mapPickBanMessage = await Context.Channel.SendMessageAsync("```css\nMap Pick/Ban: ```");
                    TenManGlobal.TeamAMessageID = teamACompositionMessage.Id;
                    TenManGlobal.TeamBMessageID = teamBCompositionMessage.Id;
                    TenManGlobal.MapPickMessaegID = mapPickBanMessage.Id;
                    TenManGlobal.RemainingPlayersMessageID = remainingPlayers.Id;
                    TenManGlobal.RemainingMapsMessageID = remainingMapMessage.Id;
                    TenManGlobal.IsBanPhase = true;
                    TenManGlobal.TeamATurn = true;
                    SocketRole[] roles = { teamA, teamB, capA, capB };
                    break;
                case "free":
                    if (TenManGlobal.participants.Count == 0) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. There is nothing to free.\nPlease initialize a game using the init command before calling free.");
                        return;
                    }
                    await Context.Channel.SendMessageAsync("Beginning deconstruction.");
                    SocketRole[] rolesToRemove = { teamA, teamB, capA, capB };
                    foreach (SocketGuildUser participant in Global.TenManGlobal.participants) {
                        await participant.RemoveRolesAsync(rolesToRemove);
                    }
                    TenManGlobal.participants = null;
                    await Context.Channel.SendMessageAsync("Players freed.");
                    break;
                case "pickcaptains":
                    if (TenManGlobal.participants.Count < 10) {
                        await Context.Channel.SendMessageAsync("A pug has not been instantiated. \nCaptains cannot be selected. \nPlease initialize a game using the init command before trying to pick captains");
                        return;
                    }
                    Random r = new Random();
                    int capAIndex = r.Next(0, Global.TenManGlobal.participants.Count);
                    SocketGuildUser cap = Context.Message.MentionedUsers.Count != 2 ? Global.TenManGlobal.participants[capAIndex]
                        : (SocketGuildUser) Context.Message.MentionedUsers.ElementAt(0);
                    embed = EmbedHelper("Team A Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(255, 0, 0));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capA);
                    var msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamAMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + " - Captain ```");

                    await cap.ModifyAsync(x => {
                        x.ChannelId = aChannel.Id;
                    });

                    TenManGlobal.playersRemaining.Remove(cap.Username);

                    int capBIndex = capAIndex;
                    while (capBIndex == capAIndex) capBIndex = r.Next(0, Global.TenManGlobal.participants.Count);
                    cap = Context.Message.MentionedUsers.Count != 2 ? Global.TenManGlobal.participants[capBIndex]
                        : (SocketGuildUser)Context.Message.MentionedUsers.ElementAt(1);
                    embed = EmbedHelper("Team B Captain: ", cap.Username, cap.GetAvatarUrl(), new Color(0, 0, 255));
                    await Context.Channel.SendMessageAsync("", false, embed);
                    await cap.AddRoleAsync(capB);

                    msg = await (Context.Channel.GetMessageAsync(Global.TenManGlobal.TeamBMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```" + msg.Content.ToString().Trim(new char[] { '`' }) + "\n" + cap.Username + " - Captain" + "```");

                    await cap.ModifyAsync(x => {
                        x.ChannelId = bChannel.Id;
                    });

                    TenManGlobal.playersRemaining.Remove(cap.Username);

                    rp = "";
                    foreach (string playername in TenManGlobal.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(TenManGlobal.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");
                    break;
                case "pickplayer":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    if (tokens.Length == 1) return;

                    if(a && !TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a player.\nPlease wait your turn.");
                        return;
                    } else if(!a && TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a player.\nPlease wait your turn.");
                        return;
                    }
                    
                    if(TenManGlobal.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All the players have been picked.\nIt's time to start the pick/ban phase.\nTo start banning and picking maps use the banmap and pickmap commands.\nThe pick ban order is B-B-P-P-B-B");
                        return;
                    }

                    var pick = Context.Message.MentionedUsers.First();

                    var user = pick as SocketGuildUser;

                    ulong id = a ? aChannel.Id : bChannel.Id;

                    await user.ModifyAsync(x => {
                        x.ChannelId = id;
                    });

                    if (user.Roles.Contains(teamB) || user.Roles.Contains(capB)
                        || user.Roles.Contains(teamA) || user.Roles.Contains(capA) || 
                        !TenManGlobal.participants.Contains(user)) 
                    {
                        await Context.Channel.SendMessageAsync(pick.Username + " is unavailable.");
                        return;
                    }

                    if (a) await (pick as IGuildUser).AddRoleAsync(teamA);
                    else await (pick as IGuildUser).AddRoleAsync(teamB);

                    TenManGlobal.playersRemaining.Remove(pick.Username);

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked:"), pick.Username, pick.GetAvatarUrl(), (a ? new Color(255, 0, 0) : new Color(0, 0, 255)));
                    await Context.Channel.SendMessageAsync("", false, embed);

                    RestUserMessage message;
                    if (a) message = await Context.Channel.GetMessageAsync(TenManGlobal.TeamAMessageID) as RestUserMessage;
                    else message = await Context.Channel.GetMessageAsync(TenManGlobal.TeamBMessageID) as RestUserMessage;
                    await message.ModifyAsync(x => x.Content = "```" + message.Content.ToString().Trim(new char[] { '`' }) + "\n" + pick.Username + "```");

                    rp = "";
                    foreach (string playername in TenManGlobal.playersRemaining) rp += playername + "\n";
                    msg = await (Context.Channel.GetMessageAsync(TenManGlobal.RemainingPlayersMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nPlayers Remaining:\n" + rp + "```");

                    if (TenManGlobal.playersRemaining.Count() == 0) {
                        await Context.Channel.SendMessageAsync("All player have been pick.\nBegin the map pick/ban phase");
                        TenManGlobal.IsBanPhase = true;
                    }
                    TenManGlobal.TeamATurn = !TenManGlobal.TeamATurn; 
                    break;
                case "pickmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }
                    string mappick = tokens[1].ToLower();
                    if (!TenManGlobal.mapThumbnails.ContainsKey(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " does not exist in the competitive pool.");
                        return;
                    }

                    if(TenManGlobal.mapsRemaining.Count <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (TenManGlobal.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                    }

                    if(TenManGlobal.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the ban phase.\nPlease wait for both teams to ban their maps before picking a map.");
                        return;
                    }

                    if(!TenManGlobal.mapsRemaining.Contains(mappick)) {
                        await Context.Channel.SendMessageAsync(mappick + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    if (a && !TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }
                    else if (!a && TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to pick a map.\nPlease wait your turn to pick a map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Picked: " : "Team B Picked: "), mappick, TenManGlobal.mapThumbnails[mappick], new Color(0, 255, 0)); ;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    TenManGlobal.mapsRemaining.Remove(mappick);

                    RestUserMessage mapPickMessage = await (Context.Channel.GetMessageAsync(TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                    await mapPickMessage.ModifyAsync(x => x.Content = "```" + mapPickMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mappick +" - Picked by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in TenManGlobal.mapsRemaining) rm += mapname + "\n";
                    msg = await (Context.Channel.GetMessageAsync(TenManGlobal.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) TenManGlobal.IsBanPhase = true;
                    TenManGlobal.TeamATurn = !TenManGlobal.TeamATurn;
                    break;
                case "banmap":
                    if (!(caller.Roles.Contains(master) || caller.Roles.Contains(capA) || caller.Roles.Contains(capB))) {
                        await Context.Channel.SendMessageAsync("You do not have permissions to issue commands.");
                        return;
                    }

                    if(TenManGlobal.mapsRemaining.Count() <= 1) {
                        await Context.Channel.SendMessageAsync("Map pick/ban phase is over.\nYou can start the game now.");
                        return;
                    }

                    if (TenManGlobal.playersRemaining.Count() != 0) {
                        await Context.Channel.SendMessageAsync("It is currently still the player pick stage.\nPlease wait for all players to be picked before moving on to the map pick/ban phase");
                        return;
                    }

                    if (!TenManGlobal.IsBanPhase) {
                        await Context.Channel.SendMessageAsync("It is currently the pick phase.\nPlease wait for both teams to pick their maps before banning a map.");
                        return;
                    }

                    if(a && !TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team B's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    } else if (!a && TenManGlobal.TeamATurn) {
                        await Context.Channel.SendMessageAsync("It is currently Team A's turn to ban a map.\nPlease wait your turn to ban a map.");
                        return;
                    }

                    string mapban = tokens[1].ToLower();
                    if (!TenManGlobal.mapThumbnails.ContainsKey(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " does not exist in the competitive pool.");
                        return;
                    }

                    if(!TenManGlobal.mapsRemaining.Contains(mapban)) {
                        await Context.Channel.SendMessageAsync(mapban + " has already been selected.\nPlease select another map.");
                        return;
                    }

                    embed = EmbedHelper((a ? "Team A Banned: " : "Team B Banned: "), mapban, TenManGlobal.mapThumbnails[mapban], new Color(255, 0, 0));;
                    await Context.Channel.SendMessageAsync("", false, embed);

                    TenManGlobal.mapsRemaining.Remove(mapban);

                    RestUserMessage mapBanMessage = await (Context.Channel.GetMessageAsync(TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                    await mapBanMessage.ModifyAsync(x => x.Content = "```" + mapBanMessage.Content.ToString().Trim(new char[] { '`' }) + "\n" + mapban + " - Banned by " + (a ? "Team A" : "Team B") + "```");

                    rm = "";
                    foreach (string mapname in TenManGlobal.mapsRemaining) rm += mapname + "\n";

                    msg = await (Context.Channel.GetMessageAsync(TenManGlobal.RemainingMapsMessageID)) as RestUserMessage;
                    await msg.ModifyAsync(x => x.Content = "```css\nRemaining Maps:\n" + rm + "```");

                    if (!a) TenManGlobal.IsBanPhase = false;
                    TenManGlobal.TeamATurn = !TenManGlobal.TeamATurn;

                    if(TenManGlobal.mapsRemaining.Count == 1) {
                        string name = TenManGlobal.mapsRemaining.First();
                        embed = EmbedHelper("Decider: ", name, TenManGlobal.mapThumbnails[name], new Color(0, 255, 0));
                        await Context.Channel.SendMessageAsync("", false, embed);
                        RestUserMessage decider = (await Context.Channel.GetMessageAsync(TenManGlobal.MapPickMessaegID)) as RestUserMessage;
                        string greeterString = string.Empty;
                        var currentDate = DateTime.Now;
                        greeterString += "https://popflash.site/scrim/TenMan" + currentDate.ToString("yyyyMMddTHH:mm:ssZ");
                        await decider.ModifyAsync(x => x.Content = "```" + decider.Content.ToString().Trim(new char[] { '`' }) + "\n" + name + " - Decider Map ```");
                        await Context.Channel.SendMessageAsync($"Map pick/ban phase is completed.\nGet ready to start the game. Join the server here:\n{greeterString}");
                    }
                    break;
                case "endgame":
                    foreach(SocketGuildUser u in TenManGlobal.participants) {
                        await u.ModifyAsync(x => {
                            x.ChannelId = mainChannel.Id;
                        });
                    }
                    string greeterMessage = "Game has been finished. Everyone will be moved back to the main channel.";
                    bool includeNextPopflashLink = (tokens.Length >= 2 && tokens[1] == "-y") ? true : false;
                    if (includeNextPopflashLink) {
                        var date = DateTime.Now;
                        greeterMessage += "When everyone is ready to start the next game, join your channels and follow this link:\n"
                            + "https://popflash.site/scrim/TenMan" + date.ToString("yyyyMMddTHH:mm:ssZ");
                    }
                    await Context.Channel.SendMessageAsync(greeterMessage);
                    break;
                default:
                    await Context.Channel.SendMessageAsync("Command is not recognized.");
                    break;
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
