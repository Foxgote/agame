using UnityEngine;

public class MoodChangeSounds : MonoBehaviour
{
	public AudioClip[] sounds;

	private float lastHunger;

	private float lastThirst;

	private float lastMood;

	private Body body => PlayerCamera.main.body;

	private void Update()
	{
		float num = body.hunger - lastHunger;
		float num2 = body.thirst - lastThirst;
		float num3 = body.happiness - lastMood;
		if (num >= 1f)
		{
			Play(0);
		}
		if (num <= -1f)
		{
			Play(1);
		}
		if (num2 >= 1f)
		{
			Play(2);
		}
		if (num2 <= -1f)
		{
			Play(3);
		}
		if (num3 >= 0.5f)
		{
			Play(4);
		}
		if (num3 <= -0.5f)
		{
			Play(5);
		}
		lastHunger = body.hunger;
		lastThirst = body.thirst;
		lastMood = body.happiness;
	}

	public void Play(int i)
	{
		if (!WorldGeneration.world.generatingWorld && body.alive)
		{
			Sound.Play(sounds[i], Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.5f, 1f, noReverb: true, ignoreMixer: true);
		}
	}
}
