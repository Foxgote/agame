using UnityEngine;
using UnityEngine.UI;

public class MenuExperimentBlink : MonoBehaviour
{
	public Sprite[] sprites;

	public Sprite[] outlineSprites;

	public Sprite[] shineSprites;

	public Sprite[] reserveCharSprites;

	public RectTransform text;

	public Vector2[] textOffset;

	private Image image;

	private float blinkTime = 5f;

	public Image outline;

	public Image shine;

	private int chr;

	private void Start()
	{
		image = GetComponent<Image>();
		chr = PlayerPrefs.GetInt("lastchar");
		switch (chr)
		{
		default:
			outline.sprite = outlineSprites[0];
			sprites[0] = reserveCharSprites[0];
			sprites[1] = reserveCharSprites[1];
			sprites[2] = reserveCharSprites[2];
			shine.sprite = shineSprites[0];
			break;
		case 1:
			outline.sprite = outlineSprites[1];
			sprites[0] = reserveCharSprites[3];
			sprites[1] = reserveCharSprites[4];
			sprites[2] = reserveCharSprites[5];
			shine.sprite = shineSprites[1];
			break;
		case 2:
			outline.sprite = outlineSprites[2];
			sprites[0] = reserveCharSprites[6];
			sprites[1] = reserveCharSprites[7];
			sprites[2] = reserveCharSprites[8];
			shine.sprite = shineSprites[2];
			break;
		}
		text.localPosition += (Vector3)textOffset[chr];
	}

	private void Update()
	{
		blinkTime -= Time.unscaledDeltaTime;
		if (chr != 2)
		{
			if (blinkTime < 0f)
			{
				image.sprite = sprites[1];
			}
			if (blinkTime < -0.05f)
			{
				image.sprite = sprites[2];
			}
			if (blinkTime < -0.1f)
			{
				image.sprite = sprites[1];
			}
			if (blinkTime < -0.15f)
			{
				blinkTime = Random.Range(2f, 8f);
			}
			if (blinkTime >= 0f)
			{
				image.sprite = sprites[0];
			}
		}
		else
		{
			if (blinkTime < 0f)
			{
				image.sprite = sprites[1];
			}
			if (blinkTime < -0.075f)
			{
				image.sprite = sprites[2];
			}
			if (blinkTime < -0.15f)
			{
				blinkTime = Random.Range(0.5f, 6f);
			}
			if (blinkTime >= 0f)
			{
				image.sprite = sprites[0];
			}
		}
	}
}
