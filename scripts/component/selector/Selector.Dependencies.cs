using System;
using GFramework.Core.extensions;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.component.selector;

public partial class Selector
{
    private SelectorModel _model = null!;
    private readonly SelectionList _selection = new();

    private async Task ReadyAsync()
    {
        // 等待框架加载完成
        await GameEntryPoint.Architecture.WaitUntilReadyAsync().ConfigureAwait(false);
        
        _model = this.GetModel<SelectorModel>();
    }
}
