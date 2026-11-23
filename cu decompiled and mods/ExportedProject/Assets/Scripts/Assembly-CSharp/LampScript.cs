using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LampScript : MonoBehaviour
{
	private void OnDestroy()
	{
		if (!(GetComponent<BuildingEntity>().health < 1f))
		{
			return;
		}
		Collider2D[] array = Physics2D.OverlapBoxAll((Vector2)base.transform.position + Vector2.down * 10f, new Vector2(6f, 20f), 0f, LayerMask.GetMask("Descriptor"));
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].TryGetComponent<TraderScript>(out var component))
			{
				component.LightBroken();
				break;
			}
		}
	}

	public void SetColor(Color color)
	{
		GetComponent<SpriteRenderer>().color = color;
		GetComponent<Light2D>().color = color;
	}
}
