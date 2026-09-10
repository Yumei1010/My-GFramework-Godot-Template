# Config 配置系统教程（YAML + JSON Schema + Source Generator）

> 本文纯教学：讲清"为什么需要 Config + 怎么用 GFramework 配置系统"。
> 框架已带完整实现（`ConfigRegistry` / `YamlConfigLoader` / `GodotYamlConfigLoader` / `GameConfigBootstrap`），无需造轮子。
> 模板已接入演示域（`config/monster`），本文以它为例从零讲起。

---

## 0. 一句话先说明白

**Config = 用 "JSON Schema 定义结构 + YAML 写数据 + 源码生成器产出强类型访问代码" 管理游戏静态内容配置。**

它解决：怪物数值、道具表、关卡、词条这类**静态内容数据**不该硬编码在 C# 里（改数值要改代码），也不该手写 JSON 解析（无类型安全、无校验）。

> **边界**：Config 管**静态内容**（只读）；玩家存档/设置这类**运行时持久化**走 Data/Storage 仓库（见 `game-services.md`）。

---

## 1. 为什么需要它（老写法的痛）

| 方案 | 痛点 |
|---|---|
| 硬编码 C# | 改数值动代码、要重新编译、策划没法参与 |
| 手写 JSON 解析 | 无类型安全（写错字段 0 报错）、无校验、无默认值 |
| 散落常量 | 一处改全局乱，无从查起 |

这套系统把配置切成三件套：**Schema 定义结构 → YAML 提供数据 → 生成器产出强类型代码**，让"写错"在**编译期或加载期**就暴露。

---

## 2. 全链路一张图

```
【设计时】                          【编译时】                        【运行时】
schemas/monster.schema.json ──► Source Generator ──► MonsterConfig（强类型类）
（结构契约，JSON Schema）           （Roslyn 自动）      MonsterTable（表包装）
                                                       GeneratedConfigCatalog（全表目录）
config/monster/slime.yaml ────► YamlConfigLoader（读取+校验）──► ConfigRegistry
（数据，一对象一文件）                 │                          （注册表）
                                      ▼
                              game 代码：Registry.GetMonsterTable().Get("slime")
```

**关键：schema 是唯一契约源**——从它生成类型、从它校验数据、从它驱动编辑器工具，三端共用，不漂移。

---

## 3. 核心概念（四个部件）

| 部件 | 是什么 | 一句话 |
|---|---|---|
| **Schema** | `schemas/*.schema.json` | 结构契约：允许哪些字段、什么类型、什么约束 |
| **YAML** | `config/<域>/*.yaml` | 实际数据：一对象一文件 |
| **Source Generator** | 编译期 Roslyn 生成 | 从 schema 生成强类型类/表/目录代码 |
| **Registry + Table** | 运行时 | 加载后的只读注册表 + 强类型表查询 |

### Schema（决定"能写什么"）

```json
// schemas/monster.schema.json
{
  "title": "Monster Config",
  "x-gframework-config-path": "config/monster",
  "type": "object",
  "required": ["id", "name", "hp"],
  "properties": {
    "id":      { "type": "string", "enum": ["slime", "goblin"] },
    "name":    { "type": "string", "default": "Slime" },
    "hp":      { "type": "integer", "minimum": 1, "maximum": 1000, "multipleOf": 5 },
    "rarity":  { "type": "string", "enum": ["common", "rare", "boss"], "default": "common" }
  }
}
```

- `x-gframework-config-path`：指定数据目录（默认用 schema 基名）
- 顶层元数据：`title`（工具显示）/ `description`（XML 注释）

### YAML（数据怎么写）

```yaml
# config/monster/slime.yaml
id: slime
name: Slime
hp: 10
rarity: common
```

**一对象一文件**：加新条目 = 增加一个 yaml，不用动代码。

### 生成代码（你拿到什么）

- `MonsterConfig`：强类型配置类（属性 = schema properties）
- `MonsterTable`：表包装（`Get(key)` / `TryGet` / `ContainsKey` / `All` / `FindByXxx`）
- `MonsterConfigBindings.Metadata`：表名/目录/schema 路径常量
- `GeneratedConfigCatalog`：项目级全表目录（枚举/按域筛选）

> 命名空间固定 `GFramework.Game.Config.Generated`，不随项目变。

---

## 4. 目录结构（模板约定）

```text
My-GFramework-Godot-Template/
├─ config/                        # 运行时加载的 YAML 数据
│  ├─ README.md                   # 注明教学示例 + 替换指引
│  └─ monster/                    # 演示域（示例数据，开发时替换为自己的域）
│     ├─ slime.yaml
│     ├─ goblin.yaml
│     └─ orc.yaml
├─ schemas/                       # 生成器自动收集（NuGet targets 已接好）
│  └─ monster.schema.json
└─ scripts/
   ├─ module/ConfigModule.cs      # 架构模块：Godot 桥接加载 + 注册
   └─ core/config/ConfigRuntime.cs# 业务读取入口（强类型）
```

