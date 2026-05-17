using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.command;

public sealed class AnnotationToolChangeToolWidthCommand : AbstractCommand
{
    public required float ToolWidth { get; set; }

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().ToolWidth = ToolWidth;

        this.SendEvent(new AnnotationToolToolWidthChangedEvent 
        { 
            ToolWidth = ToolWidth 
        });
    }
}
