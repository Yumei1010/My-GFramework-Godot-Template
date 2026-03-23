using Godot;

namespace GFrameworkGodotTemplate.scripts.utility;

public static class HandHolderHelper
{
	private static readonly Vector2[][] PositionData = new Vector2[10][]
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
		],
		[
			new Vector2(-340f, 10f),
			new Vector2(-170f, -30f),
			new Vector2(0f, -50f),
			new Vector2(170f, -30f),
			new Vector2(340f, 10f)
		],
		[
			new Vector2(-460f, 13f),
			new Vector2(-273f, -25f),
			new Vector2(-90f, -50f),
			new Vector2(90f, -50f),
			new Vector2(273f, -25f),
			new Vector2(460f, 13f)
		],
		[
			new Vector2(-534f, 18f),
			new Vector2(-365f, -14f),
			new Vector2(-189f, -39f),
			new Vector2(0f, -50f),
			new Vector2(189f, -39f),
			new Vector2(365f, -14f),
			new Vector2(534f, 18f)
		],
		[
			new Vector2(-565f, 28f),
			new Vector2(-400f, -14f),
			new Vector2(-231f, -39f),
			new Vector2(-80f, -50f),
			new Vector2(80f, -50f),
			new Vector2(231f, -39f),
			new Vector2(400f, -14f),
			new Vector2(565f, 28f)
		],
		[
			new Vector2(-600f, 37f),
			new Vector2(-445f, -2f),
			new Vector2(-300f, -29f),
			new Vector2(-150f, -45f),
			new Vector2(0f, -50f),
			new Vector2(150f, -45f),
			new Vector2(300f, -29f),
			new Vector2(445f, -2f),
			new Vector2(600f, 37f)
		],
		[
			new Vector2(-610f, 38f),
			new Vector2(-472f, 5f),
			new Vector2(-340f, -21f),
			new Vector2(-200f, -41f),
			new Vector2(-64f, -50f),
			new Vector2(64f, -50f),
			new Vector2(200f, -41f),
			new Vector2(340f, -21f),
			new Vector2(472f, 5f),
			new Vector2(610f, 38f)
		]
	};

	private static readonly float[][] AngleData = new float[10][]
	{
		[1],
		[-2f, 2f],
		[-3f, 0f, 3f],
		[-8f, -4f, 4f, 8f],
		[-8f, -4f, 0f, 4f, 8f],
		[-9f, -6f, -3f, 3f, 6f, 9f],
		[-9f, -6f, -3f, 0f, 3f, 6f, 9f],
		[-12f, -9f, -6f, -3f, 3f, 6f, 9f, 12f],
		[-12f, -9f, -6f, -3f, 0f, 3f, 6f, 9f, 12f],
		[-15f, -12f, -9f, -6f, -3f, 3f, 6f, 9f, 12f, 15f]
	};
	
	private static readonly Vector2 BaseScale = Vector2.One * 0.8f;

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

	public static Vector2 GetScale(int handSize)
	{
		return BaseScale * handSize switch
		{
			8 => 0.95f, 
			9 => 0.9f, 
			10 => 0.85f, 
			11 => 0.8f, 
			12 => 0.75f, 
			_ => 1f, 
		};
	}
}