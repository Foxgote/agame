using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DislocationMinigame : Minigame
{
	public bool hasWrench;

	private Limb limb;

	private Vector2 boneVelocity;

	private RectTransform bone;

	private TextMeshProUGUI remainingText;

	private Vector2 FinishSpot
	{
		get
		{
			if (!limb.prostheticInfo.hasProsthetic)
			{
				return new Vector2(375f, 54.6f);
			}
			return new Vector2(375f, 3f);
		}
	}

	public override HandSpriteType HandType()
	{
		if (!hasWrench)
		{
			return HandSpriteType.Grasp;
		}
		return HandSpriteType.Wrench;
	}

	public override string GuideLocaleString()
	{
		if (!limb.prostheticInfo.hasProsthetic)
		{
			return "dislocationMinigameGuide";
		}
		return "dislocationMinigameGuideAlt";
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public DislocationMinigame(Limb limb, bool wrench = false)
	{
		hasWrench = wrench;
		this.limb = limb;
	}

	public static Vector2 GetRandomPointOnRightQuarter()
	{
		float f = UnityEngine.Random.Range(-MathF.PI / 3f, MathF.PI / 3f);
		float x = Mathf.Cos(f);
		float y = Mathf.Sin(f);
		return new Vector2(x, y);
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/DislocationMinigame");
		bone = Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<RectTransform>();
		bone.anchoredPosition = FinishSpot + GetRandomPointOnRightQuarter() * (limb.dislocationTimer * 5f);
		remainingText = Minigame.game.spawnedMiniGame.GetChild(3).GetComponent<TextMeshProUGUI>();
		if (limb.prostheticInfo.hasProsthetic)
		{
			Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/dislocationBoneBasePros");
			Minigame.game.spawnedMiniGame.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/dislocationBoneGhostPros");
			Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/dislocationBoneMovePros");
		}
	}

	public void CheckForHit(List<RaycastResult> uiCasts)
	{
		if (limb.body.averagePain > 75f || boneVelocity.magnitude > 60f || !Minigame.game.handClicking || Minigame.game.handVelocity.magnitude < 4f)
		{
			return;
		}
		foreach (RaycastResult uiCast in uiCasts)
		{
			if (uiCast.gameObject == bone.gameObject)
			{
				if (hasWrench)
				{
					boneVelocity += Minigame.game.handVelocity * 24f;
					limb.pain += UnityEngine.Random.Range(4f, 10f);
				}
				else
				{
					boneVelocity += Minigame.game.handVelocity * UnityEngine.Random.Range(0.7f, 1.3f) * 20f;
					limb.pain += UnityEngine.Random.Range(15f, 24f);
				}
				Minigame.game.handVelocity *= -0.2f;
				Minigame.game.ForceUngrab();
				Sound.Play("boneHit", Vector2.zero, twoDimensional: true, pitchShift: true, null, 0.4f);
				break;
			}
		}
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		CheckForHit(uiCasts);
		bone.anchoredPosition += boneVelocity * Time.deltaTime * 3.5f;
		boneVelocity = Vector2.Lerp(boneVelocity, Vector2.zero, Time.deltaTime * 3.5f * (limb.prostheticInfo.hasProsthetic ? 10f : 1f));
		bone.anchoredPosition = new Vector2(Mathf.Clamp(bone.anchoredPosition.x, -500f, 900f), Mathf.Clamp(bone.anchoredPosition.y, -500f, 500f));
		if (limb.body.averagePain > 75f)
		{
			remainingText.text = Locale.GetOther("toomuchpain");
		}
		else
		{
			remainingText.text = Mathf.RoundToInt(limb.dislocationTimer) + "%";
		}
		remainingText.color = new Color(1f, 1f - limb.dislocationTimer * 0.01f, 1f - limb.dislocationTimer * 0.01f);
		remainingText.enabled = !WorldGeneration.unchipped;
		limb.dislocationTimer = Mathf.Clamp(Vector2.Distance(bone.anchoredPosition, FinishSpot) * 0.2f, 0f, 100f);
		if (limb.dislocationTimer < 3f)
		{
			limb.UnDislocate();
			Minigame.game.EndMinigame();
		}
	}
}
