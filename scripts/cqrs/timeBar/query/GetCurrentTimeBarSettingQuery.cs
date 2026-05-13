using CQRS.TimeBar.Query.Result;
using GFramework.Core.extensions;
using GFramework.Core.query;
using TimeToTwentyfour.scripts.data.timeBar;
using TimeToTwentyfour.scripts.model.timeBar;

namespace TimeToTwentyfour.scripts.cqrs.timeBar.query;

public sealed class GetCurrentTimeBarSettingQuery : AbstractQuery<TimeBarView>
{
    protected override TimeBarView OnDo()
    {
        var model =  this.GetModel<TimeBarModel>();

        return new TimeBarView()
        {
            TimeBar = model.GetData<TimeBarData>()
        };
    }
}
