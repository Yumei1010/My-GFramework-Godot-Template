using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.model.annotation_tool;

namespace TimeToTwentyfour.scripts.component.annotation_tool;

/// <summary>
///     标注工具契约实现类
/// </summary>
[Log]
[ContextAware]
public partial class AnnotationTool : Control, IAnnotationTool
{
    public override void _Ready()
    {
        _ = ReadyAsync();
        RegisterEvent();
    }

    /// <summary>
    ///     切换当前绘制工具
    /// </summary>
    public void ChangeTo(AnnotationToolType tool)
    {
        _currentTool = tool;
    }

    public override void _Draw()
    {
        foreach (var line in _lines)
            DrawLine(line.Start, line.End, Colors.Red, StrokeWidth);
        foreach (var circle in _circles)
            DrawCircle(circle.Center, circle.Radius, Colors.Blue, false, StrokeWidth);
        foreach (var rect in _rects)
            DrawRect(new Rect2(rect.TopLeft, rect.BottomRight - rect.TopLeft), Colors.Red, false, StrokeWidth);
        foreach (var freehand in _freehandLines)
            DrawPolyline(freehand.Points.ToArray(), Colors.White, StrokeWidth / 2.0f);
    }

    public override void _Input(InputEvent @event)
    {
        var p = GetGlobalMousePosition();

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
        {
            _drawing = mouseButton.Pressed;
            if (_drawing)
                OnDrawStart(p);
        }
        else if (@event is InputEventMouseMotion)
        {
            if (_drawing)
                OnDrawMove(p);
        }
    }

    private void OnDrawStart(Vector2 p)
    {
        switch (_currentTool)
        {
            case AnnotationToolType.Line:
                var line = new LineElement { Start = p, End = p };
                _lines.Add(line);
                _currentElement = line;
                break;
            case AnnotationToolType.Circle:
                var circle = new CircleElement { Center = p, Radius = 0.0f };
                _circles.Add(circle);
                _currentElement = circle;
                break;
            case AnnotationToolType.Rect:
                var rect = new RectElement { TopLeft = p, BottomRight = p };
                _rects.Add(rect);
                _currentElement = rect;
                break;
            case AnnotationToolType.Freehand:
                var freehand = new FreehandLine();
                freehand.Points.Add(p);
                _freehandLines.Add(freehand);
                _currentElement = freehand;
                break;
            case AnnotationToolType.Eraser:
                Erase(p);
                break;
        }
    }

    private void OnDrawMove(Vector2 p)
    {
        switch (_currentElement)
        {
            case LineElement line:
                line.End = p;
                QueueRedraw();
                break;
            case CircleElement circle:
                circle.Radius = p.DistanceTo(circle.Center);
                QueueRedraw();
                break;
            case RectElement rect:
                rect.BottomRight = p;
                QueueRedraw();
                break;
            case FreehandLine freehand:
                freehand.Points.Add(p);
                QueueRedraw();
                break;
            case null when _currentTool == AnnotationToolType.Eraser:
                Erase(p);
                break;
        }
    }

    private void Erase(Vector2 mousePos)
    {
        const float eraserSize = 5.0f;

        for (int i = _lines.Count - 1; i >= 0; i--)
        {
            var line = _lines[i];
            if (PointDistanceToSegment(mousePos, line.Start, line.End) <= eraserSize)
                _lines.RemoveAt(i);
        }

        for (int i = _circles.Count - 1; i >= 0; i--)
        {
            var circle = _circles[i];
            if (Math.Abs(mousePos.DistanceTo(circle.Center) - circle.Radius) <= eraserSize)
                _circles.RemoveAt(i);
        }

        for (int i = _rects.Count - 1; i >= 0; i--)
        {
            var rect = _rects[i];
            var m = (rect.TopLeft + rect.BottomRight) / 2.0f;
            var s = (rect.BottomRight - rect.TopLeft).Abs();
            var d = (mousePos - m).Abs();
            if (Math.Abs(d.X - s.X / 2.0f) < eraserSize && d.Y < s.Y + eraserSize)
                _rects.RemoveAt(i);
            else if (Math.Abs(d.Y - s.Y / 2.0f) < eraserSize && d.X < s.X + eraserSize)
                _rects.RemoveAt(i);
        }

        for (int i = _freehandLines.Count - 1; i >= 0; i--)
        {
            var freehand = _freehandLines[i];
            freehand.Points.RemoveAll(pt => mousePos.DistanceTo(pt) <= eraserSize);
            if (freehand.Points.Count < 2)
                _freehandLines.RemoveAt(i);
        }

        QueueRedraw();
    }

    private static float PointDistanceToSegment(Vector2 p, Vector2 a, Vector2 b)
    {
        var ab = b - a;
        var bp = p - b;
        var ap = p - a;
        var dotAbBp = ab.X * bp.X + ab.Y * bp.Y;
        var dotAbAp = ab.X * ap.X + ab.Y * ap.Y;

        if (dotAbBp > 0)
            return MathF.Sqrt(bp.X * bp.X + bp.Y * bp.Y);
        if (dotAbAp < 0)
            return MathF.Sqrt(ap.X * ap.X + ap.Y * ap.Y);

        var mod = MathF.Sqrt(ab.X * ab.X + ab.Y * ab.Y);
        return MathF.Abs(ab.X * ap.Y - ab.Y * ap.X) / mod;
    }
}
