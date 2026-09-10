# 日志系统教程（Logging）

> 本文纯教学：讲清"GFramework 日志体系怎么用 + 模板的会话文件日志怎么工作"。
> 框架自带完整日志管线（appender/filter/formatter/配置），模板已接入**控制台 + 会话文件双路输出**。
> 组件源码：`scripts/framework/logging/`；接线：`global/GameEntryPoint.cs`。

---

## 0. 一句话先说明白

**日志 = 一条管线（Logger → Appender → 输出端），支持多输出端组合（控制台 + 文件），并按级别过滤。**

模板已配好：**Godot 控制台（彩色）+ 会话文件（JSON，`user://logs/YYYYMMDD_HHmmss.log`）**，开发人员可随时按 `F12` 打开日志目录排查错误。

---

## 1. 为什么需要（发布版的痛）

| 场景 | 没有持久化日志 | 有会话文件日志 |
|---|---|---|
| 编辑器调试 | Godot 输出面板能看 | 同上，且落盘 |
| **发布版排错** | ❌ 玩家机器看不到 GD.Print | ✅ 日志在 `user://logs/`，可导出分析 |
| 玩家反馈"卡住了" | ❌ 无据可查 | ✅ 打开日志看时间线 |
| 崩溃复盘 | ❌ 现场丢失 | ✅ 崩溃前日志已落盘 |

---

## 2. 框架日志体系（全景）

```
你的代码：_log.Info("...") / this.GetLogger("X")
     │  [Log] 源生成器生成的静态 Logger
     ▼
ILoggerFactoryProvider（提供者：按名字造 Logger，带缓存）
     │
     ▼
Logger（级别过滤：IsEnabled → Write）
     │
     ▼
ILogAppender[]（输出端，可多个）
 ├─ GodotLogAppender   → GD.Print / 调试器（彩色模板）
 ├─ FileAppender       → 单文件写入
 ├─ RollingFileAppender→ 按大小轮转
 └─ AsyncLogAppender   → 异步缓冲（包装另一个 appender）
     │
     ▼
ILogFormatter（格式化：Default 文本 / Json 结构化）
+ ILogFilter（过滤：LogLevel / Namespace / Composite）
```

**核心理念**：输出端（Appender）标准化，所以可以**任意组合**——这就是"控制台 + 文件"双路的实现基础。

### Core vs Godot 的分工（框架设计边界）

| 层 | 提供 |
|---|---|
| **Core**（`GFramework.Core.Logging`） | 管线基础设施：`ILogAppender`/`CompositeLogger`/文件类 appender/Formatter/Filter/JSON 配置加载 |
| **Godot**（`GFramework.Godot.Logging`） | 宿主落点：`GodotLogAppender`（控制台）、`GodotLog` 静态入口、配置热重载 |

> 框架归档明确："Godot 包只提供宿主控制台落点；文件、JSON、滚动等由 Core 负责，可被组合。"

---

## 3. 日常用法（业务侧）

### 打日志

```csharp
[Log]                       // 源生成器生成静态 _log
[ContextAware]
public partial class MyNode : Node
{
    public override void _Ready()
    {
        _log.Debug("调试信息");            // Debug
        _log.Info("玩家 {0} 登录", id);    // 格式化（有占位符才格式化）
        _log.Warn("资源缺失");
        _log.Error("加载失败", ex);        // 带异常（会输出堆栈）
    }
}
```

### 结构化属性（⚠️ 需 `IStructuredLogger`）

```csharp
// ILogger 接口没有结构化属性重载，调用会误绑 params object[] 把属性当格式参数丢掉！
// 要带属性必须转 IStructuredLogger：
var log = (IStructuredLogger)this.GetLogger("Gameplay");
log.Log(LogLevel.Error, "计算失败", new InvalidOperationException("boom"),
        ("scope", "deck"), ("seed", 42));
// → JSON 行含 "properties":{"scope":"deck","seed":42} + "exception":{...}
```

### 按类别取 Logger

```csharp
var log = this.GetLogger("Battle");     // 类别名可用于 NamespaceFilter 精确控级
log.Info("回合开始");
```

---

## 4. 日志去哪了（模板双路输出）

| 输出端 | 内容 | 用途 |
|---|---|---|
| **Godot 控制台** | 彩色模板（Debug 模式含 BBCode 颜色） | 编辑器内实时看 |
| **会话文件** | `user://logs/YYYYMMDD_HHmmss.log`，**每行一个 JSON** | 持久化、导出、程序化分析 |

