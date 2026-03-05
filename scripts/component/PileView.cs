using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

[ContextAware]
[Log]
public partial class PileView : Button
{
    private TextureRect BackRect => GetNode<TextureRect>("BackRect");
    private TextureRect SurfaceRect => GetNode<TextureRect>("SurfaceRect");
    private TextureRect SuitRect => GetNode<TextureRect>("SuitRect");
    
    private bool _isMoving;
    private Tween _tween = null!;

    public sealed record MouseWheelUp;
    public sealed record MouseWheelDown;
    
    public void Update(Texture2D back,Texture2D surface,Texture2D suit)
    {
        BackRect.Texture = back;
        SurfaceRect.Texture = surface;
        SuitRect.Texture = suit;
    }
    
    public override void _Ready()
    {
        ConnectSignal();
    }

    public override void _Process(double delta)
    {
        if (_isMoving)
        {
            GlobalPosition = GetGlobalMousePosition() - Size / 2;
        }
    }

    public override void _GuiInput(InputEvent @event)
    {
        if (Input.IsActionJustPressed("MouseRight"))
        {

        }
        else if (Input.IsActionJustPressed("MouseWheelUp"))
        {
            this.SendEvent(new MouseWheelUp());
        }
        else if (Input.IsActionJustPressed("MouseWheelDown"))
        {
            this.SendEvent(new MouseWheelDown());
        }
    }
    
    private void ConnectSignal()
    {
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
        ButtonDown += OnButtonDown;
        ButtonUp += OnButtonUp;
    }

    private void DisplayBack()
    {
        
    }
    
    private void OnMouseEntered()
    {
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.25f);
        _tween.TweenProperty(this, "scale", new Vector2(1.0f,1.0f), 0.25f);
    }

    private void OnMouseExited()
    {
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "rotation", 0.1f, 0.125f);
        _tween.TweenProperty(this, "rotation", 0.0f, 0.125f);
    }

    private void OnButtonDown()
    {
        Input.SetMouseMode(Input.MouseModeEnum.ConfinedHidden);
        _isMoving = true;
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "scale", new Vector2(1.2f, 1.2f), 0.15f);
        _tween.TweenProperty(this, "scale", new Vector2(1.0f,1.0f), 0.35f);
    }

    private void OnButtonUp()
    {
        Input.SetMouseMode(Input.MouseModeEnum.Visible);
        _isMoving = false;
        _tween = CreateTween();
        _tween.SetEase(Tween.EaseType.Out);
        _tween.SetTrans(Tween.TransitionType.Elastic);
        _tween.TweenProperty(this, "global_position", new Vector2(240,184), 0.25f);
    }
}