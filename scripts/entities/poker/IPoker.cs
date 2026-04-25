using TimeToTwentyfour.scripts.enums.poker;
using Godot;

namespace TimeToTwentyfour.scripts.entities.poker;

/// <summary>
///     扑克接口，定义了扑克对象的基本属性和必须实现的功能。
/// </summary>
public interface IPoker
{
    /// <summary>
    ///     获取扑克的唯一标识符。
    /// </summary>
    /// <returns>扑克的 <see cref="Guid"/> 唯一标识符</returns>
    Guid GetId();

    /// <summary>
    ///     获取扑克的花色类型。
    /// </summary>
    /// <returns>当前扑克的花色 <see cref="SuitType"/></returns>
    SuitType GetSuitType();
    
    /// <summary>
    ///     获取扑克的点数数值
    /// </summary>
    /// <returns>点数的字符串值 <see cref="string"/></returns>
    string GetNumValue();
    
    /// <summary>
    ///     获取扑克的点数类型
    /// </summary>
    /// <returns>点数类型 <see cref="NumType"/> </returns>
    NumType GetNumType();
    
    /// <summary>
    ///     设置扑克的花色类型。
    /// </summary>
    /// <param name="suitType">花色类型 <see cref="SuitType"/></param>
    void SetSuitType(SuitType suitType);
    
    /// <summary>
    ///     设置扑克的点数数值。
    /// </summary>
    /// <param name="numValue">点数 <see cref="string"/></param>
    void SetNumValue(string numValue);
    
    /// <summary>
    ///     设置扑克的点数类型。
    /// </summary>
    /// <param name="numType">要设置的点数类型 <see cref="NumType"/></param>
    void SetNumType(NumType numType);
    
    /// <summary>
    ///     设置扑克的位置
    /// </summary>
    /// <param name="pos">要设置的位置向量 <see cref="Vector2"/></param>
    void SetGlobalPosition(Vector2 pos);
    
    /// <summary>
    ///     设置扑克的旋转角度
    /// </summary>
    /// <param name="angle">要设置的旋转角度 <see cref="float"/></param>
    void SetRotation(float angle);
    
    /// <summary>
    ///     设置扑克的默认旋转角度
    /// </summary>
    /// <param name="angle">要设置的旋转角度 <see cref="float"/></param>
    void SetDefaultRotation(float angle);
    
    /// <summary>
    ///     设置扑克的默认位置
    /// </summary>
    /// <param name="position">要设置的旋转角度 <see cref="Vector2"/></param>
    void SetDefaultPosition(Vector2 position);
    
    /// <summary>
    ///     获取扑克的全局位置
    /// </summary>
    /// <returns>全局位置 <see cref="Vector2"/></returns>
    Vector2 GetGlobalPosition();
    
    /// <summary>
    ///     获取鼠标的全局位置
    /// </summary>
    /// <returns>鼠标的全局位置 <see cref="Vector2"/></returns>
    Vector2 GetGlobalMousePosition();
    
    /// <summary>
    ///     获取扑克的当前尺寸
    /// </summary>
    /// <returns>当前尺寸 <see cref="Vector2"/></returns>
    Vector2 GetSize();
    
    /// <summary>
    ///     获取扑克的当前旋转角度
    /// </summary>
    /// <returns>当前旋转角度 <see cref="float"/></returns>
    float GetRotation();
    
    /// <summary>
    ///     获取扑克记录的默认位置
    /// </summary>
    /// <returns>默认位置 <see cref="Vector2"/></returns>
    Vector2 GetDefaultPosition();
    
    /// <summary>
    ///     重置扑克的位置到默认位置
    /// </summary>
    void ResetPos();

    /// <summary>
    ///     重置扑克的旋转角度为 0
    /// </summary>
    void ResetRot();
    
    /// <summary>
    ///     异步重置扑克的位置和旋转角度到默认值
    /// </summary>
    void ResetPosAndRot();

    /// <summary>
    ///     变更扑克到指定状态
    /// </summary>
    /// <param name="state">目标状态 <see cref="StateType"/></param>
    void ChangeTo(StateType state);

    /// <summary>
    ///     放置扑克到指定位置
    /// </summary>
    /// <param name="position">目标位置 <see cref="Vector2"/></param>
    void SpawnTo(Vector2 position);
    
    /// <summary>
    ///     移动扑克到指定位置
    /// </summary>
    /// <param name="position">目标位置 <see cref="Vector2"/></param>
    void MoveTo(Vector2 position);
}