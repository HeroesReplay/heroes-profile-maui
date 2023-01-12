using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HeroesProfile.Core.Models;

/*
* Change the default UserSettings in appsettings.Development.json or appsettings.Production.json
*/
public class UserSettings
{
    public bool EnablePostMatch { get; set; }
    public bool EnablePreMatch { get; set; }
    public bool EnablePredictions { get; set; }
    public bool EnableTalentsExtension { get; set; }
    public bool EnableDiscordEnhancement { get; set; }
    public bool EnableDiscordPreMatch { get; set; }
    public string BroadcasterId { get; set; }
    public string HeroesProfileTwitchKey { get; set; }
    public string HeroesProfileApiEmail { get; set; }
    public string HeroesProfileUserId { get; set; }


    [JsonIgnore]
    public Dictionary<string, string> Identity => new()
    {
        { "hp_twitch_key", HeroesProfileTwitchKey ?? "UNSET" },
        { "email", HeroesProfileApiEmail ?? "UNSET" },
        { "twitch_nickname", BroadcasterId ?? "UNSET" },
        { "user_id", HeroesProfileUserId ?? "UNSET" },
    };

    public UserSettings()
    {

    }
}