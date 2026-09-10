# GFramework 服务层扩展组件研究（Config / Data / Storage / Resource / Coroutine / RichText）

> 日期：2026-09 | 源码：`GFramework.Game/` + `GFramework.Godot/` + `GFramework.Core/`
> 对应文档：`GFramework/docs/zh-CN/game/*.md`（config-system/data/storage/scene/ui/serialization/setting/input）、`core/coroutine.md`
> 定位：续 input/pause/store 三篇，盘点其余 Game/Godot 层能力与模板接入优先级

## 总览

| 域 | 状态 | 模板现状 | 价值判断 |
|---|---|---|---|
| **Config（YAML 配置）** | ✅ 旗舰能力（20+ 文件） | ❌ 未接入 | ⭐⭐⭐ 数据驱动配置，玩法数据首选 |
| **Data（数据仓库）** | ✅ 三仓库齐全 | 部分（Unified 设置仓库） | ⭐⭐⭐ 多槽位存档 + 版本迁移 |
| **Storage（存储）** | ✅ 完整 | ✅ 已用（GodotFileStorage） | — |
| **Serialization** | ✅ JSON | ✅ 已用 | — |
| **Resource（资源管理）** | ✅ 完整（含引用计数） | ❌ 未接入 | ⭐⭐ 大资源项目必需 |
| **Coroutine（协程）** | ✅ 完整调度器 | 部分（Timing.Prewarm + 转场） | ⭐⭐ 复杂时序编排 |
| **RichText 效果** | ✅ 9 内置效果 | ❌ 未接入 | ⭐ 表现层锦上添花 |
| **UI/Scene 扩展点** | ✅ Guard/Handler | 部分（LoggingTransitionHandler） | ⭐⭐ 转场/守卫业务化 |

---

## 一、Config 配置系统（旗舰能力，最值得接入）

**一句话**：YAML 写配置 + JSON Schema 描述结构 + Source Generator 生成强类型访问代码 + VS Code 插件编辑，**AI-First 配置工作流**。

### 能力清单

| 能力 | 说明 |
|---|---|
| YAML 源文件 | 一对象一文件，目录组织 |
| JSON Schema | 结构描述 + 校验（20+ 关键字：enum/const/pattern/format/allOf/if-then-else/min-max/uniqueItems/contains/dependentRequired…） |
| Source Generator | 生成配置类型、表包装（Table）、注册辅助、项目级 `GeneratedConfigCatalog` |
| 运行时只读查询 | `registry.GetMonsterTable().Get(1)` |
| 聚合注册 | `RegisterAllGeneratedConfigTables()` 一行注册全部表 |
| 热重载 | `YamlConfigHotReloadOptions` |
| VS Code 工具 | 配置浏览/raw 编辑/schema 打开/轻量校验/嵌套表单 |

### 目录与文件格式

```
config/monster/slime.yaml     ← 配置数据（一对象一文件）
schemas/monster.schema.json   ← 结构描述（x-gframework-config-path 指定数据目录）
```

```json
{
  "title": "Monster Config",
  "x-gframework-config-path": "config/monster",
  "type": "object",
  "required": ["id", "name"],
  "properties": {
    "id": { "type": "integer" },
    "name": { "type": "string", "default": "Slime" },
    "hp": { "type": "integer", "multipleOf": 5 },
    "rarity": { "type": "string", "enum": ["common", "rare", "boss"] },
    "dropItems": { "type": "array", "uniqueItems": true, "items": { "type": "string" },
                   "x-gframework-ref-table": "item" }
  }
}
```

```yaml
# config/monster/slime.yaml
id: 1
name: Slime
hp: 10
dropItems: [potion, slime_gel]
```

### 运行时接入（三步）

```csharp
using GFramework.Game.Config;
using GFramework.Game.Config.Generated;   // 源生成器产出

// ① 建注册表
var registry = new ConfigRegistry();

// ② 加载（聚合注册全部已生成表）
var loader = new YamlConfigLoader("config-root")
    .RegisterAllGeneratedConfigTables();
await loader.LoadAsync(registry);

// ③ 强类型读取
var monsterTable = registry.GetMonsterTable();
var slime = monsterTable.Get(1);
```

