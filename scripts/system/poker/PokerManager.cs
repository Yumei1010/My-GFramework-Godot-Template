using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.scripts.system.poker;

[Log]
[ContextAware]
public partial class PokerManager : Node, ISystem
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/entities/poker/poker.tscn");

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    public IPoker Product(Card card)
    {
        var poker = _pokerScene.Instantiate<IPoker>();
        ((IPokerView)poker).Id = card.Id;
        poker.PokerSuitType = card.PokerSuitType;
        poker.NumValue = card.NumValue;
        poker.PokerNumType = card.PokerNumType;
        return poker;
    }

    private readonly Dictionary<Guid, IPokerView> _map = new();

    public void Register(Guid id, IPokerView poker) => _map[id] = poker;

    public void Unregister(Guid id) => _map.Remove(id);

    public IPokerView? Find(Guid id) => _map.GetValueOrDefault(id);
}
