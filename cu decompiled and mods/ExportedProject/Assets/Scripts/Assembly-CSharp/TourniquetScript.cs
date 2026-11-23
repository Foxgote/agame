using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class TourniquetScript : MonoBehaviour
{
	[JsonProperty]
	public float condition;

	[JsonProperty]
	public float timeApplied;

	private Limb limb;

	public List<Limb> affectedLimbs;

	private Image img;

	private void Start()
	{
		limb = GetComponent<Limb>();
		affectedLimbs = GetLowerLimbs(limb);
		affectedLimbs.ForEach((Limb x) =>
		{
			x.blockedBleeding = true;
		});
		img = WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/tourniquetIcon"), flip: false, (Limb x) => !x.GetComponent<TourniquetScript>());
	}

	public void TakeOff()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("tourniquet"), base.transform.position, Quaternion.identity) as GameObject;
		gameObject.GetComponent<Item>().condition = condition;
		limb.body.AutoPickUpItem(gameObject.GetComponent<Item>());
		affectedLimbs.ForEach((Limb x) =>
		{
			x.blockedBleeding = false;
		});
		Object.Destroy(this);
	}

	private void Update()
	{
		timeApplied += Time.deltaTime;
		img.color = new Color(1f, 1f - timeApplied / 540f, 1f - timeApplied / 540f);
		if (limb.pain < 40f)
		{
			limb.pain += Time.deltaTime * 2f;
		}
		if (limb.isHead)
		{
			limb.body.respiratoryRate = 0f;
		}
		if (!(timeApplied > 540f))
		{
			return;
		}
		foreach (Limb affectedLimb in affectedLimbs)
		{
			affectedLimb.muscleHealth -= Time.deltaTime * 2f;
			if (affectedLimb.muscleHealth < 5f)
			{
				affectedLimb.infected = true;
				if (affectedLimb.infectionAmount < 10f)
				{
					affectedLimb.infectionAmount = 10f;
				}
			}
		}
	}

	public void OnDismember()
	{
		TakeOff();
	}

	public static List<Limb> GetLowerLimbs(Limb lm, List<Limb> limbs = null)
	{
		if (limbs == null)
		{
			limbs = new List<Limb>();
		}
		if (!limbs.Contains(lm))
		{
			limbs.Add(lm);
		}
		Limb[] connectedLimbs = lm.connectedLimbs;
		foreach (Limb limb in connectedLimbs)
		{
			if (limb.distanceToHeart > lm.distanceToHeart)
			{
				limbs = GetLowerLimbs(limb, limbs);
			}
		}
		return limbs;
	}
}
