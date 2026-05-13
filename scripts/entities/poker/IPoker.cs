namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克牌复合契约，组合 <see cref="IPokerData"/>（纯数据）与 <see cref="IPokerView"/>（视图操作）。
/// </summary>
public interface IPoker : IPokerData, IPokerView;
