using UnityEngine;
using UnityEngine.UI;

public class FilledImagePP : MonoBehaviour
{
	private Image img;

	public float fillAmount;

	private void Start()
	{
		img = GetComponent<Image>();
	}

	private void Update()
	{
		float num = ((img.fillMethod == Image.FillMethod.Vertical) ? img.sprite.texture.height : img.sprite.texture.width);
		img.fillAmount = Mathf.Round(fillAmount * num) / num;
	}
}
