using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.enums.resources;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.component.mode_button;

[Log]
[ContextAware]
public partial class ModeButton : TextureButton, IModeButton
{
    public override void _Ready()
    {
        _ = ReadyAsync();
    }

    private void UpdateTexture()
    {
        TextureNormal = ModeType switch
        {
            ModeType.Add => _textureRegistry.Get(nameof(TextureKey.ModeButtonAddTexture)) as Texture2D,
            ModeType.Divide => _textureRegistry.Get(nameof(TextureKey.ModeButtonDivideTexture)) as Texture2D,
            ModeType.Multiply => _textureRegistry.Get(nameof(TextureKey.ModeButtonMultiplyTexture)) as Texture2D,
            ModeType.Subtract => _textureRegistry.Get(nameof(TextureKey.ModeButtonSubtractTexture)) as Texture2D,
            _ => throw new InvalidOperationException("didn't have this SuitType")
        };
    }

}
