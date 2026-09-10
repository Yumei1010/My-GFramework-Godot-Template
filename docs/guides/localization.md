# 本地化系统教程（Localization）

> 本文纯教学：讲清"为什么需要本地化 + 怎么用 GFramework 本地化系统"。
> 框架已带完整实现（`LocalizationManager` + `LocalizationTable` + `LocalizationString` + 内置 Formatters），无需造轮子。
> 模板已部分接入：`GodotLocalizationSettings`（设置系统已注册本地化应用器），配合 `ChangeLanguageCommand` 可驱动语言切换。

---

## 0. 一句话先说明白

**本地化 = 把 UI 文案从代码里抽出来，按语言分表存储，运行时按当前语言取词 + 支持变量/复数/条件格式化的完整系统。**

```
localization/                      ← 本地化数据目录（JSON 文件）
├─ zh/main.json                    ← 中文表：{ "hp": "生命值" }
├─ en/main.json                    ← 英文表：{ "hp": "HP" }
        │
        ▼
LocalizationManager（中央：从目录懒加载语言表）
   ├─ SetLanguage("en") → 加载 en 表（缺项自动回退 fallback 语言）
   ├─ GetText("main", "hp") → 按当前语言取词
   └─ 变量/复数/条件/紧凑数字 格式化器（{var:fmt:args}）
```

---

## 1. 为什么需要（老写法痛点）

| 方案 | 问题 |
|---|---|
| 文案硬编码字符串 | 换语言要改代码、重编译 |
| 每语言一份常量类 | 切语言要手动更新所有引用处 |
| 手写"缺项回退" | 每种语言缺词时逻辑散落 |

这套系统给的是：**按语言分表 + 缺项回退 + 切语言订阅 + 变量插值 + 复数/条件/紧凑数字格式化**。

---

## 2. 核心组件

| 组件 | 作用 |
|---|---|
| `LocalizationManager`（System） | 中央管理器：从 `LocalizationConfig` 指定目录**懒加载语言表**、`SetLanguage`、订阅语言变更、格式化器入口 |
| `LocalizationTable` | 一张语言表：`Name + Language + Fallback（回退语言链）`，由目录 JSON 加载 |
| `LocalizationString` | **懒加载字符串**：绑定表+键+变量，解析时按当前语言取文并提供格式化 |
| **Formatters**（内置） | `plural`（复数）/ `if`（条件）/ `compact`（紧凑数字 1.2K） |

> ⚠️ **机制真相**：语言表不是"代码注册"，而是从 `LocalizationConfig.LocalizationPath/{语言码}/{表名}.json` 加载（懒加载 + 回退语言自动链式）。这是它与"手写字典"的本质区别。

---

## 3. 五步上手

### 第 1 步：准备语言数据（JSON 文件）

```text
localization/
├─ zh/main.json
│   { "hp": "生命值", "enemy": "敌人", "hit": "击中 {count} 次" }
├─ en/main.json
│   { "hp": "HP", "enemy": "Enemy" }        // 缺 "hit" → 回退中文
```

### 第 2 步：配置 + 建管理器

```csharp
using GFramework.Core.Abstractions.Localization;
using GFramework.Core.Localization;

var config = new LocalizationConfig
{
    DefaultLanguage = "zh",
    FallbackLanguage = "zh",          // 缺词时回退到哪种语言
    LocalizationPath = "localization", // 语言数据根目录
    EnableHotReload = false           // 编辑器可开，改 JSON 热生效
};

var manager = new LocalizationManager(config);
manager.Initialize();   // ⚠️ 必须 Initialize（懒加载默认语言）
```

> `LocalizationManager` 是 `ISystem`——模板中注册进架构（`RegisterSystem`）即带生命周期。

### 第 3 步：切换语言

```csharp
manager.SetLanguage("en");        // 懒加载 en 表，uknown fallback 自动链
manager.GetText("main", "hp");   // "HP"
manager.GetText("main", "hit");  // en 缺 "hit" → 回退 zh → "击中 {count} 次"
```

### 第 4 步：变量插值（LocalizationString）

