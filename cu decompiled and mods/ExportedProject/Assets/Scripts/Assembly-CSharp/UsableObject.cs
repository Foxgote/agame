using UnityEngine;

public class UsableObject : MonoBehaviour
{
	public string toggleString;

	public string langToggleString;

	public bool didLangString;

	public float rangeMultiplier = 1f;

	private void Start()
	{
		if (!didLangString)
		{
			didLangString = true;
			toggleString = Locale.GetOther(langToggleString);
		}
	}
}
