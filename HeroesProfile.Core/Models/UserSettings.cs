namespace HeroesProfile.Core.Models;

/*
* Change the default UserSettings in appsettings.Development.json or appsettings.Production.json
*/
public class UserSettings
{
    public bool EnablePostMatch { get; set; }
    public bool EnablePreMatch { get; set; }

    public UserSettings()
    {

    }
}