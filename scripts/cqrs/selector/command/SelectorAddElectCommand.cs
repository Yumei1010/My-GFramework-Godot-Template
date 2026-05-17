using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.cqrs.selector.command;

public sealed class SelectorAddElectCommand : AbstractCommand
{
    public required Guid PokerId { get; set; }

    protected override void OnExecute()
    {
        this.GetModel<SelectorModel>().Selects.Add(PokerId);
    }
}
