using UnityEngine;

public class LifepodButton : MonoBehaviour
{
	public LifepodController controller;

	public int type;

	public void OnUse()
	{
		if (type == 0)
		{
			controller.ToggleHeatState();
		}
		else
		{
			controller.ActivateShower();
		}
	}
}
