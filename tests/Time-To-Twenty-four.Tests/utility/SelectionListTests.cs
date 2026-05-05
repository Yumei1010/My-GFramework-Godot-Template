namespace TimeToTwentyfour.Tests.utility;

public class SelectionListTests
{
    [Fact]
    public void Add_NewItem_ReturnsTrue()
    {
        var list = new SelectionList();
        var poker = Card(1);

        bool added = list.Add(poker);

        Assert.True(added);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Add_DuplicateItem_ReturnsFalse()
    {
        var list = new SelectionList();
        var poker = Card(1);
        list.Add(poker);

        bool added = list.Add(poker);

        Assert.False(added);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Add_OverCapacity_EvictsOldestAndFiresEvent()
    {
        var list = new SelectionList { Capacity = 2 };
        var first = Card(1);
        var second = Card(2);
        var third = Card(3);
        list.Add(first);
        list.Add(second);

        IPoker? evicted = null;
        list.Evicted += p => evicted = p;
        bool added = list.Add(third);

        Assert.True(added);
        Assert.Equal(first, evicted);
        Assert.Equal(2, list.Count);
        Assert.Equal(second, list.Items[0]);
        Assert.Equal(third, list.Items[1]);
    }

    [Fact]
    public void Add_AtCapacity_NoEviction()
    {
        var list = new SelectionList { Capacity = 3 };
        list.Add(Card(1));
        list.Add(Card(2));

        var evicted = false;
        list.Evicted += _ => evicted = true;
        list.Add(Card(3));

        Assert.False(evicted);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Add_NoCapacity_UnlimitedAdd()
    {
        var list = new SelectionList();

        for (int i = 0; i < 100; i++)
            list.Add(Card(i));

        Assert.Equal(100, list.Count);
    }

    [Fact]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var list = new SelectionList();
        var poker = Card(1);
        list.Add(poker);

        bool removed = list.Remove(poker);

        Assert.True(removed);
        Assert.Empty(list.Items);
    }

    [Fact]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var list = new SelectionList();

        bool removed = list.Remove(Card(1));

        Assert.False(removed);
    }

    [Fact]
    public void Pop_NonEmpty_ReturnsLastAdded()
    {
        var list = new SelectionList();
        var first = Card(1);
        var second = Card(2);
        list.Add(first);
        list.Add(second);

        var popped = list.Pop();

        Assert.Equal(second, popped);
        Assert.Single(list.Items);
        Assert.Equal(first, list.Items[0]);
    }

    [Fact]
    public void Pop_Empty_ReturnsNull()
    {
        var list = new SelectionList();

        var popped = list.Pop();

        Assert.Null(popped);
    }

    [Fact]
    public void TrimExcess_WhenOverCapacity_EvictsFromFront()
    {
        var list = new SelectionList { Capacity = 3 };
        list.Add(Card(1));
        list.Add(Card(2));
        list.Add(Card(3));

        var evicted = new List<IPoker>();
        list.Evicted += evicted.Add;
        list.Capacity = 1;

        Assert.Equal(1, list.Count);
        Assert.Equal(2, evicted.Count);
        // 队首优先驱逐：1 最先被加入，最先被逐出
        Assert.Equal(1, int.Parse(((PokerStub)evicted[0]).NumValue, System.Globalization.CultureInfo.InvariantCulture));
        Assert.Equal(2, int.Parse(((PokerStub)evicted[1]).NumValue, System.Globalization.CultureInfo.InvariantCulture));
        // 最后加入的 3 保留
        Assert.Equal(3, int.Parse(((PokerStub)list.Items[0]).NumValue, System.Globalization.CultureInfo.InvariantCulture));
    }

    [Fact]
    public void TrimExcess_UnderCapacity_Noop()
    {
        var list = new SelectionList { Capacity = 5 };
        list.Add(Card(1));

        var evicted = false;
        list.Evicted += _ => evicted = true;
        list.TrimExcess();

        Assert.False(evicted);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Contains_ExistingItem_ReturnsTrue()
    {
        var list = new SelectionList();
        var poker = Card(1);
        list.Add(poker);

        Assert.True(list.Contains(poker));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = new SelectionList();

        Assert.False(list.Contains(Card(1)));
    }

    [Fact]
    public void Capacity_Zero_MeansUnlimited()
    {
        var list = new SelectionList { Capacity = 0 };

        for (int i = 0; i < 10; i++)
            list.Add(Card(i));

        Assert.Equal(10, list.Count);
    }

    [Fact]
    public void Items_MaintainsInsertionOrder()
    {
        var list = new SelectionList();
        var p1 = Card(1);
        var p2 = Card(2);
        var p3 = Card(3);
        list.Add(p1);
        list.Add(p2);
        list.Add(p3);

        Assert.Equal([p1, p2, p3], list.Items);
    }
}
