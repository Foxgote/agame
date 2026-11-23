using UnityEngine;

public class Damageable : MonoBehaviour
{
	private BuildingEntity build;

	private Item item;

	private void Awake()
	{
		build = GetComponent<BuildingEntity>();
		item = GetComponent<Item>();
	}

	public void Damage(float damage)
	{
		if ((bool)build)
		{
			build.health -= damage;
		}
		if ((bool)item)
		{
			item.condition -= damage * 0.005f;
		}
	}
}
