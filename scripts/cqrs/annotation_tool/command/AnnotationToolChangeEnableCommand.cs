using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.@event;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.command;

public sealed class AnnotationToolChangeEnableCommand : AbstractCommand
{
    public required bool Enabled { get; init;}

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().Enabled = Enabled;

        this.SendEvent(new AnnotationToolEnableChangedEvent
        {
            Enabled = Enabled
        });
    }
}
