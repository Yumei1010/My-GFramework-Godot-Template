using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.model.pile;

namespace TimeToTwentyfour.global;

/// <summary>
///     全局扑克场景注册表，统一管理扑克视图实例的创建与 Id 查找。
/// </summary>
[Log]
[ContextAware]
public partial class PokerSceneRegistry : Node
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/component/poker_view/poker_view.tscn");

    private readonly Dictionary<Guid, IPokerView> _map = new();

    public IPoker Product()
    {
        return _pokerScene.Instantiate<IPoker>();
    }

    public IPoker Product(SuitType suitType, string numValue, NumType numType = NumType.Integer)
    {
        var poker = Product();
        poker.SuitType = suitType;
        poker.NumValue = numValue;
        poker.NumType = numType;
        return poker;
    }

    public IPoker Product(Card card)
    {
        var poker = Product();
        ((IPokerView)poker).Id = card.Id;
        poker.SuitType = card.SuitType;
        poker.NumValue = card.NumValue;
        poker.NumType = card.NumType;
        return poker;
    }

    public void Register(Guid id, IPokerView poker) => _map[id] = poker;

    public void Unregister(Guid id) => _map.Remove(id);

    public IPokerView? Find(Guid id) => _map.GetValueOrDefault(id);
}
