namespace TimeToTwentyfour.scripts.entities.timeBar;

public interface ITimeBar
{
    /// <summary>
    /// 开始倒计时（若已有倒计时，会重新开始）
    /// </summary>
    /// <param name="duration">持续时间 <see cref="float"/></param>
    void Start(float duration);
    
    /// <summary>
    /// 暂停倒计时
    /// </summary>
    void Pause();
    
    /// <summary>
    /// 恢复倒计时
    /// </summary>
    void Resume();
    
    /// <summary>
    /// 停止倒计时（重置状态）
    /// </summary>
    void Stop();
}