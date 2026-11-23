using UnityEngine;

public class Sound : MonoBehaviour
{
	public static AudioSource Play(AudioClip clip, Vector2 pos, bool twoDimensional = false, bool pitchShift = true, Transform follow = null, float volume = 1f, float pitch = 1f, bool noReverb = false, bool ignoreMixer = false)
	{
		if (clip != null)
		{
			GameObject gameObject = new GameObject("a", typeof(AudioSource));
			AudioSource component = gameObject.GetComponent<AudioSource>();
			gameObject.transform.position = pos;
			float num = 1f;
			if (pitchShift)
			{
				num = Random.Range(pitch - 0.15f, pitch + 0.15f);
				component.pitch = num;
			}
			else
			{
				component.pitch = pitch;
			}
			if ((bool)follow)
			{
				gameObject.transform.SetParent(follow);
			}
			if (twoDimensional)
			{
				component.spatialBlend = 0f;
			}
			else
			{
				component.spatialBlend = 1f;
			}
			component.dopplerLevel = 0f;
			if ((bool)WorldGeneration.world && !ignoreMixer)
			{
				component.outputAudioMixerGroup = WorldGeneration.world.soundMixerGroup;
			}
			component.volume = volume;
			component.clip = clip;
			component.bypassReverbZones = noReverb;
			component.PlayOneShot(clip);
			gameObject.AddComponent<PlayedSound>();
			return component;
		}
		return null;
	}

	public static AudioSource Play(string clip, Vector2 pos, bool twoDimensional = false, bool pitchShift = true, Transform follow = null, float volume = 1f, float pitch = 1f, bool noReverb = false, bool ignoreMixer = false)
	{
		return Play(Resources.Load<AudioClip>("Sounds/" + clip), pos, twoDimensional, pitchShift, follow, volume, pitch, noReverb, ignoreMixer);
	}
}
