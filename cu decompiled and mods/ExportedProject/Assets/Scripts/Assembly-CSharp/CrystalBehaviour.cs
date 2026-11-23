using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CrystalBehaviour : MonoBehaviour, ISimEntity
{
	public SpriteRenderer sprite;

	public BuildingEntity build;

	public Light2D light;

	public float spawnHeightOffset;

	private bool hasBeenExamined;

	public static List<(Func<CrystalEffect> factory, int weight)> possibleEffects = new List<(Func<CrystalEffect>, int)>
	{
		(() => new CrystalIrradiated(), 4),
		(() => new CrystalFragile(), 20),
		(() => new CrystalGravity(), 10),
		(() => new CrystalElectric(), 4),
		(() => new CrystalBurning(), 8),
		(() => new CrystalTemperature(), 8),
		(() => new CrystalSeptic(), 6),
		(() => new CrystalUnstable(), 10),
		(() => new CrystalMimic(), 8),
		(() => new CrystalTeleport(), 10),
		(() => new CrystalDripping(), 8),
		(() => new CrystalKinetic(), 10),
		(() => new CrystalEMP(), 5),
		(() => new CrystalBlinding(), 3),
		(() => new CrystalMetamorphic(), 16),
		(() => new CrystalHealing(), 12),
		(() => new CrystalShy(), 8)
	};

	private List<CrystalEffect> effects;

	public float crystalSize;

	public void SetColor(Color clr)
	{
		clr.r *= UnityEngine.Random.Range(0.9f, 1f);
		clr.g *= UnityEngine.Random.Range(0.9f, 1f);
		clr.b *= UnityEngine.Random.Range(0.9f, 1f);
		sprite.color = clr;
		light.color = clr;
		light.intensity = UnityEngine.Random.Range(0.25f, 1f);
	}

	private void Awake()
	{
		build = GetComponent<BuildingEntity>();
		sprite = GetComponent<SpriteRenderer>();
		light = GetComponent<Light2D>();
	}

	public void CheckCurrentChunk(ChunkScript chunk)
	{
		chunk.AddEntity(this);
	}

	public void OnUse()
	{
		if (!hasBeenExamined)
		{
			hasBeenExamined = true;
			UnityEngine.Object.Destroy(GetComponent<UsableObject>());
			BuildingEntity buildingEntity = build;
			buildingEntity.description = buildingEntity.description + "\n\n" + ((effects.Count == 1) ? Locale.GetBuilding("crystalproperty") : Locale.GetBuilding("crystalproperties"));
			List<string> list = new List<string>();
			for (int i = 0; i < effects.Count; i++)
			{
				list.Add(Locale.GetBuilding(effects[i].GetType().Name.ToLower()));
			}
			build.description += string.Join(", ", list);
		}
	}

	public void ChunkUpdate()
	{
		if (WorldGeneration.world.generatingWorld || !this)
		{
			return;
		}
		foreach (CrystalEffect effect in effects)
		{
			effect.Update();
		}
	}

	public void GenerationFinished()
	{
		if (WorldGeneration.world.GetBlock(base.transform.position - base.transform.up * 2f) == 0)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			GetComponent<Collider2D>().enabled = false;
			return;
		}
		base.transform.localScale = new Vector3((UnityEngine.Random.value > 0.5f) ? 1f : (-1f), 1f, 1f);
		base.transform.position += base.transform.up * spawnHeightOffset;
		Color color = ((UnityEngine.Random.value < 0.5f) ? Color.magenta : Color.cyan);
		SetColor(color);
		SetUpEffects();
	}

	public static CrystalEffect GetRandomEffect()
	{
		int maxExclusive = possibleEffects.Sum(((Func<CrystalEffect> factory, int weight) e) => e.weight);
		int num = UnityEngine.Random.Range(0, maxExclusive);
		int num2 = 0;
		foreach (var possibleEffect in possibleEffects)
		{
			num2 += possibleEffect.weight;
			if (num < num2)
			{
				return possibleEffect.factory();
			}
		}
		throw new InvalidOperationException("Invalid weighted selection");
	}

	public void SetUpEffects()
	{
		effects = new List<CrystalEffect>();
		if (UnityEngine.Random.value < 0.7f || crystalSize < 0.4f || (base.transform.position.y > (float)WorldGeneration.world.halfHeight * 0.8f && Mathf.Abs(base.transform.position.x) < (float)WorldGeneration.world.halfWidth * 0.25f))
		{
			UnityEngine.Object.Destroy(GetComponent<UsableObject>());
			UnityEngine.Object.Destroy(this);
			return;
		}
		do
		{
			CrystalEffect effect = GetRandomEffect();
			if (effects.Any((CrystalEffect e) => e.GetType() == effect.GetType()))
			{
				break;
			}
			effect.crystal = this;
			effects.Add(effect);
			effect.Start();
		}
		while (!(UnityEngine.Random.value < 0.66f));
		BuildingEntity buildingEntity;
		string description = (buildingEntity = build).description;
		buildingEntity.description = description + effects.Count switch
		{
			0 => "", 
			1 => " " + Locale.GetOther("crystaleffect1"), 
			2 => " " + Locale.GetOther("crystaleffect2"), 
			3 => " " + Locale.GetOther("crystaleffect3"), 
			4 => " " + Locale.GetOther("crystaleffect4"), 
			_ => " " + Locale.GetOther("crystaleffectmany"), 
		};
		if (effects.Count == 0)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		if (effects.Count > 1)
		{
			GameObject obj = Utils.Create("Special/CrystalDistort", base.transform);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localScale = Vector3.one;
			obj.transform.rotation = Quaternion.identity;
			obj.GetComponent<SpriteRenderer>().material.SetFloat("_DistortStrength", (float)effects.Count * 0.015f);
		}
		CheckCurrentChunk(WorldGeneration.world.GetClosestChunkScript(base.transform.position));
	}

	public void OnCollisionEnter2D(Collision2D collision)
	{
		if (WorldGeneration.world.generatingWorld || !WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled)
		{
			return;
		}
		foreach (CrystalEffect effect in effects)
		{
			effect.Touched(collision.gameObject);
		}
	}

	public void BuildingHit(AttackInfo atk)
	{
		if (WorldGeneration.world.generatingWorld || !WorldGeneration.world.GetClosestChunkRenderer(WorldGeneration.world.WorldToBlockPos(base.transform.position)).enabled)
		{
			return;
		}
		foreach (CrystalEffect effect in effects)
		{
			effect.Hit(atk);
		}
	}

	public void AddEffectAudio(string clip, float distance)
	{
		AudioSource audioSource = base.gameObject.AddComponent<AudioSource>();
		audioSource.clip = Resources.Load<AudioClip>("Sounds/" + clip);
		audioSource.loop = true;
		audioSource.maxDistance = distance;
		audioSource.dopplerLevel = 0f;
		audioSource.spatialBlend = 1f;
		audioSource.rolloffMode = AudioRolloffMode.Linear;
		audioSource.Play();
	}
}
