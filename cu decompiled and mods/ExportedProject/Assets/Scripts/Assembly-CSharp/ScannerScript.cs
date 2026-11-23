using System;
using UnityEngine;
using UnityEngine.UI;

public class ScannerScript : MonoBehaviour
{
	public RectTransform scanBack;

	public RectTransform scanLine;

	public RectTransform scanCenter;

	private float progress;

	public Vector2 pos;

	public GameObject scanDot;

	private float range = 80f;

	public static ScannerScript main;

	private float timeDone;

	private int cool;

	private void Start()
	{
		main = this;
	}

	private void Update()
	{
		progress += Time.deltaTime * 0.33f;
		if (progress <= 1f)
		{
			for (int i = 0; (float)i < Time.deltaTime * 600f; i++)
			{
				Vector2 vector = rotate(Vector2.down, (progress - 0.5f) * 90f * (MathF.PI / 180f)) * (range * UnityEngine.Random.Range(0.05f, 1f));
				Collider2D collider2D = Physics2D.OverlapPoint(pos + vector, LayerMask.GetMask("Ground"));
				if ((bool)collider2D)
				{
					GameObject obj = UnityEngine.Object.Instantiate(scanDot, scanCenter);
					obj.GetComponent<Image>().color = (collider2D.GetComponent<BuildingEntity>() ? Color.red : Color.green);
					obj.transform.localPosition = vector / range * 190f;
					UnityEngine.Object.Destroy(obj, UnityEngine.Random.Range(5f, 20f));
					cool++;
					if (cool >= 5)
					{
						cool = 0;
						Sound.Play("scannerbeep", pos, twoDimensional: true, pitchShift: false);
					}
				}
			}
			scanLine.transform.eulerAngles = new Vector3(0f, 0f, (progress - 0.5f) * 90f);
		}
		else
		{
			timeDone += Time.deltaTime;
			Color color = new Color(1f, 1f, 1f, 2f - timeDone * 0.1f);
			scanBack.GetComponent<Image>().color = color;
			scanLine.GetComponent<Image>().color = color;
			if (timeDone > 20f)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public static Vector2 rotate(Vector2 v, float delta)
	{
		return new Vector2(v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta), v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta));
	}
}
