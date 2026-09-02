# GFramework 0.7.1 语法糖研究

分支：`feat/upgrade-gframework`
日期：2026-09-02

## 结论（TL;DR）

0.7.1 新增了大量 **SourceGenerator 语法糖**，我们当前引用的包**已全部包含**（无需加包）：
- `GFramework.Core.SourceGenerators` 0.7.1 → 依赖注入字段特性 + 模块声明式注册
- `GFramework.Godot.SourceGenerators` 0.7.1 → Godot 节点/信号/页面样板生成

## 语法糖清单

### A. Core 语法糖（`GFramework.Core.SourceGenerators.Abstractions.*`）

| 特性 | 作用 | 替代当前写法 |
|---|---|---|
| `[GetSystem]` / `[GetModel]` / `[GetUtility]` / `[GetService]`（含 `[GetXxxs]` 集合版） | **字段自动注入**架构组件 | `this.GetSystem<T>()` 手动获取 |
| `[AutoRegisterModule]` + `[RegisterModel]` / `[RegisterSystem]` / `[RegisterUtility]` | **模块声明式注册**（编译期生成 Install） | 手写 `Install(IArchitecture)` 里的 RegisterXxx |
| `[GenerateEnumExtensions]` | 枚举扩展方法生成 | 手写枚举工具 |
| `[Priority]` | 系统/模块优先级声明 | — |

```csharp
// 用法：字段注入（替代 this.GetSystem<T>()）
[Log]
[ContextAware]
public partial class TemplatePage : Control, ...
{
    [GetSystem] private IUiRouter _uiRouter = null!;  // 编译期生成注入
}
```

### B. Godot 语法糖（`GFramework.Godot.SourceGenerators.Abstractions.*`）

| 特性 | 作用 | 替代当前写法 |
|---|---|---|
| `[GetNode]` | **节点字段注入**：字段名 → `%唯一名` 自动 GetNode，自动生成 `_Ready` 注入 | `.Dependencies.cs` 里 `GetNode<T>("%Xxx")` 表达式属性 |
| `[BindNodeSignal]` | **信号绑定生成**：自动 Connect + `_ExitTree` 自动 Unbind | `.Signals.cs` 手写 `Button.Pressed += ...` |
| `[AutoUiPage]` | 页面样板生成：`UiKeyStr` + `GetPage()` + 缓存字段 | `TemplatePage` 手写 `IUiPageBehavior? _page` + `GetPage()` |
| `[AutoScene]` | 场景注册样板 | — |
| `[AutoRegisterExportedCollections]` | 配置数组批量注册（`UiPageConfigs` 等遍历注册收敛） | GameEntryPoint 手写 `foreach` 注册循环 |

```csharp
// 用法：节点注入 + 信号绑定 + 页面样板（一行特性替代多个文件样板）
[AutoUiPage(nameof(UiKey.TemplatePage), nameof(UiLayer.Page))]
public partial class TemplatePage : Control
{
    [GetNode] private Button _startButton = null!;

    [BindNodeSignal(nameof(_startButton), nameof(Button.Pressed))]
    private void OnStartPressed() { }
}
```

## 对模板的影响评估

| 语法糖 | 建议 | 理由 |
|---|---|---|
| `[GetNode]` | ⭐ 值得引入 | 消除 `.Dependencies.cs` 大量重复 `GetNode<T>("%Xxx")` 样板 |
| `[AutoUiPage]` | ⭐ 值得引入 | 消除每个页面的 `_page` 缓存 + `GetPage()` + `UiKeyStr` 样板 |
| `[GetSystem]` 等字段注入 | 可选 | 替代 `this.GetXxx()`，看个人偏好 |
| `[BindNodeSignal]` | 可选 | 信号多时有用，但隐式连接不易排查 |
| `[AutoRegisterModule]` | 可选 | 4 个模块目前手写清晰，声明式会损失可见顺序 |
| `[AutoRegisterExportedCollections]` | 可选 | GameEntryPoint 注册循环收敛 |

## 注意事项

- 语法糖需 `partial class` + 不支持嵌套类
- `[GetNode]` 默认按字段名推断 `%唯一名`（`_leftContainer` → `%LeftContainer`），可显式 `Path`/`Lookup`/`Required`
- 生成器自动补 `_Ready`（若无）——注意与手写 `_Ready` 的配合（生成的是 partial 方法钩子）
- `[ContextAware]` 仍是字段注入的前置要求
- 模板骨架**逐步演进**：先改 `TemplatePage` 演示 `[AutoUiPage]`+`[GetNode]`，验证后再推广

## ✅ 验证结论（Godot 实测通过）

探针类实测（`[AutoUiPage]` + `[GetNode]`）：
- `UiKeyStr=TemplatePage` ✅（AutoUiPage 生成）
- `GetPage=PageLayerUiPageBehavior` ✅（AutoUiPage 按 UiLayer.Page 生成）
- `_startButton=Button` / `_titleLabel=Label` ✅（GetNode 字段名 → `%唯一名` 注入）

### 关键用法注意

- **`[GetNode]` + 手写 `_Ready`**：需在 `_Ready` 开头手动调 `__InjectGetNodes_Generated()`
  （否则生成器报 warning GF_Godot_GetNode_006 且注入不生效）
- **不写 `_Ready`**：生成器自动补 `_Ready` + 注入 + `partial void OnGetNodeReadyGenerated()` 钩子
- `[AutoUiPage]` 无需 `_Ready`（只生成 UiKeyStr/GetPage/缓存字段）

## 待办（下一步）

- [ ] 决定模板是否引入 `[AutoUiPage]` / `[GetNode]`（减少 .Dependencies/.Properties 样板）
- [ ] 评估五文件 partial 模式是否仍推荐（语法糖可能压缩拆分需求）
