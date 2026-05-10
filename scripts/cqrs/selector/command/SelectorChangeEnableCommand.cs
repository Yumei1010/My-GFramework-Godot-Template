using GFramework.Core.command;
using GFramework.Core.extensions;
using TimeToTwentyfour.scripts.cqrs.selector.@event;
using TimeToTwentyfour.scripts.model.selector;

namespace TimeToTwentyfour.scripts.cqrs.selector.command;

/// <summary>
///     变更选择器可用性的命令
///     修改 <see cref="SelectorModel.Enable"/> 
///     发送 <see cref="SelectorEnableChangedEvent"/> 通知所有订阅者。
/// </summary>
public sealed class SelectorChangeEnableCommand : AbstractCommand
{
    public required bool Enable { get; init; }

    protected override void OnExecute()
    {
        this.GetModel<SelectorModel>().Enable = Enable;

        this.SendEvent(new SelectorEnableChangedEvent
        {
            Enable = Enable
        });
    }
}
