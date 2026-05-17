using cqrs.poker.query.result;
using GFramework.Core.extensions;
using GFramework.Core.query;
using TimeToTwentyfour.scripts.data.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.cqrs.annotation_tool.query;

public sealed class GetCurrentAnnotationToolSettingQuery : AbstractQuery<AnnotationToolView>
{
    protected override AnnotationToolView OnDo()
    {
        var model = this.GetModel<AnnotationToolModel>();

        return new AnnotationToolView
        {
            AnnotationTool = model.GetData<AnnotationToolData>()
        };
    }
}
