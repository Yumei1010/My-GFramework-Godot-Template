using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.Godot.pool;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFrameworkGodotTemplate.scripts.enums.resources;
using GFrameworkGodotTemplate.scripts.utility;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

[ContextAware]
[Log]
public partial class Poker : Button, IPoker, IController, IPoolableNode
{
    private AnimationPlayer AnimationPlayer => GetNode<AnimationPlayer>("AnimationPlayer");
    private TextureRect Back => GetNode<TextureRect>("%Back");
    private TextureRect Surface => GetNode<TextureRect>("%Surface");
    private TextureRect Suit => GetNode<TextureRect>("%Suit");
    private Label Num => GetNode<Label>("%Num");

    private IGodotTextureRegistry _textureRegistry = null!;
    
    private bool _onDarging;
    
    // 扑克属性，从JSON加载
    private PokerDefinition _definition = null!;
    
    private IPokerPoolSystem _pool = null!;
    
    /// <summary>
    /// 节点准备就绪时的回调方法
    /// 在节点添加到场景树后调用
    /// </summary>
    public override void _Ready()
    {
        ConnectSignal();
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        Suit.Texture = _textureRegistry.Get(nameof(TextureKey.PokerSuitDiamond)) as Texture2D;
    }

    /// <summary>
    /// 每次渲染帧的回调方法
    /// 每帧调用
    /// </summary>
    public override void _Process(double delta)
    {
        if (_onDarging)
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init(PokerDefinition definition)
    {
        // 初始化PokerData
        _definition = definition;
        
        _pool = this.GetSystem<IPokerPoolSystem>()!;
        
        _log.Debug("Poker Initialized");
    }

    public void OnAcquire()
    {
        
    }

    public void OnRelease()
    {
        
    }

    public void OnPoolDestroy()
    {
        
    }

    public Node AsNode()
    {
        return this;
    }
    
    private void ConnectSignal()
    {
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnButtonDown()
    {
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        _onDarging = true;
    }

    private void OnButtonUp()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        _onDarging = false;
    }
    

    private void OnMouseEntered()
    {
        if (AnimationPlayer.IsPlaying())
        {
            AnimationPlayer.Stop();
        }
        AnimationPlayer.Play("Poker/focused");
    }

    private void OnMouseExited()
    {
        if (AnimationPlayer.IsPlaying())
        {
            AnimationPlayer.Stop();
        }
        AnimationPlayer.Play("Poker/blured");
    }
}