using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.component.behavior_tree;
using GFrameworkTemplate.scripts.component.behavior_tree.bt_node;
using Godot;

namespace GFrameworkTemplate.scripts.demo.behavior_tree;

/// <summary>
///     行为树可视化演示控制器。
///     配合 <c>scenes/behavior_tree_demo.tscn</c> 使用：场景中已用 Godot 节点拼好行为树层级
///     （BtTree → BtSelector → 子树），本控制器在运行时注入叶子逻辑并打印日志，
///     运行场景即可看到行为树逐帧执行与行为切换。
/// </summary>
[Log]
[ContextAware]
public partial class BehaviorTreeDemoController : Node
{
    private bool _hasTarget = true;
    private bool _hasAmmo = true;
    private int _attackTicks;

    /// <summary>
    ///     Godot 节点就绪时注入各叶子节点的动作逻辑。
    /// </summary>
    public override void _Ready()
    {
        InjectActions();
        _log.Info("行为树演示就绪：有目标 → 攻击（2 帧）；无弹药 → 装弹；无目标 → 巡逻");
    }

    /// <summary>
    ///     按唯一名称查找并注入各叶子节点逻辑。
    /// </summary>
    private void InjectActions()
    {
        // 条件：有目标？
        GetNode<BtConditionNode>("%HasTarget").SetCondition(() => _hasTarget);

        // 条件：有弹药？
        GetNode<BtConditionNode>("%HasAmmo").SetCondition(() => _hasAmmo);

        // 动作：攻击（模拟多帧，2 帧完成）
        GetNode<BtActionNode>("%Attack").SetAction(() =>
        {
            _attackTicks++;
            _log.Info($"攻击第 {_attackTicks} 帧");
            return _attackTicks < 2 ? NodeStatus.Running : NodeStatus.Success;
        });

        // 动作：装弹
        GetNode<BtActionNode>("%Reload").SetAction(() =>
        {
            _hasAmmo = true;
            _log.Info("装弹完成");
            return NodeStatus.Success;
        });

        // 动作：巡逻
        GetNode<BtActionNode>("%Patrol").SetAction(() =>
        {
            _log.Info("巡逻中");
            return NodeStatus.Success;
        });
    }

    /// <summary>
    ///     演示若干帧后切换目标/弹药状态，展示行为树自动改变行为。
    ///     时序：攻击（帧1-19）→ 弹药耗尽装弹（帧20-39）→ 目标消失巡逻（帧40+）。
    /// </summary>
    public override void _Process(double delta)
    {
        var frame = Engine.GetProcessFrames();

        // 帧 20：弹药耗尽（仍保持有目标），触发"装弹"分支
        if (frame == 20 && _hasTarget)
        {
            _hasAmmo = false;
            _log.Info("弹药耗尽，将触发装弹...");
        }

        // 帧 40：目标消失，触发"巡逻"分支
        if (frame == 40 && _hasTarget)
        {
            _hasTarget = false;
            _log.Info("目标消失，切换为巡逻...");
        }
    }
}
