using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.core.story;
using GFrameworkTemplate.scripts.cqrs.vn.@event;
using GFrameworkTemplate.scripts.data.story;

namespace GFrameworkTemplate.scripts.system.vn;

/// <summary>
///     故事引擎系统——视觉小说 JSON 脚本解释器的核心
///     负责递归执行 StoryCommand 数组、分支过滤、状态管理
///
///     使用方式：
///     1. 通过 DI 注册为单例工具
///     2. 调用 LoadAndPlay(jsonLogicName) 开始播放
///     3. 通过 CQRS 事件接收命令执行通知并驱动 UI
/// </summary>
[Log]
[ContextAware]
public sealed partial class StoryEngineSystem
{
    /// <summary>当前已加载的命令列表</summary>
    private List<StoryCommand> _commands = new();

    /// <summary>当前命令索引</summary>
    private int _currentIndex;

    /// <summary>是否正在播放</summary>
    private bool _isPlaying;

    /// <summary>玩家已选择的分支 ID 列表</summary>
    private readonly List<string> _talkBranch = new();

    /// <summary>已被禁用的分支 ID 列表</summary>
    private readonly List<string> _canNotChoose = new();

    /// <summary>当前播放的脚本文件路径</summary>
    private string _playingJson = string.Empty;

    /// <summary>自动播放的等待时间（秒），null 表示关闭自动播放</summary>
    private float? _autoPlayDelay;

    /// <summary>字体速度（打字机每个字符的间隔秒数）</summary>
    private float _wordSpeed = 0.02f;

    /// <summary>当前命令执行完成的信号源</summary>
    private TaskCompletionSource<bool>? _waitSource;

    /// <summary>
    ///     加载并开始播放脚本
    /// </summary>
    /// <param name="logicName">脚本的逻辑名（映射到 JSON 文件路径）</param>
    public async Task LoadAndPlay(string logicName)
    {
        var jsonPath = StoryResourceMapper.ResolveJsonPath(logicName);
        if (string.IsNullOrEmpty(jsonPath))
        {
            _log.Error($"找不到脚本: {logicName}");
            return;
        }

        var json = await StoryResourceMapper.LoadJsonAsync(jsonPath);
        if (string.IsNullOrEmpty(json))
        {
            _log.Error($"加载脚本失败: {jsonPath}");
            return;
        }

        var script = StoryParser.ParseStory(json);
        _commands = script.Content;
        _currentIndex = 0;
        _isPlaying = true;
        _playingJson = jsonPath;
        _talkBranch.Clear();
        _canNotChoose.Clear();

        this.SendEvent(new VnStoryLoadedEvent { CommandCount = _commands.Count });
        _log.Debug($"故事加载完成: {jsonPath} ({_commands.Count} 条命令)");

        await PlayLoop();
    }

    /// <summary>
    ///     递归播放循环
    /// </summary>
    private async Task PlayLoop()
    {
        while (_isPlaying && _currentIndex < _commands.Count)
        {
            var cmd = _commands[_currentIndex];
            _currentIndex++;

            // 分支过滤
            if (!ShouldExecute(cmd))
                continue;

            // 隐藏 UI 文本
            if (cmd.HideLabels)
                this.SendEvent<VnAdvanceRequestedEvent>();

            // 分发执行
            await DispatchCommand(cmd);

            // 后置等待
            if (cmd.Wait.HasValue)
                await Task.Delay(TimeSpan.FromSeconds(cmd.Wait.Value));
        }

        if (_currentIndex >= _commands.Count)
        {
            _isPlaying = false;
            this.SendEvent<VnStoryFinishedEvent>();
            _log.Debug("故事播放结束");
        }
    }

    /// <summary>
    ///     判断命令是否应该在当前分支路径上执行
    /// </summary>
    private bool ShouldExecute(StoryCommand cmd)
    {
        if (string.IsNullOrEmpty(cmd.Branch))
            return true; // 公共命令

        return _talkBranch.Contains(cmd.Branch) && !_canNotChoose.Contains(cmd.Branch);
    }

    /// <summary>
    ///     分发命令到对应的处理逻辑
    /// </summary>
    private async Task DispatchCommand(StoryCommand cmd)
    {
        switch (cmd)
        {
            case TalkCommand talk:
                await ExecuteTalk(talk);
                break;

            case BackgroundCommand bg:
                await ExecuteBackground(bg);
                break;

            case TachieCommand tachie:
                ExecuteTachie(tachie);
                break;

            case SoundCommand sound:
                ExecuteSound(sound);
                break;

            case BranchCommand branch:
                await ExecuteBranch(branch);
                break;

            case GotoCommand gt:
                await ExecuteGoto(gt);
                break;

            case EventCommand evt:
                await ExecuteEvent(evt);
                break;
        }
    }

    /// <summary>
    ///     执行对话命令——发送事件并等待玩家点击
    /// </summary>
    private async Task ExecuteTalk(TalkCommand cmd)
    {
        this.SendEvent(new VnTalkTriggeredEvent
        {
            Talker = cmd.Talker,
            Content = cmd.TalkContent,
            IsCenter = cmd.IsCenter,
            AvatarPath = cmd.AvatarPath
        });

        // 模拟打字机效果和等待
        var charCount = cmd.TalkContent.Length;
        var revealTime = charCount * _wordSpeed;
        this.SendEvent(new VnTextRevealProgressEvent { RevealedChars = 0, TotalChars = charCount });

        await WaitForAdvance(revealTime);
    }

