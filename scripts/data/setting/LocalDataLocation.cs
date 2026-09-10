using GFramework.Game.Abstractions.Data;
using GFramework.Game.Abstractions.Enums;

namespace GFrameworkTemplate.scripts.data.setting;

/// <summary>
///     本地（user://）数据位置描述，供数据仓库做 location → key 映射。
/// </summary>
public sealed record LocalDataLocation : IDataLocation
{
    /// <summary>获取数据键。</summary>
    public string Key { get; init; } = "local";

    /// <summary>获取存储类别（本地/用户等）。</summary>
    public StorageKinds Kinds { get; init; } = StorageKinds.Local;

    /// <summary>获取可选的命名空间（用于分区隔离）。</summary>
    public string? Namespace { get; init; } = "";

    /// <summary>获取可选的附加元数据。</summary>
    public IReadOnlyDictionary<string, string>? Metadata { get; init; } = null;
}
