using UnityEngine;

public class FreshItemDrop : MonoBehaviour
{
	private float timeLeft;

	private Rigidbody2D rb;

	private SpriteRenderer outline;

	private ParticleSystem part;

	public bool forceOn;

	private void Awake()
	{
		if (!Item.itemFloating)
		{
			Object.Destroy(this);
			return;
		}
		timeLeft = 10f;
		rb = GetComponent<Rigidbody2D>();
		rb.gravityScale = 0f;
	}

	private void Start()
	{
		GameObject gameObject = new GameObject("Outline", typeof(SpriteRenderer));
		gameObject.transform.SetParent(base.transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one * 1.125f;
		outline = gameObject.GetComponent<SpriteRenderer>();
		outline.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;
		outline.sprite = GetComponent<SpriteRenderer>().sprite;
		outline.material = Resources.Load<Material>("Special/ItemOutline");
		gameObject = Object.Instantiate(Resources.Load("Special/ItemOutlineParticle"), base.transform) as GameObject;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		part = gameObject.GetComponent<ParticleSystem>();
		ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape;
		shape.spriteRenderer = outline;
	}

	private void Update()
	{
		timeLeft -= Time.deltaTime;
		outline.color = new Color(1f, 1f, 1f, Mathf.Clamp01(timeLeft * 0.4f));
		if (timeLeft < 1f && (bool)part)
		{
			part.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
		}
		if (timeLeft < 0f || ((bool)base.transform.parent && !forceOn))
		{
			Object.Destroy(this);
		}
	}

	private void FixedUpdate()
	{
		rb.gravityScale = Mathf.Clamp01(1f - timeLeft * 0.5f);
		rb.velocity *= 0.96f;
		rb.angularVelocity *= 0.97f;
	}

	private void OnDestroy()
	{
		if (Item.itemFloating)
		{
			rb.gravityScale = 1f;
			if ((bool)part)
			{
				Object.Destroy(part.gameObject);
			}
			Object.Destroy(outline.gameObject);
		}
	}
}
