using GFramework.Core.extensions;
using GFramework.SourceGenerators.Abstractions.rule;
using TimeToTwentyfour.scripts.cqrs.menu.calculateMenu.@event;

namespace TimeToTwentyfour.scripts.menu.calculate_menu.optionButton;

[ContextAware]
public partial class PlayOptionButton : CalculateMenuOptionButton
{
    public override void OnMouseDown()
    {
        this.SendEvent(new CalculateMenuPlayButtonClickedEvent());
    }

    public override void OnMouseUp()
    {
        
    }

    public override void OnMouseEnter()
    {
        
    }

    public override void OnMouseExit()
    {
        
    }
}