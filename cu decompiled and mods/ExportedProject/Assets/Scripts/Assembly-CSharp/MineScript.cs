using UnityEngine;

public class MineScript : MonoBehaviour
{
	public float timeSincePressed;

	private bool pressed;

	public Sprite pressedSprite;

	public BuildingEntity build;

	private bool exploded;

	private void Awake()
	{
		build = GetComponent<BuildingEntity>();
	}

	private void OnDestroy()
	{
		if (build.health < 0.5f && !exploded)
		{
			WorldGeneration.CreateExplosion(new ExplosionParams
			{
				position = base.transform.position + Vector3.up,
				dismemberChance = 0.15f
			});
		}
	}

	private void Update()
	{
		if (pressed)
		{
			timeSincePressed += Time.deltaTime;
			if (!exploded && timeSincePressed > 0.8f)
			{
				exploded = true;
				build.health = 0f;
				WorldGeneration.CreateExplosion(new ExplosionParams
				{
					position = base.transform.position + Vector3.up,
					dismemberChance = 0.15f
				});
			}
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!pressed && (bool)collision.collider.attachedRigidbody && !collision.collider.attachedRigidbody.isKinematic && Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 50f)
		{
			pressed = true;
			Sound.Play("mine", base.transform.position);
			GetComponent<SpriteRenderer>().sprite = pressedSprite;
		}
	}
}
