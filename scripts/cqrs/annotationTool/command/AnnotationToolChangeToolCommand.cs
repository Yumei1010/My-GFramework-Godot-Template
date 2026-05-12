using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.annotationTool.@event;
using TimeToTwentyfour.scripts.enums.annotationTool;
using TimeToTwentyfour.scripts.model.annotationTool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.command;

public sealed class AnnotationToolChangeToolCommand : AbstractCommand
{
    public AnnotationToolType Tool { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().CurrentTool = Tool;

        this.SendEvent(new AnnotationToolCurrentToolChangedEvent
        {
            CurrentTool = Tool
        });
    }
}
