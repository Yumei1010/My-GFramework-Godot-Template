using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

public partial class AnnotationTool
{
    [Export] public float StrokeWidth { get; set; } = 5.0f;

    private bool _drawing;
    private AnnotationToolType _currentTool;
    private object? _currentElement;

    private readonly List<LineElement> _lines = [];
    private readonly List<CircleElement> _circles = [];
    private readonly List<RectElement> _rects = [];
    private readonly List<FreehandLine> _freehandLines = [];
}
