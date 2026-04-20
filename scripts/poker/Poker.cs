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
    private IPokerStateMachine StateMachine => GetNode<PokerStateMachine>("%StateMachine");
    private TextureRect ShadowRect => GetNode<TextureRect>("%ShadowRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");
    
    [Export] public SuitType SuitType { get; set; } = SuitType.Heart;
    [Export] public string NumValue { get; set; } = "24";
    [Export] NumType NumType { get; set; } = NumType.Integer;
    
    private Guid Id { get; } = Guid.NewGuid();
    private Vector2 DefaultPosition { get; set; }
    private float DefaultRotation { get; set; }
    
    private IGodotTextureRegistry _textureRegistry = null!;
    private ShaderMaterial _material = null!;
    private Tween _tweenPos = null!;
    private Tween _tweenRot = null!;
    private Tween _tweenScale = null!;

    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }
    
    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
        
        Vector2 shadowPos = ShadowRect.Position;
        shadowPos.X = Mathf.Lerp(0f, -Mathf.Sign(GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) * 20f, Mathf.Abs((GetGlobalPosition().X - (GetViewportRect().Size / 2f).X) / (GetViewportRect().Size / 2f).X));
        ShadowRect.Position = shadowPos;
    }

    public override void _GuiInput(InputEvent @event)
    {
        float rotX = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(-5f), Mathf.DegToRad(5f), Mathf.Clamp(GetLocalMousePosition().X / Size.X, 0f, 1f)));
        float rotY = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(5f), Mathf.DegToRad(-5f), Mathf.Clamp(GetLocalMousePosition().Y / Size.Y, 0f, 1f)));
        _material.SetShaderParameter("x_rot", rotY);
        _material.SetShaderParameter("y_rot", rotX);
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        _material = (ShaderMaterial)SurfaceRect.Material;
        
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
        this.RegisterEvent<PokerSuitTypeChangedEvent>(e =>
        {
            OnSuitTypeChangedEvent(e.SuitType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值变更事件的监听
        this.RegisterEvent<PokerNumValueChangedEvent>(e =>
        {
            OnNumValueChangedEvent(e.NumValue,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对数值类型变更事件的监听
        this.RegisterEvent<PokerNumTypeChangedEvent>(e =>
        {
            OnNumTypeChangedEvent(e.NumType,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对状态变更事件的监听
        this.RegisterEvent<PokerStateChangedEvent>(e =>
        {
            OnStateChangedEvent(e.State,e.Poker);
        }).UnRegisterWhenNodeExitTree(this);
        
        // 注册对选择器可用性变更事件的监听
        this.RegisterEvent<PokerSelectorEnableChangedEvent>(e =>
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
        if (!_tweenPos.IsNull() && _tweenPos.IsRunning()) _tweenPos.Kill();
        _tweenPos = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenPos.TweenProperty( this, "global_position", DefaultPosition, 0.25f);
    }
    
    public void ResetRot()
    {
        // 如果正在播放动画，使其终止
        if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();
        _tweenRot = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenRot.TweenProperty(this, "rotation", Mathf.DegToRad(DefaultRotation), 0.25f);
    }

    public void ResetPosAndRot()
    {
        ResetPos();
        ResetRot();
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
                
        // 如果正在播放动画，使其终止
        if (!_tweenScale.IsNull() && _tweenScale.IsRunning()) _tweenScale.Kill();
        _tweenScale = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenScale.TweenProperty(this, "scale", new Vector2(1.2f,1.2f), 0.25f);
    }

    private void OnMouseExited()
    {
        StateMachine.MouseExit();
        
        // 如果正在播放动画，使其终止
        if (!_tweenRot.IsNull() && _tweenRot.IsRunning()) _tweenRot.Kill();
        _tweenRot = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
        _tweenRot.TweenProperty(_material, "shader_parameter/x_rot", 0.0f, 0.5f);
        _tweenRot.TweenProperty(_material, "shader_parameter/y_rot", 0.0f, 0.5f);
        
        // 如果正在播放动画，使其终止
        if (!_tweenScale.IsNull() && _tweenScale.IsRunning()) _tweenScale.Kill();
        _tweenScale = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        _tweenScale.TweenProperty(this, "scale", new Vector2(1.0f,1.0f), 0.25f);
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