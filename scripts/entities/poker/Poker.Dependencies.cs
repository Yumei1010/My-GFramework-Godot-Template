using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.utility;
using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private IPokerStateMachine StateMachine => GetNode<PokerStateMachine>("%StateMachine");
    private TextureRect ShadowRect => GetNode<TextureRect>("%ShadowRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");
    private TextureRect ReserveResultRect => GetNode<TextureRect>("%ReserveResultRect");
    private Label ReserveResultLabel => GetNode<Label>("%ReserveResultLabel");
    
    private IGodotTextureRegistry _textureRegistry = null!;
    private ShaderMaterial _material = null!;
    private Tween _tweenPos = null!;
    private Tween _tweenRot = null!;
    
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
        
        // 隐藏预览运算结果悬浮框
        ReserveResultRect.Hide();
        
        // 更新点数文本显示
        UpdateNumValueLabel();

        // 更新纹理显示
        UpdateSurfaceRect();
    }
}