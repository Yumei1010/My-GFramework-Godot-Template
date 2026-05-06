using Godot;
using TimeToTwentyfour.scripts.component.mode_button;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_bar;

public partial class ModeBar : HBoxContainer
{
    private Dictionary<ModeType, IModeButton> ModeButtons = new();
}