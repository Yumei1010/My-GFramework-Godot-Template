using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.component.selector;

/// <summary>
///     选择器实现 —— 委托 <see cref="SelectionList"/> 管理核心 FIFO/LIFO 逻辑，
///     本类仅负责 Godot 生命周期与 CQRS 事件桥接。
/// </summary>
[Log]
[ContextAware]
public partial class Selector : Node, ISelector
{
    private SelectorModel Model => this.GetModel<SelectorModel>();

    public override void _Ready()
    {
        _selection.Evicted += poker =>
            this.SendEvent(new SelectorSelectChangedEvent { IsSelected = false, Poker = poker });
        RegisterEvent();
    }

    /// <summary>检查指定牌是否已被选中。</summary>
    public bool IsSelected(IPoker poker) => _selection.Contains(poker);

    /// <summary>LIFO 弹出最近选中的牌；选择器未启用或无选中项时返回 null。</summary>
    public IPoker Pop()
    {
        if (!Model.Enable) return null!;
        return _selection.Pop()!;
    }
}
