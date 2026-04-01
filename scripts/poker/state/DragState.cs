using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.events.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker.state;

[ContextAware]
public partial class DragState : Node, IPokerState
{
    private IPoker Poker { get; set; } = null!;
    
    private Vector2 _lastMousePosition;
    private float _targetRotationRad;

    public void SetPoker(IPoker poker)
    {
        Poker = poker;
    }
    
    public void Process(double delta)
    {
        Poker.SetPos(Poker.GetGlobalMousePosition() - Poker.GetSize() / 2);
        
        Vector2 currentMousePosition =  Poker.GetGlobalMousePosition();
        Vector2 gap = currentMousePosition - _lastMousePosition;
        _lastMousePosition = currentMousePosition;
        _targetRotationRad = Mathf.DegToRad(Mathf.Clamp(gap.X * 15f, -30f, 30f));
        Poker.SetRot(Mathf.LerpAngle(Poker.GetRotation(), _targetRotationRad, 10f * (float)delta));
    }

    public void Enter()
    {
        // 隐藏并锁定鼠标在窗口范围内
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
    }

    public void Exit()
    {
        // 显示鼠标
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
    }

    public void MouseDown()
    {
        
    }

    public void MouseUp()
    {
        this.SendEvent(new StateChangedEvent()
        {
            CurrentState = StateType.Drag,
            NextState = StateType.Idle
        });
    }

    public void MouseEnter()
    {
        
    }
    
    public void MouseExit()
    {
        
    }
}