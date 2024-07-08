using System.Text.Json;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Repositories;

public class UserSettingsRepository
{
    private readonly AppSettings appSettings;

    private readonly SemaphoreSlim semaphore = new(1, 1);

    private JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true, AllowTrailingCommas = true };

    public UserSettingsRepository(AppSettings appSettings)
    {
        this.appSettings = appSettings;
    }

    public async Task SaveAsync(UserSettings settings, CancellationToken token)
    {
        try
        {
            await semaphore.WaitAsync(token);
            await File.WriteAllTextAsync(appSettings.UserSettingsPath, JsonSerializer.Serialize(settings, options), token);
        }
        catch(Exception e)
        {
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<UserSettings> LoadAsync(CancellationToken token)
    {
        await semaphore.WaitAsync(token);

        try
        {            
            return JsonSerializer.Deserialize<UserSettings>(await File.ReadAllTextAsync(appSettings.UserSettingsPath, token), options);
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            semaphore.Release();
        }
    }
}