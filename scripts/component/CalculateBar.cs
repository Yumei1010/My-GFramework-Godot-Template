using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.logging;
using GFramework.SourceGenerators.Abstractions.rule;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

[ContextAware]
[Log]

public partial class CalculateBar : Control
{
    [Export] public Godot.Collections.Dictionary<string, TextureButton> CalculateButton = new();
    
    private TextureButton AddButton => GetNode<TextureButton>("AddButton");
    private TextureButton SubtractButton => GetNode<TextureButton>("SubtractButton");
    private TextureButton MultiplyButton => GetNode<TextureButton>("MultiplyButton");
    private TextureButton DivideButton => GetNode<TextureButton>("DivideButton");
    private TextureButton ModuloButton => GetNode<TextureButton>("ModuloButton");
    private TextureButton NthRootButton => GetNode<TextureButton>("NthRootButton");
    private TextureButton PowerButton => GetNode<TextureButton>("PowerButton");
    private TextureButton AbsoluteValueButton => GetNode<TextureButton>("AbsoluteValueButton");
    private TextureButton FactorialButton => GetNode<TextureButton>("FactorialButton");
    private TextureButton SquareRootButton => GetNode<TextureButton>("SquareRootButton");
    private TextureButton CeilButton => GetNode<TextureButton>("CeilButton");
    private TextureButton FloorButton => GetNode<TextureButton>("FloorButton");
    
    public override void _Ready()
    {
        ConnectSignal();
        RegisterEvent();
    }
    
    private void ConnectSignal()
    {
        
    }

    private void RegisterEvent()
    {
        
    }
}
