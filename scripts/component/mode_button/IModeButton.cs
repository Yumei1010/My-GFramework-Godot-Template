using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_button;

public interface IModeButton
{
    String Name { get; set; }

    ModeType ModeType { get; set; }
}