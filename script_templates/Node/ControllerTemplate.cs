// meta-name: 控制器类模板
// meta-description: 负责管理场景的生命周期和架构关联
using Godot;
using GFramework.Core.Abstractions.Controller;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;

/// <summary>
///     _CLASS_ 控制器——负责场景节点的生命周期与架构交互
///     [Log] + [ContextAware] 成对使用（[Log] 在前）
/// </summary>
[Log]
[ContextAware]
public partial class _CLASS_ : _BASE_, IController
{
    /// <summary>
    ///     节点准备就绪时的回调方法
    ///     按需在此调用异步初始化 / 信号连接 / 事件注册
    /// </summary>
    public override void _Ready()
    {
        // _ = ReadyAsync();
        // ConnectSignals();
        // RegisterEvents();
    }
}