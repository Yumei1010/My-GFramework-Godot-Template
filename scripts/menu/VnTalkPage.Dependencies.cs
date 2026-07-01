using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.cqrs.vn.@event;
using GFrameworkTemplate.global;
using Godot;

namespace GFrameworkTemplate.scripts.menu;

public partial class VnTalkPage
{
    /// <summary>说话人名称标签</summary>
    private Label TalkerName => GetNode<Label>("%TalkerName");

    /// <summary>对话内容标签</summary>
    private Label TalkContent => GetNode<Label>("%TalkContent");

    /// <summary>居中旁白标签</summary>
    private Label CenterContent => GetNode<Label>("%CenterContent");

    /// <summary>头像纹理</summary>
    private TextureRect Avatar => GetNode<TextureRect>("%Avatar");

    /// <summary>点击推进的透明区域</summary>
    private Control ClickArea => GetNode<Control>("%ClickArea");

    /// <summary>打字机 Tween</summary>
    private Tween? _typewriterTween;

    /// <summary>
    ///     异步等待架构就绪，获取依赖
    /// </summary>
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _log.Debug("VnTalkPage 初始化完成");
    }
}
