using Godot;

namespace GFrameworkGodotTemplate.scripts.poker.state;

public partial class UnSelectState : Node, IPokerState
{
    private IPoker Poker { get; set; } = null!;
    
    public void SetPoker(IPoker poker)
    {
        Poker = poker;
    }
    
    public void Process(double delta)
    {
        
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
        
    }
    
    public void MouseDown()
    {
        
    }

    public void MouseUp()
    {
        
    }

    public void MouseEnter()
    {
        
    }

    public void MouseExit()
    {
        
    }
}