using UnityEngine;

public class SurgicalDeviceBehavior : MonoBehaviour
{
	public void OnUse()
	{
		if (EndLabScript.instance.canStartSurgery)
		{
			MinigameBase.main.StartMinigame(new CargoMinigame(PlayerCamera.main.body.limbs[2]), null);
		}
		else
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
		}
	}
}
