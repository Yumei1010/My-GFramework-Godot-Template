using CQRS.TimeBar.Query.Result;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.cqrs.timeBar.query;

namespace TimeToTwentyfour.scripts.entities.timeBar;

/// <summary>
///     <see cref="TimeBar"/> 的 Godot 依赖注入文件。
/// </summary>
public partial class TimeBar
{
    private TextureProgressBar TimeProgressBar => GetNode<TextureProgressBar>("%TimeProgressBar");
    private Label TimeLabel => GetNode<Label>("%TimeLabel");
    
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        TimeBarView config = ContextAwareExtensions.SendQuery(this, new GetCurrentTimeBarSettingQuery());

        _totalDuration = config.TimeBar.TotalDuration;
        _timeScale = config.TimeBar.TimeScale;
    }
}