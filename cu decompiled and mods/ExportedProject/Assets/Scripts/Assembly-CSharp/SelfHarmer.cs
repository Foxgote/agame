using System.Collections;
using System.Linq;
using UnityEngine;

public class SelfHarmer : MonoBehaviour
{
	private Body body;

	[HideInInspector]
	public float timeWasStill;

	[HideInInspector]
	public float cooldownTime;

	public AudioSource scarySource;

	public float blackAmount;

	public bool harming;

	private void Start()
	{
		body = GetComponent<Body>();
		scarySource = base.gameObject.AddComponent<AudioSource>();
		scarySource.loop = true;
		scarySource.dopplerLevel = 0f;
		scarySource.reverbZoneMix = 0f;
		scarySource.volume = 0f;
		scarySource.clip = Resources.Load<AudioClip>("Sounds/warningLoop");
	}

	private void Update()
	{
		if (harming)
		{
			return;
		}
		if (body.totalHappiness <= -70f && WorldGeneration.GetRunSettingBool("lowmoodevents"))
		{
			blackAmount = Mathf.Lerp(blackAmount, Mathf.Clamp01((timeWasStill - 20f) * 0.1f) + body.horrifiedLevel * 0.008f, Time.deltaTime * 2f);
			if (cooldownTime <= 0f)
			{
				if (body.conscious && body.rb.velocity.magnitude < 5f)
				{
					timeWasStill += Time.deltaTime * ((body.happiness < -75f) ? 1f : 0.75f) * 2.5f;
				}
				else
				{
					timeWasStill -= Time.deltaTime * ((body.happiness < -75f) ? 2f : 3f) * 2.5f;
					if (timeWasStill < 0f)
					{
						timeWasStill = 0f;
					}
				}
				if (timeWasStill > 20f && !Body.censorMood)
				{
					PlayerCamera.main.SetTimeScale(PlayerCamera.SpeedType.Normal, switchSound: false);
					if (!scarySource.isPlaying)
					{
						scarySource.Play();
					}
					float num = (timeWasStill - 20f) * 0.1f;
					num *= num;
					scarySource.volume = num;
				}
				else if (scarySource.isPlaying)
				{
					scarySource.Stop();
				}
				if (timeWasStill > 30f)
				{
					AttemptHarm();
				}
			}
			else
			{
				timeWasStill = 0f;
				cooldownTime -= Time.deltaTime;
				if (scarySource.isPlaying)
				{
					scarySource.Stop();
				}
			}
		}
		else
		{
			blackAmount = Mathf.Lerp(blackAmount, body.horrifiedLevel * 0.008f, Time.deltaTime * 2f);
			timeWasStill = 0f;
			if (scarySource.isPlaying)
			{
				scarySource.Stop();
			}
		}
	}

	private IEnumerator GunSuicide(Item item)
	{
		body.forceNoMovement = true;
		PlayerCamera.main.blockItemUsage = true;
		Sound.Play("observerlaugh", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		yield return new WaitForSeconds(2f);
		if (body.HoldingItem(item) && item.GetComponent<GunScript>().racked)
		{
			item.GetComponent<GunScript>().TryRack();
		}
		body.eyeCloseTime = 2f;
		body.talker.Talk("...", null, force: true);
		Observer.main.GunSuicide();
		yield return new WaitForSeconds(1f);
		bool flag = false;
		if (body.GetItem(body.handSlot) != item)
		{
			flag = true;
		}
		if (body.HoldingItem(item))
		{
			item.GetComponent<GunScript>().Fire(suicide: true);
		}
		Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
		body.forceNoMovement = false;
		PlayerCamera.main.blockItemUsage = false;
		if (flag)
		{
			yield return new WaitForSeconds(1f);
			yield return Suicide();
		}
	}

	public void AttemptHarm()
	{
		harming = true;
		if (body.FindByIdThorough("plushie", out var it))
		{
			cooldownTime = 240f;
			it.condition = 0f;
			ClawAnim();
			body.happiness += 5f;
			body.talker.Talk(Locale.GetCharacter("angeronitem"), null, force: true, resetTalkTimer: true);
			SteamManager.TryUnlockAchievement("WHATHAVEIDONE");
			harming = false;
		}
		else if (!Body.censorMood && body.charType == 1)
		{
			if (body.totalHappiness > -95f)
			{
				body.vomiter.Vomit();
				cooldownTime = 90f;
				harming = false;
			}
			else
			{
				StartCoroutine(Suicide());
			}
		}
		else if (!Body.censorMood && body.charType == 2)
		{
			Limb limb = (from x in body.limbs.Skip(3)
				where !x.dismembered
				select x).ToList().PickRandom();
			if (!limb)
			{
				harming = false;
				cooldownTime = 30f;
			}
			else if (body.totalHappiness > -90f)
			{
				harming = false;
				ClawAnim();
				body.talker.PromptTraderResponse("selfharm");
				cooldownTime = 180f;
				limb.skinHealth -= 50f;
				limb.muscleHealth -= 50f;
				limb.bleedAmount += 10f;
				body.happiness += 6f;
				body.Eat(20f, 2.5f, 1);
				body.DoGoreSound();
				Sound.Play("eatFlesh", body.transform.position);
				Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
			}
			else
			{
				harming = false;
				ClawAnim();
				body.talker.PromptTraderResponse("selfharm");
				cooldownTime = 300f;
				limb.skinHealth -= 100f;
				limb.muscleHealth -= 100f;
				limb.bleedAmount += 20f;
				limb.BreakBone();
				body.happiness += 20f;
				body.Eat(50f, 6f, 1);
				body.DoGoreSound();
				body.Ragdoll();
				body.shock = 50f;
				Sound.Play("eatFlesh", body.transform.position);
				Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 0.8f);
			}
		}
		else if (body.totalHappiness > -90f)
		{
			if (!Body.censorMood)
			{
				StartCoroutine(SelfHarm());
				return;
			}
			body.vomiter.Vomit();
			cooldownTime = 30f;
			Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
			harming = false;
		}
		else if (!Body.censorMood)
		{
			StartCoroutine(Suicide());
		}
		else
		{
			body.TryStartFibrillation(forced: true);
			body.vomiter.Vomit();
			body.vomiter.Vomit();
			body.vomiter.Vomit();
			body.vomiter.Vomit();
			body.vomiter.Vomit();
			Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
			cooldownTime = 60f;
			harming = false;
		}
	}

