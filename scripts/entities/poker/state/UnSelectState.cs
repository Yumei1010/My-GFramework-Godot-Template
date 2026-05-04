using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker.state;

/// <summary>
///     扑克取消选中状态，从选中状态回退到空闲前的过渡状态。
/// </summary>
[ContextAware]
public partial class UnSelectState : PokerState
{
    public override void GuiInput(InputEvent inputEvent)
    {
        
    }
    
    public override void Process(double delta)
    {
        
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {

    }

    public override void MouseDown()
    {
        // 发送扑克选择器选择发生改变事件
        this.SendEvent(new SelectorSelectChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
        
        ChangeTo(StateType.OnSelect);
    }

    public override void MouseUp()
    {
        
    }

    public override void MouseEnter()
    {
        // 发送扑克选择器预览发生改变事件
        this.SendEvent(new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = true
        });
    }
    
    public override void MouseExit()
    {
        // 发送扑克选择器预览发生改变事件
        this.SendEvent(new PokerSelectorReservesChangedEvent
        {
            Poker = Poker,
            IsSelected = false
        });
    }
}