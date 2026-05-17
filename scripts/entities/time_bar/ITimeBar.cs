namespace TimeToTwentyfour.scripts.entities.time_bar;

/// <summary>
///     时间轴契约
///     定义倒计时控制器的完整生命周期操作。
///     实现类负责驱动进度条 UI、更新时间文本，并通过调用 <c>_Process(delta)</c>/> 逐帧扣减。
/// </summary>
public interface ITimeBar
{
    bool IsRunning { get; }

    bool Paused { get; }

    float TotalDuration { get; }

    float TimeScale { get; }
    
    void Start(float duration);

    void Start();

    void Pause();

    void Resume();

    void Stop();

    /// <summary>
    ///     动态调整剩余时间。
    /// </summary>
    /// <param name="delta">
    ///     调整量（秒）。正数增加剩余时间（"加时"），负数减少剩余时间（"扣时"）。
    /// </param>
    /// <remarks>
    ///     <list type="bullet">
    ///         <item>调整后若剩余时间 ≤ 0，立即触发超时并停止。</item>
    ///         <item>加时导致剩余时间超出 <see cref="TotalDuration"/> 时，
    ///              会同步拉长进度条基准（即进度条可能出现"回弹"效果）。</item>
    ///         <item>已完全停止（剩余 0 且未暂停）时调用无效果。</item>
    ///     </list>
    /// </remarks>
    void AdjustTime(float delta);
}