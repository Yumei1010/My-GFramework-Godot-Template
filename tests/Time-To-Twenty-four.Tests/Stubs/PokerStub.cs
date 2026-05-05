using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.Tests.Stubs;

internal class PokerStub : IPoker
{
    public Guid Id { get; } = Guid.NewGuid();
    public SuitType SuitType { get; set; }
    public string NumValue { get; set; } = string.Empty;
    public NumType NumType { get; set; }
    public bool IsValid => true;
    public bool Shadow { get; set; }
    public bool Animate { get; set; }
    public float AnimateTime { get; set; }
    public bool Fake3D { get; set; }
    public bool TopLevel { get; set; }
    public Vector2 Size => default;
    public Vector2 GlobalPosition { get; set; }
    public float Rotation => 0f;
    public Vector2 ResetPosition { get; set; }
    public float ResetRotation { get; set; }

    public Vector2 GetGlobalMousePosition() => default;
    public Node GetParent() => null!;
    public void Reparent(Node parent) { }
    public void ChangeTo(StateType state) { }
    public void SpawnTo(Vector2 position) { }
    public void MoveTo(Vector2 position) { }
    public void Reset(string attributeName) { }
}
