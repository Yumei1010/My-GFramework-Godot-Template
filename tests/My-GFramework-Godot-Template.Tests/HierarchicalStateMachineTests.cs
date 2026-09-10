using GFrameworkTemplate.scripts.component.hierarchical_state_machine;
using GFrameworkTemplate.scripts.component.state_machine;

namespace GFrameworkTemplate.Tests;

/// <summary>
///     分层状态机测试。
///     通过"记录每次生命周期调用"来验证状态转换、分层进入/退出、子状态机递归更新。
/// </summary>
public class HierarchicalStateMachineTests
{
    /// <summary>
    ///     测试用状态：把每次 Enter / Process / Exit 追加到历史记录。
    /// </summary>
    private sealed class TestState(string name, List<string> history) : IState
    {
        public void Enter() => history.Add($"{name}:Enter");
        public void Process(double delta) => history.Add($"{name}:Process");
        public void Exit() => history.Add($"{name}:Exit");
    }

    /// <summary>
    ///     测试用条件：由外部布尔开关控制，便于在测试中随时触发转换。
    /// </summary>
    private sealed class ToggleCondition(Func<bool> getValue) : ITransitionCondition
    {
        public bool ShouldTransition() => getValue();
    }

    [Fact]
    public void Start_EntersFirstRegisteredState()
    {
        var history = new List<string>();
        var idle = new TestState("Idle", history);
        var fsm = new HierarchicalStateMachine().AddState(idle);

        fsm.Start();

        Assert.Equal(new[] { "Idle:Enter" }, history);
        Assert.Same(idle, fsm.CurrentState);
    }

    [Fact]
    public void Transition_SwitchesState_WhenConditionIsTrue()
    {
        var history = new List<string>();
        var idle = new TestState("Idle", history);
        var run = new TestState("Run", history);
        var canRun = false;

        var fsm = new HierarchicalStateMachine()
            .AddState(idle)
            .AddState(run)
            .AddTransition(idle, run, new ToggleCondition(() => canRun));
        fsm.Start();

        // 条件未满足：仍在 Idle
        canRun = false;
        fsm.Process(0.016);
        Assert.Same(idle, fsm.CurrentState);

        // 条件满足：切到 Run
        canRun = true;
        fsm.Process(0.016);

        Assert.Same(run, fsm.CurrentState);
        Assert.Equal(
            new[]
            {
                "Idle:Enter", "Idle:Process", "Idle:Process", "Idle:Exit", "Run:Enter"
            },
            history);
    }

    [Fact]
    public void SubMachine_StartsWhenParentStateEnters_AndRecursivelyProcesses()
    {
        var history = new List<string>();
        var parent = new TestState("Parent", history);
        var childIdle = new TestState("ChildIdle", history);
        var childRun = new TestState("ChildRun", history);

        var sub = new HierarchicalStateMachine()
            .AddState(childIdle)
            .AddState(childRun)
            .AddTransition(childIdle, childRun, new ToggleCondition(() => true));

        var fsm = new HierarchicalStateMachine()
            .AddState(parent)
            .AttachSubMachine(parent, sub);
        fsm.Start();

        // 父状态进入时自动进入子状态机初始状态
        Assert.Equal(new[] { "Parent:Enter", "ChildIdle:Enter" }, history);
        Assert.Same(childIdle, fsm.ActiveMachine.CurrentState);
        Assert.Same(childIdle, sub.CurrentState);

        // 子状态机递归处理：子状态自动转换
        fsm.Process(0.016);
        Assert.Same(childRun, sub.CurrentState);
        Assert.Equal(
            new[] { "Parent:Enter", "ChildIdle:Enter", "ChildIdle:Process", "ChildIdle:Exit", "ChildRun:Enter" },
            history);

        // 父状态退出时自底向上退出：先子后父
        fsm.Stop();
        Assert.Equal(
            new[]
            {
                "Parent:Enter", "ChildIdle:Enter",
                "ChildIdle:Process", "ChildIdle:Exit", "ChildRun:Enter",
                "ChildRun:Exit", "Parent:Exit"
            },
            history);
    }

    [Fact]
    public void StateChanged_EventReportsFromAndTo()
    {
        var history = new List<string>();
        var idle = new TestState("Idle", history);
        var run = new TestState("Run", history);
        var toggle = false;

        var fsm = new HierarchicalStateMachine()
            .AddState(idle)
            .AddState(run)
            .AddTransition(idle, run, new ToggleCondition(() => toggle));

        var transitions = new List<(IState? From, IState To)>();
        fsm.OnStateChanged += (from, to) => transitions.Add((from, to));

        fsm.Start();          // Idle 进入：来源为 null
        toggle = true;
        fsm.Process(0.016);   // Idle → Run

        Assert.Equal(2, transitions.Count);
        Assert.Null(transitions[0].From);
        Assert.Same(idle, transitions[0].To);
        Assert.Same(idle, transitions[1].From);
        Assert.Same(run, transitions[1].To);
    }

    [Fact]
    public void IsRunning_ReflectsActiveState()
    {
        var history = new List<string>();
        var fsm = new HierarchicalStateMachine().AddState(new TestState("Idle", history));

        Assert.False(fsm.IsRunning);

        fsm.Start();
        Assert.True(fsm.IsRunning);
        Assert.NotNull(fsm.ActiveState);

        fsm.Stop();
        Assert.False(fsm.IsRunning);
        Assert.Null(fsm.ActiveState);
    }

    [Fact]
    public void ActiveState_ResolvesDeepestState_WhileCurrentStateIsNull()
    {
        var history = new List<string>();
        var parent = new TestState("Parent", history);
        var child = new TestState("Child", history);

        var sub = new HierarchicalStateMachine().AddState(child);
        var fsm = new HierarchicalStateMachine()
            .AddState(parent)
            .AttachSubMachine(parent, sub);

        fsm.Start();

        // CurrentState 是本机直接状态（父状态）；ActiveState 递归到最深的活动状态（子状态）
        Assert.Same(parent, fsm.CurrentState);
        Assert.Same(parent, fsm.ActiveMachine == fsm ? parent : fsm.ActiveMachine.Parent?.CurrentState);
        Assert.Same(child, fsm.ActiveState);
    }

    [Fact]
    public void AttachSubMachine_RejectsAlreadyAttachedSubMachine()
    {
        var history = new List<string>();
        var a = new TestState("A", history);
        var b = new TestState("B", history);
        var sub = new HierarchicalStateMachine().AddState(new TestState("C", history));

        var first = new HierarchicalStateMachine().AddState(a).AttachSubMachine(a, sub);

        Assert.Throws<InvalidOperationException>(
            () => new HierarchicalStateMachine().AddState(b).AttachSubMachine(b, sub));
    }
}
