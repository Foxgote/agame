using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CustomItemBehaviour : MonoBehaviour
{
	private Item item;

	[HideInInspector]
	public int state;

	[HideInInspector]
	public object[] data;

	private void Start()
	{
		item = GetComponent<Item>();
		if (item.id == "liquidcentrifuge")
		{
			data = new object[1];
			data[0] = 0f;
		}
	}

	public void Update()
	{
		InventorySlot component = null;
		Limb component2 = null;
		switch (item.id)
		{
		case "blindfold":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<Limb>(out component2))
			{
				component2.body.eyeCloseTime = 1f;
			}
			break;
		case "scubadivinggear":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<Limb>(out component2) && !component2.body.bodyAffect.wasWater && component2.body.temperature < 37f)
			{
				component2.body.wetness -= Time.deltaTime * 3f;
			}
			break;
		case "autozoomgoggles":
			if ((bool)item.transform.parent && (bool)item.transform.parent.GetComponent<Limb>() && item.battery.hasCharge)
			{
				PlayerCamera.main.zoomTime = 0.5f;
			}
			break;
		case "jetpack":
		{
			SpriteRenderer component4 = base.transform.GetChild(0).GetComponent<SpriteRenderer>();
			AudioSource component5 = GetComponent<AudioSource>();
			if (data == null)
			{
				data = new object[1] { 0f };
			}
			float current = (float)data[0];
			current = Mathf.MoveTowards(current, 0f, Time.deltaTime);
			if (current == 0f && item.isWet)
			{
				current = UnityEngine.Random.Range(-1f, 0.7f);
			}
			if (item.condition > 0f && current <= 0f && (bool)item.transform.parent && item.transform.parent.TryGetComponent<Limb>(out component2) && component2.body.conscious && !component2.body.currentClimbable && Input.GetKey(KeyBinds.GetBind("up")) && !component2.body.forceWalk)
			{
				item.condition -= Time.deltaTime * 0.0135f;
				if (!component2.body.standing)
				{
					component2.rb.MoveRotation(Mathf.LerpAngle(component2.transform.eulerAngles.z, 0f, Time.deltaTime * 15f));
					component2.rb.AddForce(component2.transform.up * 198000f * Time.deltaTime + Vector3.right * 60000f * component2.body.moveDir.x * Time.deltaTime);
				}
				else
				{
					component2.body.rb.AddForce(Vector3.up * 198000f * Time.deltaTime + Vector3.right * 60000f * component2.body.moveDir.x * Time.deltaTime);
				}
				component4.color = new Color(1f, 1f, 1f, UnityEngine.Random.value);
				component5.volume = Mathf.MoveTowards(component5.volume, 1f, Time.deltaTime * 3f);
				component5.pitch = Mathf.MoveTowards(component5.pitch, 1f, Time.deltaTime * 3f);
				if (!component5.isPlaying)
				{
					component5.Play();
				}
			}
			else
			{
				component4.color = Color.clear;
				component5.volume = Mathf.MoveTowards(component5.volume, 0f, Time.deltaTime * 5f);
				component5.pitch = Mathf.MoveTowards(component5.pitch, 0f, Time.deltaTime * 5f);
				if (component5.isPlaying && component5.volume <= 0f)
				{
					component5.Stop();
				}
			}
			data[0] = current;
			break;
		}
		case "flashlight":
		{
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && (component == component.body.slots[0] || component == component.body.slots[1]))
			{
				float num2 = Vector2.SignedAngle(Vector2.right, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - base.transform.position).normalized);
				base.transform.eulerAngles = new Vector3(0f, 0f, num2 - 90f);
			}
			if (!item.battery.hasCharge)
			{
				state = 0;
			}
			float num3 = 1f;
			Light2D component3 = base.transform.GetChild(0).GetComponent<Light2D>();
			switch (state)
			{
			case 0:
				num3 = 0f;
				component3.intensity = 0f;
				break;
			case 1:
				num3 = 1f;
				component3.intensity = 1f;
				break;
			case 2:
				num3 = 0.5f;
				component3.intensity = 0.5f;
				break;
			case 3:
				num3 = 0.5f;
				component3.intensity = (((int)(Time.time * 18f) % 2 == 0) ? 1f : 0f);
				break;
			default:
				num3 = 0f;
				break;
			}
			item.decayMultiplier = num3;
			break;
		}
		case "emergencylight":
		{
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && (component == component.body.slots[0] || component == component.body.slots[1]))
			{
				float num6 = Vector2.SignedAngle(Vector2.right, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - base.transform.position).normalized);
				base.transform.eulerAngles = new Vector3(0f, 0f, num6 - 90f);
			}
			if (item.condition <= 0f)
			{
				state = 0;
			}
			float num7 = 1f;
			Light2D component8 = base.transform.GetChild(0).GetComponent<Light2D>();
			switch (state)
			{
			case 0:
				num7 = 0f;
				component8.intensity = 0f;
				break;
			case 1:
				num7 = 1f;
				component8.intensity = 1f;
				break;
			default:
				num7 = 0f;
				break;
			}
			item.decayMultiplier = num7;
			break;
		}
		case "headlamp":
			base.transform.GetComponent<LightItem>().shouldEnable = item.battery.hasCharge;
			break;
		case "rangefinder":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && (component == component.body.slots[0] || component == component.body.slots[1]))
			{
				float z = Vector2.SignedAngle(Vector2.right, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - base.transform.position).normalized);
				base.transform.eulerAngles = new Vector3(0f, 0f, z);
			}
			break;
		case "lightbulb":
			base.transform.GetComponent<LightItem>().shouldEnable = item.battery.hasCharge;
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && component == component.body.slots[2] && component.body.charType != 2)
			{
				state += Mathf.RoundToInt(Time.deltaTime * 1000f);
				if (state > 1500)
				{
					UnityEngine.Object.Destroy(item.gameObject);
					component2 = component.limb;
					component2.skinHealth -= 30f;
					component2.muscleHealth -= 5f;
					component2.bleedAmount += 10f;
					component2.shrapnel = 5;
					component2.body.adrenaline = 60f;
					component2.pain += 80f;
					SteamManager.TryUnlockAchievement("CONSEQUENCES");
					Sound.Play("bulbbreak", base.transform.position);
				}
			}
			break;
		case "heavydrill":
		{
			AudioSource component7 = GetComponent<AudioSource>();
			bool flag2 = false;
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component))
			{
				Body body = component.body;
				if (body.GetItem(body.handSlot) == item && body.attackCooldown > -0.1f)
				{
					flag2 = true;
				}
			}
			if (WorldGeneration.world.generatingWorld)
			{
				flag2 = false;
			}
			if (flag2)
			{
				if (!component7.isPlaying)
				{
					component7.Play();
				}
				component7.pitch = Mathf.MoveTowards(component7.pitch, 1f, Time.deltaTime * 3f);
				component7.volume = Mathf.MoveTowards(component7.volume, 1f, Time.deltaTime * 3f);
			}
			else
			{
				component7.pitch = Mathf.MoveTowards(component7.pitch, 0.2f, Time.deltaTime * 1.3f);
				component7.volume = Mathf.MoveTowards(component7.volume, 0f, Time.deltaTime * 1.3f);
				if (component7.volume <= 0f && component7.isPlaying)
				{
					component7.Stop();
				}
			}
			break;
		}
		case "bloodcrystalshard":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && component.body.bloodVolume < 99f)
			{
				component.body.bloodVolume += Time.deltaTime * 0.08f;
			}
			break;
		case "soothingcrystalshard":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component))
			{
				component.body.happiness += Time.deltaTime * 0.06f;
			}
			break;
		case "reliefcrystalshard":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component) && component.body.averagePain > 0f)
			{
				Limb[] limbs = component.body.limbs;
				for (int i = 0; i < limbs.Length; i++)
				{
					limbs[i].pain -= Time.deltaTime * 20f;
				}
			}
			break;
		case "oxygencrystalshard":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component))
			{
				component.body.stamina += Time.deltaTime * 2f;
			}
			break;
		case "digestioncrystalshard":
			if ((bool)item.transform.parent && item.transform.parent.TryGetComponent<InventorySlot>(out component))
			{
				component.body.sicknessAmount -= Time.deltaTime * 0.5f;
				component.body.venomTotal = Mathf.MoveTowards(component.body.venomTotal, 0f, Time.deltaTime * 0.25f);
			}
			break;
		case "roselight":
			GetComponent<Light2D>().intensity = item.condition;
			break;
		case "craftingbottle":
			if (GetComponent<WaterContainerItem>().CurrentTotal == 0f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/craftingbottle" + (((int)(Time.unscaledTime * 2f) % 2 == 0) ? "alt" : ""));
			break;
		case "gravbag":
			if (item.condition <= 0.005f && item.battery.hasBattery && WorldGeneration.world.biomeDepth != 11)
			{
				WorldGeneration.CreateExplosion(new ExplosionParams
				{
					position = item.transform.position,
					disfigureChance = 0.15f
				});
				UnityEngine.Object.Destroy(item.gameObject);
			}
			GetComponent<Container>().encumberanceMult = (item.battery.hasCharge ? 0f : 1f);
			break;
		case "campfire":
			if (item.isWet)
			{
				item.condition -= Time.deltaTime;
			}
			if (PlayerCamera.main.body.HoldingItem(item))
			{
				PlayerCamera.main.body.DropItem(0);
				PlayerCamera.main.body.DropItem(1);
				if (item.condition < 0.995f)
				{
					PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("hitgroundhard"));
				}
			}
			break;
		case "torch":
			if (item.isWet)
			{
				item.condition -= Time.deltaTime;
				if (item.condition < 0f)
				{
					item.condition = 0f;
				}
			}
			GetComponent<AudioSource>().volume = ((item.condition > 0f) ? 0.2f : 0f);
			base.transform.GetChild(0).gameObject.SetActive(item.condition > 0f && !item.ParentContainer());
			break;
		case "liquidpouch":
		{
			int num4 = 1;
			WaterContainerItem component6 = GetComponent<WaterContainerItem>();
			float num5 = component6.CurrentTotal / component6.Capacity;
			if (num5 > 0.1f)
			{
				num4 = 2;
			}
			if (num5 > 0.4f)
			{
				num4 = 3;
			}
			if (num5 > 0.75f)
			{
				num4 = 4;
			}
			GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/waterpouch" + num4);
			break;
		}
		case "liquidcentrifuge":
		{
			float num = (float)data[0];
			num -= Time.deltaTime;
			data[0] = num;
			if (num > 0f)
			{
				bool flag = (int)(Time.unscaledTime * 2f) % 2 == 0;
				GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(flag ? "Sprites/liquidcentrifugealt" : "Sprites/liquidcentrifuge");
				base.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = flag;
			}
			else
			{
				GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/liquidcentrifugealt");
				base.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
			}
			break;
		}
		case "minifleshrebuilder":
		{
			int actualItemCount = item.container.GetActualItemCount();
			base.transform.GetComponent<LightItem>().shouldEnable = actualItemCount > 0;
			base.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = actualItemCount > 0;
			break;
		}
		case "pistol":
			base.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = GetComponent<SpriteRenderer>().enabled;
			break;
		case "exposedcore":
			if (item.condition <= 0f && WorldGeneration.world.biomeDepth != 11)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				Utils.Create("crystalenemy", base.transform.position + Vector3.right * 0.5f, 0f).GetComponent<CrystalEnemy>().SetColor(new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f)));
				Utils.Create("crystalenemy", base.transform.position + Vector3.left * 0.5f, 0f).GetComponent<CrystalEnemy>().SetColor(new Color(UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f), UnityEngine.Random.Range(0.5f, 1f)));
				Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			}
			break;
		case "cryogeniccore":
			if (PlayerCamera.main.body.temperature > 41.5f && Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 8f)
			{
				CryoActivate();
			}
			break;
		}
	}

	public void DynamiteExplode()
	{
		UnityEngine.Object.Destroy(base.gameObject);
		WorldGeneration.CreateExplosion(new ExplosionParams
		{
			position = base.transform.position,
			range = 18f,
			structuralDamage = 2000f
		});
	}

	public void CryoActivate()
	{
		if (item.condition <= 0f)
		{
			return;
		}
		Utils.Create("Special/CrystalDistort", base.transform.position, 0f).AddComponent<CrystalDeathAnimation>();
		Sound.Play("droneFire1", base.transform.position, twoDimensional: false, pitchShift: false);
		item.condition = 0f;
		if (Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 8f)
		{
			PlayerCamera.main.body.temperature -= 10f;
		}
		int num = 8;
		Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position);
		int y = vector2Int.y;
		int x = vector2Int.x;
		uint height = WorldGeneration.world.height;
		int width = (int)WorldGeneration.world.width;
		int num2 = num * num;
		int num3 = Math.Max(0, y - num);
		int num4 = Math.Min((int)(height - 1), y + num);
		int num5 = Math.Max(0, x - num);
		int num6 = Math.Min(width - 1, x + num);
		for (int i = num3; i <= num4; i++)
		{
			int num7 = i - y;
			int num8 = num7 * num7;
			for (int j = num5; j <= num6; j++)
			{
				int num9 = j - x;
				if (num9 * num9 + num8 <= num2)
				{
					if (FluidManager.main.GetLiquid(j, i) == 6)
					{
						WorldGeneration.world.SetBlock(j, i, 37);
					}
					FluidManager.main.SetLiquid(j, i, 0);
				}
			}
		}
	}
}
