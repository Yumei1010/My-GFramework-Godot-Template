using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.behavior_tree.bt_node;

/// <summary>
///     行为树顺序节点（Godot 节点版）：从左到右依次执行子节点。
///     遇失败立即整体失败；全部成功才整体成功；遇执行中暂停，下一帧从该子节点继续。
/// </summary>
[Log]
public partial class BtSequenceNode : BtNode
{
    private int _currentIndex;

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        // 从上次暂停的位置继续，避免每帧从头重跑
        while (_currentIndex < ChildNodes.Count)
        {
            var status = ChildNodes[_currentIndex].Execute();

            if (status == NodeStatus.Failure)
            {
                _currentIndex = 0;
                return NodeStatus.Failure;
            }

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            _currentIndex++;
        }

        _currentIndex = 0;
        return NodeStatus.Success;
    }
}
