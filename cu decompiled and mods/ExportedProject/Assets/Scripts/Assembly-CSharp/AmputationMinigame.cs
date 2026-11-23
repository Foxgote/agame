using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AmputationMinigame : Minigame
{
	public Limb limb;

	public Image bloodImage;

	public float cutProgress;

	private AudioSource cutSource;

	public float maxHandPos => Mathf.Lerp(120f, -380f, cutProgress);

	private bool cutting => Minigame.game.handPos.y <= maxHandPos + 10f;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Blade;
	}

	public override string GuideLocaleString()
	{
		return "amputationMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public override float HandRotOffset()
	{
		return 100f;
	}

	public AmputationMinigame(Limb limb)
	{
		this.limb = limb;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/AmputationMinigame");
		bloodImage = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(0).GetComponent<Image>();
		cutSource = Minigame.game.spawnedMiniGame.GetComponent<AudioSource>();
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		cutSource.volume = (cutting ? Mathf.Clamp01(Mathf.Abs(Minigame.game.handVelocity.x * 0.1f)) : 0f);
		bloodImage.fillAmount = cutProgress;
		if (cutProgress >= 1f)
		{
			this.limb.Dismember();
			Limb[] connectedLimbs = this.limb.connectedLimbs;
			foreach (Limb limb in connectedLimbs)
			{
				if (!limb.dismembered)
				{
					limb.infected = false;
					limb.infectionAmount = 0f;
					limb.SetDisinfect(300f);
					limb.bleedAmount *= 0.3f;
					limb.muscleHealth = 100f;
					limb.skinHealth = 66f;
				}
			}
			this.limb.body.traumaAmount -= 30f;
			Minigame.game.EndMinigame();
		}
		if (cutting)
		{
			PlayerCamera.main.body.adrenaline = Mathf.Max(PlayerCamera.main.body.adrenaline, 60f);
			this.limb.pain += Mathf.Abs(Minigame.game.handVelocity.x) * Time.deltaTime * 0.4f;
			this.limb.skinHealth -= Mathf.Abs(Minigame.game.handVelocity.x) * Time.deltaTime;
			this.limb.muscleHealth -= Mathf.Abs(Minigame.game.handVelocity.x) * Time.deltaTime;
			this.limb.bleedAmount += Mathf.Abs(Minigame.game.handVelocity.x) * Time.deltaTime * 0.3f;
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (Minigame.game.handPos.y <= maxHandPos)
		{
			Minigame.game.handPos.y = maxHandPos;
		}
		if (cutting)
		{
			cutProgress += Mathf.Abs(Minigame.game.handVelocity.x) * 7E-05f;
		}
	}
}
