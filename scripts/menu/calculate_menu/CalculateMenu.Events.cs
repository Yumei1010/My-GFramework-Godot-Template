using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.menu.calculateMenu.@event;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

public partial class CalculateMenu
{
    private void RegisterEvent()
    {
        // 注册对出牌按钮被点击事件的监听
        this.RegisterEvent<CalculateMenuPlayButtonClickedEvent>(_ =>
        {
            OnPlayButtonClickedEvent();
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对模式按钮被触摸事件的监听
        this.RegisterEvent<CalculateMenuModeButtonHoveredEvent>(_ =>
        {
            OnModeButtonHoverEvent();
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnPlayButtonClickedEvent()
    {
        
    }
    
    private void OnModeButtonHoverEvent()
    {
        
    }
    
}