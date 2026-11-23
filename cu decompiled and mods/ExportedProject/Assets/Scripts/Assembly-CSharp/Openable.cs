using UnityEngine;

public class Openable : MonoBehaviour
{
	public bool instantOpen;

	public bool isKeypad;

	public float lockpickAnglePrecision;

	private string code;

	public void OnUse()
	{
		if (instantOpen)
		{
			GetComponent<BuildingEntity>().health = 0f;
			return;
		}
		if (isKeypad)
		{
			if (string.IsNullOrEmpty(code))
			{
				code = KeypadMinigame.GenerateCode();
			}
			MinigameBase.main.StartMinigame(new KeypadMinigame(GetComponent<BuildingEntity>(), code), null);
			return;
		}
		PlayerCamera.main.body.FindByIdThorough("lockpickingkit", out var it);
		if (PlayerCamera.main.body.skills.INT < 10)
		{
			it = null;
		}
		MinigameBase.main.StartMinigame(new LockpingMinigame(GetComponent<BuildingEntity>(), lockpickAnglePrecision * WorldGeneration.GetRunSettingFloat("lockpickprecision")), it);
	}
}
