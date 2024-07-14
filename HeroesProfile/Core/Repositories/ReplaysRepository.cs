using System.Text.Json;
using System.Text.Json.Serialization;
using HeroesProfile.UI.Core.Models;

namespace HeroesProfile.UI.Core.Repositories;

public class ReplaysRepository(AppSettings appSettings)
{
    private readonly JsonSerializerOptions writeOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
    };

    private readonly JsonSerializerOptions readOptions = new()
    {
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
    };

    private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

    public async Task InitAsync(CancellationToken token)
    {
        try
        {
            await semaphore.WaitAsync(token);

            if (!File.Exists(appSettings.StoredReplaysPath))
            {
                await File.WriteAllTextAsync(appSettings.StoredReplaysPath, JsonSerializer.Serialize(Array.Empty<StoredReplay>(), writeOptions), token);
            }
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task ClearAsync(CancellationToken token)
    {
        try
        {
            await semaphore.WaitAsync(token);
            await File.WriteAllTextAsync(appSettings.StoredReplaysPath, JsonSerializer.Serialize(Array.Empty<StoredReplay>(), writeOptions), token);
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<StoredReplay> FindAsync(string path, CancellationToken token)
    {
        List<StoredReplay> store = await LoadAsync(token);
        return store.Find(replay => string.Equals(replay.Path, path, StringComparison.OrdinalIgnoreCase))!;
    }

    // public async Task InsertAsync(StoredReplay replay, CancellationToken ct)
    // {
    //     List<StoredReplay> store = await LoadAsync(ct);
    //     await SaveAsync(store.Prepend(replay).ToList(), ct);
    // }

    public async Task<List<StoredReplay>> UpdateAsync(List<StoredReplay> replays, CancellationToken ct)
    {
        List<StoredReplay> store = await LoadAsync(ct);

        foreach (StoredReplay replay in replays)
        {
            store.RemoveAll(stored => stored.Path.Equals(replay.Path));
            store.Insert(0, replay);
        }

        await SaveAsync(store, ct);

        return replays;
    }

    // public async Task InsertAsync(List<StoredReplay> replays, CancellationToken ct)
    // {
    //     List<StoredReplay> current = await LoadAsync(ct);
    //     List<StoredReplay> replacement = replays.Concat(current).Distinct().ToList();
    //     await SaveAsync(replacement, ct);
    // }

    private async Task SaveAsync(List<StoredReplay> replays, CancellationToken ct)
    {
        replays.Sort((x, y) => x.Created.CompareTo(y.Created));
        await File.WriteAllTextAsync(appSettings.StoredReplaysPath, JsonSerializer.Serialize(replays, writeOptions), ct);
    }

    public async Task<List<StoredReplay>> LoadAsync(CancellationToken ct)
    {
        string json = await File.ReadAllTextAsync(appSettings.StoredReplaysPath, ct);            
        return JsonSerializer.Deserialize<List<StoredReplay>>(json, readOptions) ?? [];
    }
}