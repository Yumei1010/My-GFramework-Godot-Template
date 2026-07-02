using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkTemplate.scripts.component.branch_option;
using GFrameworkTemplate.scripts.cqrs.visualnovel.@event;
using GFrameworkTemplate.scripts.system.visualnovel;
using Godot;

namespace GFrameworkTemplate.global;

/// <summary>
///     分支栏全局单例——根据选项数量动态创建按钮
/// </summary>
[Log]
[ContextAware]
public partial class BranchManager : CanvasLayer
{
    private VBoxContainer _container = null!;
    private StoryEngineSystem _engine = null!;
    private readonly List<(Button Button, string OptionId)> _active = new();

    public override void _Ready()
    {
        _engine = this.GetUtility<StoryEngineSystem>()!;

        _container = new VBoxContainer
        {
            Alignment = BoxContainer.AlignmentMode.End,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both
        };
        _container.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

        // 底部留出对话栏空间
        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 128);
        margin.AddThemeConstantOverride("margin_right", 128);
        margin.AddThemeConstantOverride("margin_bottom", 200);
        margin.AddThemeConstantOverride("margin_top", 360);
        margin.GrowHorizontal = Control.GrowDirection.Both;
        margin.GrowVertical = Control.GrowDirection.Both;
        margin.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        margin.AddChild(_container);
        AddChild(margin);

        Hide();
        this.RegisterEvent<VisualNovelBranchTriggeredEvent>(OnBranch).UnRegisterWhenNodeExitTree(this);
    }

    private void OnBranch(VisualNovelBranchTriggeredEvent e)
    {
        ClearButtons();

        foreach (var (optionId, option) in e.Options)
        {
            var btn = CreateBranchButton(option.Text);
            btn.Pressed += () => Choose(optionId);
            _container.AddChild(btn);
            _active.Add((btn, optionId));
        }

        Show();
    }

    private static Button CreateBranchButton(string text)
    {
        var btn = new Button
        {
            CustomMinimumSize = new Vector2(0, 56),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };

        var label = new RichTextLabel
        {
            BbcodeEnabled = true,
            Text = $"[center]{text}[/center]",
            ScrollActive = false,
            FitContent = true
        };
        label.AddThemeFontSizeOverride("normal_font_size", 22);
        label.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        label.MouseFilter = Control.MouseFilterEnum.Ignore;
        btn.AddChild(label);

        return btn;
    }

    private void Choose(string optionId)
    {
        ClearButtons();
        Hide();
        _engine.ChooseBranch(optionId);
    }

    private void ClearButtons()
    {
        foreach (var (btn, _) in _active)
        {
            btn.QueueFree();
        }
        _active.Clear();
    }
}
