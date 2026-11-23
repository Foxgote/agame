using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CargoMinigame : Minigame
{
	private Limb limb;

	public int stage;

	private List<RectTransform> clamps;

	private List<RectTransform> hoses;

	private RectTransform held;

	private Vector2 heldOffset;

	private int index;

	private int clampsDone;

	private RectTransform cargo;

	private AudioSource source;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Grasp;
	}

	public override string GuideLocaleString()
	{
		return "";
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public override float HandRotOffset()
	{
		return 0f;
	}

	public override bool CanExit()
	{
		return false;
	}

	public CargoMinigame(Limb limb)
	{
		this.limb = limb;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/CargoMinigame");
		clamps = new List<RectTransform>();
		hoses = new List<RectTransform>();
		clamps.Add(Minigame.game.spawnedMiniGame.GetChild(1).GetComponent<RectTransform>());
		clamps.Add(Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<RectTransform>());
		clamps.Add(Minigame.game.spawnedMiniGame.GetChild(3).GetComponent<RectTransform>());
		clamps.Add(Minigame.game.spawnedMiniGame.GetChild(4).GetComponent<RectTransform>());
		hoses.Add(Minigame.game.spawnedMiniGame.GetChild(6).GetComponent<RectTransform>());
		hoses.Add(Minigame.game.spawnedMiniGame.GetChild(7).GetComponent<RectTransform>());
		cargo = Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<RectTransform>();
		source = Minigame.game.spawnedMiniGame.GetComponent<AudioSource>();
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (stage == 0)
		{
			if (Minigame.game.handStartedClicking)
			{
				foreach (RaycastResult uiCast in uiCasts)
				{
					if (clamps.Contains(uiCast.gameObject.GetComponent<RectTransform>()))
					{
						held = uiCast.gameObject.GetComponent<RectTransform>();
						heldOffset = held.anchoredPosition - Minigame.game.handPos;
						index = clamps.IndexOf(held);
						break;
					}
				}
			}
			if (Minigame.game.handStoppedClicking)
			{
				held = null;
			}
		}
		if (stage == 1)
		{
			if (Minigame.game.handStartedClicking && uiCasts.Count > 0 && uiCasts[0].gameObject == cargo.gameObject)
			{
				held = uiCasts[0].gameObject.GetComponent<RectTransform>();
				heldOffset = held.anchoredPosition - Minigame.game.handPos;
			}
			if ((bool)held)
			{
				limb.pain += Time.deltaTime * 10f;
				if (Mathf.Abs(held.localPosition.x) > 980f)
				{
					stage = 2;
					held = null;
					Minigame.game.body.RemoveCargo();
					limb.skinHealth = 0f;
					limb.muscleHealth = 0f;
					limb.bleedAmount = 100f;
					Sound.Play("gore", Minigame.game.body.transform.position, twoDimensional: false, pitchShift: false);
				}
				if (uiCasts.Count > 0 && uiCasts[0].gameObject != cargo.gameObject)
				{
					held = null;
				}
			}
			source.volume = Mathf.MoveTowards(source.volume, held ? 1f : 0f, Time.deltaTime * 5f);
			if (Minigame.game.handStoppedClicking)
			{
				held = null;
			}
		}
		if (stage == 2)
		{
			source.volume = Mathf.MoveTowards(source.volume, (clampsDone == 2) ? 1f : 0f, Time.deltaTime * 5f);
			cargo.localPosition = Vector2.Lerp(cargo.localPosition, new Vector2(cargo.localPosition.x, -1000f), Time.deltaTime * 1.5f);
			if (Minigame.game.handStartedClicking)
			{
				foreach (RaycastResult uiCast2 in uiCasts)
				{
					if (hoses.Contains(uiCast2.gameObject.GetComponent<RectTransform>()))
					{
						held = uiCast2.gameObject.GetComponent<RectTransform>();
						heldOffset = held.anchoredPosition - Minigame.game.handPos;
						index = hoses.IndexOf(held);
						break;
					}
				}
			}
			for (int i = 0; i < hoses.Count; i++)
			{
				if (hoses[i] != held && hoses[i].GetComponent<Image>().raycastTarget)
				{
					hoses[i].localPosition = Vector2.Lerp(hoses[i].localPosition, (i == 0) ? new Vector2(710f, 0f) : new Vector2(-710f, 0f), Time.deltaTime * 2f);
				}
			}
			if ((bool)held)
			{
				Vector2 vector = ((index == 0) ? new Vector2(555f, -63f) : new Vector2(-555f, -63f));
				if (Vector2.Distance(held.localPosition, vector) < 55f)
				{
					clampsDone++;
					held.GetComponent<Image>().raycastTarget = false;
					held.localPosition = vector;
					Minigame.game.body.DoGoreSound();
					held = null;
				}
			}
			if (clampsDone >= 2)
			{
				for (int j = 0; j < hoses.Count; j++)
				{
					hoses[j].localPosition = ((j == 0) ? new Vector2(555f, -63f) : new Vector2(-555f, -63f)) + Random.insideUnitCircle * 10f;
				}
				limb.muscleHealth += Time.deltaTime * 75f;
				limb.skinHealth += Time.deltaTime * 75f;
				limb.bleedAmount -= Time.deltaTime * 75f;
				limb.infectionAmount -= Time.deltaTime * 75f;
				if (limb.muscleHealth > 99f)
				{
					Minigame.game.EndMinigame();
				}
			}
			if (Minigame.game.handStoppedClicking)
			{
				held = null;
			}
		}
		foreach (RectTransform clamp in clamps)
		{
			clamp.GetComponent<Image>().color = Color.white;
		}
		foreach (RectTransform hose in hoses)
		{
			hose.GetComponent<Image>().color = Color.white;
		}
		cargo.GetComponent<Image>().color = Color.white;
		float num = 0.2f + Mathf.PingPong(Time.time * 7.5f, 0.8f);
		if (stage == 0)
		{
			foreach (RectTransform clamp2 in clamps)
			{
				if (clamp2.GetComponent<Image>().raycastTarget)
				{
					clamp2.GetComponent<Image>().color = new Color(num, num, num);
				}
			}
		}
		if (stage == 1)
		{
			cargo.GetComponent<Image>().color = new Color(num, num, num);
		}
		if (stage != 2)
		{
			return;
		}
		foreach (RectTransform hose2 in hoses)
		{
			if (hose2.GetComponent<Image>().raycastTarget)
			{
				hose2.GetComponent<Image>().color = new Color(num, num, num);
			}
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (stage == 0 && (bool)held)
		{
			held.localPosition = Minigame.game.handPos + heldOffset;
			if (held.localPosition.x < 413f && held.localPosition.x > -413f)
			{
				held.localPosition = new Vector2((held.localPosition.x > 0f) ? 413f : (-413f), held.localPosition.y);
				Minigame.game.handPos = (Vector2)held.localPosition - heldOffset;
				if (((index == 0 || index == 1) && Mathf.Abs(held.localPosition.y - 248f) < 20f) || ((index == 2 || index == 3) && Mathf.Abs(held.localPosition.y + 365f) < 20f))
				{
					held.localPosition = new Vector2((held.localPosition.x > 0f) ? 288f : (-288f), held.localPosition.y);
					held.GetComponent<Image>().raycastTarget = false;
					held = null;
					Minigame.game.body.DoGoreSound();
					clampsDone++;
					if (clampsDone >= 4)
					{
						stage++;
						clampsDone = 0;
					}
				}
			}
		}
		if (stage == 1 && (bool)held)
		{
			held.transform.localPosition = new Vector2(Minigame.game.handPos.x + heldOffset.x, held.transform.localPosition.y);
			Minigame.game.handVelocity *= 0.5f;
			Minigame.game.handPos.y = held.transform.localPosition.y - heldOffset.y;
		}
		if (stage == 2 && (bool)held)
		{
			held.localPosition = Minigame.game.handPos + heldOffset;
			if (index == 0 && held.localPosition.x < 400f)
			{
				held.localPosition = new Vector2(400f, held.localPosition.y);
				Minigame.game.handPos = (Vector2)held.localPosition - heldOffset;
			}
			if (index == 1 && held.localPosition.x > -400f)
			{
				held.localPosition = new Vector2(-400f, held.localPosition.y);
				Minigame.game.handPos = (Vector2)held.localPosition - heldOffset;
			}
		}
	}
}
