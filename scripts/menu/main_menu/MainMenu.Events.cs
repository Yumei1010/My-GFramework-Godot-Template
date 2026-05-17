using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.core.state.impls;
using TimeToTwentyfour.scripts.cqrs.game.command;
using TimeToTwentyfour.scripts.cqrs.menu.main_menu.@event;

namespace TimeToTwentyfour.scripts.menu.main_menu;

/// <summary>
///     <see cref="MainMenu"/> 的 CQRS 事件订阅文件。
/// </summary>
public partial class MainMenu
{
    private void RegisterEvent()
    {
        // 注册对开始游戏按钮被点击事件的监听
        this.RegisterEvent<MainMenuStartButtonClickedEvent>(_ =>
        {
            OnStartButtonClickedEvent();
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对设置按钮被点击事件的监听
        this.RegisterEvent<MainMenuSettingButtonClickedEvent>(_ =>
        {
            OnSettingButtonClickedEvent();
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对制作者按钮被点击事件的监听
        this.RegisterEvent<MainMenuCreditsButtonClickedEvent>(_ =>
        {
            OnCreditsButtonClickedEvent();
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对退出游戏按钮被点击事件的监听
        this.RegisterEvent<MainMenuExitButtonClickedEvent>(_ =>
        {
            OnExitButtonClickedEvent();
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnStartButtonClickedEvent()
    {
        _stateMachineSystem.ChangeTo<CalculateMenuState>();
    }
    
    private void OnSettingButtonClickedEvent()
    {
        _stateMachineSystem.ChangeTo<OptionsMenuState>();
    }
    
    private void OnCreditsButtonClickedEvent()
    {
        _stateMachineSystem.ChangeTo<CreditsState>();
    }

    private void OnExitButtonClickedEvent()
    {
        this.SendCommand(new ExitGameCommand());
    }
}