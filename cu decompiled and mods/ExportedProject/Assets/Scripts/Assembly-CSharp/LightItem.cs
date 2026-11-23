using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightItem : MonoBehaviour
{
	public Light2D light;

	public bool shouldEnable;

	public bool inContainer;

	private void Start()
	{
		if (!light)
		{
			light = GetComponent<Light2D>();
		}
		shouldEnable = true;
	}

	private void Update()
	{
		light.enabled = shouldEnable && !inContainer;
	}
}
