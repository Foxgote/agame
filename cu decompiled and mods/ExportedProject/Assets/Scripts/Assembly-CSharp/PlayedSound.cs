using UnityEngine;

public class PlayedSound : MonoBehaviour
{
	private void Update()
	{
		if (!GetComponent<AudioSource>().isPlaying)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
