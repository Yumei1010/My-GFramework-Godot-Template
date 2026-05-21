using GFramework.Game.Abstractions.data;

namespace GFrameworkTemplate.scripts.data.setting;

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
