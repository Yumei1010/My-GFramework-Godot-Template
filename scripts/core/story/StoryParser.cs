using System.Text.Json;
using GFrameworkTemplate.scripts.cqrs.visualnovel.command;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     故事脚本解析器——将 JSON 文本转换为 StoryCommand 数组
///     每个命令类型自行解析自身字段（FromJson），Parser 仅负责 type 分发
/// </summary>
public static class StoryParser
{
    /// <summary>从 JSON 字符串解析故事脚本</summary>
    public static StoryScript ParseStory(string json)
    {
        var commands = new List<StoryCommand>();
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty("content", out var contentArray))
            return new StoryScript { Content = commands };

        foreach (var element in contentArray.EnumerateArray())
        {
            var cmd = ParseCommand(element);
            if (cmd != null)
                commands.Add(cmd);
        }

        return new StoryScript { Content = commands };
    }

    /// <summary>根据 type 字段分发到各命令的 FromJson 工厂</summary>
    public static StoryCommand? ParseCommand(JsonElement element)
    {
        var type = element.TryGetProperty("type", out var t) ? t.GetString() : null;

        return type switch
        {
            "talk" => TalkCommand.FromJson(element),
            "background" => BackgroundCommand.FromJson(element),
            "tachie" => TachieCommand.FromJson(element),
            "sound" => SoundCommand.FromJson(element),
            "branch" => BranchCommand.FromJson(element),
            "goto" => GotoCommand.FromJson(element),
            "event" => EventCommand.FromJson(element),
            _ => null
        };
    }
}
