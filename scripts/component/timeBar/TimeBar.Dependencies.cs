using global::TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.component.timeBar;

public partial class TimeBar
{
    private TextureProgressBar TimeProgressBar => GetNode<TextureProgressBar>("%TimeProgressBar");
    private Label TimeLabel => GetNode<Label>("%TimeLabel");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // TimeScale = 1f;
        // Start(10);
        // await ToSignal(GetTree().CreateTimer(5f), "timeout");
        // AdjustTime(35);
    }
}