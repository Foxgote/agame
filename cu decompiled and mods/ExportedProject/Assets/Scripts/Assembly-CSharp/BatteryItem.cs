using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class BatteryItem : MonoBehaviour
{
	public enum BatteryPreset
	{
		Small = 0,
		Medium = 1,
		Large = 2
	}

	[JsonProperty]
	[HideInInspector]
	public float maxCharge;

	[JsonProperty]
	[HideInInspector]
	public string batteryType;

	[JsonProperty]
	public bool batteryWasFavourited;

	private Item myItem;

	[HideInInspector]
	public float maxAllowedCharge;

	public BatteryPreset preset;

	public bool notSpawnWithBattery;

	public bool hasCharge => myItem.condition > 0f;

	public bool hasBattery => !string.IsNullOrEmpty(batteryType);

	private void Awake()
	{
		myItem = GetComponent<Item>();
		switch (preset)
		{
		case BatteryPreset.Small:
			maxAllowedCharge = 50f;
			if (!notSpawnWithBattery)
			{
				batteryType = "smallbattery";
				maxCharge = 50f;
			}
			break;
		case BatteryPreset.Medium:
			maxAllowedCharge = 100f;
			if (!notSpawnWithBattery)
			{
				batteryType = "mediumbattery";
				maxCharge = 100f;
			}
			break;
		case BatteryPreset.Large:
			maxAllowedCharge = 300f;
			if (!notSpawnWithBattery)
			{
				batteryType = "largebattery";
				maxCharge = 300f;
			}
			break;
		}
	}

	public string GetBatteryInfo()
	{
		return "<color=#5eff00>" + (hasBattery ? ("<sprite index=15 tint=1>" + Locale.GetOther("hasbattery").Replace("<>", Item.GetItem(batteryType).fullName)) : ("<sprite index=16 tint=1>" + Locale.GetOther("nobattery").Replace("<>", Locale.GetOther(preset.ToString().ToLower()))));
	}

	public void Update()
	{
		if (!hasBattery)
		{
			myItem.condition = 0f;
			maxCharge = 0f;
		}
	}

	public void LoadBattery(Item battery)
	{
		if (!hasBattery && !(battery.Stats.GetType() != typeof(BatteryInfo)))
		{
			BatteryInfo batteryInfo = (BatteryInfo)battery.Stats;
			if (!(batteryInfo.maxCharge > maxAllowedCharge))
			{
				Sound.Play("batteryinsert", base.transform.position);
				batteryType = battery.id;
				maxCharge = batteryInfo.maxCharge;
				myItem.condition = battery.condition;
				batteryWasFavourited = battery.favourited;
				Object.Destroy(battery.gameObject);
			}
		}
	}

	public void UnloadBattery(bool skipSpawn = false)
	{
		if (hasBattery)
		{
			if (!skipSpawn)
			{
				Sound.Play("batteryinsert", base.transform.position);
				Item component = (Object.Instantiate(Resources.Load(batteryType), base.transform.position, Quaternion.identity) as GameObject).GetComponent<Item>();
				component.condition = myItem.condition;
				component.favourited = batteryWasFavourited;
				PlayerCamera.main.body.AutoPickUpItem(component);
			}
			batteryWasFavourited = false;
			myItem.condition = 0f;
			maxCharge = 0f;
			batteryType = "";
		}
	}

	public void DrainCharge(float change)
	{
		if (hasBattery)
		{
			myItem.condition -= change / (maxCharge * 0.01f);
		}
	}

	public float GetCharge()
	{
		if (!hasBattery)
		{
			return 0f;
		}
		return myItem.condition * maxCharge;
	}
}
