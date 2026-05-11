using GFramework.Core.Abstractions.utility;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.utility;

/// <summary>
///     Guid ↔ <see cref="IPokerView"/> 查找表契约，供 CQRS 事件层通过 Id 查找视图节点。
/// </summary>
public interface IPokerViewRegistry : IUtility
{
    /// <summary>注册扑克视图节点到查找表。</summary>
    void Register(Guid id, IPokerView poker);

    /// <summary>从查找表移除扑克视图节点。</summary>
    void Unregister(Guid id);

    /// <summary>按 Id 查找扑克视图节点，未找到时返回 <see langword="null"/>。</summary>
    IPokerView? Find(Guid id);
}