JSON 行示例：

```json
{"timestamp":"2026-09-08T02:22:40.8061506Z","level":"INFO","logger":"GFrameworkTemplate.global.GameEntryPoint","message":"框架入口点就绪."}
{"timestamp":"2026-09-08T02:22:41.1000000Z","level":"ERROR","logger":"X","message":"计算失败","properties":{"scope":"deck"},"exception":{"type":"System.InvalidOperationException","message":"boom","stackTrace":"..."}}
```

### 查日志

```csharp
// 启动即打印会话日志完整路径（Godot 输出面板可见）
// [Log] 会话日志文件: C:/Users/.../My-GFramework-Godot-Template/logs/20260908_102559.log

// 开发模式按 F12 打开日志目录（系统文件管理器）
GameEntryPoint.OpenSessionLogDirectory();
```

---

## 5. 工作原理（模板组件）

| 文件 | 职责 |
|---|---|
| `SessionFileLoggerFactoryProvider` | `ILoggerFactoryProvider`：组合 **GodotLogAppender + FileAppender(Json)** 双路；每个 logger 共享 appenders |
| `CompositeLoggerFactory` | 内部工厂：用共享 appender 集合造 `CompositeLogger` |
| `SessionLogFileHelper` | 生成会话文件名 `yyyyMMdd_HHmmss` + 建目录（纯逻辑可测） |
| `SessionLogFileInfo` | 会话文件信息（路径/目录/文件名） |
| `LogOpenHelper` | 打印路径 + `OS.ShellOpen` 打开目录 |

**接线**（`GameEntryPoint._Ready`）：

```csharp
var logDirectory = ProjectSettings.GlobalizePath("user://logs/");   // ⚠️ FileAppender 要真实路径
SessionLogFile = SessionLogFileHelper.CreateNow(logDirectory);
_logProvider = new SessionFileLoggerFactoryProvider(SessionLogFile.FullPath) { MinLevel = LogLevel.Debug };
LogOpenHelper.PrintSessionLogPath(SessionLogFile.FullPath);
// → ArchitectureConfiguration.LoggerProperties.LoggerFactoryProvider = _logProvider

// 退出时 Flush 保证尾部日志落盘
public override void _ExitTree() { _logProvider?.Flush(); ... }
```

---

## 6. 调整日志行为

### 级别 / 模式（GodotLoggerOptions）

```csharp
new GodotLoggerFactoryProvider(new GodotLoggerOptions
{
    Mode = GodotLoggerMode.Debug,        // Debug（彩色+低级别）/ Release（纯文本+Info 以上）
    DebugMinLevel = LogLevel.Debug,
    ReleaseMinLevel = LogLevel.Info,
    DebugOutputTemplate = "[{timestamp:HH:mm:ss}] [{level:u3}] [{category:l16}] {message}{properties}",
    Colors = { [LogLevel.Error] = "red", [LogLevel.Warning] = "orange" }
});
```

模板的 `SessionFileLoggerFactoryProvider` 默认用 `GodotLogAppender()`（默认 options）；要自定义控制台行为可加构造参数传入。

### 配置文件（appsettings.json 热重载）

`GodotLog` 支持从 `res://appsettings.json` 读配置并**热重载**：

```csharp
GodotLog.Configure(options => options.DebugMinLevel = LogLevel.Trace);   // 代码覆盖（须在使用前）
var path = GodotLog.ConfigurationPath;    // 发现到的配置路径
GodotLog.UseAsDefaultProvider();          // 设为全局默认
```

### 声明式 JSON 配置（多 appender 组合）

框架 `LoggingConfigurationLoader` 支持从 JSON 声明 appender 组合（`console` / `file` / `rollingfile` / `async`）+ formatter（`default` / `json`）+ filter：

```json
{
  "minLevel": "Info",
  "appenders": [
    { "type": "console", "useColors": true },
    { "type": "rollingFile", "filePath": "logs/app.log",
      "maxFileSize": 10485760, "maxFileCount": 5, "formatter": "json" },
    { "type": "async", "bufferSize": 10000,
      "innerAppender": { "type": "file", "filePath": "logs/async.log" } }
  ],
  "loggerLevels": { "GFrameworkTemplate.scripts.cqrs": "Debug" }
}
```

---

## 7. 扩展点（自定义能力）

