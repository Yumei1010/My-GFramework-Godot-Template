using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.enums.resources;
using GFrameworkGodotTemplate.scripts.events.poker;
using GFrameworkGodotTemplate.scripts.utility;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.events.pokerSelector;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
public partial class Poker : Button, IPoker, IController
{
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("%AnimationPlayer");
    private IPokerStateMachine StateMachine => GetNode<PokerStateMachine>("%StateMachine");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");

    private Guid Id { get; set; } = Guid.Empty;
    private SuitType SuitType { get; set; } = SuitType.Heart;
    private string NumValue { get; set; } = null!;
    private NumType NumType { get; set; } = NumType.Integer;
    
    private Vector2 DefaultPosition { get; set; }
    private float DefaultRotation { get; set; }
    
    private IGodotTextureRegistry _textureRegistry = null!;
    private Tween _tween = null!;

    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }
    
    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        
        StateMachine.Init(this);
        StateMachine.ChangeTo(StateType.Idle);
    }
    
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void RegisterEvent()
    {
        // 注册对花色变更事件的监听
        this.RegisterEvent<SuitTypeChangedEvent>(e =>
        {
            OnSuitTypeChangedEvent(e.SuitType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值变更事件的监听
        this.RegisterEvent<NumValueChangedEvent>(e =>
        {
            OnNumValueChangedEvent(e.NumValue,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值类型变更事件的监听
        this.RegisterEvent<NumTypeChangedEvent>(e =>
        {
            OnNumTypeChangedEvent(e.NumType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对状态变更事件的监听
        this.RegisterEvent<StateChangedEvent>(e =>
        {
            OnStateChangedEvent(e.NextState,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择器可用性变更事件的监听
        this.RegisterEvent<EnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    public Guid GetId()
    {
        return Id;
    }

    public SuitType GetSuitType()
    {
        return SuitType;
    }

    public string GetNumValue()
    {
        return NumValue;
    }

    public NumType GetNumType()
    {
        return NumType;
    }

    public Vector2 GetSpawnPosition()
    {
        return DefaultPosition;
    }

    public void SetSuitType(SuitType suitType)
    {
        SuitType = suitType;
    }

    public void SetNumValue(string numValue)
    {
        NumValue = numValue;
    }

    public void SetNumType(NumType numType)
    {
        NumType = numType;
    }

    public void SetGlobalPosition(Vector2 pos)
    {
        GlobalPosition = pos;
    }

    public void SetRot(float angle)
    {
        Rotation = angle;
    }

    public void SetDefaultRotation(float angle)
    {
        DefaultRotation = angle;
    }
    
    public void SetDefaultPosition(Vector2 pos)
    {
        DefaultPosition = pos;
    }
    
    public void ResetPos()
    {
        // 如果正在播放动画，使其终止
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty( this, "global_position", DefaultPosition, 0.25f);
    }
    
    public void ResetRot()
    {
        // 如果正在播放动画，使其终止
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "rotation", Mathf.DegToRad(DefaultRotation), 0.25f);
    }

    public void ResetPosAndRot()
    {
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetParallel();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty( this, "global_position", DefaultPosition, 0.25f);
        _tween.TweenProperty(this, "rotation", Mathf.DegToRad(DefaultRotation), 0.25f);
    }

    public void MoveTo(Vector2 pos)
    {
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetParallel();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty( this, "global_position", pos, 0.25f);
    }

    public void ChangeTo(StateType state)
    {
        StateMachine.ChangeTo(state);
    }

    private void OnButtonDown()
    {
        StateMachine.MouseDown();
    }
    
    private void OnButtonUp()
    {
        StateMachine.MouseUp();
    }

    private void OnMouseEntered()
    {
        StateMachine.MouseEnter();
    }

    private void OnMouseExited()
    {
        StateMachine.MouseExit();
    }

    private void OnSuitTypeChangedEvent(SuitType suitType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 新值与旧值相等，返回
        if (SuitType == suitType) return;
        
        // 更新花色和贴图
        SuitType = suitType;
        SurfaceRect.Texture = suitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSuitHeart)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D,
            SuitType.Spade => _textureRegistry.Get(nameof(TextureKey.PokerSuitSpade)) as Texture2D,
            SuitType.Club => _textureRegistry.Get(nameof(TextureKey.PokerSuitClub)) as Texture2D,
            _ => null
        };
    }
    
    private void OnNumValueChangedEvent(String numValue,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 为null，返回
        if (numValue == null!) return;
        
        // 新值与旧值相等，返回
        if (string.Equals(NumValue, numValue, StringComparison.Ordinal)) return;
        
        // 更新数值和显示
        NumValue = numValue;
        NumLabel.Text = numValue;
    }
    
    private void OnNumTypeChangedEvent(NumType numType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 新值与旧值相等，返回
        if (NumType == numType) return;
        
        // 更新数值类型
        NumType = numType;
    }
    
    private void OnStateChangedEvent(StateType stateType,IPoker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        StateMachine.ChangeTo(stateType);
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        StateMachine.ChangeTo(enable ? StateType.UnSelect : StateType.Idle);
    }
}