using System.Linq;
using UnityEngine;

public class Container : MonoBehaviour
{
	public float maxWeight;

	public float maxWeightPerItem;

	public float encumberanceMult = 1f;

	private Item mItem;

	private bool playedBreakSound;

	private bool didAwake;

	public bool itemsVisible;

	public string[] tagRestriction;

	public int itemCount => base.transform.childCount;

	private void Awake()
	{
		if (tagRestriction == null)
		{
			tagRestriction = new string[0];
		}
		mItem = GetComponent<Item>();
		if (!didAwake)
		{
			didAwake = true;
			maxWeight += 0.001f;
			maxWeightPerItem += 0.001f;
		}
	}

	public static void UpdateItemLight(GameObject obj, bool inContainer)
	{
		if (obj.TryGetComponent<LightItem>(out var component))
		{
			component.inContainer = inContainer;
		}
	}

	public void UnloadAllItems()
	{
		if (itemCount == 0)
		{
			return;
		}
		foreach (Transform item in base.transform)
		{
			if (item.TryGetComponent<Item>(out var component))
			{
				item.transform.SetParent(null);
				component.rb.simulated = true;
				item.transform.position += Vector3.up * 1.5f;
				component.GetComponent<SpriteRenderer>().enabled = true;
				component.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
				UpdateItemLight(component.gameObject, inContainer: false);
			}
		}
	}

	public void ContainerBroke()
	{
		UnloadAllItems();
		if (!playedBreakSound)
		{
			playedBreakSound = true;
			Sound.Play("containerBreak", base.transform.position);
		}
	}

	public float GetHoldingWeight()
	{
		float num = 0f;
		foreach (Transform item in base.transform)
		{
			if (item.TryGetComponent<Item>(out var component))
			{
				num += component.totalWeight;
			}
		}
		return num;
	}

	public int GetActualItemCount()
	{
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.TryGetComponent<Item>(out var _))
			{
				num++;
			}
		}
		return num;
	}

	public bool AboveHoldingWeight()
	{
		return GetHoldingWeight() > maxWeight;
	}

	public float GetEncumberance()
	{
		return GetHoldingWeight() * encumberanceMult;
	}

	public bool CanHoldItem(Item item)
	{
		if (item.totalWeight <= maxWeightPerItem && GetHoldingWeight() + item.totalWeight <= maxWeight)
		{
			if (tagRestriction.Length != 0)
			{
				return tagRestriction.Intersect(item.Stats.GetTags()).Count() > 0;
			}
			return true;
		}
		return false;
	}

	public void LoadItem(Item item)
	{
		Container cont;
		if (item.TryGetComponent<Container>(out var component) && component.GetHoldingWeight() > 0f)
		{
			PlayerCamera.main.DoAlert(Locale.GetOther("alertcontainerstacking"));
		}
		else if (mItem.TryGetParentContainer(out cont))
		{
			PlayerCamera.main.DoAlert(Locale.GetOther("alertcontainerstackingalt"));
		}
		else if (!(item == mItem) && CanHoldItem(item) && Vector2.Distance(item.transform.position, base.transform.position) < 10f)
		{
			item.transform.SetParent(base.transform);
			item.rb.simulated = false;
			item.transform.position = base.transform.position;
			item.GetComponent<SpriteRenderer>().enabled = itemsVisible;
			if (itemsVisible)
			{
				item.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder;
				item.transform.localEulerAngles = new Vector3(0f, 0f, 180f);
			}
			if ((bool)item.GetComponent<Wearable>())
			{
				item.GetComponent<Wearable>().ClearSprites();
			}
			UpdateItemLight(item.gameObject, !itemsVisible);
		}
	}

	public void UnloadItem(Item item, Body body = null)
	{
		foreach (Transform item2 in base.transform)
		{
			if (item2.GetComponent<Item>() == item)
			{
				item2.transform.SetParent(null);
				item.rb.simulated = true;
				item2.transform.position += Vector3.up * 1.5f;
				item.GetComponent<SpriteRenderer>().enabled = true;
				item.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
				UpdateItemLight(item.gameObject, inContainer: false);
			}
		}
	}
}
