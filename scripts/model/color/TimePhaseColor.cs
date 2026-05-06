using Godot;

namespace TimeToTwentyfour.scripts.model.color;

/// <summary>
///     计算页时间阶段色值记录，包含阶段名称与三种颜色（背景、时间数字、文本/线条）。
/// </summary>
/// <param name="Name">阶段中文名</param>
/// <param name="HourMin">阶段起始小时（含）</param>
/// <param name="HourMax">阶段结束小时（含）</param>
/// <param name="BackgroundColor">全局底色</param>
/// <param name="TimeNumberColor">时间数字颜色</param>
/// <param name="LineColor">主文本/线条颜色</param>
public record TimePhaseColor(
    string Name,
    int HourMin,
    int HourMax,
    Color BackgroundColor,
    Color TimeNumberColor,
    Color LineColor
);
