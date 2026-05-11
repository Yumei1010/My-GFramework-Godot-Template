using Godot;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_button;

public partial class ModeButton
{
    public required new StringName Name { get; set; }

    public ModeType Mode { get; set; }
}
