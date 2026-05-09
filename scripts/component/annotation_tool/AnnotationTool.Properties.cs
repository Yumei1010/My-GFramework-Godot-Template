using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

public partial class AnnotationTool
{
    [Export] public bool Enabled { get; set; } = false;
    [Export] public float StrokeWidth { get; set; } = 5.0f;
    [Export] public float EraserRadius { get; set; } = 20.0f;
    private Color _color = Colors.Red;
    private bool _drawing;
    private Vector2 _mousePos;
    private AnnotationToolType _currentTool;
    private object? _currentElement;
    private readonly List<LineElement> _lines = [];
    private readonly List<CircleElement> _circles = [];
    private readonly List<RectElement> _rects = [];
    private readonly List<FreehandLine> _freehandLines = [];
}
