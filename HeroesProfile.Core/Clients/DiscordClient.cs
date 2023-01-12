using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Heroes.ReplayParser;
using HeroesProfile.Core.Models;

using Microsoft.Extensions.Logging;

using NetDiscordRpc;
using NetDiscordRpc.RPC;

namespace HeroesProfile.Core.Clients;

/*
 * Probably only works on Windows since the DiscordRPC uses NamedPipes which is a Windows specific service?
 */
public class DiscordClient : IDisposable
{
    private readonly AppSettings appSettings;
    private readonly ILogger<DiscordClient> logger;
    private readonly DiscordRPC discord;

    public DiscordClient(AppSettings appSettings, ILogger<DiscordClient> logger)
    {
        this.appSettings = appSettings;
        this.logger = logger;
        discord = new DiscordRPC(appSettings.DiscordApplicationId, pipe: -1, autoEvents: true, client: null);

    }

    private void TryInit()
    {
        if (!discord.IsInitialized)
            discord.Initialize();
    }

    public void ClearActivity()
    {
        try
        {
            TryInit();
            discord.ClearPresence();
            discord.Invoke();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
    }

    public void UpdatePresence(SessionData sessionData, UserSettings settings, IEnumerable<int> battleNetIds)
    {
        try
        {
            TryInit();

            if (sessionData.State == SessionState.BattleLobby)
            {
                BattleLobby(sessionData, settings, battleNetIds);
            }
            else if (sessionData.State == SessionState.StormSave)
            {
                StormSave(sessionData, settings, battleNetIds);
            }

            discord.Invoke();
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    private void BattleLobby(SessionData sessionData, UserSettings settings, IEnumerable<int> battleNetIds)
    {
        try
        {
            var presence = new RichPresence
            {
                State = $"In Lobby",
                Assets = new Assets
                {
                    LargeImageKey = "game",
                    LargeImageText = "Heroes of the Storm"
                },
                Timestamps = new Timestamps(sessionData.Files.BattleLobby.Created, end: null)
            };

            discord.SetPresence(presence);
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }

    private void StormSave(SessionData sessionData, UserSettings settings, IEnumerable<int> battleNetIds)
    {
        try
        {
            Player? player = sessionData.Players.FirstOrDefault(p => battleNetIds.Contains(p.BattleNetId));

            if (player is not null)
            {
                //int partySize = sessionData.Players.Count(x => x.PartyValue == player.PartyValue && x.Team == player.Team);
                //int partyMax = 5;

                discord.UpdateLargeAsset("abathur", player.HeroAttributeId);
                discord.UpdateState("In Game");
                discord.UpdateDetails($"{sessionData.Map} ({sessionData.GameMode})");

                if (settings.EnableDiscordPreMatch && sessionData.PreMatchUri != null)
                {
                    discord.UpdateButtons(new Button[]
                    {
                        new Button { Label = "Pre Match Details", Url = sessionData.PreMatchUri?.ToString() }
                    });
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, e.Message);
        }
    }


    public void Dispose()
    {
        discord.Dispose();
    }
}
