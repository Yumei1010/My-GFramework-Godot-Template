using GFramework.Godot.pool;
using global::GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

/// <summary>
/// 扑克对象池系统，用于管理和复用扑克对象以提高性能
/// </summary>
public class PokerPoolSystem : AbstractNodePoolSystem<string, Poker>,IPokerPoolSystem
{
    protected override PackedScene LoadScene(string key) => PokerSceneRegistry.Instance.Get(key);

    protected override void OnInit()
    {
        
    }

    public Poker Acquire(string sceneKey, Node parent)
    {
        return Acquire(sceneKey, parent);
    }
}