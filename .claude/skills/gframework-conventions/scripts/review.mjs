#!/usr/bin/env node
/**
 * GFramework 个人约定审查 —— 确定性静态规则引擎（独立 CLI）
 *
 * 来源：原 pi 扩展 conventions-review（extensions/conventions-review/index.ts）的规则引擎，
 * 规则与输出文本逐字保留，仅去掉扩展/工具的包装，改为命令行调用。
 *
 * 静态规则（零成本、确定性）：
 *   命名空间 / sealed / 属性可变性 / required / struct 禁止 / partial /
 *   [Log]+[ContextAware] 成对 / % 唯一节点名 / XML 注释 / snake_case 目录 /
 *   _Ready() 调用链等 15+ 条规则
 *
 * 用法（在项目根目录执行）：
 *   node .pi/skills/gframework-conventions/scripts/review.mjs              # 审查 git 未提交的 .cs
 *   node .pi/skills/gframework-conventions/scripts/review.mjs --staged     # 审查已暂存变更
 *   node .pi/skills/gframework-conventions/scripts/review.mjs --all        # 审查整个 scripts/
 *   node .pi/skills/gframework-conventions/scripts/review.mjs scripts/cqrs  # 审查指定文件/目录
 *
 * 可选开关：
 *   --json     输出机器可读 JSON
 *   --report   同时把完整报告写入 .pi/review-report.txt
 *   --strict   存在 error 级问题时以退出码 1 结束（默认恒为 0）
 *   -h/--help  显示帮助
 */

import { readFile, readdir, stat, mkdir, writeFile } from "node:fs/promises";
import { execFile } from "node:child_process";
import { basename, join, relative, sep } from "node:path";

// ── 根命名空间推断 ─────────────────────────────────────────
async function inferRootNamespace(cwd) {
  // 1) 从 csproj RootNamespace 读取
  try {
    const entries = await readdir(cwd);
    const csproj = entries.find((f) => f.endsWith(".csproj"));
    if (csproj) {
      const content = await readFile(join(cwd, csproj), "utf-8");
      const m = content.match(/<RootNamespace>([^<]+)<\/RootNamespace>/);
      if (m) return m[1];
    }
  } catch { /* ignore */ }
  // 2) 从 scripts 下的 namespace 声明推断最长公共前缀
  try {
    const scriptsDir = join(cwd, "scripts");
    const nsSet = new Set();
    await walkFiles(scriptsDir, 40, async (file) => {
      if (!file.endsWith(".cs")) return;
      const content = await readFile(file, "utf-8").catch(() => "");
      const m = content.match(/namespace\s+([\w.@]+);/);
      if (m) nsSet.add(m[1].replace(/\.@\w+/g, ""));
    });
    if (nsSet.size === 0) return null;
    const nsList = [...nsSet];
    const first = nsList[0].split(".");
    let prefix = "";
    for (let i = 0; i < first.length; i++) {
      const part = first[i];
      if (nsList.every((ns) => ns.split(".")[i] === part)) {
        prefix = prefix ? `${prefix}.${part}` : part;
      } else break;
    }
    return prefix || null;
  } catch {
    return null;
  }
}

// ── 文件遍历 ────────────────────────────────────────────────
async function walkFiles(dir, limit, cb, depth = 0) {
  let count = 0;
  if (depth > 8) return 0;
  let entries;
  try {
    entries = await readdir(dir);
  } catch {
    return 0;
  }
  for (const entry of entries) {
    if (count >= limit) break;
    if (entry === ".git" || entry === "node_modules" || entry === ".godot" || entry === "bin" || entry === "obj" || entry === "addons") continue;
    const full = join(dir, entry);
    let isDir = false;
    try {
      isDir = (await stat(full)).isDirectory();
    } catch { continue; }
    if (isDir) {
      count += await walkFiles(full, limit - count, cb, depth + 1);
    } else if (entry.endsWith(".cs")) {
      await cb(full);
      count++;
    }
  }
  return count;
}

// ── 基础工具 ────────────────────────────────────────────────
function toPosix(p) {
  return p.split(sep).join("/");
}

/** 查找第一个类声明及其前导特性/注释块 */
function findClassDecl(content, className) {
  const pattern = new RegExp(`public\\s+(?:sealed\\s+|abstract\\s+|partial\\s+|static\\s+)*class\\s+${className ?? "\\w+"}`);
  const m = content.match(pattern);
  if (!m || m.index === undefined) return null;
  const start = m.index;
  return { index: start, text: m[0], before: extractLeadingBlock(content, start) };
}

