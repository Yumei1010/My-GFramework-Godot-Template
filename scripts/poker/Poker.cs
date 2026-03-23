using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Godot.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.poker;
using GFrameworkGodotTemplate.scripts.enums.resources;
using GFrameworkGodotTemplate.scripts.events.poker;
using GFrameworkGodotTemplate.scripts.utility;
using GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
[Log]
public partial class Poker : Button, IPoker, IController
{
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("%AnimationPlayer");
    private TextureRect BackRect => GetNode<TextureRect>("%BackRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private TextureRect SuitRect => GetNode<TextureRect>("%SuitRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");

    private IGodotTextureRegistry _textureRegistry = null!;

    public required Guid Id { get; set; } = Guid.Empty;
    public SuitType SuitType { get; set; } = SuitType.Heart;
    public required string NumValue { get; set; } = null!;
    public NumType NumType { get; set; } = NumType.Integer;
    public IList<StateType> States { get; set; } = new List<StateType>();

    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
        RegisterEvent();
    }
    
    public override void _Process(double delta)
    {
        if (States.Contains(StateType.OnDrag))
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
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
    }
    
    private void OnButtonDown()
    {
        // 隐藏并锁定鼠标在窗口范围内
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        States.Add(StateType.OnDrag);
    }
    
    private void OnButtonUp()
    {
        // 显示鼠标
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        States.Remove(StateType.OnDrag);
    }

    private void OnMouseEntered()
    {
        // 如果正在播放动画，使其终止
        if (AnimationPlayer.IsPlaying()) AnimationPlayer.Stop();
        
        AnimationPlayer.Play("Poker/focused");
    }

    private void OnMouseExited()
    {
        // 如果正在播放动画，使其终止
        if (AnimationPlayer.IsPlaying()) AnimationPlayer.Stop();
        
        AnimationPlayer.Play("Poker/blured");
    }

    private void OnSuitTypeChangedEvent(SuitType suitType,Poker poker)
    {
        // 如果不是触发事件的poker，返回
        if(poker != this) return;
        // 新值与旧值相等，返回
        if(SuitType == suitType) return;
        
        // 更新花色和贴图
        SuitType = suitType;
        SuitRect.Texture = suitType switch
        {
            SuitType.Heart => _textureRegistry.Get(nameof(TextureKey.PokerSuitHeart)) as Texture2D,
            SuitType.Diamond => _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D,
            // SuitType.Spade => ,
            // SuitType.Club => ,
            _ => null
        };
    }
    
    private void OnNumValueChangedEvent(String numValue,Poker poker)
    {
        // 如果不是触发事件的poker，返回
        if(poker != this) return;
        // numValue为null，返回
        if(numValue == null!) return;
        // 新值与旧值相等，返回
        if (string.Equals(NumValue, numValue, StringComparison.Ordinal)) return;
        
        // 更新数值和显示
        NumValue = numValue;
        NumLabel.Text = numValue;
    }
    
    private void OnNumTypeChangedEvent(NumType numType,Poker poker)
    {
        // 如果不是触发事件的poker，返回
        if(poker != this) return;
        // 新值与旧值相等，返回
        if(NumType == numType) return;
        
        // 更新数值类型
        NumType = numType;
    }
}