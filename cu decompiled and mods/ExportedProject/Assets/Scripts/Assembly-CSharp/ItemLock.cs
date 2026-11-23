using UnityEngine;

public class ItemLock : MonoBehaviour
{
	private Vector2 pos;

	private float rot;

	private Rigidbody2D rb;

	private void Start()
	{
		pos = base.transform.position;
		rot = base.transform.eulerAngles.z;
		rb = GetComponent<Rigidbody2D>();
		rb.angularVelocity = 0f;
		rb.velocity = Vector2.zero;
		rb.freezeRotation = true;
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
	}

	private void Update()
	{
		if ((bool)base.transform.parent)
		{
			Object.Destroy(this);
			rb.freezeRotation = false;
			rb.constraints = RigidbodyConstraints2D.None;
		}
	}
}
