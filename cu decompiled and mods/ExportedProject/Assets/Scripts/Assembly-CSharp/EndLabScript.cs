using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndLabScript : MonoBehaviour
{
	public static EndLabScript instance;

	public CeilingTurret turret;

	public SpriteRenderer scientist;

	public Talker scientistTalker;

	public Sprite[] scientistSprites;

	private int scientistAnimType;

	public Coroutine scientistCoroutine;

	public Coroutine escapeCoroutine;

	public bool doingEscape;

	private float timeSinceEscape;

	public Animator scientistAnim;

	public List<LampScript> lights;

	public List<AudioClip> escapeMusic;

	public Talker announcer;

	public TextMeshProUGUI announcerUi;

	public TextMeshProUGUI thoughtUi;

	public TextMeshProUGUI timerUi;

	private int lightIndex;

	public GuardSpawner endGuardSpawner;

	public WaterContainerItem injector;

	public bool doTimer;

	private float timeLeft = 60.75f;

	public BuildingEntity surgicalDevice;

	public bool canStartSurgery;

	public GameObject removedHeart;

	public FixedJoint2D doorJoint;

	public AudioClip end1;

	public AudioClip end2;

	public CosmeticDoor surgeryDoor;

	public AnimationCurve timerTransparency;

	public Vector2 exitPoint;

	private Body body => PlayerCamera.main.body;

	private PlayerCamera cam => PlayerCamera.main;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		cam.blockItemUsage = true;
		body.forceWalk = true;
		scientistCoroutine = StartCoroutine(ScientistCutscene());
		LampScript[] array = Object.FindObjectsByType<LampScript>(FindObjectsSortMode.None);
		foreach (LampScript item in array)
		{
			lights.Add(item);
		}
		GuardScript.anyGuardKilled = false;
		injector.DrainAll();
		injector.AddLiquid("localanesthetic", 347f);
		surgicalDevice.gameObject.AddComponent<SurgicalDeviceBehavior>();
	}

	public IEnumerator ScientistCutscene()
	{
		yield return new WaitUntil(() => !WorldGeneration.world.generatingWorld);
		MusicManager.main.PlaySong(MusicManager.main.biomeMusic[11].songs[0]);
		MusicManager.main.specialLoop = true;
		MusicManager.main.biomeMusic[11].songs[1].LoadAudioData();
		MusicManager.main.biomeMusic[11].songs[2].LoadAudioData();
		Sound.Play("shuttleOpen", Vector2.zero, twoDimensional: true, pitchShift: false);
		scientistAnimType = 1;
		body.talker.Talk(Locale.GetCharacter("labenter"), null, force: false, resetTalkTimer: true);
		yield return new WaitUntil(() => body.transform.position.x < 17f);
		scientistTalker.Talk(Locale.GetOther("endsc1").Replace("<>", WoundView.view.cInfo[2].ToString()));
		yield return new WaitForSecondsRealtime(3f);
		scientistTalker.Talk(Locale.GetOther("endsc2").Replace("<>", WoundView.view.cInfo[2].ToString()));
		scientistAnimType = 0;
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		body.talker.Talk(Locale.GetCharacter("labresponse"), null, force: false, resetTalkTimer: true);
		yield return new WaitForSecondsRealtime(4f);
		scientistTalker.Talk(Locale.GetOther("endsc3"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		scientistAnimType = 1;
		scientistTalker.Talk(Locale.GetOther("endsc4"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		scientistTalker.Talk(Locale.GetOther("endsc5"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		scientistTalker.Talk(Locale.GetOther("endsc6"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		scientistAnimType = 0;
		scientistTalker.Talk(Locale.GetOther("endsc7"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		scientistAnimType = 1;
		scientistTalker.Talk(Locale.GetOther("endsc8"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		yield return new WaitForSecondsRealtime(1f);
		body.talker.Talk(Locale.GetCharacter("labthink1"), null, force: false, resetTalkTimer: true);
		if (WorldGeneration.unchipped)
		{
			cam.blockItemUsage = false;
		}
		yield return new WaitUntil(() => body.talker.FinishedTalking());
		if (WorldGeneration.unchipped)
		{
			body.talker.Talk(Locale.GetCharacter("labthink2"), null, force: false, resetTalkTimer: true);
		}
		else
		{
			body.talker.Talk(Locale.GetCharacter("labthink2chip"), null, force: false, resetTalkTimer: true);
		}
		yield return new WaitUntil(() => body.talker.FinishedTalking());
		yield return new WaitForSecondsRealtime(5f);
		scientistTalker.Talk(Locale.GetOther("endsc9"));
		yield return new WaitUntil(() => scientistTalker.FinishedTalking());
		float timer = 0f;
		int line = 10;
		while (true)
		{
			timer += Time.deltaTime;
			if (timer > 14f && line <= 13)
			{
				if (WorldGeneration.unchipped && body.FindByTagThorough("gun", out var _))
				{
					thoughtUi.text = Locale.GetOther("endsctip");
				}
				scientistTalker.Talk(Locale.GetOther("endsc" + line));
				timer = 0f;
				line++;
			}
			if (Vector2.Distance(body.transform.position, exitPoint) < 4f)
			{
				break;
			}
			yield return null;
		}
		MusicManager.main.StopSong();
		MusicManager.main.specialLoop = false;
		cam.blockItemUsage = true;
		body.forceNoMovement = true;
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		WorldGeneration.world.StartCoroutine(WorldGeneration.world.BiopodEnding());
	}

	public void BeginEscapeSequence()
	{
		if (!doingEscape)
		{
			if (scientistCoroutine != null)
			{
				StopCoroutine(scientistCoroutine);
			}
			thoughtUi.text = "";
			doingEscape = true;
			body.forceWalk = false;
			MusicManager.main.StopSong();
			scientistAnim.enabled = true;
			scientistAnim.Play("ScientistShot");
			MusicManager.main.StartCoroutine(MusicManager.main.EscapeMusic());
			body.talker.Talk(Locale.GetCharacter("labshoot"), null, force: false, resetTalkTimer: true);
			scientistTalker.Skip();
			scientistAnim.GetComponent<Collider2D>().enabled = false;
			Utils.Create("HumanBlood", scientistAnim.transform.position, 0f);
			Sound.Play("cut5", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			escapeCoroutine = StartCoroutine(EscapeCoroutine());
		}
	}

	public IEnumerator EscapeCoroutine()
	{
		yield return new WaitForSeconds(5f);
		announcer.Talk(Locale.GetOther("escapealert"));
		yield return new WaitUntil(() => Vector2.Distance(body.transform.position, new Vector2(250f, 234f)) < 2f);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		body.transform.position = new Vector2(287f, 243f);
		cam.transform.position = body.transform.position;
		endGuardSpawner.openProgress = 1.45f;
		Sound.Play("endlabDoor", body.transform.position, twoDimensional: false, pitchShift: false);
		doTimer = true;
		MusicManager.main.specialLoop = true;
		MusicManager.main.PlaySong(end1);
		injector.gameObject.AddComponent<FreshItemDrop>().forceOn = true;
		thoughtUi.text = Locale.GetOther("cargosurgery1");
		yield return new WaitUntil(() => CoUtils.instance.DurationOf("localanestheticDownTorso") > 50f);
		thoughtUi.text = Locale.GetOther("cargosurgery2");
		canStartSurgery = true;
		yield return new WaitUntil(() => (bool)MinigameBase.main.currentMinigame && MinigameBase.main.currentMinigame is CargoMinigame);
		thoughtUi.text = Locale.GetOther("cargosurgery3");
		CargoMinigame minigame = MinigameBase.main.currentMinigame as CargoMinigame;
		yield return new WaitUntil(() => ((bool)minigame && minigame.stage == 1) || !body.hasCargo);
		thoughtUi.text = Locale.GetOther("cargosurgery4");
		yield return new WaitUntil(() => !body.hasCargo);
		thoughtUi.text = Locale.GetOther("cargosurgery5");
		removedHeart.SetActive(value: true);
		yield return new WaitUntil(() => !MinigameBase.main.currentMinigame);
		thoughtUi.text = Locale.GetOther("cargosurgery6");
		surgeryDoor.forceDisable = false;
		MusicManager.main.PlaySong(end2);
		Object.Destroy(surgicalDevice.GetComponent<UsableObject>());
		doTimer = false;
		yield return new WaitUntil(() => body.transform.position.x > 353f);
		thoughtUi.text = "";
		body.forceWalk = true;
		yield return new WaitUntil(() => body.transform.position.x > 366f);
		body.forceWalk = false;
		body.Ragdoll();
		body.shock = 50f;
		body.Scream();
		Object.Destroy(doorJoint);
		Sound.Play("robothit", body.transform.position, twoDimensional: false, pitchShift: false);
		yield return new WaitUntil(() => body.transform.position.y > 270f && body.transform.position.x < 370f);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		body.forceNoMovement = true;
		WorldGeneration.world.StartCoroutine(WorldGeneration.world.EscapeEnding());
	}

	public void SetLightsColor(Color color)
	{
		for (int i = lightIndex; i < lightIndex + 4; i++)
		{
			if (i < lights.Count && (bool)lights[i])
			{
				lights[i].SetColor(color);
			}
		}
		lightIndex += 4;
		if (lightIndex >= lights.Count)
		{
			lightIndex = 0;
		}
	}

	private void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.B))
		{
			scientistTalker.Skip();
		}
		announcerUi.text = announcer.text.text;
		thoughtUi.color = Color.red.WithAlpha(Random.Range(0.1f, 0.4f));
		if (!doingEscape)
		{
			if (scientistAnimType == 0)
			{
				scientist.sprite = ((Mathf.FloorToInt(Time.time * 1.5f) % 2 == 0) ? scientistSprites[0] : scientistSprites[1]);
			}
			else if (scientistAnimType == 1)
			{
				scientist.sprite = ((Mathf.FloorToInt(Time.time * 1.5f) % 2 == 0) ? scientistSprites[2] : scientistSprites[3]);
			}
		}
		else
		{
			body.focusedLevel = 100f;
			body.horrifiedLevel = 40f;
			if (body.adrenaline < 60f)
			{
				body.adrenaline = 60f;
			}
			timeSinceEscape += Time.deltaTime;
			if (timeSinceEscape > 2f && timeSinceEscape < 6.25f)
			{
				SetLightsColor(Color.black);
			}
			if (timeSinceEscape > 6.25f)
			{
				float t = Mathf.PingPong(Time.time * 0.66f, 1f);
				SetLightsColor(Color.Lerp(Color.red, new Color32(50, 0, 0, byte.MaxValue), t));
				if ((bool)turret && !turret.enabled)
				{
					turret.enabled = true;
				}
			}
		}
		ForceBaseStats();
		if (body.brainDying && !WorldGeneration.world.cutsceneActive)
		{
			if (!doingEscape)
			{
				if (scientistCoroutine != null)
				{
					StopCoroutine(scientistCoroutine);
				}
				WorldGeneration.world.StartCoroutine(WorldGeneration.world.BiopodEndingAlt());
			}
			else
			{
				if (escapeCoroutine != null)
				{
					StopCoroutine(escapeCoroutine);
				}
				WorldGeneration.world.StartCoroutine(WorldGeneration.world.BiopodEndingAlt2());
				thoughtUi.enabled = false;
				thoughtUi.text = "";
				doTimer = false;
			}
		}
		if (doTimer)
		{
			timeLeft -= Time.deltaTime;
			if (timeLeft < 0f && doTimer)
			{
				timeLeft = 0f;
				doTimer = false;
				endGuardSpawner.guardsToSpawn = 5;
				endGuardSpawner.openProgress = 5f;
			}
			timerUi.enabled = true;
			timerUi.color = Color.red.WithAlpha(timerTransparency.Evaluate(timeLeft) * Random.Range(0.3f, 0.5f));
			timerUi.text = Mathf.Floor(timeLeft).ToString();
		}
		else
		{
			timerUi.enabled = false;
		}
	}

	public void ForceBaseStats()
	{
		if (body.thirst < 60f)
		{
			body.thirst = 60f;
		}
		if (body.hunger < 60f)
		{
			body.hunger = 60f;
		}
		if (body.energy < 90f)
		{
			body.energy = 90f;
		}
		if (body.happiness < -50f)
		{
			body.happiness = -50f;
		}
		if (body.brainHealth < 20f)
		{
			body.brainHealth = 20f;
		}
	}
}
