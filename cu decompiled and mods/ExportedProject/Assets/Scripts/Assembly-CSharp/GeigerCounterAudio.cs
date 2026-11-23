using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class GeigerCounterAudio : MonoBehaviour
{
	public AudioClip[] radiationClips = new AudioClip[6];

	private AudioSource audioSource;

	private Item item;

	[JsonProperty]
	public bool active;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.loop = true;
		audioSource.playOnAwake = false;
		item = GetComponent<Item>();
		item.decayMultiplier = (active ? 1f : 0f);
	}

	private void Update()
	{
		if (!item.battery.hasCharge)
		{
			active = false;
		}
		if (!active)
		{
			audioSource.Stop();
			return;
		}
		if (!audioSource.isPlaying)
		{
			audioSource.Play();
		}
		float irradiateIntensity = PlayerCamera.main.irradiateIntensity;
		int clipIndex = GetClipIndex(irradiateIntensity);
		if (audioSource.clip != radiationClips[clipIndex])
		{
			audioSource.clip = radiationClips[clipIndex];
			audioSource.Play();
		}
	}

	private int GetClipIndex(float intensity)
	{
		if (intensity <= 0.01f)
		{
			return 0;
		}
		if (intensity >= 1f)
		{
			return 5;
		}
		if (intensity < 0.15f)
		{
			return 1;
		}
		if (intensity < 0.4f)
		{
			return 2;
		}
		if (intensity < 0.7f)
		{
			return 3;
		}
		return 4;
	}
}
