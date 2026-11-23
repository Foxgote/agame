using UnityEngine;

public class OnLimbWound : MonoBehaviour
{
	public WoundView.LimbCondition cond;

	public Limb limb;

	private void Update()
	{
		if (cond(limb))
		{
			Object.Destroy(base.gameObject);
		}
	}
}
