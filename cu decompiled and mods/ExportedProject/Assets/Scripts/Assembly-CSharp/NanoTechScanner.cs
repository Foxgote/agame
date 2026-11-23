using UnityEngine;

public class NanoTechScanner : MonoBehaviour
{
	private bool didChipLine;

	private bool didInitialLine;

	private Talker talker;

	private void Start()
	{
		talker = GetComponent<Talker>();
	}

	private void Update()
	{
		bool flag = (bool)base.transform.parent && base.transform.parent.TryGetComponent<InventorySlot>(out var _);
		if ((PlayerCamera.main.chipHeight.HasValue && !didInitialLine && !WorldGeneration.world.generatingWorld) & flag)
		{
			didInitialLine = true;
			talker.Talk(Locale.GetOther("nanotechdepthinitial" + ((PlayerCamera.main.chipHeight.Value.x > 0f) ? "right" : "left")));
		}
		if ((PlayerCamera.main.chipHeight.HasValue && !didChipLine && !WorldGeneration.world.generatingWorld && base.transform.position.y < PlayerCamera.main.chipHeight.Value.y + 3f) & flag)
		{
			didChipLine = true;
			talker.Talk(Locale.GetOther("nanotechdepth"));
		}
		PlayerCamera.main.techScannerHeld = flag;
		if (WorldGeneration.world.generatingWorld)
		{
			didInitialLine = false;
			didChipLine = false;
		}
	}

	private void OnDestroy()
	{
		PlayerCamera.main.techScannerHeld = false;
	}
}
