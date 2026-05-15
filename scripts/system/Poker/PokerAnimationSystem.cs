using GFramework.Core.Abstractions.enums;
using GFramework.Core.Abstractions.system;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;
using TimeToTwentyfour.scripts.entities.poker;

namespace TimeToTwentyfour.scripts.system.Poker;

[Log]
[ContextAware]
public partial class PokerAnimationSystem : ISystem
{
    private struct AnimationBundle
    {
        public IPokerView Poker;
        public ShaderMaterial Material;
        public TextureRect ShadowRect;
        public Tween TweenPos;
        public Tween TweenRot;
    }

    private readonly Dictionary<Guid, AnimationBundle> Bundles = [];

    public void OnArchitecturePhase(ArchitecturePhase phase)
    {
        _log.Debug("System initialized: PokerAnimationSystem");
    }

    public void Init()
    {
        
    }

    public void Destroy()
    {
        
    }

    public void InitAnimations(IPokerView poker, ShaderMaterial material, TextureRect shadowRect)
    {
        Bundles[poker.Id] = new AnimationBundle
        {
            Poker = poker,
            Material = material,
            ShadowRect = shadowRect,
        };
    }

    public void RemoveBundle(Guid id) => Bundles.Remove(id);

    public void UpdateViewPosition(Guid id, Vector2 targetPosition)
    {
        if (!Bundles.TryGetValue(id, out var b)) return;
        var node = (Control)b.Poker;

        node.GlobalPosition = targetPosition;
    }

    public void ResetViewPosition(Guid id, Vector2 resetPosition)
    {
        if (!Bundles.TryGetValue(id, out var b)) return;
        var node = (Control)b.Poker;

        if (!b.Poker.Animate)
        {
            node.GlobalPosition = resetPosition;
            return;
        }

        if (!b.TweenPos.IsNull() && b.TweenPos.IsRunning())
            b.TweenPos.Kill();

        var tp = b.TweenPos = node.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        tp.TweenProperty(node, "global_position", resetPosition, b.Poker.AnimateTime);
    }

    public void UpdateViewRotation(Guid id, float targetRotation)
    {
        if (!Bundles.TryGetValue(id, out var b)) return;
        var node = (Control)b.Poker;

        node.Rotation = targetRotation;
    }

    public void ResetViewRotation(Guid id, float resetRotation)
    {
        if (!Bundles.TryGetValue(id, out var b)) return;
        var node = (Control)b.Poker;

        if (!b.Poker.Animate)
        {
            node.Rotation = resetRotation;
            return;
        }

        if (!b.TweenRot.IsNull() && b.TweenRot.IsRunning())
            b.TweenRot.Kill();

        var tr = b.TweenRot = node.CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Elastic);
        tr.TweenProperty(node, "rotation", resetRotation, b.Poker.AnimateTime);
    }

    public void UpdateShadowPosition(Guid id)
    {
        if (!Bundles.TryGetValue(id, out var b)) return;
        var node = (Control)b.Poker;

        if (!b.Poker.Shadow)
        {
            b.ShadowRect.Hide();
            return;
        }

        b.ShadowRect.Show();
        var shadowPos = b.ShadowRect.Position;
        var viewportCenter = node.GetViewportRect().Size / 2f;
        var offset = Mathf.Sign(node.GlobalPosition.X - viewportCenter.X);
        var ratio = Mathf.Abs((node.GlobalPosition.X - viewportCenter.X) / viewportCenter.X);
        shadowPos.X = Mathf.Lerp(0f, -offset * 20f, ratio);
        b.ShadowRect.Position = shadowPos;
    }

    public void UpdateFake3DRotation(Guid id)
    {
        if (!Bundles.TryGetValue(id, out var b) || !b.Poker.Fake3D) return;
        var node = (Control)b.Poker;

        var localMouse = node.GetLocalMousePosition();
        var size = node.Size;
        var rotX = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(-5f), Mathf.DegToRad(5f), Mathf.Clamp(localMouse.X / size.X, 0f, 1f)));
        var rotY = Mathf.RadToDeg(Mathf.LerpAngle(Mathf.DegToRad(5f), Mathf.DegToRad(-5f), Mathf.Clamp(localMouse.Y / size.Y, 0f, 1f)));
        b.Material.SetShaderParameter("x_rot", rotX);
        b.Material.SetShaderParameter("y_rot", rotY);
    }

    public void ResetFake3DRotation(Guid id)
    {
        if (!Bundles.TryGetValue(id, out var b) || !b.Poker.Fake3D) return;
        var node = (Control)b.Poker;

        if (!b.TweenRot.IsNull() && b.TweenRot.IsRunning())
            b.TweenRot.Kill();

        var t = b.TweenRot = node.CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Back);
        t.TweenProperty(b.Material, "shader_parameter/x_rot", 0.0f, b.Poker.AnimateTime);
        t.TweenProperty(b.Material, "shader_parameter/y_rot", 0.0f, b.Poker.AnimateTime);
    }
}