**csproj 零改动**：`GeWuYou.GFramework.Game.SourceGenerators` 包的 targets 自动把 `schemas/**/*.schema.json` 加为 AdditionalFiles。

---

## 5. 五分钟上手（最小完整例子）

### 第 1 步：建 schema + yaml（见第 3 节示例）

### 第 2 步：清缓存构建，生成器产出强类型代码

```bash
# ⚠️ 若新增/修改 schema 后生成代码不更新，先清 Godot 编译缓存：
rm -rf .godot/mono/temp/obj && dotnet build
```

### 第 3 步：加载（Godot 桥接版，模板 `ConfigModule` 做法）

```csharp
using GFramework.Game.Config;
using GFramework.Game.Config.Generated;
using GFramework.Godot.Config;

// 表清单从生成器元数据自动拿（不用手写表名/路径）
var tableSources = GeneratedConfigCatalog.Tables
    .Select(static m => new GodotYamlConfigTableSource(
        m.TableName, m.ConfigRelativePath, m.SchemaRelativePath))
    .ToArray();

var loader = new GodotYamlConfigLoader(new GodotYamlConfigLoaderOptions
{
    SourceRootPath = "res://",
    RuntimeCacheRootPath = "user://config_cache",
    TableSources = tableSources,
    ConfigureLoader = static y => y.RegisterAllGeneratedConfigTables()
});

var registry = new ConfigRegistry();
await loader.LoadAsync(registry);   // ⚠️ 加载+校验都在这，失败会抛 ConfigLoadException
```

### 第 4 步：强类型读取

```csharp
var slime = registry.GetMonsterTable().Get("slime");
GD.Print(slime.Hp);          // 10
GD.Print(slime.Rarity);      // common
```

---

## 6. 校验能力（加载即校验，写错就失败）

绑定 schema 的表在加载时**拒绝**：

| 类别 | 校验项 |
|---|---|
| 结构 | 缺失必填字段、未知字段（未在 schema 声明） |
| 类型 | 标量 / 数组 / 嵌套对象类型不匹配 |
| 数值 | `minimum` / `maximum` / `exclusiveMin/Max` / `multipleOf` |
| 字符串 | `minLength` / `maxLength` / `pattern` / `format` |
| 数组 | `minItems` / `maxItems` / `uniqueItems` / `contains` |
| 对象 | `minProperties` / `maxProperties` / `dependentRequired` / `dependentSchemas` / `allOf` / `if-then-else` |
| 值域 | `const` / `not` / `enum` |
| 跨表 | `x-gframework-ref-table` 引用目标缺失 |

`format` 稳定子集：`date` / `date-time` / `duration` / `email` / `time` / `uri` / `uuid`（例如 duration 只支持 `P2D`/`PT45M` 这种 day-time，time 必须带时区偏移）。

---

## 7. 跨表引用（关系配置）

```json
"dropItemId": {
  "type": "string",
  "x-gframework-ref-table": "item"
}
```

- 加载时校验目标行存在（类似外键）
- 热重载时目标表变更导致依赖失效 → **整体回滚受影响表**，注册表不进不一致状态
- 生成器暴露引用元数据：`MonsterConfigBindings.References.All`

---

## 8. Godot 桥接（编辑器 / 导出态自动处理）

`GodotYamlConfigLoader` 按环境自动切换：

| 环境 | 行为 |
|---|---|
| **编辑器** | `GlobalizePath("res://...")` 直接给底层 loader |
| **导出** | 把用到的 YAML/schema **同步到 `user://config_cache`** 再加载 |

**两个坑**：
1. **导出预设必须显式包含 `.yaml/.json/.schema.json`**，否则导出包里没文件
2. 依赖 `user://` 缓存时**热重载不可用**（明确拒绝，不假装支持）

---

## 9. 模板接入方式（ConfigModule + ConfigRuntime）

模板已接入完整链路：

### ConfigModule（架构模块，`scripts/module/ConfigModule.cs`）

- 用 `GeneratedConfigCatalog` 元数据自动列出全部表 → 零手写表名
- `GodotYamlConfigLoader` 加载（res:// + user://cache）
- 加载完成后把 `ConfigRegistry` 和 `ConfigRuntime` 注册进架构（`RegisterUtility`）
- 安装顺序：`Utility → System → Model → **Config** → State`（State 之前，状态可用配置）

### ConfigRuntime（读取入口，`scripts/core/config/ConfigRuntime.cs`）

