# GFramework NuGet 包研究：是否值得引入

分支：`feat/upgrade-gframework`
日期：2026-09-02

## NuGet 上 GFramework 系列包全景

| 包 | 最新版 | 模板当前使用 | 说明 |
|---|---|---|---|
| `GeWuYou.GFramework` | 0.7.1 | ✅ | metapackage，聚合 Core + Game |
| `GeWuYou.GFramework.Core` | 0.7.1 | ✅（传递） | 核心：架构/容器/事件/命令 |
| `GeWuYou.GFramework.Game` | 0.7.1 | ✅（传递） | 游戏层：设置/UI/场景/状态 |
| `GeWuYou.GFramework.Godot` | 0.7.1 | ✅ | Godot 集成 |
| `GeWuYou.GFramework.Godot.SourceGenerators` | 0.7.1 | ✅ | Godot 代码生成 |
| `GeWuYou.GFramework.SourceGenerators` | 0.0.220 | ⚠️ **旧版本线** | 旧代码生成器（Log/ContextAware） |
| `GeWuYou.GFramework.Core.SourceGenerators` | 0.7.1 | ❌ 未用 | **新 Core 代码生成器**（对齐 0.7.1） |
| `GeWuYou.GFramework.Game.SourceGenerators` | 0.7.1 | ❌ 未用 | Schema 配置代码生成 |
| `GeWuYou.GFramework.Cqrs.SourceGenerators` | 0.7.1 | ❌ 未用 | CQRS handler 注册生成 |
| `GeWuYou.GFramework.Ecs.Arch` | 0.7.1 | ❌ 未用 | **Arch ECS 集成** |
| `GeWuYou.GFramework.Ecs.Arch.Abstractions` | 0.7.1 | ❌ 未用 | ECS 契约层 |
| `GeWuYou.GFramework.Cqrs` / `.Cqrs.Abstractions` | 0.7.1 | ✅（传递） | CQRS 运行时（handler 模式） |
| `GeWuYou.GFramework.Generator` / `.Attributes` | 0.0.54 | ❌ | 旧代码生成器（过时） |
| `GeWuYou.GFramework.Core.Godot` | 0.0.33 | ❌ | 旧 Godot 核心（过时） |

## 重点研究结论

### 1. ⚠️ SourceGenerators 版本线不一致（建议修复）

**现状**：模板用 `GeWuYou.GFramework.SourceGenerators` **0.0.220**（旧版本线），
命名空间 `GFramework.SourceGenerators.Abstractions.Logging/Rule`。

**问题**：0.0.220 与 0.7.1 主包版本线错位，长期不维护风险。

**0.7.1 新版**：SourceGenerators 拆分为 3 包：
- `GFramework.Core.SourceGenerators` 0.7.1（含 `[Log]`/`[ContextAware]`，命名空间 `GFramework.Core.SourceGenerators.Abstractions.Logging/Rule`）
- `GFramework.Game.SourceGenerators` 0.7.1（Schema 配置生成）
- `GFramework.Cqrs.SourceGenerators` 0.7.1（handler 注册生成）

**建议**：把 `GeWuYou.GFramework.SourceGenerators` 0.0.220 替换为
`GeWuYou.GFramework.Core.SourceGenerators` 0.7.1，using 从
`GFramework.SourceGenerators.Abstractions.Logging/Rule` → `GFramework.Core.SourceGenerators.Abstractions.Logging/Rule`。
（Godot.SourceGenerators 0.7.1 应保持——它是 Godot 特定生成器。）

### 2. Ecs.Arch（Arch ECS 集成）—— 有条件值得

**是什么**：把第三方 ECS 库 [Arch](https://github.com/genaray/Arch) 集成进 GFramework 架构：
- `UseArch(options)` 接入，`World` 自动注册进容器
- `ArchSystemAdapter<T>` 桥接 GFramework 系统生命周期 ↔ Arch 系统
- 统一 `IArchEcsModule.Update(deltaTime)` 驱动

**价值**：如果做**大量实体游戏**（敌人波次、子弹、粒子等），ECS 数据驱动性能好。
**成本**：引入 `Arch 2.1.0` + `Arch.System 1.1.0` 第三方依赖；需按新范式写系统（Query/Component）。

**判断**：当前模板是"起手骨架"，ECS 属于**可选进阶能力**。建议**不引入**（保持骨架精简），
需要时按 `docs/zh-CN/ecs/arch.md` 文档按需接入。

### 3. Cqrs 独立包 —— 已随依赖引入，无需额外操作

`GFramework.Core` 0.7.1 依赖 `GFramework.Cqrs`，命令/查询走新 CQRS 运行时。
模板的 `AbstractAsyncCommand<T>` 等旧式命令 API 仍兼容（已实测），
新版 handler 模式（`AbstractCommandHandler<T>`）可按需渐进采用。

### 4. Game.SourceGenerators —— 看需求

提供 `SchemaConfigGenerator`（配置 Schema → 强类型生成）。适合配置驱动项目，
当前模板无此需求，暂不引入。

### 5. 过时包 —— 明确不引入

`GFramework.Generator`(0.0.54)、`GFramework.Core.Godot`(0.0.33)、
`SourceGenerators.Attributes`(0.0.61) 均为旧版本线，不用。

## 最终采纳结果

| 包 | 状态 | 说明 |
|---|---|---|
| `Core.SourceGenerators` 0.7.1 | ✅ 已引入 | 替换旧 SourceGenerators 0.0.220，版本线对齐 |
| `Ecs.Arch` 0.7.1 | ✅ 已引入+接入 | `GameEntryPoint.UseArch()` 接入，World 注册进容器，Arch 2.1.0 可用 |
| `Game.SourceGenerators` 0.7.1 | ✅ 已引入 | JSON Schema → 强类型配置生成，按需配 schema |
| `Cqrs.SourceGenerators` 0.7.1 | ✅ 已引入 | CQRS handler 注册生成，按需用新 handler 模式 |
| `Generator`/`Core.Godot`/旧 Attributes | ❌ 不用 | 过时版本线 |

### Ecs.Arch 接入方式（模板已含）

```csharp
// GameEntryPoint:
var arch = new GameArchitecture(...)
    .UseArch(); // Arch ECS：World 自动注册进容器，可选 UseArch(o => o.WorldCapacity = 2048)
```

使用：`context.GetService<World>()` 创建实体；`ArchSystemAdapter<T>` 写 ECS 系统。
