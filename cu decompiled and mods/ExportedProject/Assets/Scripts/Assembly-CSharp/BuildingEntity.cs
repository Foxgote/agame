using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuildingEntity : MonoBehaviour
{
	public ItemDrop[] itemsDropOnDestroy;

	[SerializeField]
	[FormerlySerializedAs("health")]
	private float _health;

	[HideInInspector]
	public Vector2Int blockPlacedOn;

	public bool requireGround;

	public string id;

	[HideInInspector]
	public string fullName;

	[HideInInspector]
	public string description;

	public bool skipDescriptionSet;

	private Rigidbody2D rb;

	public float dropChanceMultiplier = 1f;

	public int guaranteedDropAmount;

	public ItemDrop[] alwaysDrop;

	public string[] itemCategoriesToAdd;

	public AudioClip hitSound;

	public ushort blockFootstepSoundId;

	public bool cantHit;

	public bool animal;

	public bool ignoreBodyOptimize;

	public bool metallic;

	public bool indestructible;

	private bool didDestroyRoutine;

	public float health
	{
		get
		{
			return _health;
		}
		set
		{
			_health = value;
			CheckDamage();
		}
	}

	public string fullNameDisplay => fullName + " (" + Mathf.RoundToInt(health) + ")";

	public void Backgroundify()
	{
		if (TryGetComponent<SpriteRenderer>(out var component))
		{
			component.color = Color.gray;
			component.sortingOrder = -999;
		}
		Collider2D[] components = GetComponents<Collider2D>();
		for (int i = 0; i < components.Length; i++)
		{
			components[i].enabled = false;
		}
	}

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		if ((bool)rb && !ignoreBodyOptimize)
		{
			rb.bodyType = RigidbodyType2D.Static;
		}
		fullName = Locale.GetBuilding(id);
		if (!skipDescriptionSet)
		{
			description = Locale.GetBuilding(id + "dsc");
		}
	}

	public static void CreateBreakParticle(SpriteRenderer spr)
	{
		if ((bool)spr)
		{
			GameObject obj = Object.Instantiate(Resources.Load("BuildingBreakParticle"), spr.transform.position, spr.transform.rotation) as GameObject;
			ParticleSystem.ShapeModule shape = obj.GetComponent<ParticleSystem>().shape;
			shape.texture = spr.sprite.texture;
			shape.sprite = spr.sprite;
			obj.GetComponent<ParticleSystem>().Play();
		}
	}

	public void CheckDamage()
	{
		if (!(_health < 0.5f) || didDestroyRoutine)
		{
			return;
		}
		didDestroyRoutine = true;
		if (indestructible)
		{
			_health = 0f;
			return;
		}
		TryGetComponent<SpriteRenderer>(out var component);
		if ((bool)component)
		{
			GameObject obj = Object.Instantiate(Resources.Load("BuildingBreakParticle"), base.transform.position, base.transform.rotation) as GameObject;
			ParticleSystem.ShapeModule shape = obj.GetComponent<ParticleSystem>().shape;
			shape.texture = component.sprite.texture;
			shape.sprite = component.sprite;
			obj.GetComponent<ParticleSystem>().Play();
		}
		Object.Instantiate(Resources.Load<GameObject>("DustBig"), base.transform.position, Quaternion.identity);
		if (animal)
		{
			base.gameObject.SendMessage("AnimalDeath");
		}
		Sound.Play("footstep/Rock/11", base.transform.position);
		bool flag = Vector2.Distance(base.transform.position, PlayerCamera.main.body.transform.position) < 8f;
		ItemDrop[] array = itemsDropOnDestroy;
		foreach (ItemDrop itemDrop in array)
		{
			if (Random.Range(0f, 1f) < itemDrop.chance * dropChanceMultiplier)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load(itemDrop.id), base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
				gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f));
				gameObject.GetComponent<Item>().SetCondition(Random.Range(itemDrop.conditionMin, itemDrop.conditionMax));
				if (flag)
				{
					gameObject.AddComponent<FreshItemDrop>();
				}
			}
		}
		for (int j = 0; j < guaranteedDropAmount; j++)
		{
			List<ItemDrop> list = new List<ItemDrop>();
			foreach (string item in ItemLootPool.pool[itemCategoriesToAdd[Random.Range(0, itemCategoriesToAdd.Length)]])
			{
				list.Add(new ItemDrop
				{
					id = item,
					chance = 1f,
					conditionMin = 1f,
					conditionMax = 1f
				});
			}
			ItemDrop itemDrop2 = list[Random.Range(0, list.Count)];
			GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load(itemDrop2.id), base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
			gameObject2.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f));
			gameObject2.GetComponent<Item>().SetCondition(Random.Range(itemDrop2.conditionMin, itemDrop2.conditionMax));
			if (flag)
			{
				gameObject2.AddComponent<FreshItemDrop>();
			}
		}
		for (int k = 0; k < alwaysDrop.Length; k++)
		{
			ItemDrop itemDrop3 = alwaysDrop[k];
			GameObject gameObject3 = (GameObject)Object.Instantiate(Resources.Load(itemDrop3.id), base.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f)));
			gameObject3.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-7f, 7f), Random.Range(-7f, 7f));
			gameObject3.GetComponent<Item>().SetCondition(Random.Range(itemDrop3.conditionMin, itemDrop3.conditionMax));
			if (flag)
			{
				gameObject3.AddComponent<FreshItemDrop>();
			}
		}
		Object.Destroy(base.gameObject);
	}

	public void Update()
	{
		if ((bool)rb && !ignoreBodyOptimize)
		{
			rb.bodyType = ((!WorldGeneration.world.worldExists || !WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled || !(Time.timeScale <= 5f)) ? RigidbodyType2D.Static : RigidbodyType2D.Dynamic);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((bool)rb && collision.relativeVelocity.magnitude > 3f)
		{
			Vector2 pos = collision.GetContact(0).point + collision.GetContact(0).normal * -0.5f;
			Sound.Play(WorldGeneration.world.RandomStepSound(WorldGeneration.world.GetBlockInfo(WorldGeneration.world.GetBlock(pos)).stepsound), base.transform.position);
		}
	}

	private void OnBecameVisible()
	{
		CheckSeating();
	}

	public void CheckSeating()
	{
		if (requireGround && WorldGeneration.world.GetBlock(blockPlacedOn) == 0)
		{
			health = 0f;
		}
	}
}
