using System.Collections.Generic;
using UnityEngine;

public class RecipeResult
{
	public string id;

	public bool isLiquid;

	public int amount = 1;

	public float resultCondition = 1f;

	public bool dontDrainResultLiquid;

	public void SpawnResult(int recipeInt)
	{
		int num = PlayerCamera.main.body.skills.INT - recipeInt;
		float num2 = 1f;
		if (num < 0 && Random.value < 0.5f)
		{
			switch (num)
			{
			case -1:
				break;
			case -3:
			{
				Body body = PlayerCamera.main.body;
				body.DoGoreSound();
				for (int i = 5; i <= 8; i += 3)
				{
					body.limbs[i].pain += 40f;
					body.limbs[i].skinHealth -= 15f;
					body.limbs[i].bleedAmount += Random.Range(2f, 5f);
				}
				return;
			}
			default:
				return;
			}
			num2 = Random.Range(0.2f, 0.9f);
		}
		if (id == "titaniumslab")
		{
			SteamManager.TryUnlockAchievement("HEAVYMETAL");
		}
		for (int j = 0; j < amount; j++)
		{
			if (isLiquid)
			{
				bool flag = false;
				foreach (Item item in PlayerCamera.main.body.GetAllItemsThorough())
				{
					if (item.TryGetComponent<WaterContainerItem>(out var component) && component.stack.Count == 1 && component.stack[0].liquidId == id && component.SpaceLeft >= resultCondition * num2)
					{
						component.AddLiquid(id, resultCondition * num2);
						flag = true;
						return;
					}
				}
				if (!flag)
				{
					GameObject gameObject = Utils.Create("craftingbottle", PlayerCamera.main.body.transform.position, 0f);
					Item component2 = gameObject.GetComponent<Item>();
					component2.condition = resultCondition;
					PlayerCamera.main.body.AutoPickUpItem(component2);
					component2.GetComponent<WaterContainerItem>().AddLiquid(id, resultCondition * num2);
					Object.Destroy(gameObject, 300f);
				}
			}
			else
			{
				Item component3 = Utils.Create(id, PlayerCamera.main.body.transform.position, 0f).GetComponent<Item>();
				component3.condition = resultCondition * num2;
				PlayerCamera.main.body.AutoPickUpItem(component3);
				if ((bool)component3.battery)
				{
					component3.battery.UnloadBattery(skipSpawn: true);
				}
				if (!dontDrainResultLiquid && component3.TryGetComponent<WaterContainerItem>(out var component4))
				{
					component4.stack = new List<LiquidStack>();
					component3.condition = 0f;
				}
			}
		}
	}
}
