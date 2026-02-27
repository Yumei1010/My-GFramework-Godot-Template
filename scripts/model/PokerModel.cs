using GFramework.Core.extensions;
using GFramework.Core.model;
using GFrameworkGodotTemplate.scripts.enums.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.model;

public class PokerModel : AbstractModel , IPokerModel
{
    //卡牌属性
    public SuitType SuitType { get; set; }//花色类型
    public NumType NumType { get; set; }//数值类型
    public String Value { get; set; }//数值
    
    public Vector2 SpawnPosition { get; set; }//重置坐标
    public bool IsMoving { get; set; }//是否移动

    protected override void OnInit()
    {
        // SuitType = SuitType.Heart;
        // NumType = NumType.Integer;
        // Value = "24";
        //
        // SpawnPosition = new Vector2(480,270);
        // IsMoving = false;
    }
    
    public void Increment(string targetValue)
    {
        Value = targetValue;
        this.SendEvent(new ChangedValueEvent { Value = targetValue });
    }

    public void Decrement(string targetValue)
    {
        Value = targetValue;
        this.SendEvent(new ChangedValueEvent { Value = targetValue });
    }

    public void Transforment(SuitType targetSuitType)
    {
        SuitType = targetSuitType;
    }

    public void Movement()
    {
        this.SendEvent(new ChangedPositionEvent {});
    }

    public sealed record ChangedValueEvent
    {
        public String Value { get; init; }
    }
    
    public sealed record ChangedPositionEvent;
}