/**
 * 提取声明正上方的注释与特性块。
 * 从声明处向前逐行收集 `///`、`//`、`[Attr]` 行，遇到空行或其他代码即停止。
 * 此前用固定 300 字符窗口截取，中文注释一写长就会把 `/// <summary>` 挤出窗口而误报"缺少注释"。
 */
function extractLeadingBlock(content, start) {
  const lines = content.slice(0, start).split("\n");
  const kept = [];
  for (let i = lines.length - 1; i >= 0; i--) {
    const trimmed = lines[i].trim();
    if (trimmed === "") {
      // 声明前的缩进/空行：还没收集到内容时跳过（split 的尾元素恒为空串），否则视为块边界
      if (kept.length === 0) continue;
      break;
    }
    const isDocLine = trimmed.startsWith("///") || trimmed.startsWith("//") || trimmed.startsWith("[");
    if (!isDocLine) break;
    kept.unshift(lines[i]);
  }
  return kept.join("\n");
}

function hasSummary(before) {
  return /\/\/\/\s*<summary>/.test(before);
}

function hasAttribute(before, attr) {
  return new RegExp(`\\[\\s*${attr}\\s*\\]`).test(before);
}

function countLines(content, index) {
  return content.slice(0, index).split("\n").length;
}

/** 提取属性声明列表 */
function findPropertyDecls(content, afterIndex) {
  const props = [];
  const segment = content.slice(afterIndex, afterIndex + 8000);
  const re = /public\s+(required\s+)?(?:readonly\s+)?[\w.<>?,?\[\] ]+?\s+\w+\s*\{\s*get;\s*(?:init|set);\s*\}/g;
  let m;
  while ((m = re.exec(segment)) !== null) {
    props.push({
      accessor: m[0].includes("{ get; init; }") || m[0].includes("{get; init;}") ? "init" : "set",
      hasRequired: !!m[1],
      decl: m[0].trim(),
      index: afterIndex + m.index,
    });
  }
  return props;
}

/**
 * 从属性声明中取出属性名。
 * 旧版扩展用 `decl.split(/\s+/).pop()` 取名字，多行/带花括号的声明会取到 `}`，
 * 消息里出现「事件属性 } 禁止…」，已修正。
 */
