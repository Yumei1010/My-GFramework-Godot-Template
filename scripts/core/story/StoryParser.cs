using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     故事脚本解析器——将 JSON 数据转换为强类型 StoryCommand 对象数组
///     支持原始 yrdk.ymzc JSON 格式，通过类型判别字段分发解析
/// </summary>
public static class StoryParser
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    /// <summary>
    ///     从 JSON 字符串解析故事脚本
    /// </summary>
    /// <param name="json">JSON 文本</param>
    /// <returns>解析后的故事脚本</returns>
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

    /// <summary>
    ///     解析单个命令对象
    /// </summary>
    private static StoryCommand? ParseCommand(JsonElement element)
    {
        if (!element.TryGetProperty("type", out var typeProp))
            return null;

        var type = typeProp.GetString() ?? string.Empty;
        var cmd = CreateCommand(type);
        if (cmd == null)
            return null;

        // 解析公共字段
        cmd.Type = type;
        cmd.Branch = GetStringOrNull(element, "branch");
        cmd.HideLabels = GetStringOrNull(element, "hide_labels") == "1";
        cmd.Wait = GetFloatOrNull(element, "wait");
        cmd.FilePath = GetStringOrNull(element, "file_path");

        // 解析类型特定字段
        PopulateCommand(cmd, element);

        return cmd;
    }

    /// <summary>
    ///     根据类型字符串创建对应的命令实例
    /// </summary>
    private static StoryCommand? CreateCommand(string type) => type switch
    {
        "talk" => new TalkCommand(),
        "background" => new BackgroundCommand(),
        "tachie" => new TachieCommand(),
        "sound" => new SoundCommand(),
        "branch" => new BranchCommand(),
        "goto" => new GotoCommand(),
        "event" => new EventCommand(),
        _ => null
    };

    /// <summary>
    ///     填充命令的类型特定字段
    /// </summary>
    private static void PopulateCommand(StoryCommand cmd, JsonElement element)
    {
        switch (cmd)
        {
            case TalkCommand talk:
                talk.Talker = GetStringOrNull(element, "talker");
                talk.IsCenter = GetStringOrNull(element, "is_center") == "1";
                talk.TalkContent = GetStringOrNull(element, "talk_content") ?? string.Empty;
                talk.AvatarPath = GetStringOrNull(element, "avatar_path");
                break;

            case BackgroundCommand bg:
                bg.WaitTweenEnd = GetStringOrNull(element, "wait_tween_end") == "1";
                bg.Delay = GetFloatOrNull(element, "delay") ?? 0f;
                break;

            case TachieCommand tachie:
                tachie.Tachies = ParseTachies(element);
                break;

            case SoundCommand sound:
                sound.SoundType = GetStringOrNull(element, "sound_type") ?? "oneSound";
                break;

            case BranchCommand branch:
                branch.Options = ParseBranchOptions(element);
                break;

            case EventCommand evt:
                evt.EventName = GetStringOrNull(element, "event_name") ?? string.Empty;
                break;

            // GotoCommand 只有公共字段，无需额外解析
        }
    }

    /// <summary>
    ///     解析立绘配置
    /// </summary>
    private static Dictionary<string, TachieSlot> ParseTachies(JsonElement element)
    {
        var result = new Dictionary<string, TachieSlot>();
        if (!element.TryGetProperty("tachies", out var tachiesProp))
            return result;

        foreach (var tachieEntry in tachiesProp.EnumerateObject())
        {
            var slot = new TachieSlot
            {
                FilePath = GetStringOrNull(tachieEntry.Value, "file_path") ?? string.Empty,
                Type = ParseTachieType(GetStringOrNull(tachieEntry.Value, "type"))
            };
            result[tachieEntry.Name] = slot;
        }

        return result;
    }

    /// <summary>
    ///     解析立绘操作类型
    /// </summary>
    private static TachieOperation ParseTachieType(string? type) => type switch
    {
        "change" => TachieOperation.Change,
        "close" => TachieOperation.Close,
        _ => TachieOperation.Show
    };

    /// <summary>
    ///     解析分支选项
    /// </summary>
    private static Dictionary<string, BranchOption> ParseBranchOptions(JsonElement element)
    {
        var result = new Dictionary<string, BranchOption>();
        if (!element.TryGetProperty("options", out var optionsProp))
            return result;

        foreach (var optionEntry in optionsProp.EnumerateObject())
        {
            var option = new BranchOption
            {
                Text = GetStringOrNull(optionEntry.Value, "text") ?? string.Empty,
                Wait = GetFloatOrNull(optionEntry.Value, "wait")
            };
            result[optionEntry.Name] = option;
        }

        return result;
    }

    private static string? GetStringOrNull(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null
            ? prop.GetString()
            : null;
    }

    private static float? GetFloatOrNull(JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var prop) || prop.ValueKind == JsonValueKind.Null)
            return null;

        return prop.ValueKind == JsonValueKind.String
            ? float.TryParse(prop.GetString(), out var f) ? f : null
            : prop.GetSingle();
    }
}
