using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemInfo
{
	public delegate void Use(Body body, Item item);

	public delegate void UseLimb(Limb limb, Item item);

	[Flags]
	public enum DecayType : byte
	{
		NoDecayWithoutContainerItem = 1,
		NoDecayWhenNotWorn = 2,
		NoDecayWhenStill = 4,
		BatteryDecay = 0x10
	}

	public string fullName;

	public string description;

	public string category;

	public float slotRotation;

	public bool usable;

	public bool usableOnLimb;

	public float rotSpeed;

	public Use useAction;

	public UseLimb useLimbAction;

	public bool destroyAtZeroCondition;

	public float weight;

	public bool scaleWeightWithCondition;

	public bool onlyHoldInHands;

	public bool autoAttack;

	public bool usableWithLMB;

	public bool wearable;

	public bool wearableCanBeHeld;

	public string desiredWearLimb;

	public string wearSlotId;

	public float wearableArmor;

	public float wearableIsolation;

	public float wearableHitDurabilityLossMultiplier;

	public float jumpHeightMultChange;

	public bool combineable;

	public bool ignoreDepression;

	public int value;

	public bool untradeable;

	public bool secret;

	public int wearableVisualOffset = 5;

	public string tags = "";

	private string[] actualTags;

	public byte decayInfo;

	public float decayMinutes;

	public Recognition rec = new Recognition(2);

	public List<CraftingQuality> qualities = new List<CraftingQuality>();

	public bool ActuallyUsableOnLimb(Item item)
	{
		if (!item.TryGetComponent<WaterContainerItem>(out var component))
		{
			return usableOnLimb;
		}
		foreach (LiquidStack item2 in component.stack)
		{
			if (Liquids.Registry.TryGetValue(item2.liquidId, out var liquidType) && liquidType.healthUsable)
			{
				return true;
			}
		}
		return usableOnLimb;
	}

	public int GetValue(Item item)
	{
		float num = (float)value * item.condition;
		if (item.TryGetComponent<AmmoScript>(out var component) && component.itemType == AmmoScript.AmmoItemType.Magazine)
		{
			num *= (float)component.rounds / (float)component.maxRounds;
		}
		float num2 = 0f;
		if (item.TryGetComponent<WaterContainerItem>(out var component2))
		{
			foreach (LiquidStack item2 in component2.stack)
			{
				num2 += item2.amount * 0.001f * Liquids.Registry[item2.liquidId].valuePerLiter;
			}
		}
		num2 = Mathf.Min(num2, 50f);
		num += num2;
		return Mathf.RoundToInt(num);
	}

	public int DefaultValue()
	{
		float num = value;
		float num2 = 0f;
		if (this is LiquidItemInfo { defaultContents: not null } liquidItemInfo)
		{
			foreach (LiquidStack defaultContent in liquidItemInfo.defaultContents)
			{
				num2 += Mathf.Min(defaultContent.amount * 0.001f, 1.5f) * Liquids.Registry[defaultContent.liquidId].valuePerLiter;
			}
		}
		num2 = Mathf.Min(num2, 50f);
		num += num2;
		return Mathf.RoundToInt(num);
	}

	public void SetTags()
	{
		actualTags = tags.Split(',');
	}

	public string[] GetTags()
	{
		return actualTags;
	}

	public bool HasTag(string tag)
	{
		return actualTags.Contains(tag);
	}
}
