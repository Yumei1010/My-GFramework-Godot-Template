using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.enums.poker;
using TimeToTwentyfour.scripts.utility;
using TimeToTwentyfour.global;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

public partial class Poker
{
    private TextureRect ShadowRect => GetNode<TextureRect>("%ShadowRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("%SurfaceRect");
    private Label NumLabel => GetNode<Label>("%NumLabel");
    private TextureRect ReserveResultRect => GetNode<TextureRect>("%ReserveResultRect");
    private Label ReserveResultLabel => GetNode<Label>("%ReserveResultLabel");

    [ExportGroup("Style")]
    [ExportSubgroup("Suit")]
    private SuitType _suitType = SuitType.Heart;
    [Export] public SuitType SuitType
    {
        get => _suitType;
        set
        {
            _suitType = value;
            if (IsNodeReady())
                UpdateSurfaceRect();
        }
    }

    [ExportSubgroup("Value")]
    private string _numValue = "24";
    [Export] public string NumValue
    {
        get => _numValue;
        set
        {
            _numValue = value;
            NumType = DetectNumType(value);
            if (IsNodeReady())
                UpdateNumValueLabel();
        }
    }
    [Export] public NumType NumType { get; set; } = NumType.Integer;

    /// <summary>根据数值字符串自动推断 <see cref="NumType"/>：含 "/" → Fraction，含 "." → Decimal，其余 → Integer。</summary>
    private static NumType DetectNumType(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return NumType.Integer;
        var trimmed = value.Trim();
        if (trimmed.Contains('/')) return NumType.Fraction;
        if (trimmed.Contains('.')) return NumType.Decimal;
        return NumType.Integer;
    }

    [ExportGroup("Animation")]
    [Export] public bool Shadow { get; set; } = true;
    [Export] public bool Animate { get; set; } = true;
    [Export] public float AnimateTime { get; set; } = 0.25f;
    [Export] public bool Fake3D { get; set; } = true;

    [ExportGroup("Debug")]
    [Export] public Vector2 ResetPosition { get; set; } = new (0, 0);

    [Export] public float ResetRotation { get; set; }

    private IGodotTextureRegistry _textureRegistry = null!;
    private ShaderMaterial _material = null!;
    private Tween _tweenPos = null!;
    private Tween _tweenRot = null!;
    private PokerManager _manager = null!;

    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);

        // 依赖注入
        _textureRegistry = this.GetUtility<IGodotTextureRegistry>()!;
        _material = (ShaderMaterial)SurfaceRect.Material;
        _manager = GetNode<PokerManager>("/root/PokerManager");

        // 初始化状态（状态在 PokerManager.InitStates 内部以纯 C# 类注册）
        _manager.InitStates(this);
        _manager.ChangeTo(Id, StateType.Idle);

        // 隐藏预览运算结果悬浮框
        ReserveResultRect.Hide();

        // 更新点数文本显示
        UpdateNumValueLabel();

        // 更新纹理显示
        UpdateSurfaceRect();

        // 注册到查找表
        _manager.Register(Id, this);
    }
}
