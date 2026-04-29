namespace TimeToTwentyfour.scripts.entities.timeBar;

/// <summary>
///     时间轴契约
///     定义倒计时控制器的完整生命周期操作。
///     实现类负责驱动进度条 UI、更新时间文本，并通过调用 <c>_Process(delta)</c>/> 逐帧扣减。
/// </summary>
/// <remarks>
///     <code>
///         timeBar.Start(90f);          // 开始 90 秒倒计时
///         timeBar.TimeScale = 2.0f;    // 双倍速流逝
///         timeBar.Pause();             // 暂停
///         timeBar.AdjustTime(5f);      // 加 5 秒
///         timeBar.Resume();            // 恢复
///     </code>
/// </remarks>
public interface ITimeBar
{
    /// <summary>
    ///     是否正在倒计时中。
    ///     <c>true</c> 表示剩余时间 &gt; 0 且未暂停。
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    ///     是否暂停中。
    ///     设置 <c>true</c> 等价于调用 <see cref="Pause"/>，
    ///     设置 <c>false</c> 等价于调用 <see cref="Resume"/>。
    /// </summary>
    bool Paused { get; set; }

    /// <summary>
    ///     本次倒计时的总时长（秒）。
    ///     由 <see cref="Start"/> 设定；<see cref="Stop"/> 后归零。
    /// </summary>
    /// <remarks>
    ///     用于计算进度条比例：<c>value = Remaining / TotalDuration * 100</c>。
    ///     当 <see cref="AdjustTime"/> 加时超出原时长时，此值会同步增长。
    /// </remarks>
    float TotalDuration { get; set; }

    /// <summary>
    ///     时间流逝倍率。
    ///     1.0 = 正常速度，2.0 = 双倍速（每实际秒扣 2 秒），
    ///     0.5 = 半速，0 = 冻结（暂停不走时间但保持运行状态）。
    /// </summary>
    /// <remarks>
    ///     设置会钳制到 [0, +∞)，负数会被截断为 0。
    /// </remarks>
    float TimeScale { get; set; }
    
     /// <summary>
    ///     开始倒计时。
    ///     若调用前已有倒计时（运行中或暂停中），会先 <see cref="Stop"/> 重置再以新时长启动。
    /// </summary>
    /// <param name="duration">
    ///     目标倒计时时长（秒） <see cref="float"/>。
    ///     必须 &gt; 0，否则行为未定义。
    /// </param>
    void Start(float duration);

    /// <summary>
    ///     暂停倒计时。
    ///     仅当 <see cref="IsRunning"/> 为 <c>true</c> 时生效；
    ///     若已暂停或已停止，调用无副作用。
    /// </summary>
    /// <remarks>
    ///     暂停后可通过 <see cref="Resume"/> 恢复，剩余时间不变。
    /// </remarks>
    void Pause();

    /// <summary>
    ///     恢复倒计时。
    ///     仅当处于暂停状态时生效；
    ///     若正在运行或已停止，调用无副作用。
    /// </summary>
    void Resume();

    /// <summary>
    ///     停止倒计时并重置为初始状态。
    ///     剩余时间归零、暂停标记清除、<see cref="TotalDuration"/> 归零。
    ///     可安全重复调用。
    /// </summary>
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