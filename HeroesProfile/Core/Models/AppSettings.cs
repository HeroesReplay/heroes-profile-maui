namespace HeroesProfile.UI.Core.Models;

public class AppSettings
{
    public Uri HeroesProfileUri { get; } = new("https://www.heroesprofile.com");
    public Uri HeroesProfileApiUri { get; } = new("https://apitest.heroesprofile.com");

    public string GameTempDirectory
    {
        get
        {
            // TODO: ~/Library/Caches/Blizzard/Heroes of the Storm
            // ~/Users/User/AppData/Local/Temp/Heroes of the Storm
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Temp", "Heroes of the Storm");
        }
    }

    public string GameDocumentsDirectory
    {
        get
        {
            if (OperatingSystem.IsMacCatalyst())
            {
                // ~/Users/User/Library/Application Support/Blizzard/Heroes of the Storm
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Blizzard", "Heroes of the Storm");
            }
            else if(OperatingSystem.IsWindows())
            {
                // C:\Users\User\Documents\Heroes of the Storm
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Heroes of the Storm");
            }
            
            throw new PlatformNotSupportedException();
        }
    }

    public string ApplicationDataDirectory
    {
        get
        {
            // ~/Users/User/AppData/Local/Heroes Profile 2
            // ~/Users/User/Library/Application Support/Heroes Profile 2
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Heroes Profile 2");
        }
    }
    
    public string ApplicationSessionDirectory => Path.Combine(ApplicationDataDirectory, "Session");
    public string StoredReplaysPath => Path.Combine(ApplicationDataDirectory, "replays.json");
}