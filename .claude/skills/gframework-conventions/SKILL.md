---
name: gframework-conventions
description: 审查或自检 GFramework Godot C# 项目的个人代码规范——命名空间与目录映射、CQRS 事件/命令的 sealed 与属性可变性（事件 init / 命令 set）、命令输入禁 struct、Godot 节点 partial 与 [Log]+[ContextAware] 成对、GetNode % 唯一节点名、XML 中文注释、目录 snake_case、_Ready 调用链。当写完或改完 .cs 文件后自检、提交前把关、或按规范审查未提交变更时使用。仅适用于仓库根有 CONVENTIONS.md 的 GFramework 项目。
---

# GFramework 约定审查

面向 **GFramework Godot C# 项目**（仓库根有 `CONVENTIONS.md`）。这是个人框架风格的规范，
不具备泛用性——项目根没有 `CONVENTIONS.md` 就不要使用本技能。

## 流程

**1. 读真源。** 项目根的 `CONVENTIONS.md` 才是规范的权威版本，本技能只覆盖其中可静态判定的
部分，且可能滞后。动手前先读它，改动涉及哪块就读哪块（CQRS 事件 → CQRS 约定章节、
UI 页面 → UI 章节、提交 → 提交规范章节）。

**2. 跑确定性检查。** 静态规则引擎零成本、结果确定，先让它把机械性问题扫干净：

```bash
# 未提交的 .cs 变更（最常用）
node .claude/skills/gframework-conventions/scripts/review.mjs

# 已暂存变更（提交前把关）
node .claude/skills/gframework-conventions/scripts/review.mjs --staged

# 整个 scripts/ 目录 / 指定文件或目录
node .claude/skills/gframework-conventions/scripts/review.mjs --all
node .claude/skills/gframework-conventions/scripts/review.mjs scripts/cqrs/x/event
```

> 若技能不在 `.claude/skills/` 下，把命令里的目录前缀替换成实际 SKILL.md 所在目录。
> Node ≥ 18 即可，无依赖。

可选开关：`--json`（机器可读）、`--report`（完整报告写入 `.pi/review-report.txt`）、
`--strict`（有 error 时退出码 1，默认恒为 0）、`-h`。

**3. 修或报。** `error` 级必须改；`warning` 逐条判断——Godot 节点类缺 `[Log]`/`[ContextAware]`、
`GetNode` 未用 `%` 名称、事件属性缺 `required` 这类几乎都该改；`namespace-path` 之类的
warning 要结合目录结构确认后再动。报告时按 `error / warning` 分级、带 `文件:行号`。

## 脚本能查什么（15+ 条静态规则）

| 类别 | 规则（规则名） |
|------|------|
| 命名空间 | 文件范围声明、禁花括号（`namespace-scope`）· 与目录一一对应（`namespace-path`） |
| CQRS 事件 | `public sealed class`（`event-sealed`）· 属性 `{ get; init; }`（`event-immutable`）+ `required`（`event-required`） |
| CQRS 命令 | `public sealed class`（`command-sealed`）· 属性 `{ get; set; }`（`command-mutable`）+ `required`（`command-required`） |
| 命令输入 | 禁 `struct`、须实现 `ICommandInput`（`command-input-class`） |
| Godot 节点 | `partial` 且不 `sealed`（`node-partial` / `node-not-sealed`）· `[Log]`+`[ContextAware]` 成对（`log-context-aware`） |
| 节点引用 | `GetNode<T>("%Name")` 用 `%` 唯一名称（`get-node-unique-name`） |
| 注释 | 事件/命令/命令输入/接口必须有 `/// <summary>` 中文注释（`xml-summary`） |
| 结构 | 目录名 snake_case（`dir-snake-case`）· `_Ready()` 只做调用链（`ready-chain`） |

## 脚本查不到什么（必须人工/模型判断）

- 架构归属：这条命令/事件该不该存在、放哪层、是否跨越领域边界
- 语义正确性：命名是否达意、事件是否表达过去式、命令是否表达意图
- 契约完整性：schema↔config 数据目录、多语言 key 一致性、`[ContextAware]` 注入的上下文是否真的被用
- 已有测试兜底的部分：`tests/` 里的 `RepositoryConsistencyTests` 会校验 `sealed`、`[ContextAware]`+`[Log]`、
  schema↔config、多语言 key——静态扫描通过不等于测试通过，改动后跑 `dotnet test`

结论要给"改什么、为什么"，不要只贴脚本原文；脚本输出里的 `info` 级（如 `_Ready` 调用链）
按项目实际写法判断，不强制。
