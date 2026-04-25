using GFramework.Core.Abstractions.controller;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

[Log]
[ContextAware]
public partial class Deck : Control, IDeck, IController
{
    public override void _Ready()
    {
        _ = ReadyAsync();
    }
    
    public void Add(IPokerHolder pokerHolder)
    {
        // 
        if (Holders.Contains(pokerHolder) && HolderContainer.GetChildren().Contains(pokerHolder as Node)) return;
        
        Holders.Add(pokerHolder);
        HolderContainer.AddChild(pokerHolder as Node);
    }
    
    public void Remove(IPokerHolder pokerHolder)
    {
        // 
        if (!Holders.Contains(pokerHolder) && !HolderContainer.GetChildren().Contains(pokerHolder as Node)) return;
        
        Holders.Remove(pokerHolder);
        HolderContainer.RemoveChild(pokerHolder as Node);
    }
}