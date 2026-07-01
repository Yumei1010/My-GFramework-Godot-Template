namespace GFrameworkTemplate.scripts.core.story;

/// <summary>
///     故事命令基类——对应 JSON 脚本中的一条命令
/// </summary>
public abstract class StoryCommand
{
    public string Type { get; set; } = string.Empty;
    public string? Branch { get; set; }
    public bool HideLabels { get; set; }
    public float? Wait { get; set; }
    public string? FilePath { get; set; }
}

/// <summary>对话命令</summary>
public sealed class TalkCommand : StoryCommand
{
    public string? Talker { get; set; }
    public bool IsCenter { get; set; }
    public string TalkContent { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
}

/// <summary>背景切换命令</summary>
public sealed class BackgroundCommand : StoryCommand
{
    public bool WaitTweenEnd { get; set; }
    public float Delay { get; set; }
}

/// <summary>立绘操作类型</summary>
public enum TachieOperation { Show, Change, Close }

/// <summary>单个立绘槽位配置</summary>
public sealed class TachieSlot
{
    public string FilePath { get; set; } = string.Empty;
    public TachieOperation Type { get; set; } = TachieOperation.Show;
}

/// <summary>立绘命令</summary>
public sealed class TachieCommand : StoryCommand
{
    public Dictionary<string, TachieSlot> Tachies { get; set; } = new();
}

/// <summary>音频命令</summary>
public sealed class SoundCommand : StoryCommand
{
    public string SoundType { get; set; } = "oneSound";
}

/// <summary>分支选项</summary>
public sealed class BranchOption
{
    public string Text { get; set; } = string.Empty;
    public float? Wait { get; set; }
}

/// <summary>分支命令</summary>
public sealed class BranchCommand : StoryCommand
{
    public Dictionary<string, BranchOption> Options { get; set; } = new();
}

/// <summary>跳转命令</summary>
public sealed class GotoCommand : StoryCommand { }

/// <summary>自定义事件命令</summary>
public sealed class EventCommand : StoryCommand
{
    public string EventName { get; set; } = string.Empty;
}

/// <summary>故事脚本容器</summary>
public sealed class StoryScript
{
    public List<StoryCommand> Content { get; set; } = new();
}
