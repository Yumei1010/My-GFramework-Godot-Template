using Godot;
using TimeToTwentyfour.global;

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
    private HSlider StrokeWidthSlider => GetNode<HSlider>("%StrokeWidthSlider");
    private HSlider EraserRadiusSlider => GetNode<HSlider>("%EraserRadiusSlider");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}