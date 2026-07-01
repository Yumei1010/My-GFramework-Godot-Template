using GFrameworkTemplate.scripts.core.story;

namespace GFrameworkTemplate.scripts.cqrs.vn.@event;

/// <summary>
///     开始加载故事脚本
/// </summary>
public sealed class VnStoryLoadEvent
{
    /// <summary>脚本文件的逻辑名或路径</summary>
    public required string FilePath { get; init; }
}

/// <summary>
///     故事脚本加载完成
/// </summary>
public sealed class VnStoryLoadedEvent
{
    /// <summary>已加载的命令总数</summary>
    public required int CommandCount { get; init; }
}

/// <summary>
///     故事播放结束（脚本命令全部执行完毕）
/// </summary>
public sealed class VnStoryFinishedEvent;

/// <summary>
///     对话命令触发——显示对话文本
/// </summary>
public sealed class VnTalkTriggeredEvent
{
    /// <summary>说话人（null 表示旁白）</summary>
    public string? Talker { get; init; }

    /// <summary>对话内容</summary>
    public required string Content { get; init; }

    /// <summary>是否居中显示</summary>
    public bool IsCenter { get; init; }

    /// <summary>头像逻辑名</summary>
    public string? AvatarPath { get; init; }
}

/// <summary>
///     背景切换触发
/// </summary>
public sealed class VnBackgroundTriggeredEvent
{
    /// <summary>背景纹理逻辑名</summary>
    public required string FilePath { get; init; }

    /// <summary>是否等待 Tween 完成</summary>
    public bool WaitTweenEnd { get; init; }

    /// <summary>延迟时间（秒）</summary>
    public float Delay { get; init; }
}

/// <summary>
///     立绘操作触发
/// </summary>
public sealed class VnTachieTriggeredEvent
{
    /// <summary>立绘槽位配置（角色名 → 操作）</summary>
    public required Dictionary<string, TachieSlot> Tachies { get; init; }
}

/// <summary>
///     音频播放触发
/// </summary>
public sealed class VnSoundTriggeredEvent
{
    /// <summary>音频类型</summary>
    public required string SoundType { get; init; }

    /// <summary>音频文件逻辑名</summary>
    public required string FilePath { get; init; }
}

/// <summary>
///     分支选项触发——等待玩家选择
/// </summary>
public sealed class VnBranchTriggeredEvent
{
    /// <summary>选项映射（ID → 选项）</summary>
    public required Dictionary<string, BranchOption> Options { get; init; }
}

/// <summary>
///     玩家选中了某个分支选项
/// </summary>
public sealed class VnBranchChosenEvent
{
    /// <summary>被选中的选项 ID</summary>
    public required string OptionId { get; init; }
}

/// <summary>
///     跳转到另一个脚本
/// </summary>
public sealed class VnGotoTriggeredEvent
{
    /// <summary>目标脚本文件路径</summary>
    public required string TargetFilePath { get; init; }
}

/// <summary>
///     自定义事件触发——章节特定逻辑
/// </summary>
public sealed class VnCustomEventTriggeredEvent
{
    /// <summary>事件名称</summary>
    public required string EventName { get; init; }
}

/// <summary>
///     文本显示进度（打字机效果中）
/// </summary>
public sealed class VnTextRevealProgressEvent
{
    /// <summary>已显示的字符数</summary>
    public required int RevealedChars { get; init; }

    /// <summary>总字符数</summary>
    public required int TotalChars { get; init; }
}

/// <summary>
///     推进到下一个命令（玩家点击或自动播放）
/// </summary>
public sealed class VnAdvanceRequestedEvent;
