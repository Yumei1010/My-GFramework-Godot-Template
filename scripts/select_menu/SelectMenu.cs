using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.extensions;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.command.menu;
using GFrameworkGodotTemplate.scripts.component;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using Godot;
using PileAttributeConfig = GFrameworkGodotTemplate.scripts.core.resource.PileAttributeConfig;

namespace GFrameworkGodotTemplate.scripts.select_menu;

[ContextAware]
[Log]
public partial class SelectMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    [Export] public ColorRect[] ThemeColorBlock = [];
    [Export] public Godot.Collections.Dictionary<string, PileAttributeConfig> PileLib = new();
    
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    private TextureButton PrevButton => GetNode<TextureButton>("%PrevButton");
    private RichTextLabel PileNameLabel => GetNode<RichTextLabel>("%PileNameLabel");
    private RichTextLabel IntroductionLabel => GetNode<RichTextLabel>("%IntroductionLabel");
    private TextureButton NextButton => GetNode<TextureButton>("%NextButton");
    private PileAttributeContainer FlowRateContainer => GetNode<PileAttributeContainer>("%FlowRateContainer");
    private PileAttributeContainer ResilienceContainer => GetNode<PileAttributeContainer>("%ResilienceContainer");
    private PileAttributeContainer ConversionContainer  => GetNode<PileAttributeContainer>("%ConversionContainer");
    private PileAttributeContainer DrainContainer => GetNode<PileAttributeContainer>("%DrainContainer");
    private PileAttributeContainer RegenerationContainer => GetNode<PileAttributeContainer>("%RegenerationContainer");
    private PileAttributeContainer RewindContainer => GetNode<PileAttributeContainer>("%RewindContainer");
    private Button CheckButton => GetNode<Button>("%CheckButton");
    private Button ReturnButton => GetNode<Button>("%ReturnButton");
    private PileView PileView => GetNode<PileView>("%PileView");
    
    private IUiPageBehavior? _page;
    public static string UiKeyStr => nameof(UiKey.SelectMenu);
    
    private PileAttributeConfig _currentDisplayPileAttributeConfig = null!;
    private string[] _pileResourceKeys = System.Array.Empty<string>();
    private int _currentPileResourceIndex = 0;
    
    private Tween _tween = null!;
    private string _themePrefix = "[wave amp=5 freq=5.0]";
    
    public IUiPageBehavior GetPage()
    {
        _page ??= UiPageBehaviorFactory.Create<Control>(this, UiKeyStr, UiLayer.Page);
        return _page;
    }
    
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
                
        _pileResourceKeys = PileLib.Keys.ToArray();
        if (_pileResourceKeys.Length > 0)
        {
            _currentPileResourceIndex = _pileResourceKeys.IndexOf("Heart");
            _currentDisplayPileAttributeConfig = PileLib["Heart"];
            UpdatePileDisplay();
        }
    }

    private void ConnectSignal()
    {
        PrevButton.MouseEntered += OnMouseEnteredPrevButton;
        PrevButton.MouseExited += OnMouseExitedPrevButton;
        PrevButton.ButtonDown += OnMouseDownPrevButton;
        
        NextButton.MouseEntered += OnMouseEnteredNextButton;
        NextButton.MouseExited += OnMouseExitedNextButton;
        NextButton.ButtonDown += OnMouseDownNextButton;

        CheckButton.ButtonDown += () => this.SendCommand(new OpenMapMenuCommand());
        ReturnButton.ButtonDown += () => this.SendCommand(new OpenMainMenuCommand());
    }
    
    private void RegisterEvent()
    {
        this.RegisterEvent<PileView.MouseWheelUp>(e =>
        {
            IncreaseCurrentPileResourceIndex();
            UpdatePileDisplay();
        }).UnRegisterWhenNodeExitTree(this);
        
        this.RegisterEvent<PileView.MouseWheelDown>(e =>
        {
            DecreaseCurrentPileResourceIndex();
            UpdatePileDisplay();
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void UpdatePileDisplay()
    {
        foreach (ColorRect rect in ThemeColorBlock)
        {
            rect.Color = _currentDisplayPileAttributeConfig.ThemeColor;
        }
        PileNameLabel.Text = _themePrefix  + _currentDisplayPileAttributeConfig.PileName;
        IntroductionLabel.Modulate = _currentDisplayPileAttributeConfig.ThemeColor;
        IntroductionLabel.Text = _themePrefix + "特性//" + _currentDisplayPileAttributeConfig.AbilityName + "\n" + _currentDisplayPileAttributeConfig.Introduction;
        
        PileView.Update(null,null,_currentDisplayPileAttributeConfig.PileTexture);
        
        FlowRateContainer.Transform(
            _currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.FlowRateIcon,
            "流逝",
            _currentDisplayPileAttributeConfig.FlowRateLevel
            );
        
        ResilienceContainer.Transform(_currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.ResilienceIcon,
            "韧性",
            _currentDisplayPileAttributeConfig.ResilienceLevel
            );
        
        ConversionContainer.Transform(
            _currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.ConversionIcon,
            "转化",
            _currentDisplayPileAttributeConfig.ConversionLevel
            );
        
        DrainContainer.Transform(
            _currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.DrainIcon,
            "汲取",
            _currentDisplayPileAttributeConfig.DrainLevel
            );
        
        RegenerationContainer.Transform(
            _currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.RegenerationIcon,
            "再生",
            _currentDisplayPileAttributeConfig.RegenerationLevel
            );
        
        RewindContainer.Transform(
            _currentDisplayPileAttributeConfig.ThemeColor,
            _currentDisplayPileAttributeConfig.RewindIcon,
            "回溯",
            _currentDisplayPileAttributeConfig.RewindLevel
            );
    }

    private void OnMouseEnteredPrevButton()
    {
        PrevButton.Modulate = new Color("#ffffff");
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(PrevButton, "scale", new Vector2(1.2f, 1.2f), 0.25f);
        _tween.TweenProperty(PrevButton, "scale", new Vector2(1.0f,1.0f), 0.25f);
    }

    private void OnMouseExitedPrevButton()
    {
        PrevButton.Modulate = new Color("#2b2b2b");
    }

    private void OnMouseDownPrevButton()
    {
        DecreaseCurrentPileResourceIndex();
        UpdatePileDisplay();
    }
    
    private void OnMouseEnteredNextButton()
    {
        NextButton.Modulate = new Color("#ffffff");
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(NextButton, "scale", new Vector2(1.2f, 1.2f), 0.25f);
        _tween.TweenProperty(NextButton, "scale", new Vector2(1.0f,1.0f), 0.25f);
    }

    private void OnMouseExitedNextButton()
    {
        NextButton.Modulate = new Color("#2b2b2b");
    }
    
    private void OnMouseDownNextButton()
    {
        IncreaseCurrentPileResourceIndex();
        UpdatePileDisplay();
    }

    private void DecreaseCurrentPileResourceIndex()
    {
        if (_pileResourceKeys.Length == 0) return;
        _currentPileResourceIndex = (_currentPileResourceIndex - 1 + _pileResourceKeys.Length) % _pileResourceKeys.Length;
        _currentDisplayPileAttributeConfig = PileLib[_pileResourceKeys[_currentPileResourceIndex]];
    }
    
    private void IncreaseCurrentPileResourceIndex()
    {
        if (_pileResourceKeys.Length == 0) return;
        _currentPileResourceIndex = (_currentPileResourceIndex + 1) % _pileResourceKeys.Length;
        _currentDisplayPileAttributeConfig = PileLib[_pileResourceKeys[_currentPileResourceIndex]];
    }
}