using GFramework.Core.Abstractions.controller;
using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.cqrs.poker.@event;
using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.component;

[ContextAware]
public partial class CalculateBar : Control, IController
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
        NthRootButton.ButtonDown += OnButtonDownNthRootButton;
        PowerButton.ButtonDown += OnButtonDownPowerButton;
        AbsoluteValueButton.ButtonDown += OnButtonDownAbsoluteValueButton;
        FactorialButton.ButtonDown += OnButtonDownFactorialButton;
        SquareRootButton.ButtonDown += OnButtonDownSquareRootButton;
        CeilButton.ButtonDown += OnButtonDownCeilButton;
        FloorButton.ButtonDown += OnButtonDownFloorButton;
    }

    private void OnButtonDownNthRootButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.NthRoot
        });
    }

    private void OnButtonDownAddButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Add
        });
    }

    private void OnButtonDownSubtractButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Subtract
        });
    }

    private void OnButtonDownMultiplyButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Multiply
        });
    }

    private void OnButtonDownDivideButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Divide
        });
    }

    private void OnButtonDownModuloButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Modulo
        });
    }

    private void OnButtonDownSquareRootButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.SquareRoot
        });
    }

    private void OnButtonDownCeilButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Ceil
        });
    }

    private void OnButtonDownFloorButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Floor
        });
    }

    private void OnButtonDownPowerButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.Power
        });
    }
    
    private void OnButtonDownAbsoluteValueButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent
        {
            Mode = ModeType.AbsoluteValue
        });
    }

    private void OnButtonDownFactorialButton()
    {
        this.SendEvent(new PokerSelectorModeChangedEvent()
        {
            Mode = ModeType.Factorial
        });
    }
}