| 扩展点 | 用途 |
|---|---|
| 自定义 `ILogAppender` | 新增输出端（上报服务器、内存环形缓冲、崩溃快照…） |
| 自定义 `ILogFilter` | 自定义过滤规则（仅报错、采样限流…） |
| 自定义 `ILogFormatter` | 自定义输出格式（CSV、带线程 ID…） |
| 自定义 `ILoggerFactoryProvider` | 完全定制管线（模板的会话文件 provider 就是范例） |

**最简原则**：优先用现成 appender 组合（`CompositeLogger`），不要急于自写。

---

## 8. 常见坑

| 坑 | 说明 |
|---|---|
| **`user://` 路径喂给 FileAppender** | 它用 `System.IO`，必须先 `ProjectSettings.GlobalizePath("user://")` 转真实路径 |
| **结构化属性丢失** | `ILogger` 无 properties 重载 → 必须用 `IStructuredLogger` 调用 |
| **文件首行 UTF-8 BOM** | 框架 `FileAppender` 用 `Encoding.UTF8`，首行带 BOM（追加不重复）；记事本/Excel 友好，可接受 |
| **日志没落盘** | 非 AutoFlush 场景需 `Flush()`；退出时 `_ExitTree` 记得 Flush |
| **发布版日志丢了** | 导出必须包含 `logs/` 可写（`user://` 天然可写）；别把日志写进 `res://` |
| **MinLevel 配太松** | Debug 全开会拖性能/膨胀文件；发布版用 Info/Warning |
| **同目录多会话文件堆积** | 会话文件不自动清理，需自己加保留策略（v2 候选） |

---

## 9. API 速查

| API | 说明 |
|---|---|
| `_log.Debug/Info/Warn/Error/Fatal` | 源生成器静态 Logger |
| `IStructuredLogger.Log(level, msg, exception?, (key,value)[])` | 结构化日志（含属性/异常） |
| `this.GetLogger(category)` | 取指定类别 Logger |
| `ILoggerFactoryProvider` / `ILoggerFactory` / `ILogger` | 管线接口 |
| `ILogAppender.Append(entry)` / `Flush()` | 输出端契约 |
| `FileAppender(path, formatter?, filter?)` | 文件输出 |
| `RollingFileAppender(path, maxSize, maxCount, ...)` | 轮转文件输出 |
| `JsonLogFormatter` / `DefaultLogFormatter` | 格式化器 |
| `LoggingConfigurationLoader.LoadFromJson(path)` | JSON 配置 → `ILoggerFactory` |
| `GodotLoggerOptions`（Mode/MinLevel/Template/Colors） | Godot 输出选项 |
| `GodotLog.Configure(...)` / `UseAsDefaultProvider()` | Godot 静态入口 |
| `SessionFileLoggerFactoryProvider(path)` | 模板：双路输出提供者 |
| `GameEntryPoint.OpenSessionLogDirectory()` | 打开日志目录（F12） |

---

## 10. 源码阅读入口

| 路径 | 内容 |
|---|---|
| `GFramework.Core.Abstractions/Logging/` | 接口（ILogger/ILogAppender/ILogFilter/ILogFormatter…） |
| `GFramework.Core/Logging/Appenders/` | 内置 appender（Console/File/RollingFile/Async） |
| `GFramework.Core/Logging/Formatters/` | Default / Json 格式化器 |
| `GFramework.Core/Logging/LoggingConfigurationLoader.cs` | JSON 配置加载 |
| `GFramework.Godot/Logging/GodotLogAppender.cs` | Godot 控制台输出端 |
| `GFramework.Godot/Logging/GodotLog.cs` | Godot 静态入口 + 热重载配置源 |
| `scripts/framework/logging/` | 模板会话文件日志组件（+ README） |
| `docs/plan/session-file-logging.md` | 组件设计文档 |

---

## 附：模板代码验证

`tests/.../SessionLoggingTests.cs` 覆盖（6 个用例）：

| 用例 | 验证内容 |
|---|---|
| Create_固定时间戳_生成带时间戳文件名 | 会话文件命名 `yyyyMMdd_HHmmss.log` |
| Create_自动创建目录 | 目录不存在时自动创建 |
| CreateNow_使用当前时间戳 | 当前时间命名 |
| Provider_写入Json格式日志行 | JSON 落盘 + 字段正确 |
| Provider_结构化属性与异常写入Json | `properties` + `exception` 输出 |
| Provider_MinLevel过滤低级别日志 | 级别过滤生效 |

**框架升级后跑 `dotnet test` 即可发现本文档是否过期。**