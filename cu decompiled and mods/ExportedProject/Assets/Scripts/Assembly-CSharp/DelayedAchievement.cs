using UnityEngine;

public class DelayedAchievement : MonoBehaviour
{
	public float timer;

	public string ach = "";

	private void Update()
	{
		timer -= Time.deltaTime;
		if (timer <= 0f)
		{
			if (PlayerCamera.main.body.alive)
			{
				SteamManager.TryUnlockAchievement(ach);
			}
			Object.Destroy(this);
		}
	}
}
