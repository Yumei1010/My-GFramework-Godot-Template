// meta-name: 控制器类模板
// meta-description: 负责管理场景的生命周期和架构关联（语法糖版：GetNode + 字段注入）
using Godot;
using GFramework.Core.Abstractions.Controller;
using GFramework.Godot.SourceGenerators.Abstractions;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;

/// <summary>
///     _CLASS_ 控制器——负责场景节点的生命周期与架构交互
///     [Log] + [ContextAware] 成对使用（[Log] 在前）；节点/依赖用字段注入语法糖
/// </summary>
[Log]
[ContextAware]
public partial class _CLASS_ : _BASE_, IController
{
    // 示例：架构组件字段注入（编译期生成，替代 this.GetSystem<T>()）
    // [GetSystem] private ISomeSystem _system = null!;
    // [GetNode]   private Label _statusLabel = null!;

    /// <summary>
    ///     节点准备就绪时的回调方法
    ///     注意：若使用 [GetNode] 字段，需在 _Ready 开头调用 __InjectGetNodes_Generated()
    /// </summary>
    public override void _Ready()
    {
        // __InjectGetNodes_Generated();
        // _ = ReadyAsync();
        // ConnectSignals();
        // RegisterEvents();
    }
}
