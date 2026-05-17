using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.command;
using TimeToTwentyfour.scripts.enums.annotation_tool;

namespace TimeToTwentyfour.scripts.entities.annotation_tool;

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
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = AnnotationToolType.Freehand });
    }

    private void OnButtonDownLineToolButton()
    {
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = AnnotationToolType.Line });
    }

    private void OnButtonDownRectToolButton()
    {
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = AnnotationToolType.Rect });
    }

    private void OnButtonDownCircleToolButton()
    {
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = AnnotationToolType.Circle });
    }

    private void OnButtonDownEraserToolButton()
    {
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = AnnotationToolType.Eraser });
    }

    private void OnButtonDownRedButton()
    {
        this.SendCommand(new AnnotationToolChangeColorCommand { Color = Colors.Red });
    }

    private void OnButtonDownBlueButton()
    {
        this.SendCommand(new AnnotationToolChangeColorCommand { Color = Colors.Blue });
    }

    private void OnButtonDownWhiteButton()
    {
        this.SendCommand(new AnnotationToolChangeColorCommand { Color = Colors.White });
    }

    private void OnValueChangedToolWidthSlider(double value)
    {
        this.SendCommand(new AnnotationToolChangeToolWidthCommand { ToolWidth = (float)value });
    }

    private void OnButtonDownPanelButton()
    {
        if (_opening)
        {
            if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
            
                _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                _tween.TweenProperty( ToolRect, "position", new Vector2(-256,64), 0.5f);

            _opening = false;
            this.SendCommand(new AnnotationToolChangeEnableCommand { Enabled = false });
            _mousePos = Vector2.Zero;
            QueueRedraw();
        }
        else
        {
            if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();

                _tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
                _tween.TweenProperty( ToolRect, "position", new Vector2(0,64), 0.5f);

            _opening = true;
            this.SendCommand(new AnnotationToolChangeEnableCommand { Enabled = true });
        }
    }
}
