using GFramework.Core.Abstractions.controller;
using GFramework.Core.Abstractions.state;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFramework.Godot.ui;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.component;
using GFrameworkGodotTemplate.scripts.core.state.impls;
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
    private TextureButton Pile => GetNode<TextureButton>("%Pile");
    
    private IUiPageBehavior? _page;
    private IStateMachineSystem _stateMachineSystem = null!;
    private IUiRouter _uiRouter = null!;
    public static string UiKeyStr => nameof(UiKey.SelectMenu);


    private PileResource _currentDisplayPileResource = null!;
    private string[] _pileKeys = System.Array.Empty<string>();
    private int _currentIndex = 0;
    private bool _pileIsMoving;
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
        
        _pileKeys = PileLib.Keys.ToArray();
        if (_pileKeys.Length > 0)
        {
            _currentIndex = 0;
            _currentDisplayPileResource = PileLib[_pileKeys[_currentIndex]];
            UpdatePileDisplay();
        }
    }

    public override void _Process(double delta)
    {
        if (_pileIsMoving)
        {
            Pile.GlobalPosition = GetGlobalMousePosition() - Pile.Size / 2;
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
        
        Pile.MouseEntered += OnMouseEnteredPile;
        Pile.MouseExited += OnMouseExitedPile;
        Pile.ButtonDown += OnButtonDownPile;
        Pile.ButtonUp += OnButtonUpPile;
        
        CheckButton.ButtonDown += () => _stateMachineSystem.ChangeTo<ClockMenuState>();
        ReturnButton.ButtonDown += () => _stateMachineSystem.ChangeTo<MainMenuState>();
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
        Pile.TextureNormal = _currentDisplayPileResource.PileTexture;
        FlowRateContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.FlowRateIcon,"流逝",_currentDisplayPileResource.FlowRateLevel);
        ResilienceContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.ResilienceIcon,"韧性",_currentDisplayPileResource.ResilienceLevel);
        ConversionContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.ConversionIcon,"转化",_currentDisplayPileResource.ConversionLevel);
        DrainContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.DrainIcon,"汲取",_currentDisplayPileResource.DrainLevel);
        RegenerationContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.RegenerationIcon,"再生",_currentDisplayPileResource.RegenerationLevel);
        RewindContainer.Transform(_currentDisplayPileResource.ThemeColor,_currentDisplayPileResource.RewindIcon,"回溯",_currentDisplayPileResource.RewindLevel);
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
        if (_pileKeys.Length == 0) return;
        _currentIndex = (_currentIndex - 1 + _pileKeys.Length) % _pileKeys.Length;
        _currentDisplayPileResource = PileLib[_pileKeys[_currentIndex]];
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
        if (_pileKeys.Length == 0) return;
        _currentIndex = (_currentIndex + 1) % _pileKeys.Length;
        _currentDisplayPileResource = PileLib[_pileKeys[_currentIndex]];
        UpdatePileDisplay();
    }
    
    private void OnMouseEnteredPile()
    {
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(Pile, "scale", new Vector2(1.2f, 1.2f), 0.25f);
        _tween.TweenProperty(Pile, "scale", new Vector2(1.0f,1.0f), 0.25f);
    }

    private void OnMouseExitedPile()
    {
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(Pile, "rotation", 0.1f, 0.125f);
        _tween.TweenProperty(Pile, "rotation", 0.0f, 0.125f);
    }

    private void OnButtonDownPile()
    {
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        _pileIsMoving = true;
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(Pile, "scale", new Vector2(1.2f, 1.2f), 0.15f);
        _tween.TweenProperty(Pile, "scale", new Vector2(1.0f,1.0f), 0.35f);
    }

    private void OnButtonUpPile()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        _pileIsMoving = false;
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(Pile, "global_position", new Vector2(240,184), 0.25f);
    }
}