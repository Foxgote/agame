using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MedStationScript : MonoBehaviour
{
	public BuildingEntity build;

	private Talker talker;

	private LineRenderer line;

	private bool didHeal;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
		talker = GetComponent<Talker>();
		line = GetComponent<LineRenderer>();
		line.enabled = false;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!didHeal && collision.TryGetComponent<Body>(out var component))
		{
			didHeal = true;
			Sound.Play("medicalstation", base.transform.position);
			build.Backgroundify();
			StartCoroutine(HealBody(component));
		}
	}

	private IEnumerator HealBody(Body body)
	{
		float time = 0f;
		line.enabled = true;
		while (time < 3f)
		{
			time += Time.deltaTime;
			line.SetPosition(0, base.transform.position);
			line.SetPosition(1, body.limbs[1].transform.position);
			yield return null;
		}
		body.hunger = Mathf.Max(body.hunger, Mathf.Lerp(body.hunger, 100f, 0.5f));
		body.thirst = Mathf.Max(body.thirst, Mathf.Lerp(body.thirst, 100f, 0.5f));
		body.GetOrAddComponent<Painkillers>().opiateAmount += 30f;
		body.happiness += 4f;
		body.sicknessAmount *= 0.5f;
		body.stamina += 50f;
		body.energy += 30f;
		Limb[] limbs = body.limbs;
		foreach (Limb obj in limbs)
		{
			obj.bleedAmount *= 0.5f;
			obj.muscleHealth = Mathf.Lerp(obj.muscleHealth, 100f, 0.3f);
			obj.skinHealth = Mathf.Lerp(obj.skinHealth, 100f, 0.3f);
			obj.boneHealTimer *= 0.5f;
			obj.dislocationTimer *= 0.5f;
		}
		line.enabled = false;
		talker.Talk(Locale.GetOther("medstationcheer"));
	}
}
