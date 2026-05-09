using Godot;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

public partial class AnnotationTool
{
    private Color _color = Colors.Red;
    private bool _drawing;
    private Vector2 _mousePos;
    private object? _currentElement;
    private bool _opening;
}
