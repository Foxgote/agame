using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BandageMinigame : Minigame
{
	public delegate void OnBandageUse(float normalAngle);

	public float bandageAngle;

	public int lastActionAngle;

	public bool holdingBandage;

	public Image bandageSprite;

	public Image bleedSprite;

	public Transform bandageGrabReticle;

	public bool didWrap;

	public float sizeMultiplier = 0.9f;

	private Color color;

	public OnBandageUse OnUse;

	public RectTransform roll;

	public Limb limb;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Grasp;
	}

	public override string GuideLocaleString()
	{
		return "bandageMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public static Vector2 GetVectorFromAngle(float angle)
	{
		return new Vector2(Mathf.Sin(angle * (MathF.PI / 180f)), Mathf.Cos(angle * (MathF.PI / 180f)));
	}

	public static float GetAngleFromVector(Vector2 dir)
	{
		float num = Mathf.Atan2(dir.x, dir.y) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	public override float HandRotOffset()
	{
		return Minigame.game.handPos.x * 0.02f;
	}

	public BandageMinigame(OnBandageUse use, Color clr, Limb limb)
	{
		OnUse = use;
		color = clr;
		this.limb = limb;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/BandageMinigame");
		if (limb.prostheticInfo.hasProsthetic)
		{
			Minigame.game.EndMinigame();
			return;
		}
		bandageSprite = Minigame.game.spawnedMiniGame.GetChild(1).GetComponent<Image>();
		bandageGrabReticle = Minigame.game.spawnedMiniGame.GetChild(3);
		bandageSprite.color = color;
		roll = Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<RectTransform>();
		bleedSprite = Minigame.game.spawnedMiniGame.GetChild(4).GetComponent<Image>();
		roll.GetComponent<Image>().color = color;
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (Minigame.game.handStartedClicking)
		{
			holdingBandage = Vector2.Dot(GetVectorFromAngle(bandageAngle), Minigame.game.handPos.normalized) > 0.9f;
		}
		if (Minigame.game.handStoppedClicking)
		{
			holdingBandage = false;
		}
		bandageSprite.fillAmount = (didWrap ? 1f : (bandageAngle / 360f));
		bandageSprite.transform.localScale = new Vector3(sizeMultiplier, sizeMultiplier, 1f);
		bandageGrabReticle.eulerAngles = new Vector3(0f, 0f, 0f - bandageAngle);
		float num = 1f - (1f - Minigame.game.currentItem.condition) * (1f - Minigame.game.currentItem.condition);
		float num2 = Mathf.Lerp(253f, 390f, num);
		roll.localScale = Vector3.one * num;
		roll.anchoredPosition = GetVectorFromAngle(bandageAngle) * num2;
		roll.eulerAngles = new Vector3(0f, 0f, bandageAngle * -3f);
		(Sprite, Color) tuple = WoundView.view.BleedImageFromLimb(limb);
		bleedSprite.sprite = tuple.Item1;
		bleedSprite.SetNativeSize();
		bleedSprite.color = tuple.Item2;
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (holdingBandage)
		{
			Minigame.game.handVelocity = Vector2.Lerp(Minigame.game.handVelocity, Vector2.zero, deltaTime * 34f * limb.bandageMinigameSpeedMult);
			float angleFromVector = GetAngleFromVector(Minigame.game.handPos.normalized);
			float num = angleFromVector - bandageAngle;
			float num2 = angleFromVector - (float)lastActionAngle;
			if (num < 20f)
			{
				if (bandageAngle > 350f && (angleFromVector < 90f || angleFromVector == 360f))
				{
					bandageAngle -= 360f;
					lastActionAngle -= 360;
					didWrap = true;
					Sound.Play("bandage", Minigame.game.currentItem.transform.position);
					DoBandageAction();
				}
				else if (num2 > 20f)
				{
					DoBandageAction();
				}
				bandageAngle = angleFromVector;
			}
		}
		if (Vector2.Distance(Minigame.game.handPos, Vector2.zero) < 250f)
		{
			Minigame.game.handVelocity += Minigame.game.handPos.normalized * deltaTime * 25f;
			Minigame.game.handPos = Minigame.game.handPos.normalized * 250f;
			if (!holdingBandage)
			{
				Minigame.game.handVelocity *= 0.9f;
			}
		}
	}

	private void DoBandageAction()
	{
		OnUse(1f / 18f);
		lastActionAngle += 20;
		if (didWrap)
		{
			sizeMultiplier += 0.0015f;
		}
	}
}
