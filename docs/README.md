# 文档索引

本目录收录模板的**教程**、**研究**与**规划**文档。

## 教程（`docs/guides/`）— 框架能力用法教学

每个能力一篇，均配有可运行的验证测试（`tests/` 下的同名 `*Tests.cs`），框架升级后跑 `dotnet test` 即可发现文档过期。

| 文档 | 能力 |
|---|---|
| [object-pool.md](guides/object-pool.md) | 对象池（`AbstractNodePoolSystem`） |
| [ecs.md](guides/ecs.md) | Arch ECS（接入/组件/系统/驱动） |
| [store.md](guides/store.md) | Store 状态管理（Redux 风格） |
| [config.md](guides/config.md) | Config 配置系统（YAML + Schema + 源生成器） |
| [coroutine.md](guides/coroutine.md) | 协程系统（调度器/指令/CQRS 集成） |
| [localization.md](guides/localization.md) | 本地化系统（文件驱动语言表） |
| [pause.md](guides/pause.md) | 暂停栈（分组/令牌/UI 联动） |
| [audio.md](guides/audio.md) | 音频设置与音量管理 |
| [ui.md](guides/ui.md) | UI 路由与弹窗 |
| [logging.md](guides/logging.md) | 日志系统（双路输出/会话文件） |
| [input-rebind.md](guides/input-rebind.md) | 输入改键（捕获/冲突交换/快照持久化） |

## 研究（`docs/research/`）— 框架源码调研

| 文档 | 内容 |
|---|---|
| [gframework-extensions/README.md](research/gframework-extensions/README.md) | 扩展接口总览（120+ 接口三类盘点：即用/留空/边界） |
| [gframework-extensions/input-domain.md](research/gframework-extensions/input-domain.md) | 输入域三层协议（绑定/设备/UI 语义动作） |
| [gframework-extensions/pause-stack.md](research/gframework-extensions/pause-stack.md) | 暂停栈（分组引用计数） |
| [gframework-extensions/store.md](research/gframework-extensions/store.md) | Store 状态管理 |
| [gframework-extensions/game-services.md](research/gframework-extensions/game-services.md) | 服务层（Config/Data/Storage/Resource/Coroutine/RichText…） |
| [input-hot-switching/README.md](research/input-hot-switching/README.md) | 输入热切换（设备识别/提示图标/焦点归位）+ Steam Controller 实测 |
| [syntax-sugar/README.md](research/syntax-sugar/README.md) | SourceGenerator 语法糖清单 |

## 规划（`docs/plan/`）— 组件决策与设计

| 文档 | 内容 |
|---|---|
| [component-roadmap.md](plan/component-roadmap.md) | 组件决策记录（框架已提供 / 不纳入模板 / 游戏类型相关）与教程索引 |
| [session-file-logging.md](plan/session-file-logging.md) | 会话文件日志组件的设计文档 |

## 相关

- 项目规范：[CONVENTIONS.md](../CONVENTIONS.md)
- 开发指导：[CLAUDE.md](../CLAUDE.md)
- 框架官方文档：GFramework 仓库 `docs/zh-CN/`
