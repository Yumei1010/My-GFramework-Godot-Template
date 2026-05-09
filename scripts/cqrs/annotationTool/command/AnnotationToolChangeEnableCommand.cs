using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.command;

public sealed class AnnotationToolChangeEnableCommand : AbstractCommand
{
    public bool Enabled { get; init;}

    protected override void OnExecute()
    {
        this.GetModel<AnnotationToolModel>().Enabled = Enabled;
    }
}
