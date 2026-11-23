using UnityEngine;

public class DrillPod : MonoBehaviour
{
	private float timeHeld;

	private bool didTeleport;

	private bool working;

	private Body body => PlayerCamera.main.body;

	private void Update()
	{
		if (working && !didTeleport && Mathf.Abs(body.transform.position.x - base.transform.position.x) < 1.5f && Mathf.Abs(body.transform.position.y - base.transform.position.y) < 5f && body.crouching)
		{
			timeHeld += Time.deltaTime;
			PlayerCamera.main.bonusActionBarTime = timeHeld * 0.2f;
			if (timeHeld > 5f && !didTeleport)
			{
				didTeleport = true;
				WorldGeneration.world.doPod = true;
				PlayerCamera.main.bonusActionBarTime = 0f;
				SteamManager.TryUnlockAchievement("EXPRESSROUTE");
				WorldGeneration.world.StartCoroutine("RegenerateWorld", true);
				Sound.Play("drillpoduse", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
			}
		}
		else if (timeHeld > 0f)
		{
			PlayerCamera.main.bonusActionBarTime = 0f;
			timeHeld = 0f;
		}
	}

	public void OnUse()
	{
		if (PlayerCamera.main.body.FindByIdThorough("drillrepairkit", out var it))
		{
			Object.Destroy(it.gameObject);
			Object.Destroy(GetComponent<UsableObject>());
			working = true;
			Sound.Play("drillpodrepair", base.transform.position, twoDimensional: false, pitchShift: false);
			GetComponent<BuildingEntity>().description = Locale.GetBuilding("drillpoddscfixed");
		}
	}
}
