using UnityEngine;

public class PantSound : MonoBehaviour
{
	private AudioSource source;

	private Body body;

	public AudioClip[] painSounds;

	public AudioClip[] barkSounds;

	public AudioClip[] yawnSounds;

	public float painTime;

	public float yawnTime;

	private void Start()
	{
		source = base.gameObject.AddComponent<AudioSource>();
		source.spatialBlend = 1f;
		source.dopplerLevel = 0f;
		source.volume = 0f;
		source.loop = true;
		source.outputAudioMixerGroup = WorldGeneration.world.soundMixerGroup;
		source.clip = Resources.Load<AudioClip>("Sounds/pant");
		source.Play();
		body = GetComponent<Body>();
	}

	public void Bark()
	{
		if (!(body.eatTime > 0f) && body.conscious)
		{
			body.eatTime = 0.5f;
			Sound.Play(barkSounds.PickRandom(), base.transform.position, twoDimensional: false, pitchShift: true, null, 1f, body.voicePitch);
		}
	}

	public void TryGrowl()
	{
		if (body.charType != 2 && body.conscious && body.totalHappiness < -15f && Random.value < (0f - body.totalHappiness) * 0.01f)
		{
			Sound.Play("growl" + Random.Range(1, 13), base.transform.position, twoDimensional: false, pitchShift: true, base.transform, 1f, body.voicePitch);
		}
	}

	private void Update()
	{
		if (!body.conscious)
		{
			source.volume = 0f;
			return;
		}
		source.volume = ((body.breathing && !body.inWater) ? ((70f - body.stamina) * 0.01f) : 0f);
		if (body.stamina < 40f)
		{
			body.eatTime = 0.5f;
		}
		source.pitch = (body.breathing ? (1.25f - body.stamina * 0.004f) : 1f) * body.voicePitch;
		if (body.averagePain > 20f)
		{
			if (painTime < 0f && body.conscious)
			{
				painTime = Random.Range(30f, 50f);
				if (!Body.censorPain)
				{
					Sound.Play(painSounds[Random.Range(0, painSounds.Length)], base.transform.position, twoDimensional: false, pitchShift: true, base.transform, 1f, body.voicePitch);
				}
				body.eatTime = 1.2f;
			}
			painTime -= Time.deltaTime * (body.averagePain * 0.1f);
		}
		else
		{
			painTime -= Time.deltaTime;
		}
		if (body.energy < 35f)
		{
			yawnTime += Time.deltaTime;
			if (yawnTime > 60f)
			{
				yawnTime = Random.Range(-20f, 20f);
				Sound.Play(yawnSounds.PickRandom(), base.transform.position, twoDimensional: false, pitchShift: true, base.transform, 1f, body.voicePitch);
				body.eatTime = 2f;
			}
		}
	}
}
