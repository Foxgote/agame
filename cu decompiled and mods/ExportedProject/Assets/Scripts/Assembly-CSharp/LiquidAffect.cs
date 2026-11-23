using UnityEngine;

public class LiquidAffect : MonoBehaviour
{
	private Rigidbody2D rb;

	private Body body;

	private Limb limb;

	private Item item;

	public bool wasWater;

	private static float lastSplash;

	private static AudioClip[] smallSplash;

	private static AudioClip[] mediumSplash;

	private static AudioClip[] bigSplash;

	private static bool setSplash;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		if (!rb || rb.bodyType != RigidbodyType2D.Dynamic)
		{
			Object.Destroy(this);
			return;
		}
		body = GetComponent<Body>();
		limb = GetComponent<Limb>();
		item = GetComponent<Item>();
		if (!setSplash)
		{
			setSplash = true;
			bigSplash = Resources.LoadAll<AudioClip>("Sounds/Splash/Big");
			mediumSplash = Resources.LoadAll<AudioClip>("Sounds/Splash/Medium");
			smallSplash = Resources.LoadAll<AudioClip>("Sounds/Splash/Small");
		}
	}

	private void FixedUpdate()
	{
		if ((!rb.simulated && !limb) || rb.bodyType == RigidbodyType2D.Static || Mathf.Abs(base.transform.position.x) >= (float)WorldGeneration.world.halfWidth || Mathf.Abs(base.transform.position.y) >= (float)WorldGeneration.world.halfHeight)
		{
			return;
		}
		(float, float, int) tuple = FluidManager.main.WaterInfo(WorldGeneration.world.WorldToBlockPos(base.transform.position));
		if (tuple.Item3 > 0)
		{
			bool flag = (bool)body && body.hasScubaGear;
			rb.velocity *= (flag ? Mathf.Lerp(tuple.Item2, 1f, 0.5f) : tuple.Item2);
			rb.angularVelocity *= tuple.Item2;
			rb.velocity += Vector2.up * tuple.Item1;
			if ((bool)body)
			{
				body.wetness += 15f * Time.fixedDeltaTime;
				if (tuple.Item3 == 1 && body.dirtyness > 10f)
				{
					body.dirtReduceTime += Time.deltaTime * 3f;
					if (body.dirtReduceTime > 1f)
					{
						body.dirtReduceTime = 0f;
						body.dirtyness -= 10f;
						FluidManager.main.SetLiquid(base.transform.position, 5);
					}
				}
				else if (tuple.Item3 == 3)
				{
					body.dirtyness += Time.fixedDeltaTime * 10f;
				}
			}
			if (tuple.Item3 == 6)
			{
				if ((bool)limb)
				{
					float armorReduction = limb.GetArmorReduction();
					limb.DamageWearables(Time.fixedDeltaTime * 0.5f);
					limb.pain += Time.fixedDeltaTime * 20f / armorReduction;
					limb.body.temperature += Time.fixedDeltaTime * 0.1f / armorReduction;
					limb.skinHealth -= Time.fixedDeltaTime * 33.3f / armorReduction;
					limb.muscleHealth -= Time.fixedDeltaTime * 8f / armorReduction;
					limb.body.eyePanicTime = 0.2f;
					limb.bleedAmount -= 25f * Time.fixedDeltaTime;
					limb.body.adrenaline = Mathf.Max(limb.body.adrenaline, 50f);
					if (limb.body.consciousness < 50f && !body.inCardiacArrest)
					{
						limb.body.consciousness += Time.fixedDeltaTime * 15f;
					}
					if (limb.body.shock > 0f)
					{
						limb.body.shock -= Time.fixedDeltaTime * 4f;
					}
					if (limb.body.talker.FinishedTalking())
					{
						limb.body.Scream();
					}
				}
				if ((bool)item)
				{
					if (item.id == "cryogeniccore")
					{
						item.GetComponent<CustomItemBehaviour>().CryoActivate();
					}
					Object.Destroy(item.gameObject);
					return;
				}
			}
			if (wasWater)
			{
				return;
			}
			wasWater = true;
			if (Time.time - lastSplash > 0.2f)
			{
				lastSplash = Time.time;
				if (tuple.Item3 == 6)
				{
					Sound.Play("Scald", base.transform.position);
				}
				float num = (limb ? limb.body.rb.velocity.magnitude : rb.velocity.magnitude);
				if (num > 20f)
				{
					Sound.Play(bigSplash.PickRandom(), base.transform.position);
					Object.Instantiate(Resources.Load<GameObject>("DustBig"), base.transform.position, Quaternion.identity);
				}
				else if (num > 8f)
				{
					Sound.Play(mediumSplash.PickRandom(), base.transform.position);
					Object.Instantiate(Resources.Load<GameObject>("DustBig"), base.transform.position, Quaternion.identity);
				}
				else if (num > 0.5f)
				{
					Sound.Play(smallSplash.PickRandom(), base.transform.position);
					Object.Instantiate(Resources.Load<GameObject>("DustSmall"), base.transform.position, Quaternion.identity);
				}
			}
		}
		else
		{
			wasWater = false;
		}
	}
}
