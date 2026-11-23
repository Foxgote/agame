using System.Collections;
using Newtonsoft.Json;
using UnityEngine;

[JsonObject(MemberSerialization.OptIn)]
[Saveable]
public class MindwipeScript : MonoBehaviour
{
	[JsonProperty]
	public bool active;

	private Body body;

	private void Awake()
	{
		body = GetComponent<Body>();
	}

	private void Start()
	{
		if (active)
		{
			(Object.Instantiate(Resources.Load("Special/MindwipeVignette"), PlayerCamera.main.mainCanvas.transform) as GameObject).transform.SetAsFirstSibling();
			return;
		}
		StartCoroutine("WipeRoutine");
		Sound.Play("mindwipe", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
	}

	private void Update()
	{
		if (active)
		{
			body.traumaAmount = Mathf.MoveTowards(body.traumaAmount, 0f, Time.deltaTime * 5f);
			body.happiness = Mathf.MoveTowards(body.happiness, 0f, Time.deltaTime * 5f);
			if (body.brainHealth > 96f)
			{
				body.brainHealth = Mathf.MoveTowards(body.brainHealth, 96f, Time.deltaTime * 0.1f);
			}
		}
	}

	public IEnumerator WipeRoutine()
	{
		yield return new WaitForSeconds(2f);
		float timer = 0f;
		Limb[] limbs;
		while (timer < 3f)
		{
			body.Ragdoll();
			body.shock = 20f;
			body.hearingLoss += Time.deltaTime * 50f;
			PlayerCamera.main.bonusAbber += Time.deltaTime * 8f;
			limbs = body.limbs;
			foreach (Limb obj in limbs)
			{
				obj.rb.angularVelocity += Random.Range(-400f, 400f);
				obj.rb.velocity += Random.insideUnitCircle * 2f;
				obj.pain += Time.deltaTime * 100f;
			}
			timer += Time.deltaTime;
			yield return null;
		}
		limbs = body.limbs;
		for (int i = 0; i < limbs.Length; i++)
		{
			limbs[i].pain = 0f;
		}
		body.consciousness = 0f;
		body.energy = 0f;
		body.sleeping = true;
		body.hearingLoss = 80f;
		if (body.alive)
		{
			body.brainHealth += 50f;
		}
		body.strokeAmount = 0f;
		foreach (Recipe recipe in Recipes.recipes)
		{
			recipe.hasMadeBefore = false;
		}
		body.skills.INT = 0;
		body.skills.expINT = 0f;
		body.skills.UpdateExpBoundaries();
		MusicManager.main.StopSong();
		(Object.Instantiate(Resources.Load("Special/MindwipeVignette"), PlayerCamera.main.mainCanvas.transform) as GameObject).transform.SetAsFirstSibling();
		active = true;
		SteamManager.TryUnlockAchievement("MINDWIPE");
	}
}
