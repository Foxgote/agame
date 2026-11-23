using System;
using System.Collections.Generic;
using UnityEngine;

public class FluidManager : MonoBehaviour
{
	public static FluidManager main;

	public byte[,] fluid;

	private WorldGeneration world;

	private float checkTime;

	private int simIndex;

	public List<GameObject> LiquidParticlePrefabs;

	private List<ParticleSystem> liquidParticles;

	private byte tileCooldown;

	private int waterMoveCount;

	private static int floodFillCount;

	private static List<Vector2Int> fillPos;

	public Color[] liquidColors;

	public Material waterMat;

	public bool liquidPushing;

	public static Dictionary<int, string> WorldFluidToLiquidID = new Dictionary<int, string>
	{
		[0] = "water",
		[1] = "groundwater",
		[2] = "lumalgae",
		[3] = "oil",
		[4] = "sap",
		[5] = "dirtywater",
		[6] = "",
		[7] = "redsludge"
	};

	private void Awake()
	{
		main = this;
	}

	private void Start()
	{
		liquidParticles = new List<ParticleSystem>();
		foreach (GameObject liquidParticlePrefab in LiquidParticlePrefabs)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(liquidParticlePrefab, base.transform);
			liquidParticles.Add(gameObject.GetComponent<ParticleSystem>());
		}
	}

	public (RangeI, RangeI) SimulationRange()
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(PlayerCamera.main.transform.position);
		RangeI item = new RangeI(vector2Int.x - 64, vector2Int.x + 64);
		RangeI item2 = new RangeI(vector2Int.y - 64, vector2Int.y + 64);
		if (item.min < 1)
		{
			item.min = 1;
		}
		if (item2.min < 1)
		{
			item2.min = 1;
		}
		if (item.max < 1)
		{
			item.max = 1;
		}
		if (item2.max < 1)
		{
			item2.max = 1;
		}
		if (item.max > WorldGeneration.world.width - 2)
		{
			item.max = (int)(WorldGeneration.world.width - 2);
		}
		if (item2.max > WorldGeneration.world.height - 2)
		{
			item2.max = (int)(WorldGeneration.world.height - 2);
		}
		if (item.min > WorldGeneration.world.width - 2)
		{
			item.min = (int)(WorldGeneration.world.width - 2);
		}
		if (item2.min > WorldGeneration.world.height - 2)
		{
			item2.min = (int)(WorldGeneration.world.height - 2);
		}
		return (item, item2);
	}

	public (RangeI, RangeI) SimulationRangeIndex()
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(PlayerCamera.main.transform.position);
		RangeI item = new RangeI(vector2Int.x - 64, vector2Int.x + 64);
		RangeI item2 = new RangeI(vector2Int.y - 64 + simIndex, vector2Int.y - 48 + simIndex);
		if (item.min < 1)
		{
			item.min = 1;
		}
		if (item2.min < 1)
		{
			item2.min = 1;
		}
		if (item.max < 1)
		{
			item.max = 1;
		}
		if (item2.max < 1)
		{
			item2.max = 1;
		}
		if (item.max > WorldGeneration.world.width - 2)
		{
			item.max = (int)(WorldGeneration.world.width - 2);
		}
		if (item2.max > WorldGeneration.world.height - 2)
		{
			item2.max = (int)(WorldGeneration.world.height - 2);
		}
		if (item.min > WorldGeneration.world.width - 2)
		{
			item.min = (int)(WorldGeneration.world.width - 2);
		}
		if (item2.min > WorldGeneration.world.height - 2)
		{
			item2.min = (int)(WorldGeneration.world.height - 2);
		}
		simIndex += 16;
		if (simIndex >= 112)
		{
			simIndex = 0;
		}
		return (item, item2);
	}

	private void Update()
	{
		Vector2 vector = PlayerCamera.main.GetComponent<Camera>().WorldToViewportPoint(PlayerCamera.main.body.transform.position + Vector3.down * 2.5f);
		waterMat.SetFloat("_ReflecHeight", vector.y);
		if (fluid.Length >= 64 && Time.unscaledTime - checkTime > 0.1f)
		{
			checkTime = Time.unscaledTime;
			RenderFluids();
		}
	}

	public void StartFill(Vector2Int pos, byte fill, int max)
	{
		floodFillCount = 0;
		fillPos = new List<Vector2Int>();
		FloodFillDown(pos.x, pos.y, max);
		if (floodFillCount <= max)
		{
			foreach (Vector2Int fillPo in fillPos)
			{
				fluid[fillPo.x, fillPo.y] = fill;
			}
		}
		fillPos.Clear();
	}

	private void FloodFillDown(int x, int y, int maxCount)
	{
		if (x >= 1 && x < WorldGeneration.world.width && y >= 1 && y < WorldGeneration.world.height && WorldGeneration.world.GetBlock(new Vector2Int(x, y)) == 0 && !HasLiquid(new Vector2Int(x, y)) && !fillPos.Contains(new Vector2Int(x, y)))
		{
			floodFillCount++;
			if (floodFillCount <= maxCount)
			{
				fillPos.Add(new Vector2Int(x, y));
				FloodFillDown(x + 1, y, maxCount);
				FloodFillDown(x - 1, y, maxCount);
				FloodFillDown(x, y - 1, maxCount);
			}
		}
	}

	private void FixedUpdate()
	{
		SimulationStep();
	}

	public bool Empty(Vector2Int pos)
	{
		if (fluid[pos.x, pos.y] == 0)
		{
			return WorldGeneration.world.GetBlock(new Vector2Int(pos.x, pos.y)) == 0;
		}
		return false;
	}

	public bool Empty(int x, int y)
	{
		return Empty(new Vector2Int(x, y));
	}

	public void Swap(Vector2Int one, Vector2Int two)
	{
		ref byte reference = ref fluid[one.x, one.y];
		ref byte reference2 = ref fluid[two.x, two.y];
		byte b = fluid[two.x, two.y];
		byte b2 = fluid[one.x, one.y];
		reference = b;
		reference2 = b2;
	}

	public void RenderFluids()
	{
		(RangeI, RangeI) tuple = SimulationRange();
		List<ParticleSystem.Particle>[] array = new List<ParticleSystem.Particle>[LiquidParticlePrefabs.Count];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = new List<ParticleSystem.Particle>();
		}
		for (int j = tuple.Item1.min; j < tuple.Item1.max; j++)
		{
			for (int k = tuple.Item2.min; k < tuple.Item2.max; k++)
			{
				if (fluid[j, k] != 0)
				{
					bool flag = fluid[j, k + 1] == 0 && (fluid[j + 1, k] == 0 || fluid[j - 1, k] == 0);
					array[fluid[j, k] - 1].Add(new ParticleSystem.Particle
					{
						position = WorldGeneration.world.BlockToWorldPos(new Vector2Int(j, k)) + (flag ? new Vector2(0f, -0.3125f) : Vector2.zero),
						startLifetime = 999f,
						remainingLifetime = 999f,
						startColor = Color.white,
						startSize3D = new Vector2(1.25f, flag ? 0.625f : 1.25f)
					});
				}
			}
		}
		for (int l = 0; l < array.Length; l++)
		{
			liquidParticles[l].SetParticles(array[l].ToArray());
		}
	}

	public void IncrMove(Vector2 direction, Vector2Int pos)
	{
		if (liquidPushing)
		{
			waterMoveCount++;
			if (waterMoveCount > 10)
			{
				waterMoveCount = 0;
				GameObject obj = new GameObject("WaterPush", typeof(CircleCollider2D), typeof(WaterPusher));
				obj.transform.position = WorldGeneration.world.BlockToWorldPos(pos);
				CircleCollider2D component = obj.GetComponent<CircleCollider2D>();
				component.isTrigger = true;
				component.radius = 1.5f;
				obj.GetComponent<WaterPusher>().direction = direction;
				UnityEngine.Object.Destroy(obj, 0.75f);
			}
		}
	}

	public (float buoyancy, float drag, int type) WaterInfo(Vector2Int pos)
	{
		return GetLiquid(pos.x, pos.y) switch
		{
			1 => (buoyancy: 0.6f, drag: 0.915f, type: 1), 
			2 => (buoyancy: 2f, drag: 0.8f, type: 2), 
			3 => (buoyancy: 0.5f, drag: 0.75f, type: 3), 
			4 => (buoyancy: 1f, drag: 0.8f, type: 4), 
			5 => (buoyancy: 0.6f, drag: 0.915f, type: 5), 
			6 => (buoyancy: 3f, drag: 0.7f, type: 6), 
			7 => (buoyancy: 1.5f, drag: 0.65f, type: 7), 
			_ => (buoyancy: 0f, drag: 0f, type: 0), 
		};
	}

	public Color LiquidColor(Vector2Int pos)
	{
		return liquidColors[GetLiquid(pos.x, pos.y)];
	}

	public void DrinkLiquid(Vector2Int pos, Body body)
	{
		bool flag = body.HoldingItem(body.handSlot) && body.GetItem(body.handSlot).id == "filterstraw";
		switch (GetLiquid(pos.x, pos.y))
		{
		case 1:
			fluid[pos.x, pos.y] = 0;
			Liquids.Registry["groundwater"].onDrink(200f, body);
			break;
		case 2:
			fluid[pos.x, pos.y] = 0;
			Liquids.Registry["lumalgae"].onDrink(200f, body);
			break;
		case 3:
			if (!flag)
			{
				Liquids.Registry["oil"].onDrink(200f, body);
			}
			break;
		case 4:
			fluid[pos.x, pos.y] = 0;
			Liquids.Registry["sap"].onDrink(200f, body);
			break;
		case 5:
			fluid[pos.x, pos.y] = 0;
			body.Drink(12f);
			body.temperature -= 0.2f;
			Sound.Play("drink", body.transform.position);
			if (!flag)
			{
				body.sicknessAmount += UnityEngine.Random.Range(12f, 16f);
				body.talker.EatMediocre();
				body.happiness--;
			}
			break;
		case 6:
			body.Scream();
			break;
		case 7:
			fluid[pos.x, pos.y] = 0;
			Liquids.Registry["redsludge"].onDrink(200f, body);
			break;
		}
	}

	public bool HasLiquid(Vector2Int pos)
	{
		return GetLiquid(pos.x, pos.y) > 0;
	}

	public bool HasWetLiquid(Vector2Int pos)
	{
		if (GetLiquid(pos.x, pos.y) > 0)
		{
			return GetLiquid(pos.x, pos.y) != 6;
		}
		return false;
	}

	public byte GetLiquid(int x, int y)
	{
		return fluid[(int)checked((nint)Math.Clamp(x, 0L, unchecked(WorldGeneration.world.width - 1))), (int)checked((nint)Math.Clamp(y, 0L, unchecked(WorldGeneration.world.height - 1)))];
	}

	public byte GetLiquid(Vector2 ps)
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(ps);
		return fluid[(int)checked((nint)Math.Clamp(vector2Int.x, 0L, unchecked(WorldGeneration.world.width - 1))), (int)checked((nint)Math.Clamp(vector2Int.y, 0L, unchecked(WorldGeneration.world.height - 1)))];
	}

	public void SetLiquid(int x, int y, byte type)
	{
		fluid[(int)checked((nint)Math.Clamp(x, 0L, unchecked(WorldGeneration.world.width - 1))), (int)checked((nint)Math.Clamp(y, 0L, unchecked(WorldGeneration.world.height - 1)))] = type;
	}

	public void SetLiquid(Vector2 ps, byte type)
	{
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(ps);
		fluid[(int)checked((nint)Math.Clamp(vector2Int.x, 0L, unchecked(WorldGeneration.world.width - 1))), (int)checked((nint)Math.Clamp(vector2Int.y, 0L, unchecked(WorldGeneration.world.height - 1)))] = type;
	}

	public (string, string) LiquidName(Vector2Int pos)
	{
		return GetLiquid(pos.x, pos.y) switch
		{
			1 => (Locale.GetOther("water"), Locale.GetOther("waterdsc")), 
			2 => (Locale.GetOther("lumalgae"), Locale.GetOther("lumalgaedsc")), 
			3 => (Locale.GetOther("oil"), Locale.GetOther("oildsc")), 
			4 => (Locale.GetOther("sap"), Locale.GetOther("sapdsc")), 
			5 => (Locale.GetOther("dirtywater"), Locale.GetOther("dirtywaterdsc")), 
			6 => (Locale.GetOther("magma"), Locale.GetOther("magmadsc")), 
			7 => (Locale.GetOther("redsludge"), Locale.GetOther("redsludgedsc")), 
			_ => (null, null), 
		};
	}

	public void SimulationStep()
	{
		(RangeI, RangeI) tuple = SimulationRangeIndex();
		for (int i = tuple.Item1.min; i < tuple.Item1.max; i++)
		{
			for (int j = tuple.Item2.min; j < tuple.Item2.max; j++)
			{
				if (fluid[i, j] == 0)
				{
					continue;
				}
				if (Empty(i, j - 1))
				{
					if (j <= 2)
					{
						fluid[i, j] = 0;
					}
					Swap(new Vector2Int(i, j), new Vector2Int(i, j - 1));
					tileCooldown++;
					IncrMove(Vector2.down, new Vector2Int(i, j));
					if (tileCooldown > 16)
					{
						tileCooldown = 0;
						if (!(Time.timeScale > 1f))
						{
							Sound.Play("waterflow" + UnityEngine.Random.Range(1, 4), WorldGeneration.world.BlockToWorldPos(new Vector2Int(i, j)));
						}
					}
					continue;
				}
				if ((fluid[i, j] == 2 && fluid[i, j - 1] == 1) || (fluid[i, j] == 1 && fluid[i, j - 1] == 2))
				{
					fluid[i, j - 1] = 2;
					fluid[i, j] = 2;
				}
				bool flag = Empty(i + 1, j);
				bool flag2 = Empty(i - 1, j);
				if (flag & flag2)
				{
					Swap(new Vector2Int(i, j), new Vector2Int(i + ((UnityEngine.Random.value > 0.5f) ? 1 : (-1)), j));
				}
				else if (flag)
				{
					Swap(new Vector2Int(i, j), new Vector2Int(i + 1, j));
					IncrMove(Vector2.right, new Vector2Int(i, j));
				}
				else if (flag2)
				{
					Swap(new Vector2Int(i, j), new Vector2Int(i - 1, j));
					IncrMove(Vector2.left, new Vector2Int(i, j));
				}
			}
		}
	}

	public void Clear()
	{
		Array.Clear(fluid, 0, fluid.Length);
	}
}
