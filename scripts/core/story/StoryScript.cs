namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     故事脚本容器——JSON 文件的根结构
/// </summary>
public sealed class StoryScript
{
    /// <summary>命令列表</summary>
    public List<StoryCommand> Content { get; set; } = new();
}
