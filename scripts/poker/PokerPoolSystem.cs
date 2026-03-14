using GFramework.Godot.pool;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
///     扑克对象池系统，用于管理和复用扑克对象以提高性能
/// </summary>
public class PokerPoolSystem : AbstractNodePoolSystem<string, Poker> , IPokerPoolSystem
{
    protected override void OnInit()
    {
        
    }

    public Poker Acquire(string sceneKey, Node parent)
    {
        return Acquire(sceneKey, parent as Node);
    }

    public void Release(string sceneKey, Poker poker)
    {
        
    }

    protected override PackedScene LoadScene(string key)
    {
        return null!;
    }
}