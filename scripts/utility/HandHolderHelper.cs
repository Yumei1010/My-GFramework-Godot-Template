using Godot;

namespace TimeToTwentyfour.scripts.utility;

/// <summary>
/// 手牌排列坐标辅助类
/// </summary>
public static class HandHolderHelper
{
    private const float CenterX = 432f;
    private const float YEdge = 408f;
    private const float YMid = 392f;
    private const float HorizontalSpacing = 48f;
    private const float MinAngleDeg = 5f;
    private const float MaxAngleDeg = 25f;

    /// <summary>
    /// 获取指定手牌数量的第index张牌的位置
    /// </summary>
    /// <param name="handSize">手牌数量 <see cref="int"/></param>
    /// <param name="index">牌的位置 <see cref="int"/></param>
    public static Vector2 GetPosition(int handSize, int index)
    {
        if (handSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(handSize), "手牌数量必须大于0");
        if (index < 0 || index >= handSize)
            throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围");
        if (handSize == 1)
            return new Vector2(CenterX, YMid);
        float maxOffset = HorizontalSpacing * (handSize - 1);
        float dy = YEdge - YMid;
        float halfAngleRad = Mathf.Atan2(dy, maxOffset);
        float maxAngleRad = 2 * halfAngleRad;
        float minAngleRad = MinAngleDeg * Mathf.Pi / 180f;
        float maxAngleLimit = MaxAngleDeg * Mathf.Pi / 180f;
        maxAngleRad = Mathf.Clamp(maxAngleRad, minAngleRad, maxAngleLimit);
        float radius = maxOffset / Mathf.Sin(maxAngleRad);
        float centerY = YMid + radius;
        float t = (float)index / (handSize - 1);
        float angleRad = -maxAngleRad + 2 * maxAngleRad * t;
        float x = CenterX + radius * Mathf.Sin(angleRad);
        float y = centerY - radius * Mathf.Cos(angleRad);
        return new Vector2(x, y); 
    }

    /// <summary>
    /// 获取指定手牌数量的第index张牌的旋转角度（度数）
    /// </summary>
    /// <param name="handSize">手牌数量 <see cref="int"/></param>
    /// <param name="index">牌的位置 <see cref="int"/></param>
    public static float GetAngle(int handSize, int index)
    {
        if (handSize <= 0)
            throw new ArgumentOutOfRangeException(nameof(handSize), "手牌数量必须大于0");
        if (index < 0 || index >= handSize)
            throw new ArgumentOutOfRangeException(nameof(index), "索引超出范围");
        if (handSize == 1)
            return 0f;
        float maxOffset = HorizontalSpacing * (handSize - 1);
        float dy = YEdge - YMid;
        float halfAngleRad = Mathf.Atan2(dy, maxOffset);
        float maxAngleRad = 2 * halfAngleRad;
        float minAngleRad = MinAngleDeg * Mathf.Pi / 180f;
        float maxAngleLimit = MaxAngleDeg * Mathf.Pi / 180f;
        maxAngleRad = Mathf.Clamp(maxAngleRad, minAngleRad, maxAngleLimit);
        float t = (float)index / (handSize - 1);
        float angleRad = -maxAngleRad + 2 * maxAngleRad * t;
        return angleRad * 180f / Mathf.Pi;
    }
}