using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.modeButton.@event;

namespace TimeToTwentyfour.scripts.component.mode_button;

public partial class ModeButton
{
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
    }

    private void OnButtonDown()
    {
        this.SendEvent(new ModeButtonClickedEvent
        {
            ModeType = ModeType
        });
    }
}
