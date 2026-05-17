namespace TimeToTwentyfour.scripts.enums.input;

/// <summary>
///     定义输入处理的不同阶段，用于区分游戏逻辑中输入事件的优先级和状态。
/// </summary>
public enum InputPhase
{
    /// <summary>
    ///     全局输入处理阶段，始终最先执行，不受游戏状态影响。
    /// </summary>
    Global,

    /// <summary>
    ///     游戏进行中的输入处理阶段，仅在游戏未暂停时生效。
    /// </summary>
    Gameplay,

    /// <summary>
    ///     暂停状态下的输入处理阶段，仅在游戏暂停时生效。
    /// </summary>
    Paused
}