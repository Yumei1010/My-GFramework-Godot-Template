using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.enums.resources;
using TimeToTwentyfour.scripts.model.annotation_tool;
using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

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

    private readonly List<LineElement> _lines = [];
    private readonly List<CircleElement> _circles = [];
    private readonly List<RectElement> _rects = [];
    private readonly List<FreehandLine> _freehandLines = [];
    private IGodotTextureRegistry _textureRegistry = null!;
    private AnnotationToolModel _model = null!;
    private Tween _tween = null!;

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // 依赖注入
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        _model = this.GetModel<AnnotationToolModel>();

        _model.Enabled = false;
        _model.CurrentTool = AnnotationToolType.Freehand;
        _model.ToolWidth = 2;
        _color = Colors.White;

        FreehandToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolFreehandIconTexture)) as Texture2D;
        LineToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolLineIconTexture)) as Texture2D;
        RectToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolRectIconTexture)) as Texture2D;
        CircleToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolCircleIconTexture)) as Texture2D;
        EraserToolButton.TextureNormal = _textureRegistry.Get(nameof(TextureKey.AnnotationToolEraserIconTexture)) as Texture2D;
    }
}