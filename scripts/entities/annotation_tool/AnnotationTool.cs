using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using GFramework.Core.extensions;
using Godot;
using TimeToTwentyfour.scripts.enums.annotation_tool;
using TimeToTwentyfour.scripts.data.annotation_tool;
using TimeToTwentyfour.scripts.cqrs.annotation_tool.command;

namespace TimeToTwentyfour.scripts.entities.annotation_tool;

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
        ConnectSignal();
        RegisterEvent();
    }

    public void ChangeTo(AnnotationToolType tool)
    {
        this.SendCommand(new AnnotationToolChangeToolCommand { Tool = tool });
    }

    public override void _Draw()
    {
        foreach (var line in _lines)
            DrawLine(line.Start, line.End, line.Color, line.Width);
        foreach (var circle in _circles)
            DrawCircle(circle.Center, circle.Radius, circle.Color, false, circle.Width);
        foreach (var rect in _rects)
            DrawRect(new Rect2(rect.TopLeft, rect.BottomRight - rect.TopLeft), rect.Color, false, rect.Width);
        foreach (var freehand in _freehandLines)
        {
            if (freehand.Points.Count < 2)
                continue;
            DrawPolyline(freehand.Points.ToArray(), freehand.Color, freehand.Width / 2.0f);
        }

        DrawCursorIndicator();
    }

    public override void _Input(InputEvent @event)
    {
        if (!_enabled)
        {
            _drawing = false;
            return;
        }

        var p = GetGlobalMousePosition();
        var overTool = ToolRect.GetGlobalRect().HasPoint(p);

        if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
        {
            _mousePos = p;
            if (!overTool)
            {
                _drawing = mouseButton.Pressed;
                if (_drawing)
                    OnDrawStart(p);
            }
        }
        else if (@event is InputEventMouseMotion)
        {
            _mousePos = p;
            QueueRedraw();
            if (!overTool && _drawing)
                OnDrawMove(p);
        }
    }

    private void DrawCursorIndicator()
    {
        if (!_enabled || _mousePos == Vector2.Zero)
            return;

        if (_currentTool == AnnotationToolType.Eraser)
            DrawCircle(_mousePos, _toolWidth, new Color(1, 1, 1, 0.3f), false, 1.0f);
        else
            DrawCircle(_mousePos, _toolWidth / 2.0f, new Color(_color, 0.4f));
    }

    private void OnDrawStart(Vector2 p)
    {
        switch (_currentTool)
        {
            case AnnotationToolType.Line:
                var line = new LineElementData { Start = p, End = p, Color = _color, Width = _toolWidth };
                _lines.Add(line);
                _currentElement = line;
                break;
            case AnnotationToolType.Circle:
                var circle = new CircleElementData { Center = p, Radius = 0.0f, Color = _color, Width = _toolWidth };
                _circles.Add(circle);
                _currentElement = circle;
                break;
            case AnnotationToolType.Rect:
                var rect = new RectElementData { TopLeft = p, BottomRight = p, Color = _color, Width = _toolWidth };
                _rects.Add(rect);
                _currentElement = rect;
                break;
            case AnnotationToolType.Freehand:
                var freehand = new FreehandLineData { Color = _color, Width = _toolWidth };
                freehand.Points.Add(p);
                _freehandLines.Add(freehand);
                _currentElement = freehand;
                break;
            case AnnotationToolType.Eraser:
                _currentElement = null!;
                Erase(p);
                break;
        }
    }

    private void OnDrawMove(Vector2 p)
    {
        switch (_currentElement)
        {
            case LineElementData line:
                line.End = p;
                QueueRedraw();
                break;
            case CircleElementData circle:
                circle.Radius = p.DistanceTo(circle.Center);
                QueueRedraw();
                break;
            case RectElementData rect:
                rect.BottomRight = p;
                QueueRedraw();
                break;
            case FreehandLineData freehand:
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
        var r = _toolWidth;

        for (int i = _lines.Count - 1; i >= 0; i--)
        {
            var line = _lines[i];
            if (PointDistanceToSegment(mousePos, line.Start, line.End) <= r)
                _lines.RemoveAt(i);
        }

        for (int i = _circles.Count - 1; i >= 0; i--)
        {
            var circle = _circles[i];
            if (Math.Abs(mousePos.DistanceTo(circle.Center) - circle.Radius) <= r)
                _circles.RemoveAt(i);
        }

        for (int i = _rects.Count - 1; i >= 0; i--)
        {
            var rect = _rects[i];
            var tl = rect.TopLeft;
            var br = rect.BottomRight;
            if (PointDistanceToSegment(mousePos, tl, new Vector2(br.X, tl.Y)) <= r
                || PointDistanceToSegment(mousePos, new Vector2(br.X, tl.Y), br) <= r
                || PointDistanceToSegment(mousePos, br, new Vector2(tl.X, br.Y)) <= r
                || PointDistanceToSegment(mousePos, new Vector2(tl.X, br.Y), tl) <= r)
                _rects.RemoveAt(i);
        }

        for (int i = _freehandLines.Count - 1; i >= 0; i--)
        {
            var pts = _freehandLines[i].Points;
            if (pts.Count < 2)
            {
                _freehandLines.RemoveAt(i);
                continue;
            }

            var keep = new bool[pts.Count];
            for (int j = 0; j < pts.Count; j++)
                keep[j] = mousePos.DistanceTo(pts[j]) > r;

            // 也擦除穿过橡皮圆内的线段
            for (int j = 1; j < pts.Count; j++)
            {
                if (keep[j - 1] && keep[j] && PointDistanceToSegment(mousePos, pts[j - 1], pts[j]) <= r)
                    keep[j - 1] = keep[j] = false;
            }

            var color = _freehandLines[i].Color;
            var width = _freehandLines[i].Width;
            var split = SplitContiguousSegments(pts, keep, color, width);
            _freehandLines.RemoveAt(i);
            foreach (var seg in split)
                _freehandLines.Add(seg);
        }

        QueueRedraw();
    }

    private static List<FreehandLineData> SplitContiguousSegments(List<Vector2> pts, bool[] keep, Color color, float width)
    {
        var result = new List<FreehandLineData>();
        var current = new List<Vector2>();
        for (int j = 0; j < pts.Count; j++)
        {
            if (keep[j])
            {
                current.Add(pts[j]);
            }
            else
            {
                if (current.Count >= 2)
                    result.Add(new FreehandLineData { Points = current, Color = color, Width = width });
                current = [];
            }
        }
        if (current.Count >= 2)
            result.Add(new FreehandLineData { Points = current, Color = color, Width = width });
        return result;
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
