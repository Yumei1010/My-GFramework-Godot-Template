using GFramework.Game.Abstractions.data;
using GFramework.Game.Abstractions.enums;

namespace TimeToTwentyfour.scripts.data;

/// <summary>
///     表示本地数据位置的实现类，实现了IDataLocation接口
/// </summary>
public sealed record LocalDataLocation : IDataLocation
{
    /// <summary>
    ///     获取数据位置的键值，固定为"local"
    /// </summary>
    public string Key { get; init; } = "local";

    /// <summary>
    ///     获取存储类型，固定为StorageKind.Local
    /// </summary>
    public StorageKinds Kinds { get; init; } = StorageKinds.Local;

    /// <summary>
    ///     获取命名空间，固定为空字符串
    /// </summary>
    public string? Namespace { get; init; } = "";

    /// <summary>
    ///     获取元数据字典，固定为null
    /// </summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; } = null;
}