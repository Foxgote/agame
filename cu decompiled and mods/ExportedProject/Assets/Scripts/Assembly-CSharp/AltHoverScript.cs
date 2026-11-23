using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AltHoverScript : MonoBehaviour
{
	public static int allocSize = 24;

	private List<ItemLabel> spawnedLabels = new List<ItemLabel>();

	private List<ItemLabel> activeLabels = new List<ItemLabel>();

	public static bool toggleActivated;

	private bool active;

	private PlayerCamera player => PlayerCamera.main;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		for (int i = 0; i < allocSize; i++)
		{
			GameObject gameObject = Utils.Create("Special/ItemLabel", base.transform);
			spawnedLabels.Add(gameObject.GetComponent<ItemLabel>());
			gameObject.SetActive(value: false);
		}
	}

	private void Update()
	{
		if (spawnedLabels.Count > 0)
		{
			foreach (ItemLabel spawnedLabel in spawnedLabels)
			{
				spawnedLabel.gameObject.SetActive(value: false);
			}
		}
		if (activeLabels.Count > 0)
		{
			activeLabels.Clear();
		}
		if (!toggleActivated)
		{
			active = Input.GetKey(KeyBinds.GetBind("altview"));
		}
		else if (Input.GetKeyDown(KeyBinds.GetBind("altview")))
		{
			active = !active;
		}
		if (!active)
		{
			return;
		}
		Vector2 pointA = body.transform.position + new Vector3(-1f, -1f) * 10f;
		Vector2 pointB = body.transform.position + new Vector3(1f, 1f) * 10f;
		Collider2D[] array = new Collider2D[allocSize];
		int num = Physics2D.OverlapAreaNonAlloc(pointA, pointB, array, LayerMask.GetMask("Item"));
		int num2 = 0;
		if (num > 0)
		{
			for (int i = 0; i < num; i++)
			{
				if (Vector2.Distance(array[i].transform.position, body.transform.position) < 10f && array[i].TryGetComponent<Item>(out var component))
				{
					spawnedLabels[i].gameObject.SetActive(value: true);
					spawnedLabels[i].SetItem(component);
					spawnedLabels[i].transform.position = Camera.main.WorldToScreenPoint(new Vector2(Mathf.Round(component.transform.position.x * 8f) / 8f, Mathf.Round(component.transform.position.y * 8f) / 8f));
					spawnedLabels[i].image.color = (spawnedLabels[i].hovered ? Color.gray : Color.white).WithAlpha(player.dragItem ? 0.2f : 1f);
					spawnedLabels[i].icon.color = (spawnedLabels[i].hovered ? Color.gray : Color.white).WithAlpha(player.dragItem ? 0.2f : 1f);
					spawnedLabels[i].hovered = false;
					num2++;
					activeLabels.Add(spawnedLabels[i]);
				}
			}
		}
		float num3 = 4f;
		float num4 = 5000f;
		activeLabels = (from x in activeLabels
			orderby x.rect.anchoredPosition.y descending, x.rect.anchoredPosition.x
			select x).ToList();
		List<Rect> list = new List<Rect>();
		for (int num5 = 0; num5 < activeLabels.Count; num5++)
		{
			ItemLabel itemLabel = activeLabels[num5];
			Vector2 anchoredPosition = itemLabel.rect.anchoredPosition;
			anchoredPosition.x = Mathf.Round(anchoredPosition.x);
			anchoredPosition.y = Mathf.Round(anchoredPosition.y);
			Rect rect = itemLabel.GetRect(anchoredPosition);
			bool flag;
			do
			{
				flag = false;
				for (int num6 = 0; num6 < list.Count; num6++)
				{
					Rect other = list[num6];
					if (rect.Overlaps(other))
					{
						flag = true;
						float num7 = other.yMin - rect.height - num3;
						anchoredPosition.y = num7 + rect.height * 0.5f;
						anchoredPosition.y = Mathf.Max(anchoredPosition.y, itemLabel.rect.anchoredPosition.y - num4);
						rect = itemLabel.GetRect(anchoredPosition);
					}
				}
			}
			while (flag);
			itemLabel.rect.anchoredPosition = anchoredPosition;
			list.Add(rect);
		}
	}
}
