using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.poker.command;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
    
    private void OnButtonDown()
    {
        this.SendCommand(new PokerPickUpCommand { PokerId = Id });
    }

    private void OnButtonUp()
    {
        this.SendCommand(new PokerPutDownCommand { PokerId = Id });
    }

    private void OnMouseEntered()
    {
        this.SendCommand(new PokerGainFocusCommand { PokerId = Id });
    }

    private void OnMouseExited()
    {
        this.SendCommand(new PokerResetFake3DRotationCommand { PokerId = Id });
        this.SendCommand(new PokerLoseFocusCommand { PokerId = Id });
    }
}