# 对象池教程（Object Pool）

> 本文纯教学：讲清"为什么需要对象池 + 怎么用 GFramework 原生池系统"。
> 不再提供模板示例组件——框架已带完整池系统，按本文照抄即可接入自己的对象。

## 1. 为什么需要对象池？

游戏里频繁"创建-销毁"对象（子弹、特效、敌人、飘字）会带来两个问题：
- **GC 压力**：每帧 new/QueueFree 产生垃圾回收卡顿
- **实例化开销**：反复加载场景/建对象

**对象池**：预先创建一批对象存着，用的时候"取出"（复用），用完"归还"（不销毁）。
省去反复创建销毁——子弹打 1000 发也只实例化一次。

## 2. GFramework 自带池系统，别自造轮子

GFramework 已实现完整对象池（模板直接可用，无需额外包）：

| 类型 | 作用 |
|---|---|
| `AbstractObjectPoolSystem<TKey, TObject>` | 通用池系统（任意对象类型） |
| `AbstractNodePoolSystem<TKey, TNode>` | **Godot 节点池**（PackedScene 实例化）——游戏最常用 |
| `IPoolableObject` / `IPoolableNode` | 池化对象的生命周期接口 |
| `IObjectPoolSystem<TKey, TObject>` | 对外接口（Acquire/Release/Prewarm/统计…） |

命名空间：`GFramework.Core.Pool` / `GFramework.Core.Abstractions.Pool` / `GFramework.Godot.Pool`

## 3. 池系统能力一览

| API | 作用 |
|---|---|
| `Acquire(key)` | 取对象（池空则按需创建） |
| `Release(key, obj)` | 归还（超过容量则销毁） |
| `Prewarm(key, count)` | 预热：预先创建 N 个存池 |
| `SetMaxCapacity(key, n)` | 池上限（0 = 无限） |
| `GetStatistics(key)` | 统计：创建/活跃/归还/销毁数 |
| `Clear()` | 清空所有池 |

自动特性：**防双重释放**（重复 Release 会警告）、**容量溢出销毁**、**统计计数**。

## 4. 完整接入步骤（照抄 4 步）

### 第 1 步：池化节点（实现 IPoolableNode）

```csharp
using GFramework.Core.Abstractions.Pool;
using GFramework.Godot.Pool;
using Godot;

// 场景对象（子弹/特效等）——挂到 PackedScene 根节点
public partial class Bullet : Node2D, IPoolableNode
{
    private bool _flying;

    public void Fire(Vector2 pos, Vector2 dir)
    {
        GlobalPosition = pos;
        _flying = true;
        Show();
        SetPhysicsProcess(true);
    }

    // ==== 池生命周期三钩子 ====

    void IPoolableObject.OnAcquire()
    {
        // 从池取出：复位状态（随后由调用方 Fire 激活）
        _flying = false;
        Hide();
        SetPhysicsProcess(false);
    }

    void IPoolableObject.OnRelease()
    {
        // 归还池：停止一切活动
        _flying = false;
        Hide();
        SetPhysicsProcess(false);
    }

    void IPoolableObject.OnPoolDestroy()
    {
        QueueFree();   // 池销毁或溢出时才真正销毁
    }

    public Node AsNode() => this;
}
```

### 第 2 步：池系统（每对象类型一个）

```csharp
using GFramework.Godot.Pool;
using Godot;

public class BulletPool : AbstractNodePoolSystem<string, Bullet>
{
    public const string Key = "bullet";

    protected override PackedScene LoadScene(string key)
        => GD.Load<PackedScene>("res://scenes/bullet.tscn");
}
```

> 方案说明：框架泛型 `TNode` 固定一种类型 → **每对象类型建一个池系统类**
> （BulletPool/EffectPool/EnemyPool…）。类型安全，各池独立统计。

### 第 3 步：注册进架构（模块里）

```csharp
// SystemModule.Install 或你的模块：
architecture.RegisterSystem(new BulletPool());
```

### 第 4 步：使用（任意 [ContextAware] 节点）

```csharp
var pool = this.GetSystem<BulletPool>()!;

// 发射：取 → 挂载 → 激活
var bullet = pool.Acquire(BulletPool.Key);
GetNode<Node2D>("%BulletContainer").AddChild(bullet);
bullet.Fire(playerPos, aimDir);

// ... 命中/出界后归还：
bullet.GetParent()?.RemoveChild(bullet);   // 从场景摘下
pool.Release(BulletPool.Key, bullet);      // 归还池（下次复用，不再实例化）
```

**归还的完整闭环**：取出要 `AddChild` 挂到场景 → 用完先 `RemoveChild` 摘下 → 再 `Release` 归还。

## 5. 预热（避免首帧卡顿）

战斗前预创建，游戏中零延迟：

```csharp
// 系统 OnInit 里：
protected override void OnInit()
{
    Prewarm(BulletPool.Key, 20);   // 预创建 20 发子弹
}
```

## 6. 生命周期图

```
        Acquire(key)                    Release(key, obj)
    ───────────────►  ┌────────┐  ◄────────────────
   OnAcquire（复位）   │  池    │   OnRelease（复位）
    AddChild + Fire   │  Stack │    RemoveChild
                      └────────┘
    池空 → Create（实例化场景）     超容量 → OnPoolDestroy → QueueFree
```

| 钩子 | 时机 | 典型操作 |
|---|---|---|
| `OnAcquire` | 从池取出 | 复位状态、隐藏待用（由调用方激活） |
| `OnRelease` | 归还池 | 停止协程/物理、隐藏 |
| `OnPoolDestroy` | 池销毁/溢出 | QueueFree |

## 7. 常见坑

1. **归还前先 RemoveChild**——节点挂树上时 Release，下次 Acquire 再 AddChild 会报"已有父"
2. **释放后别再用**——Release 后对象进池，再操作会污染下次复用
3. **场景键管理**——多类型用独立类（每类一个 System）或枚举键
4. **统计调试**——用 `GetStatistics(key)` 看是否泄漏（活跃数异常增长 = 有对象没归还）

## 8. 配合模板其他组件

- **ECS + 对象池**：实体数据在 ECS 算，表现节点池化复用
- **TweenTree + 对象池**：动画结束（`PlayAsync` 完成）后归还节点
- **CQRS 事件**：节点"死亡/消失"发事件，监听者统一归还

## 9. 框架源码阅读入口

- `GFramework.Core/Pool/AbstractObjectPoolSystem.cs` — 池核心实现
- `GFramework.Godot/Pool/AbstractNodePoolSystem.cs` — 节点池（场景加载）
- `GFramework.Core.Tests/Pool/ObjectPoolTests.cs` — 框架自测（复用/释放语义参考）
