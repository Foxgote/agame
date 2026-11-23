using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ManualDefibMinigame : Minigame
{
	public Limb limb;

	private bool holdingDial;

	private List<TextMeshProUGUI> texts;

	private ECGVisualizer ecg;

	private RectTransform dial;

	private RectTransform charge;

	private RectTransform shock;

	private FilledImagePP battery;

	private Image chargingImage;

	private bool onTorso;

	private int shockCount;

	private float currentCharge;

	private float desiredCharge;

	private bool charging;

	private float rangeOffset;

	public override HandSpriteType HandType()
	{
		if (!(Minigame.game.handPos.x > -60f))
		{
			return HandSpriteType.Grasp;
		}
		return HandSpriteType.Point;
	}

	public override string GuideLocaleString()
	{
		return "manualDefibMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return true;
	}

	public ManualDefibMinigame(Limb limb)
	{
		this.limb = limb;
	}

	public float GetEstimationRange()
	{
		float num = 70f - limb.body.skills.INTFrom10 * 5f;
		if (num < 5f)
		{
			num = 5f;
		}
		return num;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/ManualDefibMinigame");
		ecg = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(4).GetComponent<ECGVisualizer>();
		if (limb != limb.body.limbs[1])
		{
			ecg.followBody = false;
			ecg.writeHeight = 0f;
			onTorso = false;
		}
		else
		{
			onTorso = true;
		}
		dial = Minigame.game.spawnedMiniGame.GetChild(1).GetComponent<RectTransform>();
		charge = Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<RectTransform>();
		shock = Minigame.game.spawnedMiniGame.GetChild(3).GetComponent<RectTransform>();
		texts = new List<TextMeshProUGUI>();
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>());
		battery = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(3).GetComponent<FilledImagePP>();
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(5).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(6).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(7).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(8).GetComponent<TextMeshProUGUI>());
		texts.Add(Minigame.game.spawnedMiniGame.GetChild(0).GetChild(9).GetComponent<TextMeshProUGUI>());
		chargingImage = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(10).GetComponent<Image>();
		desiredCharge = 100f;
		rangeOffset = Random.Range(-1f, 1f);
	}

	private float AngleToChargeAmount(float angle)
	{
		return angle.Remap(-170f, 170f, 200f, 10f);
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (holdingDial && Vector2.Distance(Minigame.game.handPos, dial.localPosition) > 75f)
		{
			Minigame.game.handPos = (Minigame.game.handPos - (Vector2)dial.localPosition).normalized * 75f + (Vector2)dial.localPosition;
		}
	}

	public float ChanceToWork()
	{
		return 1f - Mathf.Abs(limb.body.fibrillationProgress - currentCharge * 0.5f) / 40f;
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (!Minigame.game.currentItem.battery.hasCharge)
		{
			Minigame.game.EndMinigame();
			return;
		}
		Minigame.game.currentItem.battery.DrainCharge(Time.deltaTime / 800f);
		if (Minigame.game.handStartedClicking)
		{
			foreach (RaycastResult uiCast in uiCasts)
			{
				if (uiCast.gameObject == dial.gameObject)
				{
					holdingDial = true;
					break;
				}
				if (uiCast.gameObject == charge.gameObject)
				{
					charging = !charging;
					break;
				}
				if (uiCast.gameObject == shock.gameObject && currentCharge >= 10f)
				{
					shockCount++;
					Minigame.game.currentItem.Defibrillate(new Item.DefibInfo
					{
						chance = ChanceToWork(),
						limb = limb
					});
					Minigame.game.currentItem.battery.DrainCharge(currentCharge / 4000f);
					currentCharge = 0f;
					charging = false;
					Sound.Play("manualdefib", Vector2.zero, twoDimensional: true, pitchShift: false);
					break;
				}
			}
		}
		if (Minigame.game.handStoppedClicking)
		{
			holdingDial = false;
		}
		if (holdingDial)
		{
			float angleToPoint = GetAngleToPoint(Minigame.game.handPos - (Vector2)dial.localPosition);
			angleToPoint = Mathf.DeltaAngle(0f, angleToPoint);
			angleToPoint = Mathf.Clamp(angleToPoint, -170f, 170f);
			dial.localEulerAngles = new Vector3(0f, 0f, angleToPoint);
			desiredCharge = AngleToChargeAmount(angleToPoint);
		}
		chargingImage.enabled = charging;
		if (charging && currentCharge < desiredCharge)
		{
			currentCharge = Mathf.MoveTowards(currentCharge, desiredCharge, Time.deltaTime * 40f);
		}
		if (charging && currentCharge >= desiredCharge)
		{
			charging = false;
		}
		texts[0].text = shockCount.ToString();
		texts[1].text = Mathf.RoundToInt(currentCharge).ToString();
		texts[2].text = Mathf.RoundToInt(desiredCharge).ToString();
		texts[5].text = Mathf.RoundToInt(desiredCharge).ToString();
		battery.fillAmount = Minigame.game.currentItem.condition;
		if (onTorso)
		{
			texts[3].text = Mathf.RoundToInt(limb.body.heartRate).ToString();
			texts[6].text = limb.body.bloodPressureReadout;
			texts[7].text = Mathf.RoundToInt(limb.body.bloodOxygen) + "%";
			float num = limb.body.fibrillationProgress * 2f - GetEstimationRange();
			float num2 = limb.body.fibrillationProgress * 2f + GetEstimationRange();
			num += rangeOffset * GetEstimationRange();
			num2 += rangeOffset * GetEstimationRange();
			if (num < 10f)
			{
				num = 10f;
			}
			if (num2 > 200f)
			{
				num2 = 200f;
			}
			if (num2 < 10f)
			{
				num2 = 10f;
			}
			if (num > 200f)
			{
				num = 200f;
			}
			texts[4].text = Mathf.RoundToInt(num) + "-" + Mathf.RoundToInt(num2);
		}
		else
		{
			texts[3].text = "0";
			texts[6].text = "n/a";
			texts[7].text = "n/a";
			texts[4].text = "n/a";
		}
	}

	private float GetAngleToPoint(Vector2 point)
	{
		Vector2 normalized = point.normalized;
		return Mathf.Atan2(normalized.y, normalized.x) * 57.29578f - 90f;
	}
}
