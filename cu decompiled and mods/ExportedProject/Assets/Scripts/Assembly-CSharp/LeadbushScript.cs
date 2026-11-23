using System;
using UnityEngine;

public class LeadbushScript : MonoBehaviour
{
	private void OnWillRenderObject()
	{
		base.transform.localScale = new Vector2(1f + Mathf.Sin(Time.time) * 0.1f, 1f + Mathf.Sin(Time.time + MathF.PI / 2f) * 0.1f);
	}
}
