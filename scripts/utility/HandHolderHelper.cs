using Godot;

namespace GFrameworkGodotTemplate.scripts.utility;

public static class HandHolderHelper
{
	private static readonly Vector2[][] PositionData = new Vector2[][]
	{
		[
			new Vector2(432f, 392f)
		],
		[
			new Vector2(368f, 408f),
			new Vector2(496f, 408f)
		],
		[
			new Vector2(320f, 408f),
			new Vector2(432f, 392f),
			new Vector2(544f, 408f)
		],
		[
			new Vector2(272f, 408f),
			new Vector2(376f, 392f),
			new Vector2(488f, 392f),
			new Vector2(592f, 408f)
		]
	};

	private static readonly float[][] AngleData = new float[][]
	{
		[0],
		[-4f, 4f],
		[-6f, 0f, 6f],
		[-8f, -4f, 4f, 8f],
	};

	public static Vector2 GetPosition(int handSize, int index)
	{
		if (handSize - 1 >= PositionData.Length)
		{
			throw new ArgumentOutOfRangeException($"Hand size {handSize} is greater than {PositionData.Length + 1}!");
		}
		if (index >= PositionData[handSize - 1].Length)
		{
			throw new ArgumentOutOfRangeException($"Card index {index} is greater than {PositionData[handSize - 1].Length}!");
		}
		return PositionData[handSize - 1][index];
	}

	public static float GetAngle(int handSize, int index)
	{
		if (handSize - 1 >= PositionData.Length)
		{
			throw new ArgumentOutOfRangeException($"Hand size {handSize} is greater than {PositionData.Length + 1}!");
		}
		if (index >= PositionData[handSize - 1].Length)
		{
			throw new ArgumentOutOfRangeException($"Card index {index} is greater than {PositionData[handSize - 1].Length}!");
		}
		return AngleData[handSize - 1][index];
	}
}