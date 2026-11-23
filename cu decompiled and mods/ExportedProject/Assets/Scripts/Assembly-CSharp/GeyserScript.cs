using UnityEngine;

public class GeyserScript : MonoBehaviour
{
	private float activateTime;

	private byte liquidType;

	private float rumbleTime;

	private BuildingEntity bld;

	private float lastHp;

	private Vector2 origPos;

	public bool isLava;

	private void Start()
	{
		rumbleTime = -100f;
		activateTime = -100f;
		liquidType = (byte)((!(Random.value > 0.75f)) ? 1 : 2);
		if (isLava)
		{
			liquidType = 6;
		}
		bld = base.transform.parent.GetComponent<BuildingEntity>();
		lastHp = bld.health;
		origPos = base.transform.parent.position;
	}

	public void TryRumble()
	{
		if (!(Time.time - activateTime < 10f) && !(rumbleTime > 0f))
		{
			Sound.Play("geyserrumble", base.transform.position, twoDimensional: false, pitchShift: false);
			rumbleTime = 1f;
		}
	}

	public void Activate()
	{
		activateTime = Time.time;
		Sound.Play("geyseractivate", base.transform.position, twoDimensional: false, pitchShift: false);
	}

	private void Update()
	{
		if (rumbleTime > 0f)
		{
			rumbleTime -= Time.deltaTime;
			if (rumbleTime <= 0f)
			{
				Activate();
			}
			base.transform.parent.position = origPos + Vector2.right * Random.Range(-0.125f, 0.125f);
		}
		else
		{
			base.transform.parent.position = origPos;
		}
		if (lastHp != bld.health)
		{
			lastHp = bld.health;
			TryRumble();
		}
		if (Time.time - activateTime < 4.5f)
		{
			Vector2Int vector2Int = WorldGeneration.world.WorldToBlockPos(base.transform.position);
			FluidManager.main.fluid[vector2Int.x, vector2Int.y + Random.Range(0, 10)] = liquidType;
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((bool)collision.attachedRigidbody && collision.attachedRigidbody.bodyType == RigidbodyType2D.Dynamic && Vector2.Distance(base.transform.position, PlayerCamera.main.transform.position) < 64f)
		{
			TryRumble();
		}
	}
}
