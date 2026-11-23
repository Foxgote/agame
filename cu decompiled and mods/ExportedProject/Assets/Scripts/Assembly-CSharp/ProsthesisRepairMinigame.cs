using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProsthesisRepairMinigame : Minigame
{
	private Limb limb;

	private int patchesLeft;

	private int maxPatches;

	private int maxHoles = 15;

	private int holesPresent;

	private List<RectTransform> holes = new List<RectTransform>();

	private List<RectTransform> patches = new List<RectTransform>();

	private Image prosthesis;

	private Image brokeOverlay;

	private RectTransform mask;

	private RectTransform grabbed;

	private Vector2 heldOffset;

	private RectTransform closeHole;

	private float patchPitch;

	private bool lastInsideBroken;

	private bool startedBroken;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Grasp;
	}

	public override string GuideLocaleString()
	{
		return "prosthesisMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public ProsthesisRepairMinigame(Limb limb, int fillCount)
	{
		this.limb = limb;
		maxHoles = limb.prostheticInfo.maxHoles;
		patchesLeft = fillCount;
		maxPatches = fillCount;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/ProsthesisRepairScreen");
		prosthesis = Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<Image>();
		brokeOverlay = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(0).GetComponent<Image>();
		mask = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(1).GetComponent<RectTransform>();
		patchesLeft = Mathf.FloorToInt((float)patchesLeft * Minigame.game.currentItem.condition);
		if (patchesLeft == 0)
		{
			Minigame.game.EndMinigame();
			return;
		}
		holesPresent = maxHoles - Mathf.FloorToInt(Mathf.Lerp(0f, maxHoles, limb.muscleHealth * 0.01f));
		for (int i = 0; i < holesPresent; i++)
		{
			Transform transform = Object.Instantiate(Minigame.game.spawnedMiniGame.GetChild(3), mask);
			transform.gameObject.SetActive(value: true);
			transform.localPosition = new Vector2(Random.Range(-300f, 300f), Random.Range(-370f, 370f));
			transform.localScale = new Vector2((Random.value > 0.5f) ? 1f : (-1f), (Random.value > 0.5f) ? 1f : (-1f));
			holes.Add(transform.GetComponent<RectTransform>());
		}
		Vector2 vector = Minigame.game.spawnedMiniGame.GetChild(1).localPosition;
		for (int j = 0; j < patchesLeft; j++)
		{
			Transform transform2 = Object.Instantiate(Minigame.game.spawnedMiniGame.GetChild(2), Minigame.game.spawnedMiniGame);
			transform2.gameObject.SetActive(value: true);
			transform2.localPosition = vector + new Vector2(Random.Range(-50f, 50f), Random.Range(-50f, 50f));
			patches.Add(transform2.GetComponent<RectTransform>());
		}
		if (!limb.broken)
		{
			return;
		}
		foreach (RectTransform hole in holes)
		{
			hole.GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 0.3f);
		}
		holes.Clear();
		startedBroken = true;
	}

	private bool GrabbedInPos()
	{
		if ((bool)grabbed && Mathf.Abs(grabbed.localPosition.y) < 50f && grabbed.localPosition.x < 470f)
		{
			return grabbed.localPosition.x > Mathf.Lerp(400f, -170f, limb.boneHealTimer * 0.01f);
		}
		return false;
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (limb.muscleHealth >= 99.9f || limb.dismembered || (startedBroken && !limb.broken))
		{
			Minigame.game.EndMinigame();
			return;
		}
		if (Minigame.game.handStartedClicking)
		{
			foreach (RaycastResult uiCast in uiCasts)
			{
				if (patches.Contains(uiCast.gameObject.GetComponent<RectTransform>()))
				{
					grabbed = uiCast.gameObject.GetComponent<RectTransform>();
					heldOffset = grabbed.anchoredPosition - Minigame.game.handPos;
					break;
				}
			}
		}
		if ((bool)grabbed)
		{
			grabbed.anchoredPosition = Minigame.game.handPos + heldOffset;
			bool flag = false;
			foreach (RectTransform hole in holes)
			{
				if (Vector2.Distance(grabbed.position, hole.position) < 20f)
				{
					flag = true;
					if (!closeHole)
					{
						Sound.Play("prosthesisclick", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
					}
					closeHole = hole;
					grabbed.position = hole.position;
				}
			}
			if (limb.broken && GrabbedInPos() && !lastInsideBroken)
			{
				Sound.Play("prosthesisclick", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
				lastInsideBroken = true;
			}
			if (!limb.broken || !GrabbedInPos())
			{
				lastInsideBroken = false;
			}
			if (!flag)
			{
				closeHole = null;
			}
		}
		if (Minigame.game.handStoppedClicking)
		{
			if ((bool)closeHole)
			{
				grabbed.GetComponent<Image>().raycastTarget = false;
				grabbed.GetComponent<Image>().color = Color.gray;
				patches.Remove(grabbed);
				holes.Remove(closeHole);
				grabbed.SetParent(mask, worldPositionStays: true);
				Sound.Play("prosthesisrepair", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 0.7f + patchPitch, noReverb: true);
				limb.muscleHealth += 100f / (float)maxHoles;
				Minigame.game.currentItem.condition -= 1f / (float)maxPatches - 0.0015f;
				if (Minigame.game.currentItem.condition < 0.01f)
				{
					Minigame.game.currentItem.condition = 0f;
				}
				patchPitch += 0.05f;
				closeHole = null;
			}
			if (limb.broken && GrabbedInPos())
			{
				patches.Remove(grabbed);
				grabbed.GetComponent<Image>().raycastTarget = false;
				grabbed.GetComponent<Image>().color = Color.gray;
				grabbed.SetParent(mask, worldPositionStays: true);
				Sound.Play("prosthesisrepair", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 0.7f + (100f - limb.boneHealTimer) * 0.01f * 0.7f, noReverb: true);
				limb.boneHealTimer -= 100f / (float)maxHoles;
				if (limb.boneHealTimer < 5f)
				{
					limb.MendBone();
				}
				Minigame.game.currentItem.condition -= 1f / (float)maxPatches - 0.0015f;
				if (Minigame.game.currentItem.condition < 0.01f)
				{
					Minigame.game.currentItem.condition = 0f;
				}
			}
			grabbed = null;
			if (patchesLeft == 0)
			{
				Minigame.game.currentItem.condition = 0f;
				Minigame.game.EndMinigame();
				return;
			}
		}
		brokeOverlay.fillAmount = limb.boneHealTimer * 0.01f * (limb.broken ? 1f : 0f);
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		Minigame.game.handVelocity *= 0.9f;
		Minigame.game.handPos = Vector2.Lerp(Minigame.game.handPos, Minigame.game.GetMousePos(), deltaTime * 10f);
	}
}
