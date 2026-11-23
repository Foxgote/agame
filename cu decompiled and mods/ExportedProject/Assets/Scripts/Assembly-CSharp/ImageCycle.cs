using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageCycle : MonoBehaviour
{
	public List<Sprite> sprites;

	private Image image;

	private float lastTime;

	private int index;

	private void Awake()
	{
		lastTime = 2f;
		image = GetComponent<Image>();
	}

	private void Update()
	{
		lastTime += Time.unscaledDeltaTime;
		if (lastTime > 1f)
		{
			lastTime = 0f;
			index++;
			if (index >= sprites.Count)
			{
				index = 0;
			}
			image.sprite = sprites[index];
			GetComponent<RectTransform>().sizeDelta = PlayerCamera.ImageSizeDelta(sprites[index].texture, 4f, 32f);
		}
	}
}
