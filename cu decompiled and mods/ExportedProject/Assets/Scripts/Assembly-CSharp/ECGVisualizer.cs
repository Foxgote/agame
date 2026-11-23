using UnityEngine;
using UnityEngine.UI;

public class ECGVisualizer : MonoBehaviour
{
	private Texture2D tex;

	private Color32[,] pixelGrid;

	private Color32[] pixelBuffer;

	public Vector2Int textureSize;

	private int writeX;

	private int lastY;

	public float writeHeight;

	private RawImage img;

	private float timeToUpdate;

	private int width;

	private int height;

	public bool active = true;

	public bool followBody = true;

	private Body body => PlayerCamera.main.body;

	private void Start()
	{
		width = textureSize.x;
		height = textureSize.y;
		tex = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		tex.filterMode = FilterMode.Point;
		pixelGrid = new Color32[width, height];
		pixelBuffer = new Color32[width * height];
		img = GetComponent<RawImage>();
		img.texture = tex;
		tex.SetPixels32(pixelBuffer);
		tex.Apply();
		lastY = height / 2;
	}

	private void Update()
	{
		if (!active)
		{
			return;
		}
		timeToUpdate += Time.unscaledDeltaTime;
		if (timeToUpdate > 0.1f)
		{
			timeToUpdate = 0.1f;
		}
		while (timeToUpdate > 0.028f)
		{
			writeX++;
			if (writeX >= width)
			{
				writeX = 0;
			}
			if (followBody)
			{
				writeHeight = body.GetECGHeight(timeToUpdate - 0.028f);
			}
			int b = Mathf.RoundToInt((writeHeight + 1f) * 0.5f * (float)(height - 1));
			int num = Mathf.Min(lastY, b);
			int num2 = Mathf.Max(lastY, b);
			for (int i = num; i <= num2; i++)
			{
				pixelGrid[writeX, i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				if (i + 1 < height)
				{
					pixelGrid[writeX, i + 1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Max(50, pixelGrid[writeX, i + 1].a));
				}
				if (i - 1 >= 0)
				{
					pixelGrid[writeX, i - 1] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Max(50, pixelGrid[writeX, i - 1].a));
				}
				if (writeX + 1 < width)
				{
					pixelGrid[writeX + 1, i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Max(50, pixelGrid[writeX + 1, i].a));
				}
				if (writeX - 1 >= 0)
				{
					pixelGrid[writeX - 1, i] = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, (byte)Mathf.Max(50, pixelGrid[writeX - 1, i].a));
				}
			}
			for (int j = 0; j < width; j++)
			{
				for (int k = 0; k < height; k++)
				{
					Color32 color = pixelGrid[j, k];
					color.a = (byte)((float)(int)color.a * 0.985f);
					pixelGrid[j, k] = color;
				}
			}
			int num3 = 0;
			for (int l = 0; l < height; l++)
			{
				for (int m = 0; m < width; m++)
				{
					pixelBuffer[num3++] = pixelGrid[m, l];
				}
			}
			lastY = b;
			timeToUpdate -= 0.028f;
			if (timeToUpdate <= 0.028f)
			{
				tex.SetPixels32(pixelBuffer);
				tex.Apply(updateMipmaps: false);
			}
		}
	}
}
