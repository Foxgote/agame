using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchScript : MonoBehaviour
{
	public float bleedCooldown = 5f;

	public float genericMessageTime;

	public float weightChangeTime;

	public float fibrillationCheckTime;

	private float lastWeight;

	private Talker talker;

	private bool didline;

	private BatteryItem batt;

	private Body body => PlayerCamera.main.body;

	private void Awake()
	{
		talker = GetComponent<Talker>();
		lastWeight = PlayerCamera.main.body.weightOffset;
		batt = GetComponent<BatteryItem>();
	}

	public IEnumerator ExpLine()
	{
		yield return new WaitForSeconds(8f);
		PlayerCamera.main.body.talker.Talk(Locale.GetCharacter("watchlike"));
	}

	private void Update()
	{
		if (!batt.hasCharge || Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) > 30f)
		{
			return;
		}
		bleedCooldown -= Time.deltaTime;
		weightChangeTime += Time.deltaTime;
		genericMessageTime += Time.deltaTime;
		fibrillationCheckTime += Time.deltaTime;
		if (!PlayerCamera.main.body.conscious)
		{
			return;
		}
		if (bleedCooldown <= 0f)
		{
			bleedCooldown = 30f;
			float num = body.totalBleedSpeed - body.bloodRegenSpeed;
			if (num > 0.01f)
			{
				float num2 = body.bloodVolume / num;
				if (num2 < 600f)
				{
					TimeSpan timeSpan = TimeSpan.FromSeconds(num2);
					talker.Talk(Locale.GetOther("watchbleedout") + timeSpan.ToString("mm\\ \\:\\ ss"));
				}
			}
		}
		if (genericMessageTime > 100f)
		{
			genericMessageTime = 0f;
			List<Limb> list = new List<Limb>();
			Limb[] limbs = PlayerCamera.main.body.limbs;
			foreach (Limb limb in limbs)
			{
				if (!limb.dismembered && limb.infected)
				{
					list.Add(limb);
				}
			}
			if (list.Count > 0)
			{
				string text = Locale.GetOther("watchinfected");
				int num3 = 0;
				foreach (Limb item in list)
				{
					text = text + item.fullName + ((num3 == list.Count - 1) ? "" : ", ");
					num3++;
				}
				talker.Talk(text);
			}
			else
			{
				limbs = PlayerCamera.main.body.limbs;
				foreach (Limb limb2 in limbs)
				{
					if (!limb2.dismembered && !limb2.prostheticInfo.hasProsthetic)
					{
						if (limb2.broken)
						{
							TimeSpan timeSpan2 = TimeSpan.FromSeconds(limb2.boneHealTimer / Limb.boneHealSpeed);
							talker.Talk(limb2.fullName + Locale.GetOther("watchlimbheal") + timeSpan2.ToString("mm\\ \\:\\ ss"));
							break;
						}
						if (limb2.dislocated)
						{
							TimeSpan timeSpan3 = TimeSpan.FromSeconds(limb2.dislocationTimer / Limb.dislocationHealSpeed);
							talker.Talk(limb2.fullName + Locale.GetOther("watchlimbheal") + timeSpan3.ToString("mm\\ \\:\\ ss"));
							break;
						}
					}
				}
			}
		}
		if (weightChangeTime > 600f)
		{
			weightChangeTime = 0f;
			float num4 = PlayerCamera.main.body.weightOffset - lastWeight;
			num4 /= 3f;
			num4 *= 1000f;
			talker.Talk(Locale.GetOther("watchweight") + ((num4 < 0f) ? Locale.GetOther("watchweightlost") : Locale.GetOther("watchweightgained")) + ": " + Mathf.RoundToInt(Mathf.Abs(num4)) + "g");
			lastWeight = PlayerCamera.main.body.weightOffset;
			PlayerCamera.main.body.happiness += 3.25f;
			if (!didline)
			{
				didline = true;
				StartCoroutine("ExpLine");
			}
		}
		if (fibrillationCheckTime > 20f)
		{
			fibrillationCheckTime = 0f;
			if (PlayerCamera.main.body.fibrillationProgress > 1f)
			{
				talker.Talk(Locale.GetOther("watchfibrillation") + Mathf.RoundToInt(PlayerCamera.main.body.fibrillationProgress) + "%");
			}
		}
	}
}
