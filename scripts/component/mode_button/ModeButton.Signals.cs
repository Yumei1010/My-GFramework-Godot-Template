using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.calculator.command;
using TimeToTwentyfour.scripts.cqrs.selector.command;

namespace TimeToTwentyfour.scripts.component.mode_button;

public partial class ModeButton
{
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
    }

    private void OnButtonDown()
    {
        this.SendCommand(new SelectorChangeEnableCommand()
        {
            Enable = true
        });

        this.SendCommand(new CalculatorChangeModeCommand()
        {
            mode = ModeType
        });
    }
}
