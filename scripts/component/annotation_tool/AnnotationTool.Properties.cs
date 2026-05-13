using GFramework.Game.Abstractions.data;
using Godot;
using TimeToTwentyfour.scripts.data.annotationTool;
using TimeToTwentyfour.scripts.enums.annotationTool;

namespace TimeToTwentyfour.scripts.component.annotationTool;

public partial class AnnotationTool
{
    public Color Color => _color;
    public AnnotationToolType CurrentTool => _currentTool;
    public bool Enabled => _enabled;
    public float ToolWidth => _toolWidth;

    private readonly List<LineElementData> _lines = [];
    private readonly List<CircleElementData> _circles = [];
    private readonly List<RectElementData> _rects = [];
    private readonly List<FreehandLineData> _freehandLines = [];

    private bool _drawing;
    private Vector2 _mousePos;
    private IData? _currentElement;
    private bool _opening;
    
    private Tween _tween = null!;

    private bool _enabled;
    private AnnotationToolType _currentTool;
    private Color _color;
    private float _toolWidth;
}
