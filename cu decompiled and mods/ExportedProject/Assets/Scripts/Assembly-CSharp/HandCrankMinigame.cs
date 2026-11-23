using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandCrankMinigame : Minigame
{
	private bool held;

	public RectTransform crank;

	public RectTransform handle;

	private AudioSource source;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Grasp;
	}

	public override string GuideLocaleString()
	{
		return "handCrankMinigameGuide";
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
		return Mathf.Atan2(0f - dir.x, dir.y) * 57.29578f;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/HandCrankMinigame");
		crank = Minigame.game.spawnedMiniGame.GetChild(0).GetComponent<RectTransform>();
		handle = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(1).GetComponent<RectTransform>();
		source = Minigame.game.spawnedMiniGame.GetComponent<AudioSource>();
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (Minigame.game.body.stamina < 15f)
		{
			Minigame.game.EndMinigame();
			return;
		}
		if (Minigame.game.handStartedClicking)
		{
			held = Vector2.Distance(Minigame.game.handPos, Minigame.game.spawnedMiniGame.InverseTransformPoint(handle.position)) < 100f;
		}
		if (Minigame.game.handStoppedClicking)
		{
			held = false;
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (held)
		{
			Minigame.game.handPos = Minigame.game.handPos.normalized * 412f;
			Minigame.game.handVelocity *= 0.9f;
			float z = crank.localEulerAngles.z;
			crank.localRotation = Quaternion.Euler(0f, 0f, GetAngleFromVector(Minigame.game.handPos.normalized));
			float num = Mathf.Abs(Mathf.DeltaAngle(crank.localEulerAngles.z, z));
			Minigame.game.currentItem.battery.DrainCharge((0f - num) * 3.3E-05f);
			Minigame.game.body.stamina -= num * 0.015f;
			source.volume = num * 0.1f;
			source.pitch = 0.75f + num * 0.08f;
		}
		else
		{
			source.volume = 0f;
		}
	}
}
