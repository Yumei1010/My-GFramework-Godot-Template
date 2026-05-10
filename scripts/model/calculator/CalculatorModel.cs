using GFramework.Core.model;
using TimeToTwentyfour.scripts.enums.calculator;

namespace TimeToTwentyfour.scripts.model.calculator;

public class CalculatorModel : AbstractModel
{
    public ModeType Mode { get; set; }

    protected override void OnInit()
    {

    }
}
