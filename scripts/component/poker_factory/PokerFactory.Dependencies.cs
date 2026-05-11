using Godot;

namespace TimeToTwentyfour.scripts.component.pokerFactory;

/// <summary>
///     <see cref="PokerFactory"/> 的 Godot 依赖注入文件。
/// </summary>
public partial class PokerFactory
{
    [Export] private PackedScene _pokerScene = GD.Load<PackedScene>("res://scenes/component/poker_view/poker_view.tscn");
}