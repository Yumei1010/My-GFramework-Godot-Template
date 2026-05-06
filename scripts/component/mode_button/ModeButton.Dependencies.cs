using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.utility;

namespace TimeToTwentyfour.scripts.component.mode_button;

public partial class ModeButton
{
    private IGodotTextureRegistry _textureRegistry = null!;

    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // 依赖注入
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;

        // 更新纹理显示
        UpdateTexture();
    }
}
