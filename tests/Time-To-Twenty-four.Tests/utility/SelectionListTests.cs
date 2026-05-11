namespace TimeToTwentyfour.Tests.utility;

public class SelectionListTests
{
    private static Guid Id(int n) => Guid.Parse($"00000000-0000-0000-0000-{n:D12}");

    [Fact]
    public void Add_NewItem_ReturnsTrue()
    {
        var list = new SelectionList();
        var id = Id(1);

        bool added = list.Add(id);

        Assert.True(added);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Add_DuplicateItem_ReturnsFalse()
    {
        var list = new SelectionList();
        var id = Id(1);
        list.Add(id);

        bool added = list.Add(id);

        Assert.False(added);
        Assert.Single(list.Items);
    }

    [Fact]
    public void Add_OverCapacity_EvictsOldestAndFiresEvent()
    {
        var list = new SelectionList { Capacity = 2 };
        var first = Id(1);
        var second = Id(2);
        var third = Id(3);
        list.Add(first);
        list.Add(second);

        Guid? evicted = null;
        list.Evicted += id => evicted = id;
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
        list.Add(Id(1));
        list.Add(Id(2));

        var evicted = false;
        list.Evicted += _ => evicted = true;
        list.Add(Id(3));

        Assert.False(evicted);
        Assert.Equal(3, list.Count);
    }

    [Fact]
    public void Add_NoCapacity_UnlimitedAdd()
    {
        var list = new SelectionList();

        for (int i = 0; i < 100; i++)
            list.Add(Id(i));

        Assert.Equal(100, list.Count);
    }

    [Fact]
    public void Remove_ExistingItem_ReturnsTrue()
    {
        var list = new SelectionList();
        var id = Id(1);
        list.Add(id);

        bool removed = list.Remove(id);

        Assert.True(removed);
        Assert.Empty(list.Items);
    }

    [Fact]
    public void Remove_NonExistingItem_ReturnsFalse()
    {
        var list = new SelectionList();

        bool removed = list.Remove(Id(1));

        Assert.False(removed);
    }

    [Fact]
    public void Pop_NonEmpty_ReturnsLastAdded()
    {
        var list = new SelectionList();
        var first = Id(1);
        var second = Id(2);
        list.Add(first);
        list.Add(second);

        var popped = list.Pop();

        Assert.Equal(second, popped);
        Assert.Single(list.Items);
        Assert.Equal(first, list.Items[0]);
    }

    [Fact]
    public void Pop_Empty_ReturnsEmptyGuid()
    {
        var list = new SelectionList();

        var popped = list.Pop();

        Assert.Equal(Guid.Empty, popped);
    }

    [Fact]
    public void TrimExcess_WhenOverCapacity_EvictsFromFront()
    {
        var list = new SelectionList { Capacity = 3 };
        var id1 = Id(1);
        var id2 = Id(2);
        var id3 = Id(3);
        list.Add(id1);
        list.Add(id2);
        list.Add(id3);

        var evicted = new List<Guid>();
        list.Evicted += evicted.Add;
        list.Capacity = 1;

        Assert.Equal(1, list.Count);
        Assert.Equal(2, evicted.Count);
        Assert.Equal(id1, evicted[0]);
        Assert.Equal(id2, evicted[1]);
        Assert.Equal(id3, list.Items[0]);
    }

    [Fact]
    public void TrimExcess_UnderCapacity_Noop()
    {
        var list = new SelectionList { Capacity = 5 };
        list.Add(Id(1));

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
        var id = Id(1);
        list.Add(id);

        Assert.True(list.Contains(id));
    }

    [Fact]
    public void Contains_NonExistingItem_ReturnsFalse()
    {
        var list = new SelectionList();

        Assert.False(list.Contains(Id(1)));
    }

    [Fact]
    public void Capacity_Zero_MeansUnlimited()
    {
        var list = new SelectionList { Capacity = 0 };

        for (int i = 0; i < 10; i++)
            list.Add(Id(i));

        Assert.Equal(10, list.Count);
    }

    [Fact]
    public void Items_MaintainsInsertionOrder()
    {
        var list = new SelectionList();
        var p1 = Id(1);
        var p2 = Id(2);
        var p3 = Id(3);
        list.Add(p1);
        list.Add(p2);
        list.Add(p3);

        Assert.Equal([p1, p2, p3], list.Items);
    }
}
