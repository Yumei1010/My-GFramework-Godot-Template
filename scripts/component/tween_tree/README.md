# Tween 动画树（Tween Tree）

`scripts/component/tween_tree/` 下的 Tween 动画树组件：把 **Tween 动画包装成 Godot 节点**，
在**编辑器里拼装节点树即可可视化编排动画序列**——像动画树（AnimationTree）一样直观，
叶子节点通过 `[Export]` 属性调参，无需写代码。

## 为什么用节点树而不是直接写 Tween？

| | 手写 Tween | Tween 节点树 |
|---|---|---|
| 编排 | 代码链式 `TweenProperty(...).TweenInterval(...)` | 场景树拖拽拼装，层级即顺序 |
| 调参 | 改代码重编译 | 检查器里拖时长/曲线，即时预览 |
| 复用 | 复制代码 | 子树片段可复用 |
| 理解 | 阅读链式调用 | 看节点树一目了然 |

## 节点类型

| 节点 | 类型 | 作用 |
|---|---|---|
| `TweenTree` | 根 | 挂在场景任意位置，播放/停止整棵动画树 |
| `TweenPropertyNode` | 叶子 | 属性补间：position / modulate / scale / rotation / 任意属性 |
| `TweenIntervalNode` | 叶子 | 延时停顿 |
| `TweenSequenceNode` | 组合 | 子节点**顺序**执行（SubtweenTweener 串联） |
| `TweenParallelNode` | 组合 | 子节点**并行**执行 |

## 拼装示例（场景树）

```
TweenTree（挂到场景，如角色节点下）
└── TweenSequenceNode（顺序）
    ├── TweenPropertyNode    卡牌移到台面 (position → (300,0), 0.4s)
    ├── TweenParallelNode（并行）
    │   ├── TweenPropertyNode    淡入 (modulate → 白, 0.3s)
    │   └── TweenPropertyNode    放大 (scale → 1.2, 0.3s)
    ├── TweenIntervalNode    停 0.2s
    └── TweenPropertyNode    发光 (modulate → 黄, 0.3s)
```

## 叶子节点调参（检查器导出属性）

`TweenPropertyNode`：
| 属性 | 说明 |
|---|---|
| `TargetNode` | 目标节点路径（`%唯一名` 或相对路径）；空 = 本节点 |
| `Property` | 补间属性名：`position`/`modulate`/`scale`/`rotation`/`rotation_degrees`… |
| `TargetValue` | 目标值（Variant：position→Vector2、modulate→Color、rotation→float） |
| `Duration` | 时长（秒） |
| `Trans` / `Ease` | 过渡曲线 / 缓动（编辑器下拉选） |
| `AsRelative` | 相对当前值 |

## 播放（代码一行）

```csharp
// 播放（不等待）
GetNode<TweenTree>("%CardAnim").Play();

// 播放并等待完成（配合 ActionQueue 做更大序列，或 async/await）
await GetNode<TweenTree>("%CardAnim").PlayAsync();

// 停止 / 立即终止
tree.Stop();
tree.Kill();
```

## 与 ActionQueue 配合

TweenTree 是"一条完整动画"，ActionQueue 是"多个动作排队"。可嵌套组合：

```csharp
var queue = new ActionQueue();
queue.Enqueue(async () => await GetNode<TweenTree>("%CardAnim").PlayAsync()); // 播一段动画树
queue.Enqueue(async () => await GetNode<TweenTree>("%ScoreAnim").PlayAsync()); // 再播计分
queue.Enqueue(() => CheckWin());                                              // 再判赢
```

## 设计说明（SubtweenTweener）

组合节点用 `Tween.TweenSubtween()` 把每个子节点的 Tween **作为一步嵌入**父 Tween——
引擎自动把子 Tween 从 SceneTree 接管，序列/并行由子树结构表达。
每帧驱动一次根节点即可；绑定节点销毁时 Tween 自动清理。

## 验证

已在 Godot 4.7.2 headless 实测：顺序+并行+延时组合，总耗时符合预期（1.07s ≈ 1.1s），
position/scale/modulate 均补间到目标值。
