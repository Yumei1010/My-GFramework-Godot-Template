using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

public partial class AnnotationTool
{
    private AnnotationToolModel Model = null!;
    private Color _color = Colors.Red;
    private bool _drawing;
    private Vector2 _mousePos;
    private object? _currentElement;
    private readonly List<LineElement> _lines = [];
    private readonly List<CircleElement> _circles = [];
    private readonly List<RectElement> _rects = [];
    private readonly List<FreehandLine> _freehandLines = [];
}
