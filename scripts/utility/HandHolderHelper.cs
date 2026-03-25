using Godot;

namespace GFrameworkGodotTemplate.scripts.utility;

public static class HandHolderHelper
{
	private static readonly Vector2[][] PositionData = new Vector2[4][]
	{
		[
			new Vector2(0f, -50f)
		],
		[
			new Vector2(-100f, -50f),
			new Vector2(100f, -50f)
		],
		[
			new Vector2(-180f, -50f),
			new Vector2(0f, -59f),
			new Vector2(180f, -50f)
		],
		[
			new Vector2(-240f, -25f),
			new Vector2(-80f, -50f),
			new Vector2(80f, -50f),
			new Vector2(240f, -25f)
		]
	};

	private static readonly float[][] AngleData = new float[4][]
	{
		[1],
		[-2f, 2f],
		[-3f, 0f, 3f],
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