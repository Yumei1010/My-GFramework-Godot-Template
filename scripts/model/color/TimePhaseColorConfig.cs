using Godot;

namespace TimeToTwentyfour.scripts.model.color;

/// <summary>
///     计算页 10 段时间阶段色值配置表，按小时值解析当前阶段。
/// </summary>
public static class TimePhaseColorConfig
{
    private static readonly TimePhaseColor[] Phases =
    [
        new("沉寂", 0, 3, new Color("#1A1F2E"), new Color("#E8D58A"), new Color("#B8C0D0")),
        new("渐醒", 4, 6, new Color("#2A2F3C"), new Color("#F0D870"), new Color("#C0C8D6")),
        new("破晓", 7, 8, new Color("#3A404F"), new Color("#F4DD73"), new Color("#C8D0DC")),
        new("觉醒", 9, 10, new Color("#4C535E"), new Color("#F7E07A"), new Color("#D0D6E0")),
        new("正午", 11, 12, new Color("#D6D4C8"), new Color("#D9832B"), new Color("#3B3834")),
        new("炽烈", 13, 14, new Color("#C9BCA7"), new Color("#C15E2B"), new Color("#3E3832")),
        new("渐乏", 15, 16, new Color("#7E8182"), new Color("#CFA13A"), new Color("#D4D8DD")),
        new("黄昏", 17, 18, new Color("#3A3D4A"), new Color("#D68D3A"), new Color("#CBCFD8")),
        new("入夜", 19, 22, new Color("#1E2230"), new Color("#F2CE5C"), new Color("#BCC4D2")),
        new("午夜", 23, 24, new Color("#1A1E29"), new Color("#F2C94C"), new Color("#C8D0DC"))
    ];

    /// <summary>根据小时值（0–24）解析对应的时间阶段色值。</summary>
    /// <returns>匹配的 <see cref="TimePhaseColor"/>，未匹配时返回首项（沉寂）。</returns>
    public static TimePhaseColor Resolve(int hour)
    {
        foreach (var phase in Phases)
        {
            if (hour >= phase.HourMin && hour <= phase.HourMax)
                return phase;
        }
        return Phases[0];
    }
}