```csharp
// 任意 [ContextAware] 节点/类里：
var config = this.GetUtility<ConfigRuntime>()!;
var slime = config.GetMonster("slime");   // 强类型，不碰字符串表名
```

> 开发自己的游戏：替换 `GetMonster` 为你业务域的表方法（如 `GetDifficulty`）；ConfigModule 零改动。

---

## 10. 热重载（可选）

```csharp
var loader = new GodotYamlConfigLoader(options);
await loader.LoadAsync(registry);
if (loader.CanEnableHotReload)
{
    loader.EnableHotReload(registry, new YamlConfigHotReloadOptions
    {
        OnTableReloaded = name => GD.Print($"重载: {name}")
    });
}
```

仅编辑器态（源目录可直接读）可用；`user://` 缓存态会拒绝。

---

## 11. 已知边界与常见坑（实测）

| 坑 | 说明与规避 |
|---|---|
| **数组字段不可用** ❗ | schema 数组生成 `IReadOnlyList<T>`，但 YamlDotNet 16.3 无法反序列化（报 "No node deserializer"）。**规避：不用数组字段**（用标量/嵌套对象替代），等框架修复 |
| **生成代码不更新** | 加/改 schema 后 building 无新类型 → **先清 `.godot/mono/temp/obj` 再 build** |
| **命名空间固定** | 生成代码在 `GFramework.Game.Config.Generated`，不随项目 RootNamespace 变 |
| **不支持开放 shape** | `oneOf`/`anyOf`/`additionalProperties: true`/`patternProperties`/`prefixItems` 解析/生成期直接拒绝 |
| **`allOf` 不加字段** | `allOf` 只能叠加 required/约束，不把新字段并回父对象；字段先在 `properties` 声明 |
| **Config ≠ 存档** | 静态内容用 Config；玩家进度/设置用 Data/Storage 仓库 |
| **导出丢文件** | export_presets 显式包含 .yaml/.json/.schema.json |

---

## 12. 什么时候用 / 不用

| 情况 | 选择 |
|---|---|
| 怪物/道具/关卡/词条等**静态内容** | ✅ Config |
| 运行时动态变化、随进度写回的数据 | ❌ 走 Data/Storage |
| 玩家存档 / settings（要读写） | ❌ 走 Data 仓库 |
| 只有一两个小配置 | ⚠️ 可评估；schema 层值得但可能过度 |

---

## 13. API 速查

| API | 说明 |
|---|---|
| `YamlConfigLoader(rootPath)` | 纯 .NET 文件系统版加载器 |
| `GodotYamlConfigLoader(options)` | Godot 桥接（res:// / user://cache） |
| `loader.RegisterTable<TKey,TValue>(name, relPath, schemaPath, keySelector)` | 手动注册单表 |
| `loader.RegisterAllGeneratedConfigTables()` | **生成器聚合注册全部表**（推荐） |
| `loader.LoadAsync(registry)` | 加载 + 校验 |
| `loader.EnableHotReload(registry, options)` | 热重载 |
| `new ConfigRegistry()` | 表注册表 |
| `registry.GetXxxTable()` | 强类型表（生成扩展） |
| `table.Get(key)` / `TryGet` / `ContainsKey` / `All()` / `FindByXxx()` | 表查询 |
| `GeneratedConfigCatalog.Tables` | 全表目录元数据 |
| `XxxConfigBindings.Metadata` / `.References` | 表约定 / 引用元数据 |

---

## 14. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Game/Config/YamlConfigLoader.cs` | 加载器核心（注册/加载/热重载） |
| `GFramework.Game/Config/ConfigRegistry.cs` | 注册表 |
| `GFramework.Game.SourceGenerators/Config/SchemaConfigGenerator.cs` | 生成器（产出类/表/Bindings/Catalog） |
| `GFramework.Godot/Config/GodotYamlConfigLoader.cs` | Godot 桥接（编辑器/导出缓存） |
| `GFramework/docs/zh-CN/game/config-system.md` | 框架官方文档（1008 行，权威） |
| `docs/research/gframework-extensions/game-services.md` | 扩展研究（含数组边界记录） |

---

## 附：模板代码验证

模板 `tests/.../ConfigSystemTests.cs` 用非 Godot 的 `YamlConfigLoader` + 相对路径加载本地 YAML，验证：

| 用例 | 验证内容 |
|---|---|
| 加载_示例怪物全部读取 | 3 条目 + 主键存在 |
| 强类型读取_Slime配置正确 | 字段值 + 默认值 |
| 强类型读取_Orc为Boss稀有度 | enum 值 |
| 生成元数据_表名与路径正确 | Bindings.Metadata |
| 目录_未声明配置Key时返回默认 | 不存在主键 |

**框架升级后跑 `dotnet test` 即可发现本文档是否过期。**