生成器固化的约定（可读，避免硬编码字符串）：

```csharp
MonsterConfigBindings.ConfigDomain.ConfigDomain            // 配置域
MonsterConfigBindings.Metadata.TableName                   // "monster"
MonsterConfigBindings.Metadata.ConfigRelativePath          // 目录
MonsterConfigBindings.Metadata.SchemaRelativePath          // schema 路径
GeneratedConfigCatalog.Tables                              // 项目级表目录
```

**裁剪启动集**（只要部分表）：

```csharp
new YamlConfigLoader("config-root").RegisterAllGeneratedConfigTables(
    new GeneratedConfigRegistrationOptions
    {
        IncludedConfigDomains = new[] { MonsterConfigBindings.ConfigDomain },
        ItemComparer = StringComparer.OrdinalIgnoreCase
    });
```

### 对项目的意义

- **数据驱动**：玩法数据（关卡/牌型/词条/角色）从代码里挪出来，策划/自己用 YAML 改
- **强类型安全**：schema 校验 + 源生成器，写错字段编译期/启动期就报
- **与 AI 协作友好**：schema 是机器可读契约，AI 生成配置有据可依
- 对应 Twenty-four：`Rule` 定义、`ChallengeModifier` 词条表、关卡表都可走这套

### ⚠️ 已知边界（实测发现，2026-09）

**schema 数组字段 + 生成配置类的组合当前不可用**：

- 生成器把数组属性生成为 `IReadOnlyList<T> { get; set; }`
- 但 `YamlConfigLoader` 用 YamlDotNet 16.3 默认反序列化，**无法实例化 `IReadOnlyList<T>`**（报 "No node deserializer was able to deserialize the node into type IReadOnlyList`1"）
- 铁证：最小 case（YamlDotNet 直反序列化 IReadOnlyList 属性）同样失败

**规避**：schema 演示暂不用数组字段（用标量/嵌套对象替代）。若项目必须数组配置，需等框架修复或自行扩展 loader（当前不建议 fork）。

---

## 二、Data 数据仓库（三个仓库，按需选）

### 三仓库分工

| 仓库 | 适合场景 | 落盘形态 |
|---|---|---|
| `DataRepository` | 单份玩家档案 / 运行时缓存 / 一条 location 一个文件 | 每 key 一文件（可选 `.backup`） |
| `UnifiedSettingsDataRepository` | 音频/图形/语言多 section 统一落一份 | `settings.json` 单文件 |
| `SaveRepository<TSaveData>` | **多槽位存档 + 版本迁移 + 列举/删除槽位** | 按 `SaveRoot/SlotPrefix/FileName` 组织 |

### 关键语义（易踩坑）

```csharp
// DataRepository
await repo.LoadAsync<T>(location);   // ⚠️ 文件不存在返回 new T()，不抛异常
await repo.DeleteAsync(location);    // 只在真实删除时发删除事件
await repo.SaveAllAsync(...);        // 批量提交：抑制逐项事件，只发一次 BatchSaved

// SaveRepository<TSaveData>
await save.LoadAsync(slot);          // ⚠️ 槽位不存在返回新实例，不是 null
await save.ListSlotsAsync();         // 只返回真实存在的槽位，升序
// 版本迁移成功后自动回写槽位文件
```

### 版本迁移链（与 `ISaveMigration` 配套）

```csharp
public sealed class SaveV1ToV2 : ISaveMigration<GameSave>
{
    public int FromVersion => 1;
    public int ToVersion => 2;
    public GameSave Migrate(GameSave old) => old with { NewField = 默认值 };
}
// 仓库加载时按 IVersionedData 判断版本，串起迁移链
```

### 边界（文档明确）

- Data 仓库管**怎么落盘/回读/组织槽位**，不放宽配置契约
- 复杂 schema（`oneOf`/`anyOf`/复杂 `additionalProperties`）不归 repository 管，回 Config 系统处理

