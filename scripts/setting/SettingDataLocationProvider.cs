using GFramework.Game.Abstractions.data;
using TimeToTwentyfour.scripts.data.model;

namespace TimeToTwentyfour.scripts.setting;

public class SettingDataLocationProvider : IDataLocationProvider
{
    public IDataLocation GetLocation(Type type)
    {
        return new LocalDataLocation
        {
            Key = type.Name,
            Namespace = "settings"
        };
    }
}