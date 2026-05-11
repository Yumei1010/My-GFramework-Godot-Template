using Godot;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_button;

public interface IModeButton
{
    StringName Name { get; set; }

    ModeType Mode { get; set; }
}