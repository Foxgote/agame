using UnityEngine;

public class GrapplingHook : MonoBehaviour
{
	public Rigidbody2D hook;

	public Transform barrel;

	public GameObject hookPrefab;

	private bool fired;

	private bool hookLatched;

	private bool pulling;

	public Sprite normSprite;

	public Sprite firedSprite;

	private LineRenderer lineRenderer;

	public static Vector3 Facing(Body body)
	{
		return (body.targetLookPos - body.transform.position).normalized;
	}

	private void Start()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	public void Use(Body body)
	{
		if (!(GetComponent<Item>().condition <= 0f))
		{
			if (!fired)
			{
				hook = Object.Instantiate(hookPrefab, base.transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
				hook.GetComponent<HookPoint>().hook = this;
				hook.transform.right = Facing(body);
				hook.gameObject.SetActive(value: true);
				hook.transform.position = barrel.transform.position;
				hook.velocity = Facing(body) * 100f;
				fired = true;
				lineRenderer.enabled = true;
				GetComponent<SpriteRenderer>().sprite = firedSprite;
				Object.Instantiate(Resources.Load<GameObject>("DustSmall"), barrel.transform.position, Quaternion.identity);
				Sound.Play("hookfire", base.transform.position);
				GetComponent<Item>().battery.DrainCharge(0.02f);
			}
			else if (!pulling && hookLatched)
			{
				pulling = true;
				hookLatched = false;
				hook.bodyType = RigidbodyType2D.Dynamic;
				body.SetVelocity((Vector2)(hook.transform.position - base.transform.position).normalized * 50f);
				Sound.Play("hookpull", base.transform.position);
				PlayerCamera.main.shaker.Shake(50f);
			}
		}
	}

	private void FixedUpdate()
	{
		if (pulling)
		{
			hook.velocity = Vector3.Lerp(hook.velocity, (barrel.transform.position - hook.transform.position).normalized * 100f, 0.08f);
		}
	}

	private void Update()
	{
		if (pulling && Vector2.Distance(hook.transform.position, barrel.transform.position) < 1.5f)
		{
			Object.Destroy(hook.gameObject);
			pulling = false;
			fired = false;
			lineRenderer.enabled = false;
			GetComponent<SpriteRenderer>().sprite = normSprite;
			Sound.Play("hookreturn", base.transform.position);
		}
		if (fired)
		{
			lineRenderer.SetPositions(new Vector3[2]
			{
				barrel.transform.position,
				hook.transform.position
			});
			if (WorldGeneration.world.generatingWorld)
			{
				pulling = true;
				hookLatched = false;
				hook.bodyType = RigidbodyType2D.Dynamic;
			}
		}
	}

	public void HookHit()
	{
		if (!pulling && !hookLatched)
		{
			hook.bodyType = RigidbodyType2D.Static;
			hookLatched = true;
			hook.GetComponent<Collider2D>().enabled = false;
			Object.Instantiate(Resources.Load<GameObject>("DustSmall"), hook.transform.position, Quaternion.identity);
			Sound.Play("hookhit", hook.transform.position);
		}
	}
}
