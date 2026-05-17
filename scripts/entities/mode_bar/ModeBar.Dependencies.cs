using Godot;
using TimeToTwentyfour.global;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.player;
using TimeToTwentyfour.scripts.component.mode_button;

namespace TimeToTwentyfour.scripts.entities.mode_bar;

public partial class ModeBar
{
    [Export] private PackedScene _buttonScene = GD.Load<PackedScene>("res://scenes/component/mode_button/mode_button.tscn");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        /// <summary>根据给定的计算方式列表动态构建模式按钮。</summary>
        foreach (var modeType in this.GetModel<PlayerModel>().CurrentEnabledModes)
        {
            IModeButton button = (IModeButton)_buttonScene.Instantiate<TextureButton>();
            button.Name = $"{modeType}Button";
            button.Mode = modeType;
            ModeButtons[modeType] = button;
            AddChild(button as Node);
        }
    }
}