---

## 三、Storage 存储层

| 实现 | 说明 |
|---|---|
| `IStorage` / `IFileStorage` | 存储抽象（读写/删除/存在性） |
| `FileStorage`（Game 层） | 通用文件存储实现 |
| `GodotFileStorage`（Godot 层） | **Godot 路径适配**（`user://` 等）——模板已用 |
| `ScopedStorage` | 作用域存储（`IScopedStorage`），限定子目录/命名空间 |

**路径语义**：Godot 用 `user://` / `res://`，Storage 层负责适配；上层 repository 基于它落盘。模板已在 `UtilityModule` 注册 `GodotFileStorage`。

> 与上层关系：`Storage`（怎么读写文件）→ `DataRepository`（怎么组织业务数据）→ Model（怎么用数据）。

---

## 四、Serialization

- `ISerializer` / `JsonSerializer`（Game 层实现）——模板已注册
- `IRuntimeTypeSerializer`：运行时类型感知序列化
- 文档 `game/serialization.md` 讲"配置生命周期"与存储/配置系统的关系

---

## 五、Resource 资源管理（模板未接入）

### 能力

```csharp
public interface IResourceManager : IUtility
{
    int LoadedResourceCount { get; }
    T? Load<T>(string path) where T : class;
    Task<T?> LoadAsync<T>(string path) where T : class;
    IResourceHandle<T>? GetHandle<T>(string path) where T : class;  // 带引用计数
    bool Unload(string path);
    void UnloadAll();
    bool IsLoaded(string path);
    void RegisterLoader<T>(IResourceLoader<T> loader) where T : class;
    Task PreloadAsync<T>(string path) where T : class;
    IEnumerable<string> GetLoadedResourcePaths();
    void SetReleaseStrategy(IResourceReleaseStrategy strategy);
}
```

### 关键机制

| 机制 | 说明 |
|---|---|
| `IResourceLoader<T>` | 按类型注册加载器（`Load/LoadAsync/Unload/CanLoad`）——资源类型可扩展 |
| `IResourceHandle<T>` | 句柄 + **引用计数**（`AddReference/RemoveReference/ReferenceCount`） |
| `AutoReleaseStrategy` | 引用数 ≤ 0 时自动释放 |
| `ManualReleaseStrategy` | **永不自动释放**，由 `Unload` 显式控制 |
| `ResourceCache` / `ResourceCacheEntry` | 缓存层 |

**接入路径**：注册 `ResourceManager` → 为每类资源实现 `IResourceLoader<T>`（如 Godot 的纹理/场景加载器）→ 用 `Load/GetHandle` 取用 → 按策略卸载。

> 模板现状：模板用 `GodotTextureRegistry`（注册表）+ 直接 `GD.Load`，**未走 ResourceManager**。项目资源量大时建议接入（引用计数 + 自动释放）。

---

## 六、Coroutine 协程（比 Godot 原生强）

### 能力

| 组件 | 说明 |
|---|---|
| `CoroutineScheduler` | 调度核心：`DeltaTime`/`RealtimeDeltaTime`/`ExecutionStage`/`ActiveCoroutineCount` |
| 可观测性 | `Statistics`、`GetActiveSnapshots()`、`TryGetSnapshot(handle)`、`WaitForCompletionAsync(handle)` |
| 事件 | `OnCoroutineException`、`OnCoroutineFinished` |
| `IYieldInstruction` | 等待指令体系（时间/帧/条件/Task/事件） |
| `CoroutineHelper` | 辅助创建 |
| 协程组合 | 支持组合（文档 `core/coroutine.md`） |
| `GodotTimeSource` | Godot 时间源适配 |
| `Timing` | 静态入口（模板 `GameEntryPoint` 调了 `Timing.Prewarm()`） |

### 常用等待指令（文档）

- **时间与帧**：等待秒数 / 帧数 / 下一帧
- **条件等待**：等到条件成立
- **Task 桥接**：等待 C# Task
- **等待事件**：等事件触发

