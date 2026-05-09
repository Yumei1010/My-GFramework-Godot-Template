using GFramework.Core.model;
using TimeToTwentyfour.scripts.entities.poker;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.model.pileModel;

/// <summary>
///     牌堆模型基类
///     定义牌堆的基本属性和方法接口
///     提供添加、移除和随机获取牌，以及初始化牌堆的抽象方法。
/// </summary>
public class PileModel : AbstractModel
{
    public IList<Card> Pile { get; set; } = [];

    /// <summary>
    ///     将指定牌加入牌堆。基类不实现具体逻辑，由子类根据需要覆盖。
    /// </summary>
    /// <param name="card"> 要加入的牌  <see cref="Card"/></param>
    public virtual void AddCard(Card card){ }

    /// <summary>
    ///     移除牌堆中的指定牌（如果存在）。基类不实现具体逻辑，由子类根据需要覆盖。
    /// </summary>
    /// <param name="card"> 要移除的牌  <see cref="Card"/></param>
    public virtual void RemoveCard(Card card){ }

    /// <summary>
    ///     从牌堆中随机获取一张牌。基类不实现具体逻辑，由子类根据需要覆盖。
    /// </summary>
    /// <returns> 随机获取的牌 <see cref="Card"/></returns>
    public virtual Card GetRandomCard(){ return null!; }

    /// <summary>
    ///     基类不实现具体初始化逻辑，由子类根据需要覆盖
    /// </summary>
    protected override void OnInit(){ }
}
