using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlueHeart : MonoBehaviour
{
	public static BlueHeart main;

	private Vector2 origPos;

	public Material mat;

	private float pulseTime;

	private float timeSincePulsed;

	private bool didTrapRemoval;

	private bool cutsceneStarted;

	private float pulseSpeedAdd;

	private bool wasClose;

	private Light2D light;

	private PlayerCamera cam => PlayerCamera.main;

	private Body body => cam.body;

	private bool canStartCutscene
	{
		get
		{
			if (body.standing)
			{
				return Vector2.Distance(body.transform.position, base.transform.position) < 10f;
			}
			return false;
		}
	}

	private void Awake()
	{
		main = this;
	}

	private void Start()
	{
		origPos = base.transform.position;
		light = GetComponent<Light2D>();
	}

	public IEnumerator Cutscene()
	{
		Sound.Play("blueheartsfx", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		Vector2 oldPos = origPos;
		Vector2 newPos = origPos + Vector2.up * 7f;
		float timer = 0f;
		while (timer < 5f)
		{
			timer += Time.deltaTime;
			pulseSpeedAdd += Time.deltaTime * 1.5f;
			origPos = Vector2.Lerp(oldPos, newPos, Utils.InQuad(timer * 0.2f));
			light.intensity = 3f + timer * 30f * Utils.InQuint(timer * 0.2f);
			Observer.main.distance = 50f - timer * 9f;
			yield return null;
		}
		cam.StartCoroutine(cam.CommenceBlinding());
		Object.Destroy(base.gameObject);
		if (!WorldGeneration.GetRunSettingBool("endless"))
		{
			body.EmbedCargo();
			SteamManager.TryUnlockAchievement("MILLIONSTOONE");
			MusicManager.main.cutsceneVolMult = 0f;
		}
		else
		{
			Sound.Play("observerlaugh", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			MusicManager.main.cutsceneVolMult = 1f;
			MusicManager.main.StopSong();
			WorldGeneration.world.biomeDepth = 0;
			WorldGeneration.world.StartCoroutine(WorldGeneration.world.RegenerateWorld());
			switch (WorldGeneration.world.endlessLoops)
			{
			case 0:
				PlayerCamera.main.body.talker.TalkDelayed(12f, Locale.GetCharacter("endlessloop0"), null, force: false, resetTalkTimer: true);
				body.eyePanicTime = 11f;
				body.eyeScareTime = 20f;
				break;
			case 1:
				PlayerCamera.main.body.talker.TalkDelayed(12f, Locale.GetCharacter("endlessloop1"), null, force: false, resetTalkTimer: true);
				body.eyeCloseTime = 15f;
				body.eyeScareTime = 20f;
				SteamManager.TryUnlockAchievement("ROUNDANDROUNDWEGO");
				break;
			case 2:
				PlayerCamera.main.body.talker.TalkDelayed(12f, Locale.GetCharacter("endlessloop2"), null, force: false, resetTalkTimer: true);
				body.StartCoroutine(body.Cry());
				break;
			case 3:
				PlayerCamera.main.body.talker.TalkDelayed(12f, Locale.GetCharacter("endlessloop3"), null, force: false, resetTalkTimer: true);
				body.eyeScareTime = 12f;
				break;
			default:
				PlayerCamera.main.body.talker.TalkDelayed(12f, Locale.GetCharacter("endlessloopdefault"), null, force: false, resetTalkTimer: true);
				break;
			}
			WorldGeneration.world.endlessLoops++;
		}
		body.forceNoMovement = false;
	}

	private void Update()
	{
		if (WorldGeneration.world.generatingWorld)
		{
			return;
		}
		if (!didTrapRemoval)
		{
			didTrapRemoval = true;
			Collider2D[] array = Physics2D.OverlapAreaAll(base.transform.position + new Vector3(-64f, -64f), base.transform.position + new Vector3(32f, 32f), LayerMask.GetMask("Ground"));
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].TryGetComponent<BuildingEntity>(out var component) && component.id != "highpowereddrillpod")
				{
					Object.Destroy(component.gameObject);
				}
			}
		}
		if (!cutsceneStarted && canStartCutscene)
		{
			cutsceneStarted = true;
			body.forceNoMovement = true;
			cam.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false, force: true);
			StartCoroutine(Cutscene());
			if (!WorldGeneration.GetRunSettingBool("endless"))
			{
				body.talker.Talk(Locale.GetCharacter("cargo2"));
			}
			Sound.Play("blueHeartObtain", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		}
		float num = Vector2.Distance(base.transform.position, body.transform.position) / 500f;
		float num2 = Mathf.Max(body.transform.position.y - base.transform.position.y, 0f) / 500f;
		if (num < 0.075f)
		{
			cam.customLookPos = Vector2.Lerp(base.transform.position + Vector3.up * 3f, body.transform.position, num / 0.075f);
		}
		if (!wasClose && num < 0.06f)
		{
			wasClose = true;
			body.happiness += 5f;
			if (!WorldGeneration.GetRunSettingBool("endless"))
			{
				body.talker.Talk(Locale.GetCharacter("cargo1"));
			}
		}
		MusicManager.main.cutsceneVolMult = Mathf.Clamp01((num - 0.1f) * 30f);
		float num3 = 1f + pulseSpeedAdd;
		if (num < 0.5f)
		{
			num3 += (0.5f - num) * 6f;
		}
		timeSincePulsed += Time.deltaTime * num3;
		pulseTime -= Time.deltaTime * (num3 * num3);
		if (pulseTime < 0f)
		{
			Pulse();
			Sound.Play("blueheartbeat", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f - num);
		}
		float value = timeSincePulsed * 50f;
		float value2 = 0.5f + num * 8f;
		float num4 = (1f - num2) * Mathf.Clamp01((10f - timeSincePulsed) * 0.1f);
		mat.SetFloat("_DesiredDistance", value);
		mat.SetFloat("_EdgeSize", value2);
		mat.SetColor("_Color", Color.cyan.WithAlpha(num4 * num4 * num4));
		base.transform.position = origPos + Random.insideUnitCircle * 0.1f;
		base.transform.localScale = new Vector2(1f + Mathf.Sin(Time.time * 3f) * 0.25f + Random.value * pulseSpeedAdd, 1f);
	}

	public void Pulse()
	{
		pulseTime = 10f;
		timeSincePulsed = 0f;
	}
}
