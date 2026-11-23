using UnityEngine;

public class UITooltip : MonoBehaviour
{
	public string localeName;

	public string localeDesc;

	[HideInInspector]
	public string tipName;

	[HideInInspector]
	public string tipDesc;

	[HideInInspector]
	public bool skipLocale;

	private void Start()
	{
		if (!skipLocale)
		{
			tipName = Locale.GetOther(localeName);
			tipDesc = Locale.GetOther(localeDesc);
		}
	}
}
