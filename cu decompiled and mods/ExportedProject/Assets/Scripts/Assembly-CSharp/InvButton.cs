using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InvButton : MonoBehaviour
{
	public int slot;

	public Image itemImg;

	public Image itemCond;

	public Image locked;

	public Image waterFill;

	public float maxImageSize;

	private UITooltip tooltip;

	private Image image;

	public Item refItem;

	public bool woundView;

	[HideInInspector]
	public bool hovered;

	[HideInInspector]
	public bool miniHovered;

	public bool centerButtons;

	public Sprite mutedSprite;

	public Sprite normalSprite;

	private Body body => PlayerCamera.main.body;

	public bool isBody => slot >= 0;

	public bool Overlaps(List<RaycastResult> casts)
	{
		if (!centerButtons)
		{
			return true;
		}
		float num = Vector2.Distance(PlayerCamera.main.radialMenu.position, Input.mousePosition) / PlayerCamera.uiScale;
		if (centerButtons)
		{
			if (num <= 281f)
			{
				return num > 152f;
			}
			return false;
		}
		return true;
	}

	private void Awake()
	{
		UpdateRefs();
	}

	public void UpdateRefs()
	{
		image = GetComponent<Image>();
		tooltip = GetComponent<UITooltip>();
		if (!waterFill)
		{
			GameObject gameObject = new GameObject("Waterfill", typeof(Image));
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.SetAsFirstSibling();
			gameObject.transform.localPosition = itemImg.transform.localPosition;
			gameObject.transform.localRotation = itemImg.transform.localRotation;
			gameObject.transform.localScale = Vector3.one;
			waterFill = gameObject.GetComponent<Image>();
			waterFill.color = Color.clear;
			waterFill.fillAmount = 0f;
			waterFill.type = Image.Type.Filled;
			waterFill.fillMethod = Image.FillMethod.Vertical;
			waterFill.fillOrigin = 0;
			waterFill.raycastTarget = false;
			waterFill.enabled = false;
		}
		tooltip.skipLocale = true;
	}

	public void UpdateGraphic()
	{
		Item item = GetItem();
		float num = 1f;
		if ((bool)item)
		{
			itemImg.enabled = true;
			itemImg.sprite = item.GetComponent<SpriteRenderer>().sprite;
			if (woundView)
			{
				itemImg.color = (item.Stats.ActuallyUsableOnLimb(item) ? Color.white : new Color(1f, 1f, 1f, 0.1f));
				num = (item.Stats.ActuallyUsableOnLimb(item) ? 1f : 0.1f);
				locked.enabled = !body.aboveMedicalCutoff && !item.Stats.ignoreDepression;
			}
			itemImg.rectTransform.sizeDelta = PlayerCamera.ImageSizeDelta(itemImg.sprite.texture, 3f, maxImageSize);
			Color color = PlayerCamera.ConditionToColor(item.condition);
			color.a = num;
			itemCond.color = color;
			if (!item.container)
			{
				itemCond.fillAmount = item.condition;
			}
			else
			{
				itemCond.fillAmount = item.container.GetHoldingWeight() / item.container.maxWeight;
			}
			(string, string) tuple = PlayerCamera.ItemHoverDescription(item);
			tooltip.tipName = tuple.Item1;
			tooltip.tipDesc = tuple.Item2;
			if (item.Stats is LiquidItemInfo liquidItemInfo && item.TryGetComponent<WaterContainerItem>(out var component) && component.hasSprite)
			{
				waterFill.sprite = component.fillSprite;
				waterFill.rectTransform.sizeDelta = PlayerCamera.ImageSizeDelta(component.fillSprite.texture, 3f, maxImageSize);
				waterFill.color = component.AverageColor().WithAlpha(num);
				waterFill.fillAmount = component.CurrentTotal / liquidItemInfo.capacity;
				waterFill.enabled = true;
			}
			else
			{
				waterFill.enabled = false;
			}
		}
		else
		{
			itemImg.enabled = false;
			num = (woundView ? 0.1f : 0.5f);
			itemCond.fillAmount = 0f;
			tooltip.tipName = string.Empty;
			if (!isBody)
			{
				Object.Destroy(base.gameObject);
			}
			if (woundView)
			{
				locked.enabled = !body.aboveMedicalCutoff;
			}
			waterFill.enabled = false;
		}
		if (isBody && !woundView)
		{
			image.sprite = (item ? mutedSprite : normalSprite);
		}
		Color color2 = ((!isBody || (body.slots[slot].canPickUp && (!PlayerCamera.main.dragItem || !PlayerCamera.main.dragItem.Stats.onlyHoldInHands || body.slots[slot].isHand))) ? Color.white : Color.red);
		if ((bool)PlayerCamera.main.dragItem && (bool)item && item.TryGetComponent<Container>(out var component2) && !component2.CanHoldItem(PlayerCamera.main.dragItem))
		{
			color2 = Color.red;
		}
		color2.a = num;
		if (miniHovered)
		{
			color2.r *= 0.55f;
			color2.g *= 0.55f;
			color2.b *= 0.55f;
		}
		if ((bool)item && item.colorSwatch > 0)
		{
			color2.r *= Item.ColorSwatches[item.colorSwatch].r;
			color2.g *= Item.ColorSwatches[item.colorSwatch].g;
			color2.b *= Item.ColorSwatches[item.colorSwatch].b;
		}
		image.color = color2;
	}

	private void Update()
	{
		base.transform.localScale = Vector3.Lerp(base.transform.localScale, hovered ? (Vector3.one * 0.85f) : Vector3.one, Time.deltaTime * 7.5f);
		hovered = false;
		UpdateGraphic();
		miniHovered = false;
	}

	public bool TryGetItem(out Item item)
	{
		if (isBody)
		{
			item = body.GetItem(slot);
			return item;
		}
		item = refItem;
		return item;
	}

	public Item GetItem()
	{
		if (isBody)
		{
			return body.GetItem(slot);
		}
		return refItem;
	}
}
