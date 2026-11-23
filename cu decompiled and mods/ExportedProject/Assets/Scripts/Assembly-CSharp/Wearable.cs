using UnityEngine;

public class Wearable : MonoBehaviour
{
	public string[] secondaryLimbs;

	public Sprite[] secondaryLimbSprites;

	public GameObject[] secondaryObjects;

	private void Awake()
	{
		secondaryObjects = new GameObject[secondaryLimbs.Length];
	}

	public void CreateSprites(Body body, WearableSpriteSwap swap = null)
	{
		ClearSprites();
		float num = 1f;
		if ((bool)swap && swap.customScale.Count > body.charType && swap.customScale[body.charType] != 0f)
		{
			num = swap.customScale[body.charType];
		}
		for (int i = 0; i < secondaryLimbs.Length; i++)
		{
			Limb limb = body.LimbByName(secondaryLimbs[i]);
			GameObject gameObject = new GameObject(secondaryLimbSprites[i].name, typeof(SpriteRenderer), typeof(WearableExtension));
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			gameObject.transform.SetParent(limb.transform);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one * num;
			gameObject.GetComponent<WearableExtension>().reference = GetComponent<Item>();
			component.sprite = secondaryLimbSprites[i];
			component.sortingOrder = limb.GetComponent<SpriteRenderer>().sortingOrder + GetComponent<Item>().Stats.wearableVisualOffset;
			component.sortingLayerName = limb.GetComponent<SpriteRenderer>().sortingLayerName;
			component.material = WorldGeneration.world.defaultMat;
			component.color = limb.GetComponent<SpriteRenderer>().color;
			secondaryObjects[i] = gameObject;
		}
	}

	public void ClearSprites()
	{
		for (int i = 0; i < secondaryObjects.Length; i++)
		{
			if (secondaryObjects[i] != null)
			{
				Object.Destroy(secondaryObjects[i]);
			}
		}
	}

	private void OnDestroy()
	{
		ClearSprites();
	}
}