```csharp
// 惰性求值：取词时替换 {变量}
var hitStr = manager.GetString("main", "hit")
    .WithVariable("count", 3);
hitStr.Format();           // 取值用 Format()（不是 ToString）→ "击中 3 次"

// 带 formatter
var gold = manager.GetString("main", "gold")
    .WithVariable("amount", 1234)
    .WithFormatter("compact");   // "1.2K" 之类
```

### 第 5 步：订阅语言切换（UI 刷新）

```csharp
manager.SubscribeToLanguageChange(code => RefreshAllTexts());   // 切语言时重刷
manager.UnsubscribeFromLanguageChange(handler);
```

---

## 4. Formatters（内置三件）

| 名字 | 类 | 用途 | 示例 |
|---|---|---|---|
| `plural` | `PluralFormatter` | 复数规则（按数量取形态） | `{count} item(s)` |
| `if` | `ConditionalFormatter` | 条件文本 | `{enabled:if:true=开启|false=关闭}` |
| `compact` | `CompactNumberLocalizationFormatter` | 紧凑数字 | `1234 → 1.2K` |

### 自定义格式化器

```csharp
public sealed class MyFormatter : ILocalizationFormatter { ... }
manager.RegisterFormatter("my", new MyFormatter());
// 之后 {x:my:参数...} 可用
```

---

## 5. Godot 集成（模板已接）

模板的 `ModelModule` 已注册 `GodotLocalizationSettings`（本地化应用器），配合设置系统：

```csharp
// 切语言（走 CQRS 命令 → 设置 → 应用器 → 本地化)
this.SendCommand(new ChangeLanguageCommand("zh"));

// Godot 侧 LocalizationMap（GFramework.Godot.Setting.Data）
// 管理语言列表/显示名映射，供设置 UI 用
```

**链路**：`ChangeLanguageCommand` → 设置模型更新 → `GodotLocalizationSettings` 应用器 → `LocalizationManager.SetLanguage` → 订阅者刷新。

---

## 6. 常见坑

| 坑 | 说明 |
|---|---|
| **Fallback 表方向** | fallback 是"缺项时去查的表"，链式支持（zh→en→default） |
| **字段名写错** | `GetText` 取不到 key 返回什么需确认（建议用 `TryGetText` 或先 `ContainsKey`） |
| **变量名大小写** | `{count}` 变量名区分大小写（regex 匹配 `[a-zA-Z_]`） |
| **切语言晚刷新** | 已显示的文本不会自动变，需订阅 `SubscribeToLanguageChange` 手动重刷 |
| **表/语言文件没放对** | 表按 `{LocalizationPath}/{语言码}/{表名}.json` 组织，放错目录/命名 GetText 取空或回退 |

---

## 7. API 速查

| API | 说明 |
|---|---|
| `new LocalizationConfig { DefaultLanguage, FallbackLanguage, LocalizationPath, EnableHotReload }` | 配置（数据目录/默认/回退语言） |
| `new LocalizationManager(config)` + `Initialize()` | 建管理器并懒加载默认语言 |
| `manager.SetLanguage(code)` | 切换语言（懒加载表 + 广播订阅者） |
| `manager.GetText(table, key)` / `TryGetText(...)` | 取词（缺词回退链） |
| `manager.GetString(table, key)` | 取懒加载字符串（可加变量/格式化器） |
| `manager.RegisterFormatter(name, fmt)` | 注册自定义格式化器 |
| `manager.SubscribeToLanguageChange(cb)` / `Unsubscribe...` | 语言变更订阅 |
| 数据目录 `{LocalizationPath}/{语言码}/{表名}.json` | `{ "key": "text" }` 字典 + `{var}` / `{var:fmt:args}` |
| `localizationString.WithVariable(name, value)` / `.WithFormatter(name)` | 变量/格式化器 |

---

## 8. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Core/Localization/LocalizationManager.cs` | 管理器（语言/表/格式化器/订阅） |
| `GFramework.Core/Localization/LocalizationTable.cs` | 语言表（fallback/merge） |
| `GFramework.Core/Localization/LocalizationString.cs` | 懒加载字符串 + 变量/格式化器解析 |
| `GFramework.Core/Localization/Formatters/` | `Plural` / `Conditional` / `CompactNumber` |
| `GFramework.Godot/Setting/GodotLocalizationSettings.cs` + `LocalizationMap.cs` | Godot 设置联动 |
| `docs/zh-CN/game/localization.md`（若有） | 框架文档 |