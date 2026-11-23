using UnityEngine;

public class Observer : MonoBehaviour
{
	public static Observer main;

	public float distance = 60f;

	private bool immortalSnail;

	private SpriteRenderer spr;

	private Body body => PlayerCamera.main.body;

	private void Awake()
	{
		main = GetComponent<Observer>();
		spr = GetComponent<SpriteRenderer>();
	}

	public void RolledLastStand()
	{
		distance = 1f;
	}

	public void GunSuicide()
	{
		distance = 5f;
	}

	private void Start()
	{
		if (WorldGeneration.GetRunSettingBool("immortalsnail"))
		{
			immortalSnail = true;
		}
	}

	public void Update()
	{
		if (immortalSnail)
		{
			if (WorldGeneration.world.generatingWorld)
			{
				base.transform.position = body.transform.position + Vector3.up * 20f;
			}
			if (WorldGeneration.world.generatingWorld || WorldGeneration.world.cutsceneActive || body.forceWalk)
			{
				spr.enabled = false;
				return;
			}
			spr.enabled = true;
			Vector2 vector = body.transform.position;
			float num = 1f;
			float num2 = Vector2.Distance(base.transform.position, vector);
			if (num2 > 32f)
			{
				num *= 3f;
			}
			if (!body.conscious)
			{
				num = 0f;
			}
			base.transform.position = Vector2.MoveTowards(base.transform.position, vector, Time.deltaTime * num);
			Vector2 vector2 = (base.transform.position - body.transform.position).normalized;
			float num3 = Mathf.Atan2(vector2.y, vector2.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num3 + 180f);
			if (num2 < 15f)
			{
				PlayerCamera.main.SetIrradiateIntensity(1f - (num2 - 3f) / 13f);
			}
			if (num2 < 3f)
			{
				Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false);
				body.consciousness = 0f;
				base.transform.position = body.transform.position + Vector3.up * 75f;
				Limb limb = body.limbs[Random.Range(3, body.limbs.Length)];
				if (!limb.dismembered && !limb.prostheticInfo.hasProsthetic)
				{
					limb.Dismember();
					return;
				}
				body.limbs[0].muscleHealth = 0f;
				body.limbs[1].muscleHealth = 0f;
				body.limbs[2].muscleHealth = 0f;
				body.limbs[0].skinHealth = 0f;
				body.limbs[0].bleedAmount = 60f;
				body.Disfigure();
				body.RemoveEye();
				body.RemoveEye();
				body.DoGoreSound();
			}
		}
		else
		{
			Vector2 b = PlayerCamera.main.transform.position;
			Vector2 vector3 = (base.transform.position - PlayerCamera.main.transform.position).normalized;
			distance = Mathf.MoveTowards(distance, (WorldGeneration.world.biomeDepth == 7) ? 47.5f : (60f - (float)WorldGeneration.world.biomeDepth), Time.unscaledDeltaTime * 3f);
			spr.color = Color.white.WithAlpha(immortalSnail ? 1f : Mathf.Clamp(1f - (distance - 10f) * 0.04f, 0.1f, 1f));
			b += vector3 * distance;
			base.transform.position = Vector2.Lerp(base.transform.position, b, Time.unscaledDeltaTime);
			float num4 = Mathf.Atan2(vector3.y, vector3.x) * 57.29578f;
			base.transform.rotation = Quaternion.Euler(0f, 0f, num4 + 180f);
		}
	}
}
