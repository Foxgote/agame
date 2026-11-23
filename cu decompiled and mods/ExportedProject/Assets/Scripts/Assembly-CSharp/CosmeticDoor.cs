using UnityEngine;

public class CosmeticDoor : MonoBehaviour
{
	private bool open;

	private float side;

	private float progress;

	private Transform inner;

	public bool forceDisable;

	private void Start()
	{
		inner = base.transform.GetChild(0);
	}

	private void OnWillRenderObject()
	{
		if (open)
		{
			progress = Mathf.Lerp(progress, side, Time.deltaTime * 4f);
			inner.transform.localPosition = new Vector2(progress * 1.75f, -0.25f);
			inner.transform.localScale = new Vector2(progress, 1f);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (Utils.GetBody(collision.gameObject, out var body) && !forceDisable && !open)
		{
			open = true;
			GetComponent<Collider2D>().enabled = false;
			side = ((body.transform.position.x < base.transform.position.x) ? 1 : (-1));
			Sound.Play("endlabDoor", base.transform.position);
		}
	}
}
