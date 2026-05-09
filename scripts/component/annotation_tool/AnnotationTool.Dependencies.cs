using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.model.annotation_tool;

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

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        Model = this.GetModel<AnnotationToolModel>();
    }
}