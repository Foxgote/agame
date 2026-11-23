using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShrapnelMinigame : Minigame
{
	public bool hasTweezers;

	private List<RectTransform> objects;

	private RectTransform currentlyHeld;

	private Vector2 heldOffset;

	private Limb limb;

	private AudioSource source;

	public override HandSpriteType HandType()
	{
		if (!hasTweezers)
		{
			return HandSpriteType.Grasp;
		}
		return HandSpriteType.Tweezers;
	}

	public override string GuideLocaleString()
	{
		return "shrapnelMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public ShrapnelMinigame(Limb limb, bool tweezers = false)
	{
		hasTweezers = tweezers;
		this.limb = limb;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/ShrapnelMinigame");
		objects = new List<RectTransform>();
		for (int i = 0; i < 5; i++)
		{
			objects.Add(Minigame.game.spawnedMiniGame.GetChild(i).GetComponent<RectTransform>());
		}
		int num = 5 - limb.shrapnel;
		foreach (RectTransform @object in objects)
		{
			@object.transform.position += (Vector3)Random.insideUnitCircle * 20f;
			if (num > 0)
			{
				num--;
				@object.transform.position += Vector3.up * 8000f;
			}
			if (limb.prostheticInfo.hasProsthetic)
			{
				@object.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/shrapnelprosthesis");
			}
		}
		source = Minigame.game.spawnedMiniGame.GetComponent<AudioSource>();
	}

	public void BreakGrasp()
	{
		if ((bool)currentlyHeld)
		{
			currentlyHeld = null;
			Minigame.game.ForceUngrab();
			limb.skinHealth -= Random.Range(4f, 6f);
			limb.bleedAmount += Random.Range(0.4f, 1f);
			if (!limb.prostheticInfo.hasProsthetic)
			{
				limb.body.DoGoreSound();
			}
			else
			{
				Sound.Play("steel", limb.transform.position);
			}
			limb.pain += Random.Range(9f, 16f);
			if (limb.isHead && Random.value < 0.8f)
			{
				limb.body.brainHealth -= Random.value;
			}
		}
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if ((hasTweezers && !Minigame.game.currentItem) || limb.dismembered)
		{
			Minigame.game.EndMinigame();
			return;
		}
		if (Minigame.game.handStartedClicking && Minigame.game.handPos.y > -170f)
		{
			foreach (RaycastResult uiCast in uiCasts)
			{
				if (objects.Contains(uiCast.gameObject.GetComponent<RectTransform>()))
				{
					currentlyHeld = uiCast.gameObject.GetComponent<RectTransform>();
					heldOffset = currentlyHeld.anchoredPosition - Minigame.game.handPos;
					break;
				}
			}
		}
		source.volume = Mathf.MoveTowards(source.volume, ((bool)currentlyHeld && Mathf.Abs(Minigame.game.handVelocity.y) > 0.2f && currentlyHeld.anchoredPosition.y <= 35f && !limb.prostheticInfo.hasProsthetic) ? 1f : 0f, Time.deltaTime * 5f);
		if ((bool)currentlyHeld)
		{
			if (currentlyHeld.anchoredPosition.y > 35f)
			{
				currentlyHeld.anchoredPosition = Minigame.game.handPos + heldOffset;
			}
			else
			{
				currentlyHeld.anchoredPosition = new Vector2(currentlyHeld.anchoredPosition.x, Minigame.game.handPos.y + heldOffset.y);
				if (currentlyHeld.anchoredPosition.y < -364f)
				{
					currentlyHeld.anchoredPosition = new Vector2(currentlyHeld.anchoredPosition.x, -364f);
				}
				if (Mathf.Abs(Minigame.game.handVelocity.y) > 2.2f && !hasTweezers)
				{
					BreakGrasp();
					return;
				}
				if (Mathf.Abs(currentlyHeld.anchoredPosition.x - Minigame.game.handPos.x) > 42f)
				{
					BreakGrasp();
					return;
				}
			}
		}
		int num = 0;
		foreach (RectTransform @object in objects)
		{
			if (@object.anchoredPosition.y < 35f)
			{
				num++;
			}
		}
		limb.shrapnel = num;
		if (num == 0)
		{
			Minigame.game.EndMinigame();
		}
		if (Minigame.game.handStoppedClicking)
		{
			currentlyHeld = null;
		}
	}
}
