using UnityEngine;
using UnityEngine.Events;

public class TutorialCraftButtonBehaviour : MonoBehaviour
{
	public UnityEvent onReset;

	private void Awake()
	{
		onReset = new UnityEvent();
	}

	public void OnUse()
	{
		onReset.Invoke();
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Click);
	}
}
