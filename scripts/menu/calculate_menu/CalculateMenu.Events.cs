using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using TimeToTwentyfour.scripts.cqrs.calculator.command;
using TimeToTwentyfour.scripts.cqrs.modeButton.@event;
using TimeToTwentyfour.scripts.cqrs.selector.command;
using TimeToTwentyfour.scripts.model.calculator;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.menu.calculate_menu;

/// <summary>
///     <see cref="CalculateMenu"/> 的 CQRS 事件订阅文件。
/// </summary>
public partial class CalculateMenu
{
    private void RegisterEvent()
    {
        this.RegisterEvent<ModeButtonClickedEvent>(e =>
        {
            var selectorModel = this.GetModel<SelectorModel>();
            var calculateModel = this.GetModel<CalculatorModel>();

            if (selectorModel.Enable && calculateModel.Mode == e.ModeType)
            {
                // 再次点击同一模式按钮 → 禁用选择器
                this.SendCommand(new SelectorChangeEnableCommand { Enable = false });
            }
            else
            {
                // 首次点击或切换模式 → 启用选择器
                this.SendCommand(new SelectorChangeEnableCommand { Enable = true });
                this.SendCommand(new CalculatorChangeModeCommand { mode = e.ModeType });
            }
        }).UnRegisterWhenNodeExitTree(this);
    }
}