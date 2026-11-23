using UnityEngine;

public class BioTerminalScript : MonoBehaviour
{
	private BuildingEntity building;

	private void Start()
	{
		building = GetComponent<BuildingEntity>();
	}

	public void OnUse()
	{
		bool flag = false;
		foreach (Item item in PlayerCamera.main.body.GetAllItemsThorough())
		{
			if (item.TryGetComponent<WaterContainerItem>(out var component) && component.AmountOf("redblood") >= 100f)
			{
				flag = true;
				component.Drain(component.CalculateDrainSingleLiquid("redblood", 100f));
				break;
			}
		}
		if (flag)
		{
			building.Backgroundify();
			Sound.Play("beep", base.transform.position);
			Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, 6f);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].TryGetComponent<BuildingEntity>(out var component2) && component2.id == "reinforceddoor")
				{
					component2.Backgroundify();
				}
			}
		}
		else
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
		}
	}
}