function propName(decl) {
  const m = decl.match(/(\w+)\s*\{\s*get;/);
  return m ? m[1] : decl;
}

/** 文件是否属于 CQRS 类别 */
function categorize(file) {
  const p = toPosix(file);
  const name = basename(file);
  if (p.includes("/input/") && name.endsWith("CommandInput.cs")) return "command-input";
  if (name.endsWith("Event.cs")) return "event";
  if (name.endsWith("Command.cs")) return "command";
  if (name.endsWith("Query.cs")) return "query";
  return "other";
}

/** 类是否继承 Godot 节点类型 */
function isGodotNode(content, afterIndex) {
  const segment = content.slice(afterIndex, afterIndex + 600);
  const braceIdx = segment.indexOf("{");
  const header = (braceIdx > -1 ? segment.slice(0, braceIdx) : segment).replace(/\n/g, " ");
  return /:\s*[\w.<>]+(?:\s*,\s*[\w.<>]+)*\s*$/.test(header) &&
    /(?:Node|Control|Button|CanvasLayer|Panel|Label|TextureRect|RichTextLabel|ScrollContainer|HBoxContainer|VBoxContainer|MarginContainer|CenterContainer|GridContainer|Container|PanelContainer|Sprite2D|Area2D|CharacterBody2D)\b/.test(header);
}

// ── 规则检查器 ──────────────────────────────────────────────
function checkFile(content, file, rootNs) {
  const issues = [];
  const rel = toPosix(file);
  const relToScripts = rel.includes("/scripts/") ? rel.slice(rel.indexOf("/scripts/") + 1) : rel;
  const category = categorize(file);
  const name = basename(file);

  // ── 1. 命名空间：文件范围声明，禁止花括号 ────────────────
  const blockNs = content.match(/namespace\s+[\w.@]+\s*\{/);
  if (blockNs) {
    issues.push({
      severity: "error",
      rule: "namespace-scope",
      file: rel,
      line: countLines(content, blockNs.index ?? 0),
      message: "禁止传统花括号命名空间，应使用文件范围声明 `namespace X.Y.Z;`",
      suggestion: "namespace X.Y.Z;",
    });
  }
  const fileScopeNs = content.match(/namespace\s+([\w.@]+);/);
  if (!fileScopeNs && !blockNs && content.includes("namespace")) {
    issues.push({
      severity: "warning",
      rule: "namespace-scope",
      file: rel,
      message: "未找到文件范围命名空间声明（namespace X.Y.Z;）",
    });
  }

  // ── 2. 命名空间与目录路径一致性 ──────────────────────────
  if (fileScopeNs && rootNs && relToScripts.startsWith("scripts/")) {
    const ns = fileScopeNs[1];
    // 去掉根命名空间前缀和 scripts 段（目录比较从 scripts/ 之下开始）
    if (ns === rootNs || ns.startsWith(rootNs + ".")) {
      const nsTail = ns.slice(rootNs.length + 1).split(".").slice(1);
      const dirTail = relToScripts
        .replace(/^scripts\//, "")
        .split("/")
        .slice(0, -1);
      // 目录段（event 目录 ↔ @event；文件数可能少于 ns 段——允许 ns 长于目录，如 command 类含子目录）
      let ok = true;
      for (let i = 0; i < dirTail.length; i++) {
        const dirSeg = dirTail[i];
        const nsSeg = nsTail[i]?.replace(/^@/, "");
        if (nsSeg !== dirSeg) { ok = false; break; }
      }
      if (!ok) {
        issues.push({
          severity: "warning",
          rule: "namespace-path",
          file: rel,
          line: countLines(content, fileScopeNs.index ?? 0),
          message: `命名空间 ${ns} 与目录路径 scripts/${dirTail.join("/")} 不一致`,
          suggestion: "命名空间必须与目录层次一一对应",
        });
      }
    }
  }

  const classDecl = findClassDecl(content);

  // ── 3. CQRS 事件检查 ─────────────────────────────────────
  if (category === "event") {
    if (classDecl && !/public\s+sealed\s+class/.test(classDecl.text)) {
      issues.push({
        severity: "error",
        rule: "event-sealed",
        file: rel,
        line: countLines(content, classDecl.index),
        message: `事件类 ${name.replace(".cs", "")} 必须是 public sealed class`,
        suggestion: "public sealed class 添加 sealed 修饰符",
      });
    }
    if (classDecl) {
      for (const prop of findPropertyDecls(content, classDecl.index)) {
        if (prop.accessor === "set") {
          issues.push({
            severity: "error",
            rule: "event-immutable",
            file: rel,
            line: countLines(content, prop.index),
            message: `事件属性 ${propName(prop.decl)} 禁止 { get; set; }（事件不可变）`,
            suggestion: "改用 { get; init; } + required",
          });
        } else if (!prop.hasRequired) {
          issues.push({
            severity: "warning",
            rule: "event-required",
            file: rel,
            line: countLines(content, prop.index),
            message: `事件属性 ${propName(prop.decl)} 缺少 required 修饰符`,
            suggestion: "public required Type Prop { get; init; }",
          });
        }
      }
    }
    if (classDecl && !hasSummary(classDecl.before)) {
      issues.push({
        severity: "warning",
        rule: "xml-summary",
        file: rel,
        line: countLines(content, classDecl.index),
        message: `事件类 ${name.replace(".cs", "")} 缺少 /// <summary> 中文注释`,
      });
    }
  }

  // ── 4. CQRS 命令检查 ─────────────────────────────────────
  if (category === "command") {
    if (classDecl && !/public\s+sealed\s+class/.test(classDecl.text)) {
      issues.push({
        severity: "error",
        rule: "command-sealed",
        file: rel,
        line: countLines(content, classDecl.index),
        message: `命令类 ${name.replace(".cs", "")} 必须是 public sealed class`,
        suggestion: "添加 sealed 修饰符",
      });
    }
    if (classDecl) {
      for (const prop of findPropertyDecls(content, classDecl.index)) {
        if (prop.accessor === "init") {
          issues.push({
            severity: "error",
            rule: "command-mutable",
            file: rel,
            line: countLines(content, prop.index),
            message: `命令属性 ${propName(prop.decl)} 禁止 { get; init; }（命令需要可写性）`,
            suggestion: "改用 { get; set; } + required",
          });
        } else if (!prop.hasRequired) {
          issues.push({
            severity: "warning",
            rule: "command-required",
            file: rel,
            line: countLines(content, prop.index),
            message: `命令属性 ${propName(prop.decl)} 缺少 required 修饰符`,
            suggestion: "public required Type Prop { get; set; }",
          });
        }
      }
    }
    if (classDecl && !hasSummary(classDecl.before)) {
      issues.push({
        severity: "warning",
        rule: "xml-summary",
        file: rel,
        line: countLines(content, classDecl.index),
        message: `命令类 ${name.replace(".cs", "")} 缺少 /// <summary> 中文注释`,
      });
    }
  }

  // ── 5. 命令输入检查 ──────────────────────────────────────
  if (category === "command-input") {
    if (/public\s+(?:sealed\s+)?struct\s+\w+/.test(content)) {
      issues.push({
        severity: "error",
        rule: "command-input-class",
        file: rel,
        message: "命令输入禁止使用 struct，必须是 sealed class : ICommandInput",
        suggestion: "public sealed class XxxCommandInput : ICommandInput",
      });
    }
    if (classDecl && !classDecl.text.includes("ICommandInput") && !/:/.test(content.slice(classDecl.index, classDecl.index + 300))) {
      issues.push({
        severity: "warning",
        rule: "command-input-class",
        file: rel,
        line: countLines(content, classDecl.index),
        message: "命令输入类未实现 ICommandInput 接口",
        suggestion: "public sealed class XxxCommandInput : ICommandInput",
      });
    }
    if (classDecl && !hasSummary(classDecl.before)) {
      issues.push({
        severity: "warning",
        rule: "xml-summary",
        file: rel,
        line: countLines(content, classDecl.index),
        message: `命令输入类 ${name.replace(".cs", "")} 缺少 /// <summary> 中文注释`,
      });
    }
  }

  // ── 6. Godot 节点检查 ────────────────────────────────────
  if (classDecl && isGodotNode(content, classDecl.index)) {
    const line = countLines(content, classDecl.index);
    const isAbstract = /public\s+abstract\s+class/.test(classDecl.text);
    if (!isAbstract && !/public\s+partial\s+class/.test(classDecl.text)) {
      issues.push({
        severity: "warning",
        rule: "node-partial",
        file: rel,
        line,
        message: `Godot 节点类 ${name.replace(".cs", "")} 必须是 public partial class（Godot 源代码生成器要求）`,
        suggestion: "public partial class 添加 partial 修饰符",
      });
    }
    if (/public\s+sealed\s+class/.test(classDecl.text)) {
      issues.push({
        severity: "warning",
        rule: "node-not-sealed",
        file: rel,
        line,
        message: `Godot 节点类 ${name.replace(".cs", "")} 不能 sealed（Godot 需要生成派生类）`,
        suggestion: "移除 sealed，使用 partial",
      });
    }
    const hasLog = hasAttribute(classDecl.before, "Log");
    const hasContextAware = hasAttribute(classDecl.before, "ContextAware");
    if (!hasLog || !hasContextAware) {
      issues.push({
        severity: "warning",
        rule: "log-context-aware",
        file: rel,
        line,
        message: `Godot 节点类 ${name.replace(".cs", "")} 缺少 [Log] 或 [ContextAware]（需成对标注，[Log] 在前）`,
        suggestion: `[Log]\n[ContextAware]\n${classDecl.text}`,
      });
    }
  }

  // ── 7. GetNode 必须使用 % 唯一名称 ───────────────────────
  const getNodeRe = /GetNode<[^>]+>\("([^"]+)"\)/g;
  let gm;
  while ((gm = getNodeRe.exec(content)) !== null) {
    if (!gm[1].startsWith("%")) {
      issues.push({
        severity: "warning",
        rule: "get-node-unique-name",
        file: rel,
        line: countLines(content, gm.index),
        message: `GetNode<...>("${gm[1]}") 未使用 % 唯一名称语法`,
        suggestion: `GetNode<...>("%${gm[1]}")`,
      });
    }
  }

  // ── 8. 接口必须带 XML 注释 ───────────────────────────────
  if (name.startsWith("I") && name.endsWith(".cs") && classDecl && !hasSummary(classDecl.before)) {
    issues.push({
      severity: "warning",
      rule: "xml-summary",
      file: rel,
      line: countLines(content, classDecl.index),
      message: `接口 ${name.replace(".cs", "")} 缺少完整的 /// <summary> 注释`,
    });
  }

  // ── 9. _Ready() 应只做调用链 ─────────────────────────────
  const readyRe = /public\s+override\s+void\s+_Ready\s*\(\s*\)\s*\{/;
  const readyMatch = content.match(readyRe);
  if (readyMatch && readyMatch.index !== undefined) {
    const start = readyMatch.index + readyMatch[0].length;
    const body = content.slice(start, start + 600);
    const endIdx = body.indexOf("}");
    const bodyContent = endIdx > -1 ? body.slice(0, endIdx) : body;
    const stmts = bodyContent
      // 先剥掉行尾注释（如 `RegisterEvents();   // 说明`），否则整句不以 `)` 结尾而被误判为非调用链
      .split(";")
      .map((s) => s.replace(/\/\/[^\n]*/g, "").trim())
      .filter(Boolean);
    const nonCallChain = stmts.filter(
      // 接受可选的赋值前缀（如 `_ = ReadyAsync()`——项目规范要求的丢弃 Task 写法）
      (s) => !/^(?:await\s+)?(?:[\w.]+\s*=\s*)?[\w.]+\(.*\)$/.test(s) && !/^(?:\/\/.*)?$/.test(s),
    );
    if (nonCallChain.length > 0) {
      issues.push({
        severity: "info",
        rule: "ready-chain",
        file: rel,
        line: countLines(content, readyMatch.index),
        message: "_Ready() 中不应直接编写业务逻辑，应委托给 ReadyAsync() → ConnectSignal() → RegisterEvent()",
        suggestion: "_Ready() 只保留方法调用链",
      });
    }
  }

  return issues;
}

// ── 目录名 snake_case 检查 ─────────────────────────────────
async function checkDirNames(cwd) {
  const issues = [];
  const scriptsDir = join(cwd, "scripts");
  const seen = new Set();
  const walk = async (dir, depth) => {
    if (depth > 8 || seen.has(dir)) return;
    seen.add(dir);
    let entries;
    try {
      entries = await readdir(dir);
    } catch { return; }
    for (const entry of entries) {
      if ([".git", "node_modules", ".godot", "bin", "obj", "addons"].includes(entry)) continue;
      const full = join(dir, entry);
      let isDir = false;
      try { isDir = (await stat(full)).isDirectory(); } catch { continue; }
      if (!isDir) continue;
      if (/[A-Z]/.test(entry)) {
        issues.push({
          severity: "warning",
          rule: "dir-snake-case",
          file: toPosix(relative(cwd, full)),
          message: `目录名 "${entry}" 违反 snake_case 规范（禁止驼峰/大写）`,
          suggestion: "使用全小写 + 下划线，如 mode_button、time_bar",
        });
      }
      await walk(full, depth + 1);
    }
  };
  await walk(scriptsDir, 0);
  return issues;
}

// ── 主审查入口 ─────────────────────────────────────────────
async function reviewPaths(cwd, files) {
  const issues = [];
  const rootNs = await inferRootNamespace(cwd);
  let fileCount = 0;

  const unique = [...new Set(files)].filter((f) => f.endsWith(".cs")).slice(0, 200);
  for (const file of unique) {
    const full = join(cwd, file);
    let content = "";
    try {
      content = await readFile(full, "utf-8");
    } catch {
      continue;
    }
    fileCount++;
    issues.push(...checkFile(content, file, rootNs));
  }
  issues.push(...(await checkDirNames(cwd)));

  const filesWithIssues = new Set(issues.map((i) => i.file)).size;
  return { issues, fileCount, filesWithIssues };
}

function getGitChangedFiles(cwd, staged) {
  const args = ["diff", "--name-only", "--diff-filter=ACMR"];
  if (staged) args.push("--cached");
  return new Promise((resolve) => {
    execFile("git", args, { cwd, maxBuffer: 10 * 1024 * 1024 }, (err, stdout) => {
      if (err) resolve([]);
      else resolve(stdout.split("\n").filter(Boolean).filter((f) => f.endsWith(".cs")));
    });
  });
}

function formatResult(result) {
  if (result.issues.length === 0) {
    return `✅ 未发现问题（检查 ${result.fileCount} 个文件）。`;
  }
  const errs = result.issues.filter((i) => i.severity === "error");
  const warns = result.issues.filter((i) => i.severity === "warning");
  const infos = result.issues.filter((i) => i.severity === "info");
  const lines = [];
  lines.push(`📋 审查完成：${result.fileCount} 个文件，${result.filesWithIssues} 个文件有问题`);
  lines.push(`   error=${errs.length}  warning=${warns.length}  info=${infos.length}`);
  lines.push("");
  let current = "";
  for (const issue of result.issues) {
    const key = `${issue.severity === "error" ? "❌" : issue.severity === "warning" ? "⚠️" : "💡"} [${issue.rule}]`;
    if (issue.file !== current) {
      lines.push("");
      lines.push(`📄 ${issue.file}`);
      current = issue.file;
    }
    const loc = issue.line ? `:${issue.line}` : "";
    lines.push(`  ${key} ${issue.message}${issue.suggestion ? `\n      → ${issue.suggestion}` : ""}`);
  }
  return lines.join("\n");
}

// ── CLI ────────────────────────────────────────────────────
/** 预期内的用户输入错误：直接打印消息，不加"审查失败"前缀 */
class CliError extends Error {}

const USAGE = `GFramework 约定审查（静态规则引擎）

用法: node review.mjs [目标] [选项]

  目标        文件或目录路径（相对项目根）；省略时审查 git 未提交的 .cs 变更
  --all       审查整个 scripts/ 目录
  --staged    审查已暂存的变更（git diff --cached）
  --json      输出机器可读 JSON
  --report    同时把完整报告写入 .pi/review-report.txt
  --strict    存在 error 级问题时以退出码 1 结束（默认恒为 0）
  -h, --help  显示本帮助`;

function parseArgs(argv) {
  const opts = { target: null, all: false, staged: false, json: false, report: false, strict: false, help: false };
  for (const a of argv) {
    if (a === "--all") opts.all = true;
    else if (a === "--staged") opts.staged = true;
    else if (a === "--json") opts.json = true;
    else if (a === "--report") opts.report = true;
    else if (a === "--strict") opts.strict = true;
    else if (a === "-h" || a === "--help") opts.help = true;
    else if (a.startsWith("-")) {
      console.error(`未知选项: ${a}`);
      process.exit(2);
    } else opts.target = a;
  }
  return opts;
}

async function resolveFiles(cwd, opts) {
  const collectDir = async (dir) => {
    const files = [];
    await walkFiles(dir, 200, async (f) => {
      files.push(toPosix(relative(cwd, f)));
    });
    return files;
  };
  if (opts.all) {
    const files = await collectDir(join(cwd, "scripts"));
    if (files.length === 0) throw new CliError("未找到 scripts/ 目录或其中的 .cs 文件。");
    return files;
  }
  if (opts.staged) return getGitChangedFiles(cwd, true);
  if (opts.target) {
    const full = join(cwd, opts.target);
    let st;
    try {
      st = await stat(full);
    } catch {
      throw new CliError(`路径不存在: ${opts.target}`);
    }
    if (st.isDirectory()) return collectDir(full);
    return [toPosix(relative(cwd, full))];
  }
  return getGitChangedFiles(cwd, false);
}

async function main() {
  const opts = parseArgs(process.argv.slice(2));
  if (opts.help) {
    console.log(USAGE);
    return 0;
  }
  const cwd = process.cwd();

  // 软性提示：本规则集只适用于 GFramework 风格项目
  try {
    const entries = await readdir(cwd);
    if (!entries.includes("CONVENTIONS.md")) {
      console.error("提示: 当前目录没有 CONVENTIONS.md，可能不是 GFramework 项目——本规则集按该规范检查，结论仅供参考。");
    }
  } catch { /* ignore */ }

  const files = await resolveFiles(cwd, opts);
  if (files.length === 0) {
    if (opts.json) console.log(JSON.stringify({ fileCount: 0, filesWithIssues: 0, issues: [] }, null, 2));
    else console.log("没有检测到 git 未提交的 .cs 变更，无需审查。");
    return 0;
  }

  const result = await reviewPaths(cwd, files);
  const errs = result.issues.filter((i) => i.severity === "error").length;
  const warns = result.issues.filter((i) => i.severity === "warning").length;
  const infos = result.issues.filter((i) => i.severity === "info").length;

  if (opts.json) {
    console.log(JSON.stringify({ ...result, summary: { error: errs, warning: warns, info: infos } }, null, 2));
  } else {
    console.log(formatResult(result));
  }

  if (opts.report) {
    try {
      await mkdir(join(cwd, ".pi"), { recursive: true });
      await writeFile(join(cwd, ".pi", "review-report.txt"), formatResult(result), "utf-8");
      if (!opts.json) console.log("\n（完整报告已保存到 .pi/review-report.txt）");
    } catch { /* ignore */ }
  }

  return opts.strict && errs > 0 ? 1 : 0;
}

main()
  .then((code) => process.exit(code))
  .catch((err) => {
    if (err instanceof CliError) {
      console.error(err.message);
    } else {
      console.error(`审查失败: ${err instanceof Error ? err.message : String(err)}`);
    }
    process.exit(2);
  });
