using UnityEngine;
using UnityEngine.UI;

public class ItemLabel : MonoBehaviour
{
	public Image icon;

	public Image image;

	public Item refItem;

	public UITooltip tip;

	public RectTransform rect;

	public bool hovered;

	public void SetItem(Item item)
	{
		refItem = item;
		icon.sprite = item.GetComponent<SpriteRenderer>().sprite;
		icon.rectTransform.sizeDelta = PlayerCamera.ImageSizeDelta(icon.sprite.texture, 5f, 40f);
		tip.skipLocale = true;
		image.raycastTarget = !PlayerCamera.main.dragItem;
	}

	public Rect GetRect(Vector2 pos)
	{
		return new Rect(pos.x - rect.sizeDelta.x * 0.5f, pos.y - rect.sizeDelta.y * 0.5f, rect.sizeDelta.x, rect.sizeDelta.y);
	}
}
