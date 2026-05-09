using Godot;
using TimeToTwentyfour.global;

namespace TimeToTwentyfour.scripts.menu.map_menu;

/// <summary>
///     <see cref="MapMenu"/> 的 Godot 依赖注入文件。
/// </summary>
public partial class MapMenu
{
    private Container CircleContainer => GetNode<Container>("%CircleContainer");

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
}