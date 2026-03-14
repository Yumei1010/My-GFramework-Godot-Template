using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克工厂类，负责根据定义创建扑克实例
/// </summary>
public class PokerFactory
{
    public Poker Product(SuitType suitType, string numValue, NumType numType)
    {
        var poker =  new Poker
        {
            Id = Guid.NewGuid(),
            SuitType = suitType,
            NumValue = numValue,
            NumType = numType
        };
        return poker;
    }
}