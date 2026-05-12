using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.annotationTool.@event;
using TimeToTwentyfour.scripts.model.annotationTool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.command;

public sealed class AnnotationToolChangeToolWidthCommand : AbstractCommand
{
    public float ToolWidth { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().ToolWidth = ToolWidth;

        this.SendEvent(new AnnotationToolToolWidthChangedEvent { 
            ToolWidth = ToolWidth 
        });
    }
}
