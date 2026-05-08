using TimeToTwentyfour.scripts.enums.calculator;
using Godot;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.modeButton.@event;

namespace TimeToTwentyfour.scripts.component.mode_bar;

public partial class ModeBar
{
    private void ConnectSignal()
    {
        if (ModeButtons.TryGetValue(ModeType.Add, out var addButton))
        {
            (addButton as Node)?.Connect("pressed", new Callable(this, nameof(OnMouseDownAddButton)));
        }
        if (ModeButtons.TryGetValue(ModeType.Subtract, out var subtractButton))
        {
            (subtractButton as Node)?.Connect("pressed", new Callable(this, nameof(OnMouseDownSubtractButton)));
        }
        if (ModeButtons.TryGetValue(ModeType.Multiply, out var multiplyButton))
        {
            (multiplyButton as Node)?.Connect("pressed", new Callable(this, nameof(OnMouseDownMultiplyButton)));
        }
        if (ModeButtons.TryGetValue(ModeType.Divide, out var divideButton))
        {
            (divideButton as Node)?.Connect("pressed", new Callable(this, nameof(OnMouseDownDivideButton)));
        }
    }

    private void OnMouseDownAddButton()
    {
        this.SendEvent(new ModeButtonClickedEvent()
        {
            ModeType = ModeType.Add
        });
    }

    private void OnMouseDownSubtractButton()
    {
        this.SendEvent(new ModeButtonClickedEvent()
        {
            ModeType = ModeType.Subtract
        });
    }

    private void OnMouseDownMultiplyButton()
    {
        this.SendEvent(new ModeButtonClickedEvent()
        {
            ModeType = ModeType.Multiply
        });
    }

    private void OnMouseDownDivideButton()
    {
        this.SendEvent(new ModeButtonClickedEvent()
        {
            ModeType = ModeType.Divide
        });
    }
}
