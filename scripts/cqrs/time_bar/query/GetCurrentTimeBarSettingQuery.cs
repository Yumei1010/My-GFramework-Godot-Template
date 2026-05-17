using CQRS.TimeBar.Query.Result;
using GFramework.Core.extensions;
using GFramework.Core.query;
using TimeToTwentyfour.scripts.data.time_bar;
using TimeToTwentyfour.scripts.model.time_bar;

namespace TimeToTwentyfour.scripts.cqrs.time_bar.query;

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