### 为什么比 Godot 原生协程强

Godot 的 `await ToSignal(...)` 无法：统计活跃协程数、拿快照、统一异常处理、等待完成状态。GFramework 的调度器都可做——**调试和资源清理更可控**。

模板现状：`SceneTransitionManager` 用了 `IYieldInstruction`，`GameEntryPoint` 调了 `Timing.Prewarm()`——**框架协程已在部分使用**。

---

## 七、RichText 效果系统（Godot 表现层）

### 能力

| 组件 | 说明 |
|---|---|
| `GfRichTextLabel` | 富文本控件（超集） |
| `RichTextProfile` | 效果预设资源（`CreateBuiltInDefault()`） |
| `IRichTextEffectRegistry` / `DefaultRichTextEffectRegistry` | 效果注册表 |
| `RichTextEffectsController` | 效果控制器（`RefreshEffects()`） |
| `RichTextMarkup` / `RichTextEffectPlan` | 标记解析 + 效果编排 |
| **9 个内置效果** | Blue / FadeIn / FlyIn / Gold / Green / Jitter / Red / Sine（`Text/Effects/`） |
| `RichTextEffectBase` | 自定义效果基类 |

**用途**：对话文字逐字淡入、伤害数字抖动/金色、关键提示呼吸（Sine）等——**VN/叙事类项目直接可用**。

---

## 八、UI / Scene 扩展点（业务化钩子）

### 路由守卫（`IUiRouteGuard` / `ISceneRouteGuard`）

```csharp
Task<bool> CanEnterAsync(key, param);   // 进入前检查
Task<bool> CanLeaveAsync(key);          // 离开前检查
```

适合：未保存拦截、解锁条件、新手引导限制。

### 过渡处理器（Handler 管道）

```csharp
uiRouter.RegisterHandler(IUiTransitionHandler handler, UiTransitionHandlerOptions? options);
sceneRouter.RegisterHandler(ISceneTransitionHandler handler, options);
sceneRouter.RegisterAroundHandler(ISceneAroundTransitionHandler handler, options);
```

适合：转场动画（黑幕/淡入淡出/loading）、统一日志（模板已用 `LoggingTransitionHandler`）、栈变化埋点、指标采集。

> 文档建议：**复杂过渡逻辑放进 Handler，不要把 Router 派生类做成巨型协调器。**

### UI 分层行为

`PageLayer/OverlayLayer/ModalLayer/ToastLayer/TopmostLayer` 各层 `UiPageBehaviorBase`（Godot 层）——分层语义的现成实现。

---

## 九、模板接入优先级建议

| 优先级 | 项 | 理由 |
|---|---|---|
| **P0** | **Config 配置系统** | 数据驱动的地基；Twenty-four 的 Rule/词条/关卡数据直接受益 |
| **P0** | **SaveRepository + ISaveMigration** | 多槽位存档 + 版本迁移，Twenty-four run 存档必需 |
| **P1** | Resource 资源管理 | 资源量大时必需（引用计数/自动释放） |
| **P1** | Coroutine 调度器 | 已部分使用，补全统计/快照能力 |
| **P2** | UI/Scene RouteGuard + Handler | 业务化钩子（拦截/转场动画） |
| **P2** | RichText 效果系统 | 表现层，叙事类项目优先 |
| **P3** | DataRepository / ScopedStorage | 按需（有单档案/命名空间隔离需求时） |

**已接入无需动**：Storage（GodotFileStorage）、Serialization（JsonSerializer）、Setting（UnifiedSettingsDataRepository）。

## 相关

- 扩展总览：`docs/research/gframework-extensions/README.md`
- 已研究：`input-domain.md`（输入三层协议）、`pause-stack.md`（分组暂停）、`store.md`（Redux 状态）
- 框架文档：`docs/zh-CN/game/config-system.md`（1008 行，含完整接入模板）、`config-tool.md`、`data.md`、`storage.md`、`scene.md`、`ui.md`、`serialization.md`、`setting.md`、`docs/zh-CN/core/coroutine.md`
