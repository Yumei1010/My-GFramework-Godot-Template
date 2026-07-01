using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>分支选项</summary>
public sealed class BranchOption
{
    public string Text { get; set; } = string.Empty;
    public float? Wait { get; set; }
}

/// <summary>
///     分支命令——显示选项并等待玩家选择
/// </summary>
public sealed class BranchCommand : StoryCommand
{
    public Dictionary<string, BranchOption> Options { get; set; } = new();

    /// <summary>从 JSON 元素构造 BranchCommand</summary>
    public static BranchCommand FromJson(JsonElement element)
    {
        var options = new Dictionary<string, BranchOption>();
        if (element.TryGetProperty("options", out var opts))
        {
            foreach (var entry in opts.EnumerateObject())
            {
                options[entry.Name] = new BranchOption
                {
                    Text = GetString(entry.Value, "text") ?? string.Empty,
                    Wait = GetFloat(entry.Value, "wait")
                };
            }
        }

        var cmd = new BranchCommand { Options = options };
        cmd.FillCommon(element);
        return cmd;
    }
}
