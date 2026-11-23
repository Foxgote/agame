using UnityEngine;
using UnityEngine.Rendering.Universal;

public class WWMRLight : MonoBehaviour
{
	public static WWMRLight inst;

	public Light2D light;

	private PlayerCamera cam => PlayerCamera.main;

	private void Awake()
	{
		inst = this;
		light = GetComponent<Light2D>();
	}

	private void Start()
	{
		light.enabled = false;
	}

	private void Update()
	{
		if (light.enabled)
		{
			Vector2 vector = cam.transform.position;
			vector *= 0.74f;
			base.transform.position = vector;
		}
	}
}
