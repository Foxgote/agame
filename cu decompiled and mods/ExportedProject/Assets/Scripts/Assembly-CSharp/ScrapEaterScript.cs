using UnityEngine;

public class ScrapEaterScript : MonoBehaviour
{
	public float scrapAmount;

	public static float target = 1.5f;

	public BuildingEntity build;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
	}

	private void Update()
	{
		build.description = Locale.GetBuilding(build.id + "dsc").Replace("<>", Mathf.RoundToInt(scrapAmount / target * 100f).ToString());
	}

	public void OnUse()
	{
		if (!PlayerCamera.main.body.FindByIdThorough("scrapmetal", out var it))
		{
			return;
		}
		scrapAmount += it.condition;
		it.condition = 0f;
		if (!(scrapAmount >= target))
		{
			return;
		}
		build.Backgroundify();
		Collider2D[] array = Physics2D.OverlapCircleAll(base.transform.position, 2f);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].TryGetComponent<BuildingEntity>(out var component) && component.id == "reinforceddoor")
			{
				component.Backgroundify();
			}
		}
		Sound.Play("beep", base.transform.position);
	}
}
