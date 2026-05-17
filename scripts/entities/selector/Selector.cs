using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace TimeToTwentyfour.scripts.entities.selector;

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
    }
}
