# config/ — 配置数据目录（示例）

本目录演示 GFramework Config 配置系统：**JSON Schema（`schemas/`）定义结构 → Source Generator 生成强类型访问代码 → 本目录 YAML 提供数据**。

## ⚠️ 这是教学示例

`monster/` 只是用来演示配置系统能力的**示例数据**（展示枚举/数值校验/默认值等），不指向任何具体游戏项目。

**开始你自己的游戏开发时，请删除/替换为我自己的配置域**，例如：

```text
config/
├─ my_domain/        # 你的业务配置域（一对象一文件）
│  ├─ entry_a.yaml
│  └─ entry_b.yaml
schemas/
└─ my_domain.schema.json   # 对应结构契约
```

## 用法

在 `schemas/` 新建 `xxx.schema.json`（可加 `x-gframework-config-path` 指定数据目录），生成器自动收集并生成强类型代码（命名空间固定 `GFramework.Game.Config.Generated`）。参考 `monster.schema.json`。

## 已知边界（0.7.1）

- **数组字段暂不可用**：schema 数组会生成 `IReadOnlyList<T>`，但 YamlDotNet 无法反序列化（框架限制，见 `docs/research/gframework-extensions/game-services.md`）
- 不支持 `oneOf` / `anyOf` / `additionalProperties: true` 等开放形状（支持子集见框架文档 `docs/zh-CN/game/config-system.md`）