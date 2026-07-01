using Godot;

namespace GFrameworkTemplate.scripts.data.story;

/// <summary>
///     故事资源映射器——管理逻辑名到实际文件路径的映射
///     对应原始项目中的 CommandMapEntry / TextureMapEntry 资源体系
///
///     使用字典维护映射表，后续可扩展为从 .tres 资源文件加载
/// </summary>
public static class StoryResourceMapper
{
    /// <summary>脚本逻辑名 → JSON 文件路径</summary>
    private static readonly Dictionary<string, string> JsonPathMap = new();

    /// <summary>纹理逻辑名 → 纹理资源路径</summary>
    private static readonly Dictionary<string, string> TexturePathMap = new();

    /// <summary>
    ///     注册脚本映射
    /// </summary>
    /// <param name="logicalName">逻辑名（JSON 中 goto 引用的名称）</param>
    /// <param name="jsonPath">实际 JSON 文件路径</param>
    public static void RegisterJson(string logicalName, string jsonPath)
    {
        JsonPathMap[logicalName] = jsonPath;
    }

    /// <summary>
    ///     注册纹理映射
    /// </summary>
    /// <param name="logicalName">逻辑名（JSON 中 background/tachie/avatar 引用的名称）</param>
    /// <param name="texturePath">实际纹理资源路径</param>
    public static void RegisterTexture(string logicalName, string texturePath)
    {
        TexturePathMap[logicalName] = texturePath;
    }

    /// <summary>
    ///     批量注册脚本映射
    /// </summary>
    public static void RegisterJsonBatch(Dictionary<string, string> map)
    {
        foreach (var (name, path) in map)
            JsonPathMap[name] = path;
    }

    /// <summary>
    ///     批量注册纹理映射
    /// </summary>
    public static void RegisterTextureBatch(Dictionary<string, string> map)
    {
        foreach (var (name, path) in map)
            TexturePathMap[name] = path;
    }

    /// <summary>
    ///     解析脚本逻辑名到实际文件路径
    /// </summary>
    public static string? ResolveJsonPath(string logicalName)
    {
        return JsonPathMap.TryGetValue(logicalName, out var path) ? path : null;
    }

    /// <summary>
    ///     解析纹理逻辑名到实际纹理路径
    /// </summary>
    public static string? ResolveTexturePath(string logicalName)
    {
        return TexturePathMap.TryGetValue(logicalName, out var path) ? path : null;
    }

    /// <summary>
    ///     异步加载 JSON 文件内容
    /// </summary>
    public static async Task<string?> LoadJsonAsync(string jsonPath)
    {
        try
        {
            using var file = FileAccess.Open(jsonPath, FileAccess.ModeFlags.Read);
            if (file == null)
                return null;

            return await Task.Run(() => file.GetAsText());
        }
        catch (Exception ex)
        {
            GD.PrintErr($"StoryResourceMapper: 加载 JSON 失败 {jsonPath}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     清空所有映射
    /// </summary>
    public static void Clear()
    {
        JsonPathMap.Clear();
        TexturePathMap.Clear();
    }
}
