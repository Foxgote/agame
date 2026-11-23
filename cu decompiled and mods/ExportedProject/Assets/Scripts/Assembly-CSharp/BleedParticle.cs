using UnityEngine;

public class BleedParticle : MonoBehaviour
{
	private ParticleSystem part;

	private ParticleSystem.Particle[] particles;

	private GameObject wallBleed;

	private GameObject groundBleed;

	private byte spawned;

	public bool forceOn;

	public bool vomit;

	public bool isHuman;

	private int every;

	private void Start()
	{
		every = (vomit ? 1 : 3);
		part = GetComponent<ParticleSystem>();
		wallBleed = (vomit ? Resources.Load<GameObject>("wallvomit") : (isHuman ? Resources.Load<GameObject>("wallbloodhuman") : Resources.Load<GameObject>("wallblood")));
		groundBleed = (vomit ? Resources.Load<GameObject>("Special/blockvomit") : (isHuman ? Resources.Load<GameObject>("Special/blockbloodhuman") : Resources.Load<GameObject>("Special/blockblood")));
		particles = new ParticleSystem.Particle[part.main.maxParticles];
	}

	private void Update()
	{
		_ = part.emission;
		int num = part.GetParticles(particles);
		if (num <= 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			if (!(particles[i].remainingLifetime <= 0.02f))
			{
				continue;
			}
			spawned++;
			if (spawned >= every)
			{
				spawned = 0;
				Sound.Play("drip" + Random.Range(1, 25), particles[i].position, twoDimensional: false, pitchShift: true, null, 0.3f);
				Vector2 vector = particles[i].position + Vector3.down * 0.8f;
				if (WorldGeneration.world.GetBlock(vector) != 0 && WorldGeneration.world.GetBlock(vector + Vector2.up) == 0)
				{
					GameObject obj = Object.Instantiate(groundBleed, WorldGeneration.world.BlockToWorldPos(WorldGeneration.world.WorldToBlockPos(vector)), Quaternion.identity);
					obj.AddComponent<GroundBlood>();
					obj.transform.localScale = new Vector2(Random.Range(0.7f, 1.3f), Random.Range(0.94f, 1.06f));
					SpriteRenderer component = obj.GetComponent<SpriteRenderer>();
					component.flipX = Random.value > 0.5f;
					component.color = new Color(component.color.r, component.color.g, component.color.b, Random.Range(0.2f, 0.8f));
					Object.Destroy(obj, 120f);
				}
				else
				{
					GameObject obj2 = Object.Instantiate(wallBleed, particles[i].position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
					Object.Destroy(obj2, 120f);
					obj2.transform.localScale = new Vector2(Random.Range(0.3f, 1.2f), Random.Range(0.3f, 1.2f));
					obj2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Random.Range(0.4f, 1f));
				}
			}
			particles[i].startLifetime = -1f;
		}
	}
}
