# Twenty-four Roguelike 开发路线图

## 项目概述

基于 Godot 4.6 + C# (.NET 10.0) 的 24 点 Roguelike 卡牌游戏。玩家从默认牌组起步，通过成就逐步解锁更多牌组（单一花色牌组、高级花色牌组），利用 12 种扩展运算解决原版 24 点的大量无解死局。时间即生命，时间即财富——限时内解出 24 点即胜利，盈余时间可购买道具。

### 核心机制三角

```
        时间
       /    \
    生命    财富
    (超时   (购买
     即败)   道具)
```

---

## 阶段一：牌组预设系统

**目标：** 让"开局选择牌组"成为可能

### 1.1 扩展花色系统

当前 `SuitType` 仅有 4 种传统花色，需扩展：

```csharp
// scripts/enums/poker/SuitType.cs
public enum SuitType
{
    // 传统花色
    Heart,      // 红桃
    Diamond,    // 方块
    Spade,      // 黑桃
    Club,       // 梅花

    // 高级花色（阶段五落地特效）
    BlueHeart,       // 蓝桃 — 参与计算时奖励 +1 秒
    FireFlower,      // 火花 — 乘方/开方运算结果翻倍
    CrystalDiamond,  // 晶钻 — 打出时揭示运算路径
    ShadowSpade,     // 影桃 — 可当作任意花色使用
}
```

### 1.2 牌组预设数据结构

```csharp
// scripts/entities/deck/DeckPreset.cs
public class DeckPreset
{
    public string Id { get; set; }           // 唯一标识，如 "default"、"heart_only"
    public string Name { get; set; }         // 显示名称
    public string Description { get; set; }  // 描述
    public SuitType[] Suits { get; set; }    // 包含的花色
    public int MinRank { get; set; }         // 最小点数 (1=A)
    public int MaxRank { get; set; }         // 最大点数 (13=K)
    public NumType NumType { get; set; }     // 数值类型
    public int HandSize { get; set; }        // 初始手牌数
    public bool IsUnlockedByDefault { get; set; } // 是否默认解锁
}
```

### 1.3 牌组预设注册表

```csharp
// scripts/entities/deck/DeckPresetRegistry.cs
public interface IDeckPresetRegistry
{
    void Register(DeckPreset preset);
    DeckPreset Get(string id);
    IReadOnlyList<DeckPreset> GetAll();
    IReadOnlyList<DeckPreset> GetUnlocked();
    bool IsUnlocked(string id);
    void Unlock(string id);
}
```

### 1.4 花色特效接口

```csharp
// scripts/entities/poker/ISuitEffect.cs
public interface ISuitEffect
{
    SuitType SuitType { get; }
    string Description { get; }

    // 事件钩子
    void OnCardDrawn(IPoker poker);
    void OnCardSelected(IPoker poker);
    void OnCalculationStart(IPoker poker, ModeType mode);
    void OnCalculationEnd(IPoker poker, string result);
}
```

### 1.5 UI：牌组选择界面

- 在主菜单 "开始游戏" 后展示
- 列表/网格显示已解锁牌组
- 未解锁牌组灰显并显示解锁条件

### 预设牌组清单

| ID | 名称 | 花色 | 解锁条件 |
|----|------|------|----------|
| `default` | 标准牌组 | ♠♥♣♦ A~K | 默认解锁 |
| `heart_only` | 红桃牌组 | ♥ A~K | 完成 5 局游戏 |
| `spade_only` | 黑桃牌组 | ♠ A~K | 完成 5 局游戏 |
| `diamond_only` | 方块牌组 | ♦ A~K | 完成 5 局游戏 |
| `club_only` | 梅花牌组 | ♣ A~K | 完成 5 局游戏 |
| `blue_heart` | 蓝桃牌组 | 蓝桃 A~K | 用红桃牌组赢 10 局 |
| `fire_flower` | 火花牌组 | 火花 A~K | 单局使用乘方运算 5 次 |
| `crystal_diamond` | 晶钻牌组 | 晶钻 A~K | 单局连续答对 10 题 |
| `shadow_spade` | 影桃牌组 | 影桃 A~K | 累计使用 4 种不同运算 |