	private IEnumerator WaitForMinigame(int cuts, bool isEye = false)
	{
		if ((bool)MinigameBase.main.currentMinigame)
		{
			MinigameBase.main.EndMinigame();
		}
		MinigameBase.main.StartMinigame(new SelfHarmMinigame(cuts, isEye), null);
		yield return new WaitUntil(() => !MinigameBase.main.currentMinigame);
	}

	private IEnumerator Suicide()
	{
		if (body.FindByTagThorough("gun", out var it))
		{
			if (it.transform.parent.TryGetComponent<Container>(out var component))
			{
				component.UnloadItem(it);
			}
			body.DropItem(body.handSlot);
			body.DropItem(it);
			body.PickUpItem(it, body.handSlot);
			it.GetComponent<GunScript>().roundInChamber = GunScript.RoundInChamber.Round;
			body.armsAnimator.Play("ArmsGunSuicide");
			cooldownTime = 600f;
			harming = false;
			StartCoroutine(GunSuicide(it));
			yield break;
		}
		bool isEye = body.totalHappiness <= -99.9f && !body.bothEyesGone && Random.value < 0.5f;
		yield return WaitForMinigame(isEye ? 1 : 5, isEye);
		harming = false;
		Limb limb = (isEye ? body.limbs[0] : ((Random.Range(0f, 1f) < 0.5f) ? body.limbs[4] : body.limbs[7]));
		if (limb.dismembered || limb.prostheticInfo.hasProsthetic)
		{
			limb = body.limbs[1];
		}
		limb.muscleHealth -= 30f;
		limb.skinHealth -= 70f;
		limb.bleedAmount += Random.Range(45f, 60f);
		limb.pain += Random.Range(25f, 35f);
		body.adrenaline += 50f;
		body.rb.AddForce(Random.insideUnitCircle * 700f, ForceMode2D.Impulse);
		if (!isEye)
		{
			body.talker.Talk(Locale.GetCharacter("suicide"), null, force: true);
		}
		body.talker.PromptTraderResponse("selfharm");
		body.happiness += (isEye ? 16f : 12.5f);
		Sound.Play("suicide", body.transform.position);
		MusicManager.main.PlaySong(Resources.Load<AudioClip>("Sounds/music/whathaveidone"));
		cooldownTime = 300f;
		Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 0.7f);
		WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/suicideWound"), flip: false, (Limb l) => l.skinHealth >= 95f);
		if (isEye)
		{
			PlayerCamera.main.body.RemoveEye();
		}
		ClawAnim();
	}

	private IEnumerator SelfHarm()
	{
		yield return WaitForMinigame(1);
		Limb limb = null;
		while (limb == null || limb.dismembered || limb.prostheticInfo.hasProsthetic)
		{
			limb = body.limbs[Random.Range(1, body.limbs.Length)];
		}
		limb.muscleHealth -= 13f;
		limb.skinHealth -= 35f;
		limb.bleedAmount += Random.Range(6f, 10f);
		limb.pain += Random.Range(20f, 30f);
		WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/selfHarmWound"), flip: false, (Limb l) => l.skinHealth >= 95f);
		body.rb.AddForce(Random.insideUnitCircle * 500f, ForceMode2D.Impulse);
		body.talker.Talk(Locale.GetCharacter("selfharm"), null, force: true);
		body.talker.PromptTraderResponse("selfharm");
		body.happiness += 4f;
		Sound.Play("selfharm", body.transform.position);
		Sound.Play("harmSting", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
		cooldownTime = 180f;
		harming = false;
		ClawAnim();
	}

	private void ClawAnim()
	{
		GameObject obj = Object.Instantiate(Resources.Load<GameObject>("ClawAnimSlow"));
		obj.transform.position = body.limbs[1].transform.position + Vector3.left * 2f;
		obj.transform.SetParent(base.transform);
		Object.Destroy(obj, 5f);
	}
}
