using UnityEngine;
using UnityEngine.EventSystems;

public class WoundViewLimb : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler
{
	public int limb;

	public WoundView woundview;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!PlayerCamera.main.body.limbs[limb].dismembered)
		{
			woundview.limbLookingAt = limb;
			woundview.limbImageFlash[limb] = 1f;
		}
	}
}
