using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.selector;

/// <summary>
///     选择器实现 —— 队列入队（FIFO 淘汰）+ 栈式弹出（LIFO 提取）。
/// </summary>
/// <remarks>
///     <para>
///         <b>入队（Add）</b>：新牌从队尾加入；若 <see cref="Capacity"/> &gt; 0
///         且数量超限，队首（最早选中）被静默淘汰。
///     </para>
///     <para>
///         <b>弹出（Pop）</b>：移除并返回队尾（最新选中），即"撤销最近一次选择"。
///     </para>
///     <para>
///         内部存储为 <see cref="List{T}"/>，对于扑克牌这种小集合（通常 ≤ 10）足够高效。
///     </para>
/// </remarks>
[Log]
[ContextAware]
public partial class Selector : Node, ISelector
{
    public override void _Ready()
    {
        RegisterEvent();
    }
    
    public bool IsSelected(IPoker poker)
    {
        return _selected.Contains(poker);
    }
    
    public IPoker Pop()
    {
        if (_selected.Count == 0) return null!;

        var lastIndex = _selected.Count - 1;
        var poker = _selected[lastIndex];
        _selected.RemoveAt(lastIndex);
        return poker;
    }
    
    private void Add(IPoker poker)
    {
        // 去重
        if (_selected.Contains(poker)) return;

        // 若已达上限，淘汰队首（最早选中的）
        if (Capacity > 0 && _selected.Count >= Capacity)
        {
            _selected[0].ChangeTo(StateType.Idle);
            _selected.RemoveAt(0);
        }
        
        _selected.Add(poker);
    }
    
    private void Remove(IPoker poker)
    {
        _selected.Remove(poker);
    }
}