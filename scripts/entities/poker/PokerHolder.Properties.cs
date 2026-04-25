namespace TimeToTwentyfour.scripts.entities.poker;

public partial class PokerHolder
{
    /// <summary>
    /// 所持有的扑克对象 <see cref="IPoker"/> 
    /// </summary>
    private IPoker Poker { get; set; } = null!;
}