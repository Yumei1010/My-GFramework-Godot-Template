using cqrs.poker.query.result;
using GFramework.Core.extensions;
using GFramework.Core.query;
using TimeToTwentyfour.scripts.data.annotationTool;
using TimeToTwentyfour.scripts.model.annotationTool;

namespace TimeToTwentyfour.scripts.cqrs.annotationTool.query;

public class GetCurrentAnnotationToolSettingQuery : AbstractQuery<AnnotationToolView>
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
