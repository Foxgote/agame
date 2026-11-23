using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class AEDMinigame : Minigame
{
	private List<RectTransform> pads;

	private static Vector2[] desiredSpots = new Vector2[2]
	{
		new Vector2(-296f, 153f),
		new Vector2(273f, -141f)
	};

	private int state;

	private RectTransform currentlyHeld;

	private RectTransform button;

	private Vector2 heldOffset;

	public Limb limb;

	private TextMeshProUGUI text;

	private float timer;

	public override HandSpriteType HandType()
	{
		if (state <= 1)
		{
			return HandSpriteType.Grasp;
		}
		return HandSpriteType.Point;
	}

	public override string GuideLocaleString()
	{
		return "aedMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public AEDMinigame(Limb limb)
	{
		this.limb = limb;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/AEDMinigame");
		pads = new List<RectTransform>();
		for (int i = 1; i < 3; i++)
		{
			pads.Add(Minigame.game.spawnedMiniGame.GetChild(i).GetComponent<RectTransform>());
		}
		foreach (RectTransform pad in pads)
		{
			pad.localPosition = Random.insideUnitCircle * 150f;
		}
		text = Minigame.game.spawnedMiniGame.GetChild(4).GetComponent<TextMeshProUGUI>();
		button = Minigame.game.spawnedMiniGame.GetChild(3).GetComponent<RectTransform>();
		Minigame.game.currentItem.battery.DrainCharge(0.01f);
		Sound.Play("aedstart", Vector2.zero, twoDimensional: true, pitchShift: false);
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (!Minigame.game.currentItem.battery.hasCharge)
		{
			Minigame.game.EndMinigame();
			return;
		}
		if (Minigame.game.handStartedClicking)
		{
			foreach (RaycastResult uiCast in uiCasts)
			{
				if (pads.Contains(uiCast.gameObject.GetComponent<RectTransform>()))
				{
					currentlyHeld = uiCast.gameObject.GetComponent<RectTransform>();
					heldOffset = currentlyHeld.anchoredPosition - Minigame.game.handPos;
					break;
				}
			}
		}
		if (state == 0)
		{
			float num = 0f;
			for (int i = 0; i < pads.Count; i++)
			{
				num = Mathf.Max(num, Vector2.Distance(pads[i].localPosition, desiredSpots[i]));
			}
			if (!currentlyHeld && num < 150f)
			{
				state = 1;
				Sound.Play("aedanalyze", Vector2.zero, twoDimensional: true, pitchShift: false);
			}
			text.text = Locale.GetOther("aedstate0");
		}
		if (state == 1)
		{
			timer += Time.deltaTime;
			if (timer > 4.25f)
			{
				if (limb.body.fibrillationProgress > 0f || limb != limb.body.limbs[1])
				{
					state = 2;
					Sound.Play("aedstart", Vector2.zero, twoDimensional: true, pitchShift: false);
				}
				else
				{
					state = 5;
					Sound.Play("aedfail", Vector2.zero, twoDimensional: true, pitchShift: false);
				}
				Minigame.game.currentItem.battery.DrainCharge(0.02f);
				timer = 0f;
				Minigame.game.UpdateHandSprite(reset: true);
			}
			text.text = Locale.GetOther("aedstate1") + "...".Substring(2 - (int)(Time.time * 3f) % 3);
		}
		if (state >= 2 && state != 3)
		{
			text.text = Locale.GetOther("aedstate" + state);
			if (Minigame.game.handStartedClicking)
			{
				foreach (RaycastResult uiCast2 in uiCasts)
				{
					if (uiCast2.gameObject == button.gameObject)
					{
						state = 3;
						Sound.Play("aedcharge", Vector2.zero, twoDimensional: true, pitchShift: false);
						break;
					}
				}
			}
		}
		if (state == 3)
		{
			timer += Time.deltaTime;
			if (timer > 3.5f)
			{
				state = 4;
				Minigame.game.currentItem.Defibrillate(new Item.DefibInfo
				{
					chance = 1f,
					limb = limb
				});
				Minigame.game.currentItem.battery.DrainCharge(0.14f);
				timer = 0f;
			}
			text.text = Locale.GetOther("aedstate3") + "...".Substring(2 - (int)(Time.time * 3f) % 3);
		}
		if (Minigame.game.handStoppedClicking)
		{
			currentlyHeld = null;
		}
		if ((bool)currentlyHeld && state == 0)
		{
			currentlyHeld.anchoredPosition = Minigame.game.handPos + heldOffset;
		}
	}
}
