using GFramework.Core.Abstractions.model;
using GFrameworkGodotTemplate.scripts.enums.poker;
using Godot;

namespace GFrameworkGodotTemplate.scripts.model;

public interface IPokerModel : IModel
{
    //卡牌属性
    SuitType SuitType { get; set; } //花色类型
    NumType NumType { get; set; }//数值类型
    String Value { get; set; }//数值
    
    Vector2 SpawnPosition { get; set; }//重置坐标
    bool IsMoving { get; set; }//是否移动
    
    void Increment(String targetValue);
    
    void Decrement(String targetValue);

    void Transforment(SuitType targetSuitType);

    void Movement();
}