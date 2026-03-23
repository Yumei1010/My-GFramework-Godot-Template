using GFrameworkGodotTemplate.global;
using Godot;

namespace GFrameworkGodotTemplate.scripts.component;

public partial class CalculateBar : Control
{
    private TextureButton AddButton => GetNode<TextureButton>("%AddButton");
    private TextureButton SubtractButton => GetNode<TextureButton>("%SubtractButton");
    private TextureButton MultiplyButton => GetNode<TextureButton>("%MultiplyButton");
    private TextureButton DivideButton => GetNode<TextureButton>("%DivideButton");
    private TextureButton ModuloButton => GetNode<TextureButton>("%ModuloButton");
    private TextureButton NthRootButton => GetNode<TextureButton>("%NthRootButton");
    private TextureButton PowerButton => GetNode<TextureButton>("%PowerButton");
    private TextureButton AbsoluteValueButton => GetNode<TextureButton>("%AbsoluteValueButton");
    private TextureButton FactorialButton => GetNode<TextureButton>("%FactorialButton");
    private TextureButton SquareRootButton => GetNode<TextureButton>("%SquareRootButton");
    private TextureButton CeilButton => GetNode<TextureButton>("%CeilButton");
    private TextureButton FloorButton => GetNode<TextureButton>("%FloorButton");
    
    public override void _Ready()
    {
        _ = ReadyAsync();
        ConnectSignal();
    }
    
    private async Task ReadyAsync()
    {
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
    }
    
    private void ConnectSignal()
    {
        AddButton.ButtonDown += OnButtonDownAddButton;
        SubtractButton.ButtonDown += OnButtonDownSubtractButton;
        MultiplyButton.ButtonDown += OnButtonDownMultiplyButton; 
        DivideButton.ButtonDown += OnButtonDownDivideButton;
        ModuloButton.ButtonDown += OnButtonDownModuloButton;
        NthRootButton.ButtonDown += OnButtonDownSquareRootButton;
        PowerButton.ButtonDown += OnButtonDownPowerButton;
        AbsoluteValueButton.ButtonDown += OnButtonDownAbsoluteValueButton;
        FactorialButton.ButtonDown += OnButtonDownFactorialButton;
        SquareRootButton.ButtonDown += OnButtonDownSquareRootButton;
        CeilButton.ButtonDown += OnButtonDownCeilButton;
        FloorButton.ButtonDown += OnButtonDownFloorButton;
    }
    
    private void OnButtonDownAddButton()
    {
        
    }

    private void OnButtonDownSubtractButton()
    {
        
    }

    private void OnButtonDownMultiplyButton()
    {
        
    }

    private void OnButtonDownDivideButton()
    {
        
    }

    private void OnButtonDownModuloButton()
    {
        
    }

    private void OnButtonDownSquareRootButton()
    {
        
    }

    private void OnButtonDownCeilButton()
    {
        
    }

    private void OnButtonDownFloorButton()
    {
        
    }

    private void OnButtonDownPowerButton()
    {
        
    }
    
    private void OnButtonDownAbsoluteValueButton()
    {
        
    }

    private void OnButtonDownFactorialButton()
    {
        
    }
}
