using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.annotationTool.@event;
using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.entities.annotationTool;

public partial class AnnotationTool
{
    private void RegisterEvent()
    {
        this.RegisterEvent<AnnotationToolEnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enabled);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<AnnotationToolCurrentToolChangedEvent>(e =>
        {
            OnCurrentToolChangedEvent(e.CurrentTool);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<AnnotationToolCurrentColorChangedEvent>(e =>
        {
            OnCurrentColorChangedEvent(e.CurrentColor);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<AnnotationToolToolWidthChangedEvent>(e =>
        {
            OnToolWidthChangedEvent(e.ToolWidth);
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void OnEnableChangedEvent(bool enabled)
    {
        _enabled = enabled;
        if (!enabled)
            _mousePos = Vector2.Zero;
        QueueRedraw();
    }

    private void OnCurrentToolChangedEvent(AnnotationToolType tool)
    {
        _currentTool = tool;
        QueueRedraw();
    }

    private void OnCurrentColorChangedEvent(Color color)
    {
        _color = color;
        QueueRedraw();
    }

    private void OnToolWidthChangedEvent(float toolWidth)
    {
        _toolWidth = toolWidth;
        QueueRedraw();
    }
}