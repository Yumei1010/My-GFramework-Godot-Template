using GFramework.Core.Abstractions.Architectures;
using GFramework.Core.Abstractions.Localization;
using GFramework.Core.Localization;
using GFramework.Game.Setting;
using GFrameworkTemplate.scripts.core.localization;
using GFrameworkTemplate.scripts.core.scene;
using GFrameworkTemplate.scripts.core.ui;

namespace GFrameworkTemplate.scripts.module;

/// <summary>
///     系统模块类，负责安装和注册框架所需的各种系统组件
/// </summary>
public class SystemModule : IArchitectureModule
{
    public void Install(IArchitecture architecture)
    {
        architecture.RegisterSystem(new UiRouter());
        architecture.RegisterSystem(new SceneRouter());
        architecture.RegisterSystem(new SettingsSystem());

        // 本地化系统：语言表从 localization/ 目录加载（编辑器直读 res://，导出自动同步至 user://）
        // 注册后 GodotLocalizationSettings 应用器才能解析到 ILocalizationManager 并联动语言切换
        architecture.RegisterSystem(new LocalizationManager(new LocalizationConfig
        {
            DefaultLanguage = "zhs",
            FallbackLanguage = "eng",
            LocalizationPath = LocalizationPathResolver.Resolve()
        }));
    }
}
