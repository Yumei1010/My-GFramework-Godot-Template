using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.component.pokerFactory;

/// <summary>
///     扑克工厂类，负责根据定义创建扑克实例
/// </summary>
[ContextAware]
public partial class PokerFactory : Node, IPokerFactory
{
    /// <summary>创建一个新的扑克实体实例。</summary>
    public IPoker Product()
    {
        return _pokerScene.Instantiate<IPoker>();
    }

    /// <summary>创建扑克实体实例并直接设置花色、数值与数值类型。</summary>
    public IPoker Product(SuitType suitType, string numValue, NumType numType = NumType.Integer)
    {
        var poker = Product();
        poker.SuitType = suitType;
        poker.NumValue = numValue;
        poker.NumType = numType;
        return poker;
    }
}