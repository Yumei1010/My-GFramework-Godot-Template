using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkTemplate.scripts.component.behavior_tree.bt_node;

/// <summary>
///     行为树选择节点（Godot 节点版）：从左到右依次尝试子节点，找到第一个成功的执行。
///     遇成功立即整体成功；全部失败才整体失败；遇执行中暂停，下一帧从该子节点继续。
/// </summary>
[Log]
public partial class BtSelectorNode : BtNode
{
    private int _currentIndex;

    /// <inheritdoc />
    public override NodeStatus Execute()
    {
        // 从上次暂停的位置继续
        while (_currentIndex < ChildNodes.Count)
        {
            var status = ChildNodes[_currentIndex].Execute();

            if (status == NodeStatus.Success)
            {
                _currentIndex = 0;
                return NodeStatus.Success;
            }

            if (status == NodeStatus.Running)
                return NodeStatus.Running;

            _currentIndex++;
        }

        _currentIndex = 0;
        return NodeStatus.Failure;
    }
}
