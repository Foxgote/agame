using TMPro;
using UnityEngine;

public class UILocalizer : MonoBehaviour
{
	public string key;

	public bool upper;

	private void Start()
	{
		if (TryGetComponent<TextMeshProUGUI>(out var component))
		{
			component.text = Locale.GetOther(key);
			if (upper)
			{
				component.text = component.text.ToUpper();
			}
		}
	}
}
