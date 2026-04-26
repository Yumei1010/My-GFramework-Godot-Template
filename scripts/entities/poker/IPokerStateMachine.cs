using Godot;
using TimeToTwentyfour.scripts.component.stateMachine;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克状态机接口，定义了扑克状态机的基本属性和必须实现的功能
/// </summary>
public interface IPokerStateMachine : IStateMachine
{
    /// <summary>
    ///     初始化状态机
    /// </summary>
    /// <param name="poker">要代理的扑克 <see cref="IPoker"/> 实例</param>
    void Init(IPoker poker);

    /// <summary>
    ///     变更到指定状态
    /// </summary>
    /// <param name="state">目标状态 <see cref="StateType"/> </param>
    void ChangeTo(StateType state);
    
    /// <summary>
    ///     获取鼠标输入时调用的方法
    /// </summary>
    /// <param name="inputEvent">鼠标输入 <see cref="InputEvent"/></param>
    void GuiInput(InputEvent inputEvent);
    
    /// <summary>
    ///     鼠标点击卡牌时调用的方法
    /// </summary>
    void MouseDown();
    
    /// <summary>
    ///     鼠标释放卡牌时调用的方法
    /// </summary>
    void MouseUp();
    
    /// <summary>
    ///     鼠标进入卡牌时调用的方法
    /// </summary>
    void MouseEnter();
    
    /// <summary>
    ///     鼠标离开卡牌时调用的方法
    /// </summary>
    void MouseExit();
}