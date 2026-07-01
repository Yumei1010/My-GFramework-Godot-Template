using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     故事命令抽象基类——对应 JSON 脚本中的一条命令
/// </summary>
public abstract class StoryCommand
{
    public string Type { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public bool HideLabels { get; set; }
    public float? Wait { get; set; }
    public string? FilePath { get; set; }

    /// <summary>从 JsonElement 填充公共字段</summary>
    internal void FillCommon(JsonElement element)
    {
        Type = GetString(element, "type") ?? string.Empty;
        Branch = GetString(element, "branch");
        HideLabels = GetString(element, "hide_labels") == "1";
        Wait = GetFloat(element, "wait");
        FilePath = GetString(element, "file_path");
    }

    protected static string? GetString(JsonElement element, string name) =>
        element.TryGetProperty(name, out var p) && p.ValueKind != JsonValueKind.Null ? p.GetString() : null;

    protected static float? GetFloat(JsonElement element, string name)
    {
        if (!element.TryGetProperty(name, out var p) || p.ValueKind == JsonValueKind.Null)
            return null;
        return p.ValueKind == JsonValueKind.String
            ? float.TryParse(p.GetString(), out var f) ? f : null
            : p.GetSingle();
    }
}