---

## 阶段二：元进度 & 成就系统

**目标：** 成就驱动的牌组解锁，游戏进度可持久化

### 2.1 扩展存档数据

```csharp
// scripts/data/model/GameSaveData.cs（扩展）
public class GameSaveData
{
    // 现有字段
    public int Version { get; set; } = 1;
    public DateTime SaveTime { get; set; }
    public string SlotDescription { get; set; }

    // 新增：元进度
    public List<string> UnlockedDeckIds { get; set; } = new();
    public Dictionary<string, float> AchievementProgress { get; set; } = new();
    public List<string> CompletedAchievementIds { get; set; } = new();
    public int PermanentUpgradeLevel { get; set; }

    // 新增：累计统计
    public int TotalGamesPlayed { get; set; }
    public int TotalWins { get; set; }
    public int TotalRoundsSolved { get; set; }
    public Dictionary<string, int> UsageCountByMode { get; set; } = new();      // 各运算使用次数
    public Dictionary<string, int> UsageCountByDeck { get; set; } = new();      // 各牌组使用次数
}
```

### 2.2 成就定义

```csharp
// scripts/cqrs/achievement/AchievementDefinition.cs
public class AchievementDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string RewardDeckId { get; set; }  // 解锁的牌组 ID，null 表示无牌组奖励
    public Func<GameSaveData, bool> Condition { get; set; }
}
```

### 2.3 CQRS 事件

- `AchievementUnlockedEvent` — 成就解锁
- `AchievementProgressUpdatedEvent` — 进度更新
- `DeckPresetUnlockedEvent` — 牌组解锁

---

## 阶段三：单局运行系统

**目标：** 关卡制流程跑通

### 3.1 单局流程

```
选牌组 → 第 1 关（发牌 → 解题 → 结算）→ 商店 → 第 2 关 → ... → 通关 或 时间耗尽
```

### 3.2 核心类

```csharp
// scripts/core/run/RunManager.cs
public interface IRunManager
{
    DeckPreset SelectedDeck { get; }
    int CurrentLevel { get; }
    float TotalTimeRemaining { get; }   // 全局时间
    int Score { get; }

    void StartRun(DeckPreset deck);
    void CompleteLevel();
    void EnterShop();
    void StartNextLevel();
    void EndRun(bool isWin);
}
```

```csharp
// scripts/core/run/LevelConfig.cs
public class LevelConfig
{
    public int LevelNumber { get; set; }
    public int CardCount { get; set; }       // 本关手牌数（初始 4）
    public float TimeLimit { get; set; }     // 本关时限（秒）
    public int TargetNumber { get; set; }    // 目标值（默认 24）
    public float DifficultyMultiplier { get; set; }
}
```

### 3.3 难度递进

| 关卡 | 手牌数 | 时限 | 说明 |
|------|--------|------|------|
| 1~3 | 4 张 | 60s | 入门，默认 24 点 |
| 4~6 | 4 张 | 45s | 常规 |
| 7~9 | 5 张 | 45s | 加牌增加组合复杂度 |
| 10+ | 5 张 | 30s | 高压 |

### 3.4 结算公式

- 过关条件：在时限内计算出目标值
- 剩余时间转换为货币：`货币 = ceil(剩余秒数 × 关卡倍率)`
- 超时：扣除全局时间，若全局时间归零则游戏结束

---

## 阶段四：道具 & 商店系统

**目标：** 盈余时间的消费出口

### 4.1 道具定义

```csharp
// scripts/core/item/ItemDefinition.cs
public class ItemDefinition
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }       // Consumable / Permanent
    public int Price { get; set; }
    public int MaxLevel { get; set; }        // 永久升级最大等级
    public IItemEffect Effect { get; set; }
}

public enum ItemType { Consumable, Permanent }
```

```csharp
// scripts/core/item/IItemEffect.cs
public interface IItemEffect
{
    void Apply(IRunManager run);
}
```

### 4.2 消耗型道具