    /// <summary>
    ///     执行背景命令——发送事件并等待 Tween
    /// </summary>
    private async Task ExecuteBackground(BackgroundCommand cmd)
    {
        if (cmd.Delay > 0)
            await Task.Delay(TimeSpan.FromSeconds(cmd.Delay));

        this.SendEvent(new VnBackgroundTriggeredEvent
        {
            FilePath = cmd.FilePath ?? string.Empty,
            WaitTweenEnd = cmd.WaitTweenEnd,
            Delay = cmd.Delay
        });

        if (cmd.WaitTweenEnd)
            await Task.Delay(TimeSpan.FromSeconds(0.5f)); // Tween 默认时长
    }

    /// <summary>
    ///     执行立绘命令——即发即忘
    /// </summary>
    private void ExecuteTachie(TachieCommand cmd)
    {
        this.SendEvent(new VnTachieTriggeredEvent { Tachies = cmd.Tachies });
    }

    /// <summary>
    ///     执行音频命令——即发即忘
    /// </summary>
    private void ExecuteSound(SoundCommand cmd)
    {
        this.SendEvent(new VnSoundTriggeredEvent
        {
            SoundType = cmd.SoundType,
            FilePath = cmd.FilePath ?? string.Empty
        });
    }

    /// <summary>
    ///     执行分支命令——发送选项并等待玩家选择
    /// </summary>
    private async Task ExecuteBranch(BranchCommand cmd)
    {
        this.SendEvent(new VnBranchTriggeredEvent { Options = cmd.Options });

        // 注册分支选择的事件处理
        var chosenId = await WaitForBranchChoice();
        if (chosenId != null)
            _talkBranch.Add(chosenId);
    }

    /// <summary>
    ///     执行跳转命令——终止当前链、加载新脚本
    /// </summary>
    private async Task ExecuteGoto(GotoCommand cmd)
    {
        var target = cmd.FilePath;
        if (string.IsNullOrEmpty(target))
        {
            _log.Warn("Goto 命令缺少 file_path");
            return;
        }

        this.SendEvent(new VnGotoTriggeredEvent { TargetFilePath = target });
        _isPlaying = false; // 终止当前循环

        await LoadAndPlay(target);
    }

    /// <summary>
    ///     执行自定义事件——发送事件并等待回调
    /// </summary>
    private async Task ExecuteEvent(EventCommand cmd)
    {
        this.SendEvent(new VnCustomEventTriggeredEvent { EventName = cmd.EventName });
        await WaitForAdvance(0.1f);
    }

    /// <summary>
    ///     等待玩家点击推进或自动播放计时器
    /// </summary>
    private async Task WaitForAdvance(float minDuration)
    {
        if (_autoPlayDelay.HasValue)
        {
            var waitTime = Math.Max(minDuration, _autoPlayDelay.Value);
            await Task.Delay(TimeSpan.FromSeconds(waitTime));
        }
        else
        {
            // 等待外部信号（VnAdvanceRequestedEvent）
            _waitSource = new TaskCompletionSource<bool>();
            await _waitSource.Task;
            _waitSource = null;
        }
    }

    /// <summary>
    ///     等待玩家选择分支
    /// </summary>
    private async Task<string?> WaitForBranchChoice()
    {
        var tcs = new TaskCompletionSource<string?>();
        var subscription = this.RegisterEvent<VnBranchChosenEvent>(e =>
        {
            tcs.TrySetResult(e.OptionId);
        });

        var result = await tcs.Task;
        subscription.UnRegister();
        return result;
    }

    /// <summary>
    ///     外部触发推进（玩家点击）
    /// </summary>
    public void Advance()
    {
        _waitSource?.TrySetResult(true);
        this.SendEvent<VnAdvanceRequestedEvent>();
    }

    /// <summary>
    ///     选择分支选项
    /// </summary>
    public void ChooseBranch(string optionId)
    {
        this.SendEvent(new VnBranchChosenEvent { OptionId = optionId });
    }

    #region 状态访问器

    public bool IsPlaying => _isPlaying;
    public string CurrentJsonPath => _playingJson;
    public int CurrentIndex => _currentIndex;
    public IReadOnlyList<string> TalkBranch => _talkBranch;
    public IReadOnlyList<string> CanNotChoose => _canNotChoose;

    public void SetAutoPlay(float? delaySeconds) => _autoPlayDelay = delaySeconds;
    public void SetWordSpeed(float speed) => _wordSpeed = speed;
    public void AddCannotChoose(string branchId) => _canNotChoose.Add(branchId);
    public void RemoveCannotChoose(string branchId) => _canNotChoose.Remove(branchId);

    /// <summary>
    ///     强制停止当前播放
    /// </summary>
    public void Stop()
    {
        _isPlaying = false;
        _waitSource?.TrySetResult(false);
    }

    #endregion
}
