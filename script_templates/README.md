# script_templates 组织说明

Godot 脚本模板与页面参考示例按域分装：

```
script_templates/
├── .editorconfig          # 标记模板目录为 generated_code（Godot 生成代码规范）
├── Node/                  # Godot 右键"附加脚本"模板（单文件生成起点）
│   ├── ControllerTemplate.cs           # 通用控制器（IController）
│   ├── SimplePageControllerTemplate.cs # 简单 UI 页面（ISimpleUiPage + Page 层）
│   └── SimpleModelControllerTemplate.cs# 模态 UI 页面（ISimpleUiPage + Modal 层）
└── UiPage/                # UI 页面五文件 partial 参考（完整形态，可编译代码）
    ├── TemplatePage.cs                # 核心：_Ready 调用链
    ├── TemplatePage.Dependencies.cs   # 节点引用 + ReadyAsync 初始化
    ├── TemplatePage.Properties.cs     # 字段/属性/UiKeyStr
    ├── TemplatePage.Events.cs         # CQRS 事件订阅（RegisterEvents）
    └── TemplatePage.Signals.cs        # Godot 信号桥接（ConnectPageSignals）
```

## Node/（Godot 右键模板）

Godot 编辑器右键节点 → "附加脚本" → 选择模板，生成单文件脚本。
模板用 `_CLASS_` / `_BASE_` 占位符（Godot 自动替换）。

- 生成后**无 namespace**——按目录规范补 `namespace GFrameworkTemplate.scripts.xxx;`
- 生成后**按需拆 partial**（复杂页面拆 .Dependencies/.Events/.Signals 等）

## UiPage/（五文件 partial 参考）

`TemplatePage*` 是从 `scripts/menu/` 迁移的**完整可编译示例**，
演示规范推荐的 UI 页面五文件 partial 拆分粒度：

| 文件 | 职责 |
|---|---|
| `*.cs` | 核心：`_Ready()` 只做 `ReadyAsync()` → `ConnectPageSignals()` → `RegisterEvents()` |
| `*.Dependencies.cs` | `GetNode<T>("%Xxx")` 节点引用、`ReadyAsync()` 异步初始化 |
| `*.Properties.cs` | 字段、属性、`UiKeyStr` |
| `*.Events.cs` | `RegisterEvents()` 内订阅 CQRS 事件（`.UnRegisterWhenNodeExitTree(this)`） |
| `*.Signals.cs` | `ConnectPageSignals()` 内桥接 Godot 信号 → CQRS 事件 |

**新页面步骤**（参照 TemplatePage 拆分）：
1. `UiKey` 枚举加页面键
2. 复制 5 文件改名（TemplatePage → 你的页面名）
3. 按场景节点补 `*.Dependencies.cs` 引用、`*.Events.cs` / `*.Signals.cs` 逻辑
4. 在 `GameEntryPoint` 场景配置 `UiPageConfigs` 注册

> 注意：`UiPage/` 下的 .cs 作为参考**不参与编译**（csproj 排除了 script_templates），
> 实际使用请复制到 `scripts/menu/`（或业务目录）再改命名空间。
