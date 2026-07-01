using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     背景命令——切换场景背景
/// </summary>
public sealed class BackgroundCommand : StoryCommand
{
    public bool WaitTweenEnd { get; set; }
    public float Delay { get; set; }

    /// <summary>从 JSON 元素构造 BackgroundCommand</summary>
    public static BackgroundCommand FromJson(JsonElement element)
    {
        var cmd = new BackgroundCommand
        {
            WaitTweenEnd = GetString(element, "wait_tween_end") == "1",
            Delay = GetFloat(element, "delay") ?? 0f
        };
        cmd.FillCommon(element);
        return cmd;
    }
}
