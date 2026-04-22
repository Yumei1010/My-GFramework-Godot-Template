using GFramework.Core.Abstractions.logging;
using GFramework.Core.extensions;
using GFramework.Core.logging;
using GFrameworkGodotTemplate.scripts.utility;
using GFrameworkGodotTemplate.global;
using GFrameworkGodotTemplate.scripts.enums.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.poker;

public partial class Poker
{
    private IPokerStateMachine StateMachine => GetNode<PokerStateMachine>("%StateMachine");
    private TextureRect ShadowRect => GetNode<TextureRect>("%ShadowRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");
    
    private IGodotTextureRegistry _textureRegistry = null!;
    private ShaderMaterial _material = null!;
    private Tween _tweenPos = null!;
    private Tween _tweenRot = null!;
    private Tween _tweenScale = null!;

    private readonly ILogger _logger = LoggerFactoryResolver.Provider.CreateLogger("Poker");
    
    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        // 依赖注入
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        _material = (ShaderMaterial)SurfaceRect.Material;
        
        // 初始化状态机
        StateMachine.Init(this);
        StateMachine.ChangeTo(StateType.Idle);
    }
}