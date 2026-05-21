# My GFramework Godot Template

基于 [GFramework](https://github.com/GeWuYou/GFramework) (v0.0.177) 的 Godot 4.6 项目起手模板，**经 [Twenty-four](https://github.com/Yumei1010/Twenty-four) 项目重度实战魔改后反向提炼**，贴合个人使用习惯。

## 项目渊源

```
GFramework（上游 CQRS/ECS 框架）
    ↓
Twenty-four（24 点游戏，重度魔改 GFramework 用法）
    ↓
My-GFramework-Godot-Template（从 Twenty-four 剥离业务逻辑，保留骨架）
```

本模板并非 GFramework 官方模板，而是 Twenty-four 项目在大量实战中沉淀下来的**个人偏好配置**——包括 DI 模块划分、partial class 拆分粒度、CQRS 事件/命令的 `sealed` + `init` 约束、XML 注释规范等，都经过了真实项目的反复打磨。

## 技术栈

- **引擎：** Godot 4.6 (.NET)
- **运行时：** .NET 10
- **框架：** GFramework 0.0.177（NuGet: `GeWuYou.GFramework`）
- **语言：** C# (LangVersion preview)

## 骨架包含

| 层级 | 内容 |
|---|---|
| DI 引导 | `GameArchitecture` + 4 模块（Model/System/Utility/State） |
| 路由 | `UiRouter`、`SceneRouter`、`UiFactory` |
| 状态机 | `GameStateMachineSystem` + `AppState` 示例 |
| 全局节点 | `GameEntryPoint`、`UiRoot`、`SceneRoot`、`SceneTransitionManager` |
| CQRS 示例 | 音量控制、分辨率/全屏切换、设置存取、退出游戏 |
| 通用组件 | `VolumeContainer`、`IState` |
| 编码模板 | `script_templates/` 下 3 个 Godot 脚本模板 |
| 编码规范 | `CONVENTIONS.md` — 命名空间、CQRS、partial class、XML 注释等全套约束 |
| 素材 | 像素字体、场景过渡 Shader、文本特效 Shader |

## 快速开始

```bash
# 克隆
git clone https://github.com/Yumei1010/My-GFramework-Godot-Template.git MyNewProject
cd MyNewProject

# 构建
dotnet build

# 用 Godot 打开 project.godot 即可开始开发
```

## 与新项目对接

1. 全局替换命名空间 `GFrameworkTemplate` → 你的项目名
2. 重命名 `.csproj`、`.sln` 文件
3. 在 `scripts/enums/ui/UiKey.cs` 中添加你的 UI 页面键
4. 在 `scripts/enums/scene/SceneKey.cs` 中添加你的场景键
5. 在 `scripts/core/state/impls/` 下创建你的状态
6. 在 `scripts/module/StateModule.cs` 中注册新状态
7. 开始在 `scripts/cqrs/`、`scripts/entities/`、`scripts/menu/` 下添加业务代码

## 编码规范

详见 [CONVENTIONS.md](CONVENTIONS.md) 和 [CLAUDE.md](CLAUDE.md)（后者供 Claude Code 使用）。
