using System.Collections;
using UnityEngine;

public class CaveTickSpawner : MonoBehaviour
{
	private bool started;

	private void Update()
	{
		if (!WorldGeneration.world.worldExists)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (!started)
		{
			Limb limb = other.GetComponent<Limb>();
			if (!limb && other.TryGetComponent<Body>(out var component))
			{
				limb = component.limbs[0];
			}
			if ((bool)limb)
			{
				started = true;
				StartCoroutine(SpawnSpiders());
				GetComponent<ParticleSystem>().Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
				Object.Destroy(base.gameObject, 10f);
				Sound.Play("caveticks", Vector2.zero, twoDimensional: true, pitchShift: false, null, 0.7f);
			}
		}
	}

	private IEnumerator SpawnSpiders()
	{
		for (int i = 0; i < 16; i++)
		{
			Object.Instantiate(Resources.Load("cavetick"), base.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), Quaternion.Euler(0f, 0f, Random.value * 360f));
			yield return new WaitForSeconds(0.1f);
		}
		Object.Destroy(this);
	}
}
