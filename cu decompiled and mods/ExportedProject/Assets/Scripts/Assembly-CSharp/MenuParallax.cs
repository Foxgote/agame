using System;
using System.Collections.Generic;
using UnityEngine;

public class MenuParallax : MonoBehaviour
{
	[Serializable]
	[SerializeField]
	public class ParallaxLayerInfo
	{
		[HideInInspector]
		public Vector2 originalPos;

		public float mouseMoveScale;

		public RectTransform reference;
	}

	[Serializable]
	[SerializeField]
	public class ParallaxImage
	{
		[SerializeField]
		public List<ParallaxLayerInfo> layers;
	}

	[SerializeField]
	public ParallaxImage parallax;

	public Vector2 mousePrevFirst;

	public Vector2 mousePrevLast;

	public static MenuParallax instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		foreach (ParallaxLayerInfo layer in parallax.layers)
		{
			layer.originalPos = layer.reference.anchoredPosition;
		}
		parallax.layers[2].originalPos += Vector2.right * UnityEngine.Random.Range(-1000f, 1000f);
	}

	private void Update()
	{
		Vector2 mousePos = GetMousePos();
		mousePrevFirst = Vector2.Lerp(mousePrevFirst, mousePos, Time.deltaTime * 3f);
		mousePrevLast = Vector2.Lerp(mousePrevLast, mousePrevFirst, Time.deltaTime * 2.9f);
		Vector2 vector = new Vector2(Mathf.PerlinNoise(Time.unscaledTime * 0.25f, 0f) - 0.5f, Mathf.PerlinNoise(0f, Time.unscaledTime * 0.25f) - 0.5f) * 200f;
		foreach (ParallaxLayerInfo layer in parallax.layers)
		{
			Vector2 anchoredPosition = layer.originalPos + (mousePrevLast + vector) * layer.mouseMoveScale;
			layer.reference.anchoredPosition = anchoredPosition;
		}
	}

	public Vector2 GetMousePos()
	{
		return Vector3.ClampMagnitude((Input.mousePosition - new Vector3((float)Screen.width * 0.5f, (float)Screen.height * 0.5f)) * 0.25f, 225f) * -1f;
	}
}
