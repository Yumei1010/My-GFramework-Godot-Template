using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;

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
        return  _pokerScene.Instantiate<IPoker>();
    }
}