using Godot;

namespace TimeToTwentyfour.scripts.cqrs.color.@event;

/// <summary>
///     色彩方案变更事件，由时间阶段切换时发布，携带当前阶段名称与三种色值。
/// </summary>
public sealed class ColorSchemeChangedEvent
{
    /// <summary>阶段中文名 <see cref="string"/></summary>
    public required string PhaseName { get; init; }

    /// <summary>全局底色 <see cref="Color"/></summary>
    public required Color BackgroundColor { get; init; }

    /// <summary>时间数字颜色 <see cref="Color"/></summary>
    public required Color TimeNumberColor { get; init; }

    /// <summary>主文本/线条颜色 <see cref="Color"/></summary>
    public required Color LineColor { get; init; }
}
