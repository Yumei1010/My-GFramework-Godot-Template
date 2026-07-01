using Godot;

namespace GFrameworkTemplate.scripts.menu;

public partial class VnTalkPage
{
    /// <summary>
    ///     连接 Godot 信号——点击推进
    /// </summary>
    private void ConnectPageSignals()
    {
        ClickArea.GuiInput += args =>
        {
            if (args is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            {
                // 使用者在此调用故事引擎的 Advance() 方法
                // 例如: this.GetSystem<IStoryEngine>().Advance();
            }
        };
    }
}
