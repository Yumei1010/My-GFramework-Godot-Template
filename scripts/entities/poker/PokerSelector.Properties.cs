using GFrameworkGodotTemplate.scripts.enums.poker;

namespace GFrameworkGodotTemplate.scripts.entities.poker;

public partial class PokerSelector
{
    /// <summary>
    ///     先前模式 <see cref="IPokerSelectorMode"/>
    /// </summary>
    private IPokerSelectorMode PreviousMode { get; set; } = null!;
    
    /// <summary>
    ///     当前模式 <see cref="IPokerSelectorMode"/>
    /// </summary>
    private IPokerSelectorMode CurrentMode { get; set; } = null!;
    
    /// <summary>
    ///     模式表 <see cref="IList{IPokerSelectorMode}"/>
    /// </summary>
    private Dictionary<ModeType, IPokerSelectorMode> Modes { get; set; } = new();
    
    /// <summary>
    ///     选择对象 <see cref="IList{IPoker}"/>
    /// </summary>
    private IList<IPoker> Selects { get; set; } = new List<IPoker>();
    
    /// <summary>
    ///     预览对象 <see cref="IList{IPoker}"/>
    /// </summary>
    private IList<IPoker> Reserves { get; set; } = new List<IPoker>();
    
    /// <summary>
    ///     可用性 <see cref="bool"/>
    /// </summary>
    private bool Enable { get; set; }
}