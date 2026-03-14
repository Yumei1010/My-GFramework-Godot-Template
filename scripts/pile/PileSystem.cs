using GFramework.Core.extensions;
using GFramework.Core.system;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.pile;

public class PileSystem : AbstractSystem , IPileSystem
{
    protected Godot.Collections.Dictionary<string, Poker> Pile = new();
    protected Godot.Collections.Dictionary<string, int> IndexMap = new();
    protected Random Random = new Random();
    protected PokerPoolSystem? Pool;
    
    protected override void OnInit()
    {
        Pool = this.GetSystem<PokerPoolSystem>();
    }
    
    protected Poker GetRandomPoker()
    {
        if (Pile.Count == 0) return null!;
        
        var keys = Pile.Keys.ToList();
        var randomIndex = Random.Next(keys.Count);
        return Pile[keys[randomIndex]];
    }

    protected string GenerateUniqueKey (Poker poker)
    {
        var key = $"{poker.SuitType}_{poker.NumValue}";
        var index = 0;
        // 循环查找可用的索引（从0开始递增，直到找到一个不存在的键）
        while (Pile.ContainsKey($"{key}_{index}"))
        {
            index++;
        }
        return $"{key}_{index}";
    }

    public virtual Poker Draw()
    {
        return null!;
    }

    public virtual Poker Get(string key)
    {
        return null!;
    }

    public virtual void Put(Poker poker)
    {
        Pile.Add(poker.Name, poker);
    }

    public virtual void Add(Poker poker)
    {
        Pile.Add(poker.Name, poker);
    }

    public virtual void Remove(string key)
    {
        
    }

    public virtual void Shuffle()
    {
        
    }

    public virtual void Clear()
    {
        Pile.Clear();
    }
}