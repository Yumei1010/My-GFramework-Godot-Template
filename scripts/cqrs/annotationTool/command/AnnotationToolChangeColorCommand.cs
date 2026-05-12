using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.annotationTool.@event;
using TimeToTwentyfour.scripts.model.annotationTool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.command;

public class AnnotationToolChangeColorCommand : AbstractCommand
{
    public Color Color { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().CurrentColor = Color;

        this.SendEvent(new AnnotationToolCurrentColorChangedEvent
        {
            CurrentColor = Color
        });
    }
}
