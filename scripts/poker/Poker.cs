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
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");

    private IGodotTextureRegistry _textureRegistry = null!;
    private Tween _tween = null!;

    public required Guid Id { get; set; } = Guid.Empty;
    public SuitType SuitType { get; set; } = SuitType.Heart;
    public required string NumValue { get; set; } = null!;
    public NumType NumType { get; set; } = NumType.Integer;
    
    public IList<StateType> States { get; set; } = new List<StateType>();
    public Vector2 HandPosition { get; set; }
    public float HandRotation { get; set; }

    private Vector2 _lastMousePosition;
    private float _targetRotationRad;

    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }
    
    public override void _Process(double delta)
    {
        // 如果拥有状态拖拽中，则每帧调用方法变为拖拽卡牌
        if (States.Contains(StateType.OnDrag))
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
            
            Vector2 currentMousePosition =  GetGlobalMousePosition();
            Vector2 gap = currentMousePosition - _lastMousePosition;
            _lastMousePosition = currentMousePosition;
            _targetRotationRad = Mathf.DegToRad(Mathf.Clamp(gap.X * 15f, -30f, 30f));
            Rotation = Mathf.LerpAngle(Rotation, _targetRotationRad, 10f * (float)delta);
        }
    }

    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
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
        
        // 注册对选择器可用性变更事件的监听
        this.RegisterEvent<EnableChangedEvent>(e =>
        {
            OnEnableChangedEvent(e.Enable);
        }).UnRegisterWhenNodeExitTree(this);
    }
    
    private void OnButtonDown()
    {
        this.SendEvent(new SelectChangedEvent()
        {
            Poker = this
        });
        
        // 如果拥有状态未选择，则点击效果变为选择卡牌
        if (States.Contains(StateType.UnSelect)) 
        {
            // 上浮一定距离以突出
            var pos = GlobalPosition;
            pos.Y -= Size.Y / 2;
            GlobalPosition = pos;
            return;
        }
        
        // 如果拥有状态选择中，则点击效果变为取消选择卡牌
        if (States.Contains(StateType.OnSelect)) 
        {
            // 下沉一定距离以回归
            var pos = GlobalPosition;
            pos.Y += Size.Y / 2;
            GlobalPosition = pos;
            return;
        }
        
        // 默认点击效果为开始拖拽卡牌，隐藏并锁定鼠标在窗口范围内
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        States.Add(StateType.OnDrag);
    }
    
    private void OnButtonUp()
    {
        // 如果拥有状态手牌中，复位卡牌
        if (States.Contains(StateType.InHand)) _ = ResetPosAndRot();
        
        // 默认释放效果为结束拖拽卡牌，显示鼠标
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        States.Remove(StateType.OnDrag);
    }

    private void OnMouseEntered()
    {
        // 如果拥有状态未选择，则动画效果变为聚焦
        if (States.Contains(StateType.UnSelect)) 
        {
            AnimationPlayer.Play("Poker/focused");
            return;
        }
        
        // 如果拥有状态选择中，返回
        if (States.Contains(StateType.OnSelect)) return;
        
        // 如果正在播放动画，使其终止
        if (AnimationPlayer.IsPlaying()) AnimationPlayer.Stop();
        AnimationPlayer.Play("Poker/blured");
        ResetRot();
    }

    private void OnMouseExited()
    {
        // 如果拥有状态未选择，返回
        if (States.Contains(StateType.UnSelect)) return;
        
        // 如果拥有状态选择中，返回
        if (States.Contains(StateType.OnSelect)) return;
        
        // 如果正在播放动画，使其终止
        if (AnimationPlayer.IsPlaying()) AnimationPlayer.Stop();
        AnimationPlayer.Play("Poker/blured");
    }

    private void OnSuitTypeChangedEvent(SuitType suitType,Poker poker)
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
    
    private void OnNumValueChangedEvent(String numValue,Poker poker)
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
    
    private void OnNumTypeChangedEvent(NumType numType,Poker poker)
    {
        // 如果不是触发事件的poker，返回
        if (poker != this) return;
        
        // 新值与旧值相等，返回
        if (NumType == numType) return;
        
        // 更新数值类型
        NumType = numType;
    }
    
    private void OnEnableChangedEvent(bool enable)
    {
        // 如果选择器被启用，添加状态未选择，否则移除
        if (enable)
        {
            // 如果拥有状态未选择，返回
            if(States.Contains(StateType.UnSelect)) return;
            
            States.Add(StateType.UnSelect);
        }
        else
        {
            // 如果未拥有状态未选择，返回
            if(!States.Contains(StateType.UnSelect)) return;
            
            States.Remove(StateType.UnSelect);
        }
    }

    private void ResetPos()
    {
        // 如果正在播放动画，使其终止
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty( this, "global_position", HandPosition, 0.5f);
    }
    
    private void ResetRot()
    {
        // 如果正在播放动画，使其终止
        if (!_tween.IsNull() && _tween.IsRunning()) _tween.Kill();
        
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.InOut);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "rotation", Mathf.DegToRad(HandRotation), 0.5f);
    }
    
    private async Task ResetPosAndRot()
    {
        ResetPos();
        await ToSignal(_tween, Tween.SignalName.Finished);
        ResetRot();
    }
}