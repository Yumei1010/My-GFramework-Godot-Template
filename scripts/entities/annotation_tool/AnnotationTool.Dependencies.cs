using cqrs.poker.query.result;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.query;
using TimeToTwentyfour.scripts.enums.resources;
using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.entities.annotation_tool;

public partial class AnnotationTool
{
    private TextureRect ToolRect => GetNode<TextureRect>("%ToolRect");
    private TextureButton RedButton => GetNode<TextureButton>("%RedButton");
    private TextureButton BlueButton => GetNode<TextureButton>("%BlueButton");
    private TextureButton WhiteButton => GetNode<TextureButton>("%WhiteButton");
    private TextureButton FreehandToolButton => GetNode<TextureButton>("%FreehandToolButton");
    private TextureButton LineToolButton => GetNode<TextureButton>("%LineToolButton");
    private TextureButton RectToolButton => GetNode<TextureButton>("%RectToolButton");
    private TextureButton CircleToolButton => GetNode<TextureButton>("%CircleToolButton");
    private TextureButton EraserToolButton => GetNode<TextureButton>("%EraserToolButton");
    private HSlider ToolWidthSlider => GetNode<HSlider>("%ToolWidthSlider");
    private TextureButton PanelButton => GetNode<TextureButton>("%PanelButton");

    private IGodotTextureRegistry _textureRegistry = null!;

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;

        FreehandToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolFreehandIconTexture)) as Texture2D;
        LineToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolLineIconTexture)) as Texture2D;
        RectToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolRectIconTexture)) as Texture2D;
        CircleToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolCircleIconTexture)) as Texture2D;
        EraserToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolEraserIconTexture)) as Texture2D;
    
        AnnotationToolView config = ContextAwareExtensions.SendQuery(this, new GetCurrentAnnotationToolSettingQuery());

        _color = config.AnnotationTool.CurrentColor;
        _toolWidth = config.AnnotationTool.ToolWidth;
        _currentTool = config.AnnotationTool.CurrentTool;
        _enabled = config.AnnotationTool.Enabled;
    }
}