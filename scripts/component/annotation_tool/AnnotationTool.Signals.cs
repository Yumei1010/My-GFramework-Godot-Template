using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.component.calculator.mode;
using TimeToTwentyfour.scripts.enums.annotationTool;
namespace TimeToTwentyfour.scripts.component.annotationTool;

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
        ToolWidthSlider.ValueChanged += OnValueChangedToolWidthSlider;
        PanelButton.ButtonDown += OnButtonDownPanelButton;
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

    private void OnValueChangedToolWidthSlider(double value)
    {
        _model.ToolWidth = (float)value;
    }

    private void OnButtonDownPanelButton()
    {
        if (_opening)
        {
            if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
            
                _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                _tween.TweenProperty( ToolRect, "position", new Vector2(-256,64), 0.5f);

            _opening = false;
            _model.Enabled = false;
            _mousePos = Vector2.Zero;
            QueueRedraw();
        }
        else
        {
            if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();

                _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                _tween.TweenProperty( ToolRect, "position", new Vector2(0,64), 0.5f);

            _opening = true;
            _model.Enabled = true;
        }
    }
}
