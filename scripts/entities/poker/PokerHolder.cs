using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.cqrs.poker.@event;

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
    
    public IPoker GetContent()
    {
        return Poker;
    }
    
    public void Insert(IPoker poker)
    {
        // 如果已经包含，返回
        if (Contains(poker)) throw new InvalidOperationException("already have Poker"); 
        
        // 更新扑克
        Poker =  poker;

        // 如果子节点数量不为零，清空子节点
        if (GetChildren().Count != 0)
        {
            foreach (Node child in GetChildren())
            {
                RemoveChild(child);
            }
        }
        
        // 如果扑克已有父节点，更换父节点，否则，直接添加
        if (poker.GetParent() != null!)
        {
            poker.Reparent(this);
        }
        else
        {
            AddChild(Poker as Node);   
        }
        
        this.SendEvent(new PokerHolderInsertedEvent
        {
            Poker = poker
        });
    }
    
    public IPoker Extract()
    {
        // 如果未包含扑克，返回为空
        if (Poker == null!) throw new InvalidOperationException("didn't have Poker");
        
        RemoveChild(Poker as Node);

        IPoker poker = Poker;
        Poker = null!;
        
        this.SendEvent(new PokerHolderExtractedEvent
        {
            Poker = poker
        });
        
        return poker;
    }

    public void Exchange(IPokerHolder holder)
    {
        // 如果目标卡套不包含扑克，返回
        if (holder.GetContent() == null!) throw new ArgumentNullException(nameof(holder), "didn't have Poker");
        
        // 如果未包含扑克，返回为空
        if (Poker == null!) throw new InvalidOperationException("didn't have Poker");
        
        IPoker poker = holder.GetContent();
        holder.Clear();
        
        RemoveChild(Poker as Node);
        
        holder.Insert(Poker);
        
        Poker =  poker;
    }

    public void Neaten()
    {
        Poker.SpawnTo(GlobalPosition);
    }

    public void Clear()
    {
        // 如果内容物为空，返回
        if (Poker == null!) throw new InvalidOperationException("didn't have Poker");
        
        RemoveChild(Poker as Node);
        Poker = null!;
    }
    
    public void SpawnTo(Vector2 position)
    {
        GlobalPosition = position;
    }
    
    private Vector2 GetPokerPosition()
    {
        return Poker.GetGlobalPosition();
    }
}