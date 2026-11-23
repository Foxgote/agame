using UnityEngine;

public class DestroyWithChance : MonoBehaviour
{
	public float chanceToLive;

	private void Start()
	{
		if (Random.value > chanceToLive)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
