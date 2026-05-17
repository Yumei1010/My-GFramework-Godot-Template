using GFramework.Core.command;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.command;

public sealed class AnnotationToolChangeColorCommand : AbstractCommand
{
    public required Color Color { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().CurrentColor = Color;

        this.SendEvent(new AnnotationToolCurrentColorChangedEvent
        {
            CurrentColor = Color
        });
    }
}
