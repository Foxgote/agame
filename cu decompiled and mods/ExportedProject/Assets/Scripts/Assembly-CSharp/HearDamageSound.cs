using UnityEngine;

public class HearDamageSound : MonoBehaviour
{
	private AudioSource source;

	private void Start()
	{
		source = GetComponent<AudioSource>();
	}

	private void Update()
	{
		source.volume = Mathf.Max(PlayerCamera.main.body.hearingLoss * 0.01f, 1f - PlayerCamera.main.body.brainHealth * 0.01f);
		if (!PlayerCamera.main.body.alive)
		{
			source.volume = 0f;
		}
		if (source.volume > 0f && !source.isPlaying)
		{
			source.Play();
		}
		if (source.volume <= 0f && source.isPlaying)
		{
			source.Stop();
		}
	}
}
