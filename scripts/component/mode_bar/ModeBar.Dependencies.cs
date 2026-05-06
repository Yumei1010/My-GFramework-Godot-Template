using TimeToTwentyfour.scripts.component.calculator;
using Godot;
using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.component.mode_bar;

public partial class ModeBar
{
    [Export] private PackedScene _buttonScene = GD.Load<PackedScene>("res://scenes/component/mode_button/mode_button.tscn");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}
