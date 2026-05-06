using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.component.mode_button;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_bar;

[Log]
[ContextAware]
public partial class ModeBar : HBoxContainer
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();

        init(new List<ModeType>
        {
            ModeType.Add,
            ModeType.Subtract,
            ModeType.Multiply,
            ModeType.Divide
        });
    }

    /// <summary>根据给定的计算方式列表动态构建模式按钮。</summary>
    public void init(IReadOnlyList<ModeType> modes)
    {
        foreach (var modeType in modes)
        {
            IModeButton button = (IModeButton)_buttonScene.Instantiate<TextureButton>();
            button.Name = $"{modeType}Button";
            button.ModeType = modeType;
            ModeButtons[modeType] = button;
            AddChild(button as Node);
        }
    }
}
