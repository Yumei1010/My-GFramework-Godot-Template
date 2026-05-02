using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.menu.calculateMenu.@event;

namespace TimeToTwentyfour.scripts.menu.calculate_menu.optionButton;

[ContextAware]
public partial class ModeOptionButton : CalculateMenuOptionButton
{
    public override void OnMouseDown()
    {
        
    }

    public override void OnMouseUp()
    {
        
    }

    public override void OnMouseEnter()
    {
        this.SendEvent(new CalculateMenuModeButtonHoveredEvent());
    }

    public override void OnMouseExit()
    {
        
    }
}