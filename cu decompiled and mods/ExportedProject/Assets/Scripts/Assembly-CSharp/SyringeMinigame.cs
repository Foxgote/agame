using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SyringeMinigame : Minigame
{
	public delegate void OnSyringeUse(float speedMult);

	public RectTransform syringe;

	public Image syringeFill;

	public const float syringeInjectStartHeight = -219f;

	public const float syringeInjectEndHeight = -345f;

	private AudioSource injectSource;

	public OnSyringeUse OnUse;

	private bool holdingSyringe;

	private Vector2 syringeHoldOffset;

	private float lastXPos;

	private bool wasInjectingBefore;

	private Color clr;

	private Limb limb;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Grasp;
	}

	public override string GuideLocaleString()
	{
		return "syringeMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public override float HandRotOffset()
	{
		return 65f;
	}

	public SyringeMinigame(OnSyringeUse onUse, Limb limb, Color? color = null)
	{
		clr = color ?? ((Color)new Color32(0, 132, 173, byte.MaxValue));
		OnUse = onUse;
		if (!limb.prostheticInfo.hasProsthetic)
		{
			WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/injectionWound"), flip: false);
		}
		this.limb = limb;
	}

	public override void Start()
	{
		if (limb.prostheticInfo.hasProsthetic)
		{
			Minigame.game.EndMinigame();
			return;
		}
		Minigame.game.CreateScreen("Special/SyringeMinigame");
		syringe = Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<RectTransform>();
		syringeFill = syringe.GetChild(0).GetComponent<Image>();
		syringeFill.color = clr;
		injectSource = Minigame.game.spawnedMiniGame.GetComponent<AudioSource>();
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		syringeFill.fillAmount = Minigame.game.currentItem.condition;
		if (Minigame.game.handStartedClicking && new Rect(syringe.anchoredPosition - new Vector2(90f, 0f) * 0.5f, new Vector2(90f, 492f)).Contains(Minigame.game.handPos))
		{
			holdingSyringe = true;
			syringeHoldOffset = syringe.anchoredPosition - Minigame.game.handPos;
		}
		if (holdingSyringe)
		{
			syringe.anchoredPosition = Minigame.game.handPos + syringeHoldOffset;
			if (syringe.anchoredPosition.y < -219f)
			{
				syringe.anchoredPosition = new Vector2(lastXPos, syringe.anchoredPosition.y);
				float num = lastXPos - syringeHoldOffset.x - Minigame.game.handPos.x;
				syringe.eulerAngles = new Vector3(0f, 0f, num * 0.125f);
				if (Mathf.Abs(num) > 80f)
				{
					limb.shrapnel = Math.Max(limb.shrapnel, 1);
					limb.pain += 20f;
					Sound.Play("bullethit", Vector2.zero, twoDimensional: true);
					Minigame.game.EndMinigame();
					return;
				}
				if (!wasInjectingBefore)
				{
					wasInjectingBefore = true;
					Sound.Play("syringe", Vector2.zero, twoDimensional: true, pitchShift: false);
				}
				if (Minigame.game.currentItem.condition > 0f)
				{
					float num2 = Body.Remap(syringe.anchoredPosition.y, -219f, -345f, 0f, 1f);
					injectSource.volume = num2;
					OnUse(num2 * Time.deltaTime);
				}
				else
				{
					injectSource.volume = 0f;
				}
			}
			else
			{
				lastXPos = syringe.anchoredPosition.x;
				if (wasInjectingBefore)
				{
					wasInjectingBefore = false;
				}
				injectSource.volume = 0f;
				syringe.eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(syringe.eulerAngles.z, 0f, Time.deltaTime * 4f));
			}
			if (syringe.anchoredPosition.y < -345f)
			{
				syringe.anchoredPosition = new Vector2(lastXPos, -345f);
				Minigame.game.handPos.y = syringe.anchoredPosition.y - syringeHoldOffset.y;
				if (Minigame.game.handVelocity.y < 0f)
				{
					Minigame.game.handVelocity.y = 0f;
				}
			}
		}
		else
		{
			injectSource.volume = 0f;
		}
		injectSource.pitch = 1.6f - Minigame.game.currentItem.condition;
		if (Minigame.game.handStoppedClicking)
		{
			holdingSyringe = false;
		}
	}
}
