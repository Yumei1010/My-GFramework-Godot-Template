using TimeToTwentyfour.scripts.enums.audio;

namespace TimeToTwentyfour.scripts.cqrs.audio.@event;

/// <summary>
/// 背景音乐变更事件类
/// 用于表示背景音乐类型发生变化的事件
/// </summary>
public sealed class BgmChangedEvent
{
    /// <summary>
    /// 获取背景音乐类型
    /// </summary>
    public required BgmType BgmType { get; init; }
}