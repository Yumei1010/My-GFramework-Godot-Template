using GFrameworkTemplate.scripts.component.behavior_tree;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     行为树测试：验证节点执行、Sequence/Selector 组合、Running 暂停-继续语义。
/// </summary>
public class BehaviorTreeTests
{
    [Fact]
    public void ActionNode_ReturnsResult()
    {
        var success = new ActionNode(() => NodeStatus.Success);
        var failure = new ActionNode(() => NodeStatus.Failure);
        var running = new ActionNode(() => NodeStatus.Running);

        Assert.Equal(NodeStatus.Success, success.Execute());
        Assert.Equal(NodeStatus.Failure, failure.Execute());
        Assert.Equal(NodeStatus.Running, running.Execute());
    }

    [Fact]
    public void Sequence_FailsFast_OnFirstFailure()
    {
        var executed = new List<string>();
        var sequence = new SequenceNode(
            new ActionNode(() => { executed.Add("A"); return NodeStatus.Success; }),
            new ActionNode(() => { executed.Add("B"); return NodeStatus.Failure; }),
            new ActionNode(() => { executed.Add("C"); return NodeStatus.Success; }));

        var status = sequence.Execute();

        // B 失败后整体失败，C 不再执行
        Assert.Equal(NodeStatus.Failure, status);
        Assert.Equal(new[] { "A", "B" }, executed);
    }

    [Fact]
    public void Sequence_Succeeds_WhenAllSucceed()
    {
        var sequence = new SequenceNode(
            new ActionNode(() => NodeStatus.Success),
            new ActionNode(() => NodeStatus.Success));

        Assert.Equal(NodeStatus.Success, sequence.Execute());
    }

    [Fact]
    public void Selector_Succeeds_OnFirstSuccess()
    {
        var executed = new List<string>();
        var selector = new SelectorNode(
            new ActionNode(() => { executed.Add("A"); return NodeStatus.Failure; }),
            new ActionNode(() => { executed.Add("B"); return NodeStatus.Success; }),
            new ActionNode(() => { executed.Add("C"); return NodeStatus.Success; }));

        var status = selector.Execute();

        // B 成功后就整体成功，C 不再尝试
        Assert.Equal(NodeStatus.Success, status);
        Assert.Equal(new[] { "A", "B" }, executed);
    }

    [Fact]
    public void Selector_Fails_WhenAllFail()
    {
        var selector = new SelectorNode(
            new ActionNode(() => NodeStatus.Failure),
            new ActionNode(() => NodeStatus.Failure));

        Assert.Equal(NodeStatus.Failure, selector.Execute());
    }

    [Fact]
    public void Sequence_ResumesFromRunningChild()
    {
        var ticks = 0;
        // 模拟"走向目标"动作：前 2 帧 Running，第 3 帧完成
        var move = new ActionNode(() =>
        {
            ticks++;
            return ticks < 3 ? NodeStatus.Running : NodeStatus.Success;
        });

        var follow = new ActionNode(() => NodeStatus.Success);
        var sequence = new SequenceNode(move, follow);

        // 帧1：move Running
        Assert.Equal(NodeStatus.Running, sequence.Execute());
        // 帧2：move 继续 Running
        Assert.Equal(NodeStatus.Running, sequence.Execute());
        // 帧3：move 完成，接着执行 follow，整体成功
        Assert.Equal(NodeStatus.Success, sequence.Execute());
    }

    [Fact]
    public void BehaviorTree_Tick_RunsFromRoot()
    {
        var tree = new BehaviorTree(new ActionNode(() => NodeStatus.Success));

        Assert.Equal(NodeStatus.Success, tree.Tick());
        Assert.Equal(NodeStatus.Success, tree.LastStatus);
    }

    [Fact]
    public void CombatAI_Example_BehavesCorrectly()
    {
        // 模拟 AI：有目标就攻击（没弹药先装弹），没目标就巡逻
        var hasTarget = false;
        var hasAmmo = true;
        var attackTicks = 0;
        var events = new List<string>();

        var tree = new BehaviorTree(
            new SelectorNode(
                // 策略1：有目标 → 攻击序列
                new SequenceNode(
                    new ConditionNode(() => hasTarget),
                    new SelectorNode(
                        new SequenceNode(
                            new ConditionNode(() => hasAmmo),
                            new ActionNode(() =>
                            {
                                attackTicks++;
                                events.Add("attack");
                                return attackTicks < 2 ? NodeStatus.Running : NodeStatus.Success;
                            })),
                        new ActionNode(() =>
                        {
                            events.Add("reload");
                            hasAmmo = true;
                            return NodeStatus.Success;
                        }))),
                // 策略2：没目标 → 巡逻
                new ActionNode(() =>
                {
                    events.Add("patrol");
                    return NodeStatus.Success;
                })));

        // 无目标：巡逻
        tree.Tick();
        Assert.Equal(new[] { "patrol" }, events);

        // 有目标且有弹药：攻击（两帧完成）
        hasTarget = true;
        tree.Tick();
        Assert.Equal(new[] { "patrol", "attack" }, events);
        Assert.Equal(NodeStatus.Running, tree.LastStatus);
        tree.Tick();
        Assert.Equal(new[] { "patrol", "attack", "attack" }, events);
        Assert.Equal(NodeStatus.Success, tree.LastStatus);

        // 有目标但没弹药：装弹
        hasAmmo = false;
        tree.Tick();
        Assert.Equal(new[] { "patrol", "attack", "attack", "reload" }, events);
        Assert.Equal(NodeStatus.Success, tree.LastStatus);
    }
}
