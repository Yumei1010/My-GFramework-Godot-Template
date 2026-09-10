<#
.SYNOPSIS
    以本模板开新项目时，一键重命名命名空间与项目名。

.DESCRIPTION
    执行内容：
      1. 替换全部源码/文档中的根命名空间  GFrameworkTemplate        → <Namespace>
      2. 替换项目名                        My-GFramework-Godot-Template → <ProjectName>
      3. 重命名 .sln / .csproj / 测试项目目录
      4. 执行 dotnet build 验证（可用 -SkipBuild 跳过）

    脚本自身不会被替换；.git / .godot / obj / bin / assets 等目录会被跳过。

.PARAMETER Namespace
    新的 C# 根命名空间（如 MyGame 或 MyCompany.MyGame）。

.PARAMETER ProjectName
    新的项目名（用于 sln / csproj / 测试目录名）。默认与 -Namespace 相同。

.PARAMETER SkipBuild
    跳过最后的 dotnet build 验证。

.EXAMPLE
    pwsh -File tools/rename-project.ps1 -Namespace MyGame

.EXAMPLE
    pwsh -File tools/rename-project.ps1 -Namespace MyCompany.MyGame -ProjectName MyGame

.EXAMPLE
    # Windows PowerShell 5.1
    powershell -NoProfile -ExecutionPolicy Bypass -File tools\rename-project.ps1 -Namespace MyGame
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)*$')]
    [string]$Namespace,

    [ValidatePattern('^[A-Za-z0-9_.\-]+$')]
    [string]$ProjectName = '',

    [switch]$SkipBuild
)

$ErrorActionPreference = 'Stop'

$OldNamespace = 'GFrameworkTemplate'
$OldProjectName = 'My-GFramework-Godot-Template'

if ([string]::IsNullOrWhiteSpace($ProjectName)) {
    # 命名空间含点号时，项目名取最后一段（如 MyCompany.MyGame → MyGame）
    $ProjectName = ($Namespace -split '\.')[-1]
}

if ($Namespace -eq $OldNamespace) {
    throw "新命名空间 '$Namespace' 与当前命名空间相同，无需重命名。"
}
if ($ProjectName -eq $OldProjectName) {
    throw "新项目名 '$ProjectName' 与当前项目名相同，无需重命名。"
}

$Root = Split-Path -Parent $PSScriptRoot
if (-not (Test-Path (Join-Path $Root 'project.godot'))) {
    throw "未找到 project.godot，请在模板仓库根目录下运行本脚本。"
}
Set-Location $Root

$SkipDirs = @('.git', '.godot', 'obj', 'bin', '.vs', '.idea', 'node_modules', 'assets', 'addons')
$TextExts = @('.cs', '.csproj', '.sln', '.md', '.tscn', '.godot', '.json', '.yaml', '.yml',
              '.cfg', '.props', '.targets', '.editorconfig', '.ps1', '.sh', '.txt')
$SpecialNames = @('.gitignore', '.gitattributes', 'LICENSE')
$SelfPath = $MyInvocation.MyCommand.Path

Write-Host ""
Write-Host "重命名模板：" -ForegroundColor Cyan
Write-Host "  命名空间: $OldNamespace → $Namespace"
Write-Host "  项目名:   $OldProjectName → $ProjectName"
Write-Host ""

# ── 1) 文本内容替换 ─────────────────────────────────────────────
$changed = 0
Get-ChildItem -Path $Root -Recurse -File -Force | ForEach-Object {
    $file = $_
    if ($file.FullName -eq $SelfPath) { return }   # 跳过脚本自身

    $rel = $file.FullName.Substring($Root.Length).TrimStart('\', '/')
    $segments = $rel -split '[\\/]'
    if ($segments | Where-Object { $SkipDirs -contains $_ }) { return }

    $ext = $file.Extension.ToLowerInvariant()
    if (-not ($TextExts -contains $ext -or $SpecialNames -contains $file.Name)) { return }

    $bytes = [System.IO.File]::ReadAllBytes($file.FullName)
    if ($bytes.Length -eq 0) { return }

    $text = [System.Text.Encoding]::UTF8.GetString($bytes)
    if (-not ($text.Contains($OldNamespace) -or $text.Contains($OldProjectName))) { return }

    $hasBom = $text.Length -gt 0 -and $text[0] -eq [char]0xFEFF
    if ($hasBom) { $text = $text.Substring(1) }   # 去掉 BOM 字符，改由编码补回

    $updated = $text.Replace($OldNamespace, $Namespace).Replace($OldProjectName, $ProjectName)

    $encoding = New-Object System.Text.UTF8Encoding($hasBom)
    [System.IO.File]::WriteAllText($file.FullName, $updated, $encoding)
    $changed++
}
Write-Host "内容替换：$changed 个文件" -ForegroundColor Green

# ── 2) 重命名 sln / csproj / 测试项目目录 ────────────────────────
$sln = Join-Path $Root "$OldProjectName.sln"
if (Test-Path $sln) {
    Rename-Item -Path $sln -NewName "$ProjectName.sln"
    Write-Host "重命名：$OldProjectName.sln → $ProjectName.sln" -ForegroundColor Green
}

$csproj = Join-Path $Root "$OldProjectName.csproj"
if (Test-Path $csproj) {
    Rename-Item -Path $csproj -NewName "$ProjectName.csproj"
    Write-Host "重命名：$OldProjectName.csproj → $ProjectName.csproj" -ForegroundColor Green
}

# 测试项目：先改 csproj 文件名，再改目录名
$testCsprojOld = Join-Path $Root "tests/$OldProjectName.Tests/$OldProjectName.Tests.csproj"
if (Test-Path $testCsprojOld) {
    Rename-Item -Path $testCsprojOld -NewName "$ProjectName.Tests.csproj"
    Write-Host "重命名：$OldProjectName.Tests.csproj → $ProjectName.Tests.csproj" -ForegroundColor Green
}

$testDir = Join-Path $Root "tests/$OldProjectName.Tests"
if (Test-Path $testDir) {
    Rename-Item -Path $testDir -NewName "$ProjectName.Tests"
    Write-Host "重命名：tests/$OldProjectName.Tests → tests/$ProjectName.Tests" -ForegroundColor Green
}

# ── 3) 构建验证 ────────────────────────────────────────────────
if (-not $SkipBuild) {
    Write-Host ""
    Write-Host "构建验证中（dotnet build）..." -ForegroundColor Cyan
    dotnet build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "构建失败，请检查替换结果。" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host "构建通过。" -ForegroundColor Green
    Write-Host ""
    Write-Host "建议随后执行：dotnet test 与 godot --headless --path . --quit-after 300" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "完成。若使用 Git，可用 'git status' 查看改动范围。" -ForegroundColor Cyan
