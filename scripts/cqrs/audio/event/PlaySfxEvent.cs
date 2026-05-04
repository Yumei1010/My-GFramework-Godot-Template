using TimeToTwentyfour.scripts.enums.audio;

namespace TimeToTwentyfour.scripts.cqrs.audio.@event;

/// <summary>
///     音效播放事件类，用于触发特定类型的音效播放
/// </summary>
public sealed class PlaySfxEvent
{
    /// <summary>
    ///     获取要播放的音效类型
    /// </summary>
    public required SfxType SfxType { get; init; }
}