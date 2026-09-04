# 对象池（Object Pool）

`scripts/component/object_pool/` 下的对象池示例组件——**基于 GFramework 原生池系统**（方案 A：少造轮子），
演示如何在模板项目中按框架方式做节点复用（子弹/特效/敌人等高频生成场景）。

## 为什么用框架原生而不是自研？

GFramework 已提供完整的对象池：
- `AbstractObjectPoolSystem<TKey, TObject>`：Acquire/Release/Prewarm/统计/容量限制/防双重释放
- `AbstractNodePoolSystem<TKey, TNode>`：Godot 节点版（PackedScene 实例化）
- `IPoolableNode`：节点池化接口（OnAcquire/OnRelease/OnPoolDestroy 生命周期）

模板只需**实现一个具体系统**（每对象类型一个，方案 A1）+ 示例节点，即插即用。

## 文件

| 文件 | 说明 |
|---|---|
| `BulletPoolSystem.cs` | 示例池系统：继承 `AbstractNodePoolSystem<string, BulletNode>` |
| `BulletNode.cs` | 示例池化节点：实现 `IPoolableNode`（Node2D，生命周期复位） |
| `scenes/objects/bullet.tscn` | 示例子弹场景 |

## 使用步骤

### 1. 定义你的池化节点（实现 IPoolableNode）

```csharp
// MyNode.cs : Node2D, IPoolableNode
public partial class MyNode : Node2D, IPoolableNode
{
    void IPoolableObject.OnAcquire()   { /* 取池复位 */ ResetState(); Hide(); }
    void IPoolableObject.OnRelease()   { /* 归还复位 */ StopAll(); Hide(); }
    void IPoolableObject.OnPoolDestroy(){ QueueFree(); }
    public Node AsNode() => this;
}
```

### 2. 定义你的池系统（继承 AbstractNodePoolSystem）

```csharp
// MyPoolSystem.cs : AbstractNodePoolSystem<string, MyNode>
public class MyPoolSystem : AbstractNodePoolSystem<string, MyNode>
{
    public const string Key = "my_node";
    protected override PackedScene LoadScene(string key) => GD.Load<PackedScene>("res://.../my_node.tscn");
}
```

### 3. 注册为 System（模块里）

```csharp
// SystemModule.Install:
architecture.RegisterSystem(new MyPoolSystem());
```

### 4. 使用（任意 [ContextAware] 节点）

```csharp
var pool = this.GetSystem<MyPoolSystem>()!;

// 取：复用池中空闲或新实例化
var node = pool.Acquire(MyPoolSystem.Key);
GetNode<Node2D>("%Container").AddChild(node);
node.Init(...);              // 业务初始化

// ... 用完后归还：
node.GetParent()?.RemoveChild(node);   // 从场景摘下
pool.Release(MyPoolSystem.Key, node);  // 归还池（下次复用）
```

## 框架 API 速查

| 方法 | 作用 |
|---|---|
| `Acquire(key)` | 取对象（池空则创建） |
| `Release(key, obj)` | 归还（超容量则销毁） |
| `Prewarm(key, count)` | 预热：预先创建 N 个 |
| `SetMaxCapacity(key, n)` | 池容量上限（0=无限） |
| `GetStatistics(key)` | 统计（创建/活跃/归还/销毁数） |

## 生命周期

| 回调 | 时机 | 建议 |
|---|---|---|
| `OnAcquire` | 从池取出 | 复位状态（位置/速度/激活）、隐藏待用 |
| `OnRelease` | 归还池 | 停止协程/物理、隐藏 |
| `OnPoolDestroy` | 池销毁或超容量 | QueueFree |

## 验证

已在 Godot 4.7.2 headless 实测：取 3 发创建 3 个 → 归还池满 3 → 再取复用（TotalCreated 仍 3，无新增实例）。

## 备注

- 挂载/摘下由调用方负责（池只管复用，不碰场景树——保持框架纯净）
- 每对象类型一个池系统类（方案 A1 类型安全）；若想单类多场景可考虑改 A2（统一 PoolableNode 基类）
- 示例场景键 `Key` 为常量；多子弹类型可扩展为枚举键
