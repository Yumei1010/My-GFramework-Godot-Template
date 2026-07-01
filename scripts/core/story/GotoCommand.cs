using System.Text.Json;

namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     跳转命令——跳转到另一个 JSON 脚本
/// </summary>
public sealed class GotoCommand : StoryCommand
{
    /// <summary>从 JSON 元素构造 GotoCommand</summary>
    public static GotoCommand FromJson(JsonElement element)
    {
        var cmd = new GotoCommand();
        cmd.FillCommon(element);
        return cmd;
    }
}
