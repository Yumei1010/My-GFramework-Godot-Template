# ECS 教程（Arch ECS）

> 本模板已集成 GFramework 的 **Arch ECS**（`GeWuYou.GFramework.Ecs.Arch` 0.7.1 + `Arch` 2.1.0），
> 通过 `GameEntryPoint` 里的 `UseArch()` 接入。本文教你从零用它管理游戏实体。

## 1. 为什么用 ECS？

游戏对象多到一定程度（上千子弹、敌人、粒子），传统"每个对象一个 Node 脚本"会遇到：
- **性能**：每个 Node 的 `_Process` 单独调度，CPU 缓存不友好
- **数据碎片**：血量/位置/速度散落在不同类里，难以批量处理
- **耦合**：对象逻辑和表现强绑定

**ECS（Entity-Component-System）** 把对象拆成三部分，数据集中、逻辑批量：

| 概念 | 是什么 | 示例 |
|---|---|---|
| **Entity** | 一个 id（无数据） | 第 3 号子弹 |
| **Component** | 纯数据（struct） | `Position`、`Velocity`、`Health` |
| **System** | 处理数据的逻辑 | 移动系统：批量改所有子弹位置 |

Arch 用 **struct 组件 + 内存连续存储**，遍历上千实体性能远超逐个 Node。

## 2. 模板已接好，直接用

模板的 `GameEntryPoint` 已调用 `UseArch()`——架构初始化时会自动：
1. 创建 Arch `World`（默认容量）并注册进容器
2. 收集所有 `ArchSystemAdapter<float>` 系统并初始化

```csharp
// GameEntryPoint 现状（已接入，无需再改）：
var arch = new GameArchitecture(...)
    .UseArch();  // 可传参：UseArch(o => o.WorldCapacity = 2048)
```

## 3. 核心流程（照抄三步）

### 第一步：定义组件（纯数据 struct）

```csharp
// 组件 = 纯数据，用 struct（值类型，内存友好）
public struct Position { public float X, Y; }
public struct Velocity { public float X, Y; }
public struct Health   { public int Value; }
```

### 第二步：写系统（继承 ArchSystemAdapter<float>）

```csharp
using Arch.Core;
using GFramework.Ecs.Arch;

public sealed class MovementSystem : ArchSystemAdapter<float>
{
    private QueryDescription _query;

    protected override void OnArchInitialize()
    {
        // 声明处理哪些组件组合
        _query = new QueryDescription().WithAll<Position, Velocity>();
    }

    protected override void OnUpdate(in float deltaTime)
    {
        // 批量更新所有"有 Position 且 Velocity"的实体
        World.Query(in _query, (ref Position pos, ref Velocity vel) =>
        {
            pos.X += vel.X * deltaTime;
            pos.Y += vel.Y * deltaTime;
        });
    }
}
```

### 第三步：注册系统 + 驱动更新

```csharp
// 1. 注册进架构（模块里）
architecture.RegisterSystem(new MovementSystem());

// 2. 每帧驱动（Godot 节点里）
public override void _Process(double delta)
{
    var ecs = this.GetService<IArchEcsModule>();   // 从架构容器取 ECS 模块
    ecs.Update((float)delta);                       // 驱动所有系统
}
```

> ArchEcsModule 会按优先级收集容器里所有 `ArchSystemAdapter<float>`，
> 注册后**无需手动逐个 Update**——`ecs.Update(delta)` 一次驱动全部。

## 4. 创建/销毁实体（World API）

```csharp
var world = this.GetService<World>();

// 创建实体 + 附加组件
var bullet = world.Create(
    new Position { X = 0, Y = 0 },
    new Velocity { X = 300, Y = 0 });

// 读取组件
ref var pos = ref world.Get<Position>(bullet);
GD.Print($"x={pos.X}");

// 查询是否含组件
if (world.Has<Health>(bullet)) { /* ... */ }

// 销毁实体
world.Destroy(bullet);
```

**用 ref 读组件**（`world.Get<T>` 返回 ref）——避免复制 struct。

## 5. 完整示例：子弹射击（配合对象池思路）

```csharp
// 组件
public struct BulletData { public float LifeTime; }  // 计时

// 系统1：移动
public sealed class BulletMoveSystem : ArchSystemAdapter<float>
{
    private QueryDescription _q;
    protected override void OnArchInitialize() => _q = new QueryDescription().WithAll<Position, Velocity>();
    protected override void OnUpdate(in float d) => World.Query(in _q, (ref Position p, ref Velocity v) => {
        p.X += v.X * d; p.Y += v.Y * d;
    });
}

// 系统2：寿命到点销毁
public sealed class BulletLifetimeSystem : ArchSystemAdapter<float>
{
    private QueryDescription _q;
    protected override void OnArchInitialize() => _q = new QueryDescription().WithAll<BulletData>();
    protected override void OnUpdate(in float d)
    {
        var toDestroy = new System.Collections.Generic.List<Arch.Core.Entity>();
        World.Query(in _q, (Arch.Core.Entity e, ref BulletData data) =>
        {
            data.LifeTime -= d;
            if (data.LifeTime <= 0) toDestroy.Add(e);
        });
        foreach (var e in toDestroy) World.Destroy(e);
    }
}
```

## 6. 系统生命周期钩子

`ArchSystemAdapter<float>` 提供可重写的钩子：

| 钩子 | 时机 |
|---|---|
| `OnArchInitialize()` | 系统初始化（建 Query） |
| `OnBeforeUpdate(in float)` | 每帧更新前 |
| `OnUpdate(in float)` | 主更新 |
| `OnAfterUpdate(in float)` | 每帧更新后 |
| `OnDestroy()`（继承 AbstractSystem） | 系统销毁 |

## 7. 与模板其他组件的关系

| 技术 | 适用场景 | 配合方式 |
|---|---|---|
| **ECS** | 海量实体的**数据与逻辑** | 位置/血量/速度在 ECS 里算 |
| **对象池** | 节点的**创建/销毁复用** | 实体表现节点池化，减少实例化 |
| **TweenTree** | 单个对象的**动画** | 表现层动画 |
| **HFSM/行为树** | 单个实体的**决策** | 敌人 AI 决策 → ECS 存状态 |
| **CQRS 事件** | 跨系统**通信** | ECS 事件（死亡等）发 CQRS 事件通知 UI |

**推荐分层**：ECS 管"世界数据"，Godot 节点管"表现"，中间用事件/查询桥接——
实体变化发事件 → UI/表现层响应。

## 8. 性能提示

- 组件用 **struct**（避免装箱/GC）
- 批量操作用 `World.Query`（不要逐实体 Get）
- 频繁增删的实体类型配合**实体预创建/对象池**
- Query 用 `WithAll<A,B>` 精确声明，避免无关实体匹配

## 9. 下一步

- 系统学习 Arch 库：`Arch` 支持 Jobs/多线程（`Arch.System` 的 `ISystem<T>`）、事件、快照等
- 看框架示例组件：`GFramework.Ecs.Arch/Components`（Position/Velocity）、`Systems/MovementSystem`
- 官方 Arch 文档：https://github.com/genaray/Arch
