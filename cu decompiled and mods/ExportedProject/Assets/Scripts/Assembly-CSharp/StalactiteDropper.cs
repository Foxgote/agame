using UnityEngine;

public class StalactiteDropper : MonoBehaviour
{
	private BuildingEntity build;

	private bool dropped;

	private bool counting;

	private float countTime;

	public int behaviour;

	private float lastVel;

	public float dropMass = 10f;

	private void Start()
	{
		build = GetComponent<BuildingEntity>();
		build.ignoreBodyOptimize = true;
		countTime = ((Random.value > 0.5f) ? Random.Range(0f, 3f) : 0f);
		if (behaviour == 2)
		{
			countTime *= 0.2f;
		}
	}

	public void CheckSeating()
	{
		if (!dropped && WorldGeneration.world.GetBlock(build.blockPlacedOn) == 0)
		{
			Drop();
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((behaviour == 1 || behaviour == 2) && collision.relativeVelocity.magnitude > 1f)
		{
			build.health = 0f;
			Sound.Play("glass", base.transform.position);
		}
		if (!dropped && (bool)collision.gameObject.GetComponent<Item>() && collision.rigidbody.velocity.magnitude > 6f)
		{
			Sound.Play(build.hitSound, base.transform.position);
			if (Random.value < 0.5f)
			{
				Drop();
			}
		}
	}

	public void Drop()
	{
		dropped = true;
		GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
		GetComponent<Rigidbody2D>().mass = dropMass;
		GetComponent<Rigidbody2D>().gravityScale = 1f;
		if (behaviour == 2)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(lastVel * 0.3f, -5f);
		}
		build.ignoreBodyOptimize = false;
		build.description = Locale.GetBuilding("stalactitedscdropped");
		Sound.Play("stalactitefall", base.transform.position);
	}

	public void OnWillRenderObject()
	{
		RaycastHit2D raycastHit2D = Physics2D.Raycast(base.transform.position, Vector2.down, 40f, LayerMask.GetMask("Body", "Limb"));
		if ((bool)raycastHit2D && !counting)
		{
			counting = true;
		}
		if ((bool)raycastHit2D)
		{
			lastVel = raycastHit2D.rigidbody.velocity.x;
		}
		if (counting)
		{
			countTime -= Time.deltaTime;
			if (countTime <= 0f && !dropped)
			{
				Drop();
			}
		}
	}
}
