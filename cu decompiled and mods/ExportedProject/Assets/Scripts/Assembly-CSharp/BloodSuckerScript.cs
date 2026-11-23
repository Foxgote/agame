using System;
using System.Linq;
using UnityEngine;

public class BloodSuckerScript : MonoBehaviour
{
	private float suckTime;

	private Limb limb;

	private LineRenderer line;

	private Vector2 linePos;

	private const float maxRadius = 12f;

	public int segments = 24;

	public float waveSpeed = 6f;

	public float waveFrequency = 3f;

	public float pulseStrength = 0.35f;

	private const float baseWidth = 0.6f;

	private void Start()
	{
		line = base.transform.GetChild(0).GetComponent<LineRenderer>();
		line.positionCount = segments;
	}

	private void OnWillRenderObject()
	{
		suckTime -= Time.deltaTime;
		linePos = Vector2.Lerp(linePos, limb ? ((Vector2)base.transform.InverseTransformPoint(limb.transform.position)) : Vector2.zero, Time.deltaTime * 5f * (limb ? 3.5f : 1f));
		for (int i = 0; i < segments; i++)
		{
			float t = (float)i / (float)(segments - 1);
			Vector2 vector = Vector2.Lerp(Vector2.zero, linePos, t);
			line.SetPosition(i, vector);
		}
		if ((bool)limb)
		{
			limb.body.bloodVolume -= Time.deltaTime * 8f;
			limb.pain += Time.deltaTime * 18f;
			AnimationCurve animationCurve = new AnimationCurve();
			float num = Time.time * (0f - waveSpeed);
			for (int j = 0; j < segments; j++)
			{
				float num2 = (float)j / (float)(segments - 1);
				float num3 = Mathf.Sin(num2 * MathF.PI * 2f * waveFrequency - num);
				float num4 = 1.6f + num3 * pulseStrength;
				animationCurve.AddKey(num2, 0.6f * num4);
			}
			line.widthCurve = animationCurve;
			if (suckTime < 0f)
			{
				limb = null;
			}
			else if (Vector2.Distance(limb.body.transform.position, base.transform.position) > 12f)
			{
				limb = null;
			}
		}
		else
		{
			AnimationCurve widthCurve = new AnimationCurve(new Keyframe(0f, 0.6f), new Keyframe(0f, 0.6f));
			line.widthCurve = widthCurve;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Utils.GetBody(collision.gameObject, out var body) && suckTime < -10f)
		{
			BeginSucc(body.limbs.Where((Limb l) => !l.dismembered).ToList().PickRandom());
		}
	}

	public void BeginSucc(Limb lm)
	{
		limb = lm;
		lm.skinHealth -= 15f;
		limb.body.eyeScareTime = 2.5f;
		limb.body.Scream();
		limb.body.DoGoreSound();
		suckTime = 1.25f;
	}
}
