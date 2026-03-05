using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.command.menu.select;
using GFrameworkGodotTemplate.scripts.component;
using GFrameworkGodotTemplate.scripts.core.ui;
using GFrameworkGodotTemplate.scripts.enums.ui;
using GFrameworkGodotTemplate.scripts.pile;
using Godot;

namespace GFrameworkGodotTemplate.scripts.select_menu;

[ContextAware]
[Log]
public partial class SelectMenu : Control, IController, IUiPageBehaviorProvider, ISimpleUiPage
{
    [Export] public ColorRect[] ThemeColorBlock = [];
    [Export] public Godot.Collections.Dictionary<string, PileResource> PileLib = new();
    
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
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    public static string UiKeyStr => nameof(UiKey.SelectMenu);
    
    private PileResource _currentDisplayPileResource = null!;
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
        
        _pileResourceKeys = PileLib.Keys.ToArray();
        if (_pileResourceKeys.Length > 0)
        {
            _currentPileResourceIndex = _pileResourceKeys.IndexOf("Heart");
            _currentDisplayPileResource = PileLib["Heart"];
            UpdatePileDisplay();
        }
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _uiRouter = this.GetSystem<IUiRouter>()!;
        _stateMachineSystem = this.GetSystem<IStateMachineSystem>()!;
    }

    private void ConnectSignal()
    {
        PrevButton.MouseEntered += OnMouseEnteredPrevButton;
        PrevButton.MouseExited += OnMouseExitedPrevButton;
        PrevButton.ButtonDown += OnMouseDownPrevButton;
        
        NextButton.MouseEntered += OnMouseEnteredNextButton;
        NextButton.MouseExited += OnMouseExitedNextButton;
        NextButton.ButtonDown += OnMouseDownNextButton;
        
        CheckButton.ButtonDown += () => this.SendCommand(new CheckPileCommand());
        ReturnButton.ButtonDown += () => this.SendCommand(new ReturnMainMenuCommand());
    }
    
    private void RegisterEvent()
    {
        this.RegisterEvent<PileView.MouseWheelUp>(e =>
        {
            IncreaseCurrentPileResourceIndex();
            UpdatePileDisplay();
        });
        
        this.RegisterEvent<PileView.MouseWheelDown>(e =>
        {
            DecreaseCurrentPileResourceIndex();
            UpdatePileDisplay();
        });
    }
    
    private void UpdatePileDisplay()
    {
        foreach (ColorRect rect in ThemeColorBlock)
        {
            rect.Color = _currentDisplayPileResource.ThemeColor;
        }
        PileNameLabel.Text = _themePrefix  + _currentDisplayPileResource.PileName;
        IntroductionLabel.Modulate = _currentDisplayPileResource.ThemeColor;
        IntroductionLabel.Text = _themePrefix + "特性//" + _currentDisplayPileResource.AbilityName + "\n" + _currentDisplayPileResource.Introduction;
        
        PileView.Update(null,null,_currentDisplayPileResource.PileTexture);
        
        FlowRateContainer.Transform(
            _currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.FlowRateIcon,
            "流逝",
            _currentDisplayPileResource.FlowRateLevel
            );
        
        ResilienceContainer.Transform(_currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.ResilienceIcon,
            "韧性",
            _currentDisplayPileResource.ResilienceLevel
            );
        
        ConversionContainer.Transform(
            _currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.ConversionIcon,
            "转化",
            _currentDisplayPileResource.ConversionLevel
            );
        
        DrainContainer.Transform(
            _currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.DrainIcon,
            "汲取",
            _currentDisplayPileResource.DrainLevel
            );
        
        RegenerationContainer.Transform(
            _currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.RegenerationIcon,
            "再生",
            _currentDisplayPileResource.RegenerationLevel
            );
        
        RewindContainer.Transform(
            _currentDisplayPileResource.ThemeColor,
            _currentDisplayPileResource.RewindIcon,
            "回溯",
            _currentDisplayPileResource.RewindLevel
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
        _currentDisplayPileResource = PileLib[_pileResourceKeys[_currentPileResourceIndex]];
    }
    
    private void IncreaseCurrentPileResourceIndex()
    {
        if (_pileResourceKeys.Length == 0) return;
        _currentPileResourceIndex = (_currentPileResourceIndex + 1) % _pileResourceKeys.Length;
        _currentDisplayPileResource = PileLib[_pileResourceKeys[_currentPileResourceIndex]];
    }
}