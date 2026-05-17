using TimeToTwentyfour.scripts.cqrs.poker.query.result;
using GFramework.Core.extensions;
using GFramework.Core.query;
using TimeToTwentyfour.global;
using TimeToTwentyfour.scripts.enums.poker;

namespace TimeToTwentyfour.scripts.cqrs.poker.query;

public sealed class GetSuitQuery : AbstractQuery<PokerView>
{
    public Guid Id { get; init; }

    protected override PokerView OnDo()
    {
        throw new NotImplementedException();
    }
}