using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     自定义事件命令——触发章节特定逻辑
/// </summary>
public sealed class EventCommand : StoryCommand
{
    public string EventName { get; set; } = string.Empty;

    /// <summary>从 JSON 元素构造 EventCommand</summary>
    public static EventCommand FromJson(JsonElement element)
    {
        var cmd = new EventCommand
        {
            EventName = GetString(element, "event_name") ?? string.Empty
        };
        cmd.FillCommon(element);
        return cmd;
    }
}
