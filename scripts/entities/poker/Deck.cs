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
        ConnectSignal();
        RegisterEvent();
    }

    public void Add(IPoker poker)
    {
        IPokerHolder holder = HolderFactory.Product();
        
        holder.Insert(poker);
        
        Album.Add(holder, poker);
        
        HolderContainer.AddChild(holder as Node);
    }

    public IPoker Remove(IPoker poker)
    {
        // 如果卡册中未包含，返回
        if (!Album.ContainsValue(poker)) return null!;
        
        IPokerHolder holder = FindHolderFromPoker(poker);

        if (holder != null!)
        {
            Album.Remove(holder);
            
            HolderContainer.RemoveChild(holder as Node);
            
            return holder.Extract();
        }
        return null!; 
    }

    private void Clear()
    {
        Album.Clear();
    }

    private IPokerHolder FindHolderFromPoker(IPoker poker)
    {
        foreach (var (holder, p) in Album)
        {
            if (EqualityComparer<IPoker>.Default.Equals(p, poker))
                return holder;
        }
        return null!;
    }
}