| 道具 | 效果 | 价格 |
|------|------|------|
| 时间胶囊 | 当前关卡 +10 秒 | 5 |
| 提示之光 | 高亮一张可参与运算的牌 | 3 |
| 跳过关卡 | 跳过当前题，不扣全局时间 | 8 |
| 重抽之手 | 重新发牌 | 4 |
| 时间冻结 | 冻结计时 3 秒 | 6 |
| 万能牌 | 将一张牌的数值临时改为任意值 | 10 |

### 4.3 永久升级

| 升级 | 效果（每级） | 最大等级 | 价格递增 |
|------|-------------|----------|----------|
| 时间储备 | 初始全局时间 +15s | 5 | 10/20/40/80/160 |
| 慢速流逝 | 时间流逝速度 -5% | 5 | 同上 |
| 手牌扩展 | 起始手牌 +1 | 3 | 15/30/60 |
| 商店达人 | 商店额外 +1 商品格 | 3 | 同上 |
| 幸运开局 | 开局 10% 概率获得随机消耗道具 | 3 | 同上 |

### 4.4 商店管理器

```csharp
// scripts/core/item/ShopManager.cs
public interface IShopManager
{
    IReadOnlyList<ItemDefinition> CurrentItems { get; }  // 当前可购买的随机道具
    void Refresh();                                        // 刷新商品
    bool Purchase(string itemId, out string error);
}
```

### 4.5 UI：商店界面

- 展示 4~6 个随机道具
- 显示价格和余额
- 购买/刷新按钮
- 永久升级显示当前等级

---

## 阶段五：高级花色效果落地

**目标：** 让蓝桃、火花等真正影响玩法

### 5.1 花色特效

| 花色 | 特效名称 | 触发时机 | 效果描述 |
|------|----------|----------|----------|
| 蓝桃 `BlueHeart` | 时间回响 | 参与计算时 | 每次作为计算输入时奖励 +1 秒 |
| 火花 `FireFlower` | 烈焰乘方 | 计算结束 | 参与乘方/开方运算时，结果翻倍 |
| 晶钻 `CrystalDiamond` | 洞察之辉 | 牌被选中时 | 高亮该牌可参与的运算路径（提示） |
| 影桃 `ShadowSpade` | 暗影化身 | 计算时 | 可当作任意花色参与计算 |

### 5.2 特效实现方式

- 每种特效实现 `ISuitEffect` 接口
- 通过 CQRS 事件系统挂载到对应游戏事件上
- 特效不修改核心计算逻辑，而是通过事件钩子注入

---

## 实施优先级

```
阶段一（牌组预设）
    ↓
阶段三（单局系统）
    ↓
阶段四（道具商店）
    ↓
阶段二（成就解锁）
    ↓
阶段五（高级花色特效）
```

**理由：** 先让"选牌组 → 解题 → 过关"的核心循环跑通，再加入商店和元进度，最后落地高级花色的差异化体验。

---

## 技术选型备注

- **架构：** 继续使用 GFramework CQRS 模式，命令/事件/查询分离
- **数据持久化：** 复用现有 `SaveStorageUtility`，扩展 `GameSaveData`
- **UI：** Godot 原生 Control 节点 + 自定义主题
- **特效系统：** 基于 CQRS 事件钩子，不侵入核心逻辑

---

## 附录：现有运算方式

| 类型 | 运算 | `ModeType` |
|------|------|------------|
| 二元 | 加 | `Add` |
| 二元 | 减 | `Subtract` |
| 二元 | 乘 | `Multiply` |
| 二元 | 除 | `Divide` |
| 二元 | 模 | `Modulo` |
| 二元 | 指数幂 | `Power` |
| 二元 | N 次根 | `NthRoot` |
| 一元 | 绝对值 | `AbsoluteValue` |
| 一元 | 阶乘 | `Factorial` |
| 一元 | 平方根 | `SquareRoot` |
| 一元 | 向上取整 | `Ceil` |
| 一元 | 向下取整 | `Floor` |

---

*最后更新：2026-05-02*
