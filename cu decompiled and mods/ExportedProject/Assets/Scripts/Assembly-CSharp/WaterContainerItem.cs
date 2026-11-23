using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[Saveable]
public class WaterContainerItem : MonoBehaviour
{
	[JsonProperty]
	public List<LiquidStack> stack;

	private Item item;

	public Sprite fillSprite;

	private SpriteRenderer fillRenderer;

	private SpriteRenderer _renderer;

	private Material fillMaterial;

	public bool hasSprite { get; private set; }

	public float Capacity
	{
		get
		{
			if (item.Stats is LiquidItemInfo liquidItemInfo)
			{
				return liquidItemInfo.capacity;
			}
			return 0f;
		}
	}

	public bool AutoFill
	{
		get
		{
			if (item.Stats is LiquidItemInfo liquidItemInfo)
			{
				return liquidItemInfo.autoFill;
			}
			return false;
		}
	}

	public float CurrentTotal => stack.Sum((LiquidStack s) => s.amount);

	public float SpaceLeft => Mathf.Max(0f, Capacity - CurrentTotal);

	public bool HasLiquid(string id)
	{
		foreach (LiquidStack item in stack)
		{
			if (item.liquidId == id)
			{
				return true;
			}
		}
		return false;
	}

	public Color AverageColor()
	{
		if (stack.Count == 0)
		{
			return Color.clear;
		}
		Color color = Color.white;
		for (int i = 0; i < stack.Count; i++)
		{
			if (Liquids.Registry.TryGetValue(stack[i].liquidId, out var value))
			{
				color = ((i != 0) ? Color.Lerp(color, value.color, stack[i].amount / stack[0].amount * 0.5f) : value.color);
			}
		}
		return color;
	}

	public float AmountOf(string liquid)
	{
		if (!HasLiquid(liquid))
		{
			return 0f;
		}
		foreach (LiquidStack item in stack)
		{
			if (item.liquidId == liquid)
			{
				return item.amount;
			}
		}
		return 0f;
	}

	public float AddLiquid(string liquidId, float amount)
	{
		if (string.IsNullOrEmpty(liquidId))
		{
			return 0f;
		}
		float num = Mathf.Min(SpaceLeft, amount);
		if (num <= 0f)
		{
			return 0f;
		}
		LiquidStack liquidStack = stack.FirstOrDefault((LiquidStack s) => s.liquidId == liquidId);
		if (liquidStack != null)
		{
			liquidStack.amount += num;
		}
		else
		{
			stack.Add(new LiquidStack(liquidId, num));
		}
		UpdateCondition();
		return num;
	}

	public List<float> CalculateDrain(float amount)
	{
		float currentTotal = CurrentTotal;
		if (currentTotal <= 0f || amount <= 0f)
		{
			return new List<float>(new float[stack.Count]);
		}
		float num = Mathf.Min(amount, currentTotal);
		List<float> list = new List<float>(new float[stack.Count]);
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = stack[i].amount * (num / currentTotal);
		}
		return list;
	}

	public List<float> CalculateDrainSingleLiquid(string id, float max)
	{
		if (max <= 0f)
		{
			return new List<float>(new float[stack.Count]);
		}
		List<float> list = new List<float>(new float[stack.Count]);
		for (int i = 0; i < list.Count; i++)
		{
			list[i] = ((stack[i].liquidId == id) ? Mathf.Min(stack[i].amount, max) : 0f);
		}
		return list;
	}

	public void Drain(List<float> toRemove)
	{
		if (toRemove.Count == stack.Count)
		{
			for (int i = 0; i < toRemove.Count; i++)
			{
				stack[i].amount -= toRemove[i];
			}
			stack.RemoveAll((LiquidStack s) => s.amount < 0.5f);
			UpdateCondition();
		}
	}

	public void DrainAll()
	{
		stack.Clear();
		UpdateCondition();
	}

	public void Drink(Body body, float amount = 100f, string sound = "drink")
	{
		List<float> list = CalculateDrain(amount);
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (Liquids.Registry.TryGetValue(stack[i].liquidId, out var value))
			{
				value.onDrink(list[i], body);
			}
		}
		Drain(list);
		Sound.Play(sound, body.transform.position);
	}

	public void ApplyToLimb(Limb limb, float amount = 100f)
	{
		List<float> list = CalculateDrain(amount);
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (Liquids.Registry.TryGetValue(stack[i].liquidId, out var value) && value.healthUsable && !limb.prostheticInfo.hasProsthetic)
			{
				value.onHealthUse(list[i], limb);
			}
		}
		Drain(list);
	}

	public void Inject(Limb limb, float amount = 100f)
	{
		List<float> list = CalculateDrain(amount);
		if (list.Count == 0)
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			if (Liquids.Registry.TryGetValue(stack[i].liquidId, out var value))
			{
				if (value.injectionSickness > 0f)
				{
					limb.body.sicknessAmount += value.injectionSickness * 0.35f * list[i];
				}
				limb.body.bloodViscosity -= value.injectionSickness * 0.1f;
				if (value.injectable)
				{
					value.onHealthUse(list[i], limb);
				}
			}
		}
		Drain(list);
	}

	public void UpdateCondition()
	{
		item.condition = CurrentTotal / Capacity;
	}

	private void Awake()
	{
		item = GetComponent<Item>();
		if (stack != null)
		{
			return;
		}
		List<LiquidStack> list = new List<LiquidStack>();
		if (item.Stats is LiquidItemInfo { defaultContents: not null } liquidItemInfo)
		{
			foreach (LiquidStack defaultContent in liquidItemInfo.defaultContents)
			{
				list.Add(new LiquidStack(defaultContent.liquidId, defaultContent.amount));
			}
		}
		stack = list;
	}

	private void Update()
	{
		if (hasSprite)
		{
			fillRenderer.enabled = _renderer.enabled;
		}
		if ((!base.transform.parent || !base.transform.parent.GetComponent<Container>()) && SpaceLeft != 0f && AutoFill)
		{
			Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position);
			if (item.condition < 1f && FluidManager.main.GetLiquid(vector2Int.x, vector2Int.y) > 0 && AddLiquid(FluidManager.WorldFluidToLiquidID[FluidManager.main.GetLiquid(vector2Int.x, vector2Int.y)], 200f) > 0f)
			{
				FluidManager.main.SetLiquid(vector2Int.x, vector2Int.y, 0);
			}
		}
	}

	private void OnWillRenderObject()
	{
		if (hasSprite)
		{
			fillRenderer.sortingOrder = _renderer.sortingOrder - 1;
			fillRenderer.color = AverageColor();
			fillRenderer.sharedMaterial.SetFloat("_FillAmount", CurrentTotal / Capacity);
		}
	}

	private void OnDestroy()
	{
		Object.Destroy(fillMaterial);
	}

	private void Start()
	{
		UpdateCondition();
		hasSprite = fillSprite;
		_renderer = GetComponent<SpriteRenderer>();
		if (hasSprite)
		{
			GameObject gameObject = new GameObject("LiquidFill", typeof(SpriteRenderer));
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.GetComponent<SpriteRenderer>().sprite = fillSprite;
			fillRenderer = gameObject.GetComponent<SpriteRenderer>();
			fillRenderer.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
			fillMaterial = new Material(Resources.Load<Material>("Special/LiquidFillMaterial"));
			fillRenderer.sharedMaterial = fillMaterial;
		}
	}
}
