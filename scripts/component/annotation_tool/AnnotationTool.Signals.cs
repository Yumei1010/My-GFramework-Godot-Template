using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

public partial class AnnotationTool
{
    private void ConnectSignal()
    {
        RedButton.ButtonDown += OnButtonDownRedButton;
        BlueButton.ButtonDown += OnButtonDownBlueButton; 
        WhiteButton.ButtonDown += OnButtonDownWhiteButton;
        FreehandToolButton.ButtonDown += OnButtonDownFreehandToolButton;
        LineToolButton.ButtonDown += OnButtonDownLineToolButton;
        RectToolButton.ButtonDown += OnButtonDownRectToolButton;
        CircleToolButton.ButtonDown += OnButtonDownCircleToolButton;
        EraserToolButton.ButtonDown += OnButtonDownEraserToolButton;
        StrokeWidthSlider.ValueChanged += OnValueChangedStrokeWidthSlider;
        EraserRadiusSlider.ValueChanged += OnValueChangedEraserRadiusSlider;
    }

    private void OnButtonDownFreehandToolButton()
    {
        ChangeTo(AnnotationToolType.Freehand);
    }

    private void OnButtonDownLineToolButton()
    {
        ChangeTo(AnnotationToolType.Line);
    }

    private void OnButtonDownRectToolButton()
    {
        ChangeTo(AnnotationToolType.Rect);
    }

    private void OnButtonDownCircleToolButton()
    {
        ChangeTo(AnnotationToolType.Circle);
    }

    private void OnButtonDownEraserToolButton()
    {
        ChangeTo(AnnotationToolType.Eraser);
    }

    private void OnButtonDownRedButton()
    {
        _color = Colors.Red;
    }

    private void OnButtonDownBlueButton()
    {
        _color = Colors.Blue;
    }

    private void OnButtonDownWhiteButton()
    {
        _color = Colors.White;
    }

    private void OnValueChangedStrokeWidthSlider(double value)
    {
        StrokeWidth = (float)value;
    }

    private void OnValueChangedEraserRadiusSlider(double value)
    {
        EraserRadius = (float)value;
    }
}
