using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class LockpingMinigame : Minigame
{
	public RectTransform lockRect;

	public BuildingEntity toDestroy;

	public float correctAngle;

	public Vector2 lastHandPos;

	public bool clickingInside;

	public float lockProgress;

	private float timeWasStuck;

	private AudioSource pickLoop;

	private AudioSource breakSound;

	public float anglePrecision;

	public int pickLevel = -1;

	public float HandAngle => VectorToAngle(Minigame.game.handPos.normalized);

	public float HandDistance => Minigame.game.handPos.magnitude;

	public override HandSpriteType HandType()
	{
		if (pickLevel < 0)
		{
			return HandSpriteType.Point;
		}
		return HandSpriteType.Lockpick;
	}

	public override string GuideLocaleString()
	{
		return "lockpickMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return pickLevel >= 0;
	}

	public override float HandRotOffset()
	{
		return 0f;
	}

	public LockpingMinigame(BuildingEntity build, float anglePrecision)
	{
		toDestroy = build;
		this.anglePrecision = anglePrecision;
	}

	public static float VectorToAngle(Vector2 dir)
	{
		float num = Mathf.Atan2(dir.y, dir.x) * 57.29578f;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	public float MaxTurnProgress()
	{
		float num = Mathf.Abs(HandAngle - correctAngle);
		if (num < anglePrecision)
		{
			num = 0f;
		}
		return Mathf.Clamp01(1f - num / 90f);
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/LockpickMinigame");
		lockRect = Minigame.game.spawnedMiniGame.GetChild(1).GetComponent<RectTransform>();
		correctAngle = Random.Range(5f, 175f);
		AudioSource[] components = Minigame.game.spawnedMiniGame.GetComponents<AudioSource>();
		pickLoop = components[0];
		breakSound = components[1];
		Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0}{1:0.0} {2}", Locale.GetOther("lockprecision"), anglePrecision * 2f, Locale.GetOther("degree" + ((anglePrecision == 0.5f) ? "" : "s")));
		if ((bool)Minigame.game.currentItem)
		{
			pickLevel = Minigame.game.body.skills.INT - 10;
		}
		else
		{
			pickLevel = -1;
		}
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (!toDestroy)
		{
			Minigame.game.EndMinigame();
		}
		float num = pickLevel + 1;
		lockRect.anchoredPosition = Vector2.zero;
		if (Minigame.game.handStartedClicking)
		{
			lastHandPos = Minigame.game.handPos;
			clickingInside = HandAngle > 0f && HandAngle < 180f && HandDistance > 195f && HandDistance < 247f;
		}
		float num2 = 0f;
		if (clickingInside)
		{
			pickLoop.volume = Mathf.MoveTowards(pickLoop.volume, 1f, Time.deltaTime * 5f);
			lockProgress += Time.deltaTime * (0.66f + num * 0.065f);
			if (lockProgress >= MaxTurnProgress())
			{
				lockProgress = MaxTurnProgress();
				if (lockProgress == 1f)
				{
					toDestroy.health = 0f;
					Minigame.game.EndMinigame();
					Sound.Play("unlock", Minigame.game.body.transform.position, twoDimensional: true, pitchShift: false);
				}
				else
				{
					if (!breakSound.isPlaying)
					{
						breakSound.Play();
					}
					lockRect.anchoredPosition = Random.insideUnitCircle * 10f * timeWasStuck;
					num2 = Random.Range(-10f, 10f) * timeWasStuck;
					timeWasStuck += Time.deltaTime;
					anglePrecision += Time.deltaTime * 0.03f;
					Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("{0}{1:0.0} {2}", Locale.GetOther("lockprecision"), anglePrecision * 2f, Locale.GetOther("degree" + ((anglePrecision < 0.525f) ? "" : "s")));
					if (timeWasStuck > 0.5f)
					{
						if (pickLevel < 0)
						{
							Minigame.game.body.slots[Minigame.game.body.handSlot].limb.pain += 20f;
							if (!Minigame.game.body.GetAttackingLimb().prostheticInfo.hasProsthetic)
							{
								Minigame.game.body.clawHealth -= 15f;
							}
							else
							{
								Minigame.game.body.GetAttackingLimb().muscleHealth -= 4f;
							}
						}
						else
						{
							Minigame.game.currentItem.SetCondition(Minigame.game.currentItem.condition - 0.03f);
						}
						Sound.Play("gore2", Minigame.game.body.transform.position, twoDimensional: true);
						timeWasStuck = 0f;
						breakSound.Stop();
					}
				}
			}
		}
		else
		{
			lockProgress = Mathf.MoveTowards(lockProgress, 0f, Time.deltaTime * Mathf.Clamp(2f - num * 0.4f, 0.1f, 2f));
			pickLoop.volume = Mathf.MoveTowards(pickLoop.volume, 0f, Time.deltaTime * 10f);
		}
		lockRect.eulerAngles = new Vector3(0f, 0f, (0f - lockProgress) * 180f + num2);
		if (Minigame.game.handStoppedClicking)
		{
			clickingInside = false;
			timeWasStuck = 0f;
			breakSound.Stop();
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (clickingInside)
		{
			Minigame.game.handPos = lastHandPos;
			Minigame.game.handVelocity = Vector2.zero;
		}
	}
}
