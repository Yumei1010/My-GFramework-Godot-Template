using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.command;

public sealed class AnnotationToolChangeToolCommand : AbstractCommand
{
    public required AnnotationToolType Tool { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().CurrentTool = Tool;

        this.SendEvent(new AnnotationToolCurrentToolChangedEvent
        {
            CurrentTool = Tool
        });
    }
}
