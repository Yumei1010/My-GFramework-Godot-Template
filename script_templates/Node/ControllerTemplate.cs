// meta-name: 控制器类模板
// meta-description: 负责管理场景的生命周期和架构关联
using Godot;
using GFramework.Core.Abstractions.Controller;
using GFramework.Core.SourceGenerators.Abstractions.Logging;
using GFramework.Core.SourceGenerators.Abstractions.Rule;


[Log]
[ContextAware]
public partial class _CLASS_ :_BASE_,IController
{
    /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        
    }
}

