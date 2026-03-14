using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.poker;

namespace GFrameworkGodotTemplate.scripts.pile;

[ContextAware]
[Log]
public partial class DrawPile : PileSystem
{
    public override Poker Draw()
    {
        return GetRandomPoker();
    }

    public override Poker Get(string key)
    {
        return base.Get(key);
    }

    public override void Put(Poker poker)
    {
        Pile.Add(GenerateUniqueKey(poker), poker);
    }

    public override void Add(Poker poker)
    {
        Pile.Add(GenerateUniqueKey(poker), poker);
    }

    public override void Remove(string key)
    {
        
    }

    public override void Shuffle()
    {
        _log.Info("Shuffled");
    }

    public override void Clear()
    {
        Pile.Clear();
    }
}