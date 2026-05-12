using Godot;

namespace TimeToTwentyfour.scripts.component.annotationTool;

public partial class AnnotationTool
{
    private Color _color = Colors.Red;
    private bool _drawing;
    private Vector2 _mousePos;
    private object? _currentElement;
    private bool _opening;
}
