using GFramework.Core.command;
using GFramework.Core.extensions;
using GFramework.Game.Abstractions.enums;
using GFramework.Game.Abstractions.ui;
using GFrameworkGodotTemplate.scripts.options_menu;

namespace GFrameworkGodotTemplate.scripts.command.menu;

public class OpenOptionsMenuCommand : AbstractCommand
{
    protected override void OnExecute()
    {
        this.GetSystem<IUiRouter>()!.Show(OptionsMenu.UiKeyStr, UiLayer.Modal);
    }
}