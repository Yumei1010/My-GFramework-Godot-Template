using GFrameworkGodotTemplate.scripts.enums.calculate;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克选择器模式抽象基类
/// </summary>
public abstract partial class PokerSelectorMode : Node, IPokerSelectorMode
{
    /// <summary>
    ///     扑克选择器模式标识符 <see cref="Mode"/>
    /// </summary>
    [Export] protected ModeType Mode { get; set; }
    
    /// <summary>
    ///     该模式计算所需的扑克数量 <see cref="int"/>
    /// </summary>
    [Export] protected int Capacity { get; set; }
    
    /// <summary>
    ///     参与计算的扑克 <see cref="IPoker"/> 实例列表
    /// </summary>
    protected IList<IPoker> Pokers { get; private set; } = new List<IPoker>();
    
    public abstract void Calculate();
    
    public abstract string GetReserveResult();
    
    public void SetPokers(IList<IPoker> pokers)
    {
        Pokers = pokers;
    }

    public ModeType GetModeType()
    {
        return Mode;
    }

    public int GetCapacity()
    {
        return Capacity;
    }

    public IList<IPoker> GetPokers()
    {
        return Pokers;
    }
}