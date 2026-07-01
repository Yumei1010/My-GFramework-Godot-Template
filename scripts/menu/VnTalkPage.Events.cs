using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFrameworkTemplate.scripts.cqrs.vn.@event;
using Godot;

namespace GFrameworkTemplate.scripts.menu;

public partial class VnTalkPage
{
    /// <summary>
    ///     注册 CQRS 事件订阅——监听故事引擎的事件并更新 UI
    /// </summary>
    private void RegisterEvents()
    {
        this.RegisterEvent<VnTalkTriggeredEvent>(e =>
        {
            TalkerName.Text = e.Talker ?? "";
            TalkerName.Visible = !e.IsCenter;
            Avatar.Visible = !e.IsCenter;
            CenterContent.Visible = e.IsCenter;

            if (e.IsCenter)
                PlayTypewriter(CenterContent, e.Content);
            else
                PlayTypewriter(TalkContent, e.Content);
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<VnBackgroundTriggeredEvent>(e =>
        {
            _log.Debug($"背景切换: {e.FilePath}");
            // 使用者在此实现背景 TextureRect 交叉淡入淡出
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<VnBranchTriggeredEvent>(e =>
        {
            _log.Debug($"分支选项: {e.Options.Count} 个选项");
            // 使用者在此显示分支选项按钮
        }).UnRegisterWhenNodeExitTree(this);

        this.RegisterEvent<VnAdvanceRequestedEvent>(_ =>
        {
            if (_typewriterTween?.IsRunning() == true)
                SkipTypewriter();
        }).UnRegisterWhenNodeExitTree(this);
    }

    private void PlayTypewriter(Label label, string content)
    {
        _typewriterTween?.Kill();
        label.Text = content;
        label.VisibleRatio = 0f;

        _typewriterTween = CreateTween();
        _typewriterTween.TweenProperty(label, "visible_ratio", 1.0f, content.Length * 0.02f);
    }

    private void SkipTypewriter()
    {
        _typewriterTween?.Kill();
        _typewriterTween = null;
        TalkContent.VisibleRatio = 1f;
        CenterContent.VisibleRatio = 1f;
    }
}
