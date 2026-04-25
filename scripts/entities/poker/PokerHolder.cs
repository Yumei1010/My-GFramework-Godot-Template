using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

[Log]
[ContextAware]
public partial class PokerHolder : Control, IPokerHolder, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    public bool Contains(IPoker poker)
    {
        return Poker == poker;
    }
    
    public IPoker GetPoker()
    {
        return Poker;
    }
    
    public void SetPoker(IPoker poker)
    {
        if (Poker == poker) return;
        
        RemoveChild(Poker as Node);
        
        Poker =  poker;
        
        AddChild(Poker as Node);
    }
    
    private Vector2 GetPokerPosition()
    {
        return Poker.GetGlobalPosition();
    }
    
    public void SpawnTo(Vector2 position)
    {
        GlobalPosition = position;
    }
}