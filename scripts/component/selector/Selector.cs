using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.component.selector;

/// <summary>
///     选择器实现 —— 委托 <see cref="SelectionList"/> 管理核心 FIFO/LIFO 逻辑，
///     本类仅负责 Godot 生命周期与 CQRS 事件桥接。
/// </summary>
[Log]
[ContextAware]
public partial class Selector : Node, ISelector
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public bool IsSelected(IPoker poker)
    {
        return  _selection.Contains(poker);
    }

    public IPoker Pop()
    {
        if (!_model.Enable) return null!;
        return _selection.Pop()!;
    }
}
