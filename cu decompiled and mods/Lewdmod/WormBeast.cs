using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ScavArmory;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace LewdStatus
{
	// Token: 0x02000137 RID: 311
	internal class WormBeast : MonoBehaviour
	{
		// Token: 0x1700011A RID: 282
		// (get) Token: 0x0600075E RID: 1886 RVA: 0x000714FF File Offset: 0x0006F6FF
		public Vector2 HeadPos
		{
			get
			{
				return this.joints[0];
			}
		}

		// Token: 0x1700011B RID: 283
		// (get) Token: 0x0600075F RID: 1887 RVA: 0x00071510 File Offset: 0x0006F710
		public Vector2 Maw
		{
			get
			{
				return this.joints[0] + this.dir * ((WormArt.Head.Length != 0) ? (WormArt.Head[0].bounds.size.x * 0.45f * 2f) : 2f);
			}
		}

		// Token: 0x1700011C RID: 284
		// (set) Token: 0x06000760 RID: 1888 RVA: 0x00071570 File Offset: 0x0006F770
		public bool Hidden
		{
			set
			{
				foreach (SpriteRenderer spriteRenderer in this.parts)
				{
					if (spriteRenderer)
					{
						spriteRenderer.enabled = !value;
					}
				}
			}
		}

		// Token: 0x06000761 RID: 1889 RVA: 0x000715D0 File Offset: 0x0006F7D0
		public void Build(Vector2 at, int layer, int order)
		{
			this.home = at;
			this.wanderTarget = at;
			this.spacing = (WormArt.Seg ? (WormArt.Seg.bounds.size.x * 0.72f) : 2f) * 2f;
			int num = 10;
			for (int i = 0; i < num; i++)
			{
				this.joints.Add(at - Vector2.right * this.spacing * (float)i);
				SpriteRenderer spriteRenderer = new GameObject((i == 0) ? "LewdWormHead" : "LewdWormSeg").AddComponent<SpriteRenderer>();
				spriteRenderer.sprite = ((i == 0) ? WormArt.Head[0] : ((i == num - 1) ? WormArt.Tail : WormArt.Seg));
				if (WorldGeneration.world)
				{
					spriteRenderer.sharedMaterial = WorldGeneration.world.defaultMat;
				}
				spriteRenderer.sortingLayerID = layer;
				spriteRenderer.sortingOrder = order - 400 + 40 - i;
				this.parts.Add(spriteRenderer);
			}
			this.parts[0].transform.SetParent(base.transform, false);
			for (int j = 0; j < this.parts.Count; j++)
			{
				GameObject gameObject = this.parts[j].gameObject;
				CircleCollider2D circleCollider2D = gameObject.AddComponent<CircleCollider2D>();
				circleCollider2D.isTrigger = false;
				gameObject.AddComponent<Rigidbody2D>().bodyType = 1;
				circleCollider2D.radius = ((j == 0) ? WormArt.Head[0].bounds.extents.y : WormArt.Seg.bounds.extents.y) * 0.8f;
				gameObject.layer = LayerMask.NameToLayer("Descriptor");
				BuildingEntity buildingEntity = gameObject.AddComponent<BuildingEntity>();
				buildingEntity.animal = true;
				buildingEntity.health = 100000f;
				buildingEntity.id = "lewd_worm";
				buildingEntity.ignoreBodyOptimize = true;
				buildingEntity.itemsDropOnDestroy = new ItemDrop[0];
				buildingEntity.alwaysDrop = new ItemDrop[0];
				buildingEntity.itemCategoriesToAdd = new string[0];
				buildingEntity.hitSound = Resources.Load<AudioClip>("Sounds/goo");
				gameObject.AddComponent<WormHitRelay>().worm = this;
				this.entities.Add(buildingEntity);
			}
			this.entity = this.entities[0];
			this.hp = Mathf.Max(1f, Plugin.WormHealth.Value);
			Light2D light2D = this.parts[0].gameObject.AddComponent<Light2D>();
			light2D.lightType = 3;
			light2D.color = new Color32(200, 120, 110, byte.MaxValue);
			light2D.intensity = 0.35f;
			light2D.pointLightOuterRadius = 14f;
		}

		// Token: 0x06000762 RID: 1890 RVA: 0x000718A0 File Offset: 0x0006FAA0
		public void Provoke(TurretScript t)
		{
			if (!t || this.leaving || this.mode == WormBeast.Mode.Holding)
			{
				return;
			}
			this.foeTurret = t;
			this.turretUntil = Time.time + 30f;
			if (this.mode == WormBeast.Mode.Wander || this.mode == WormBeast.Mode.Flee)
			{
				this.StartHunt(t.transform.position, "is shot by a turret");
			}
		}

		// Token: 0x06000763 RID: 1891 RVA: 0x0007190C File Offset: 0x0006FB0C
		private static Scavenger NearestPrey(Vector2 from, float range)
		{
			Scavenger scavenger = null;
			float num = range;
			foreach (Scavenger scavenger2 in Scavengers.All)
			{
				if (scavenger2 && !scavenger2.slimeHost && scavenger2.mode != Scavenger.Mode.Dead && (!scavenger2.hit || scavenger2.hit.health > 0f))
				{
					float num2 = Vector2.Distance(from, scavenger2.Chest);
					if (num2 < num)
					{
						num = num2;
						scavenger = scavenger2;
					}
				}
			}
			return scavenger;
		}

		// Token: 0x06000764 RID: 1892 RVA: 0x000719AC File Offset: 0x0006FBAC
		private void Update()
		{
			float deltaTime = Time.deltaTime;
			if (deltaTime <= 0f || this.joints.Count == 0)
			{
				return;
			}
			if (this.bursting)
			{
				return;
			}
			if (this.sated > 0f)
			{
				this.sated -= deltaTime;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Vector2 vector = (body ? body.transform.position : this.HeadPos);
			float num = Vector2.Distance(this.Maw, vector);
			if ((this.mode == WormBeast.Mode.Hunt || this.mode == WormBeast.Mode.Lunge) && (!body || !body.alive || !Worm.CanHunt(body)) && (!this.foeTurret || Time.time >= this.turretUntil) && !WormBeast.NearestPrey(this.Maw, 22f))
			{
				this.mode = WormBeast.Mode.Wander;
			}
			Vector2 vector3;
			float num2;
			float num3;
			switch (this.mode)
			{
			case WormBeast.Mode.Hunt:
			case WormBeast.Mode.Lunge:
			{
				bool flag = this.Senses(body, num);
				BuildingEntity buildingEntity = ((this.foeTurret && Time.time < this.turretUntil) ? this.foeTurret.GetComponent<BuildingEntity>() : null);
				bool flag2 = buildingEntity && buildingEntity.health >= 350f;
				Scavenger scavenger = (flag2 ? null : WormBeast.NearestPrey(this.Maw, 22f));
				bool flag3 = scavenger && (!flag || Vector2.Distance(this.Maw, scavenger.Chest) < num);
				Vector2 vector2 = (flag2 ? this.foeTurret.transform.position : (flag3 ? scavenger.Chest : Vector2.zero));
				if (flag)
				{
					this.lastSeen = vector;
					this.lostFor = 0f;
				}
				else if (Time.time - this.noiseTime < Plugin.WormNoiseMemory.Value)
				{
					this.lastSeen = this.noiseAt;
					this.lostFor = 0f;
				}
				else if (flag2 || flag3)
				{
					this.lostFor = 0f;
				}
				else
				{
					this.lostFor += deltaTime;
				}
				if (this.lostFor >= 5f)
				{
					this.GiveUp(body);
					vector3 = this.wanderTarget;
					num2 = 3f;
					num3 = 60f;
				}
				else
				{
					vector3 = (flag ? vector : this.lastSeen);
					this.preyClose = (flag2 || flag3) && Vector2.Distance(this.Maw, vector2) < 14f;
					if (flag2 || flag3)
					{
						vector3 = vector2;
						this.lostFor = 0f;
						if (Vector2.Distance(this.Maw, vector2) < 3.2f && Time.time > this.campBite)
						{
							this.campBite = Time.time + 2f;
							this.Growl(1f);
							if (flag2)
							{
								buildingEntity.health = Mathf.Min(buildingEntity.health, 300f);
								Sound.Play("eatFlesh", vector2, false, true, null, 1f, 0.6f, false, false);
								Plugin.Log.LogInfo(string.Format("The giant worm crushes a turret at {0}", vector2));
								this.foeTurret = null;
								this.preyClose = false;
								this.gulpUntil = Time.time + 0.7f;
							}
							else
							{
								Sound.Play("eatFlesh", vector2, false, true, null, 1f, 0.5f, false, false);
								Sound.Play("gore" + Random.Range(1, 6).ToString(), vector2, false, true, null, 0.7f, 1f, false, false);
								scavenger.SwallowedByWorm();
								this.preyClose = false;
								this.gulpUntil = Time.time + 0.7f;
								this.sated = Mathf.Max(this.sated, 20f);
								this.mode = WormBeast.Mode.Wander;
							}
						}
					}
					if (this.mode == WormBeast.Mode.Hunt)
					{
						num2 = Plugin.WormSpeed.Value;
						num3 = 140f;
						if (flag && num < 12f)
						{
							this.mode = WormBeast.Mode.Lunge;
							this.Growl(1f);
						}
						if (this.preyClose)
						{
							num2 = Plugin.WormSpeed.Value * 1.6f;
							num3 = 260f;
						}
					}
					else
					{
						num2 = Plugin.WormSpeed.Value * 1.6f;
						num3 = 260f;
						if (num > 16f || !flag)
						{
							this.mode = WormBeast.Mode.Hunt;
						}
						if (body && body.alive && num < 3.5f)
						{
							Worm.Swallow(body, this);
						}
					}
				}
				break;
			}
			case WormBeast.Mode.Holding:
				this.Animate(deltaTime, 0f);
				return;
			case WormBeast.Mode.Flee:
				this.fleeTimer -= deltaTime;
				vector3 = this.HeadPos + (this.HeadPos - vector).normalized * 20f;
				num2 = (this.leaving ? 7f : 10f);
				num3 = 200f;
				if (this.leaving && this.OutOfSight(vector))
				{
					Plugin.Log.LogInfo("The giant worm has gone, out of sight");
					Worm.Beasts.Remove(this);
					Object.Destroy(base.gameObject);
					return;
				}
				if (this.fleeTimer <= 0f && !this.leaving)
				{
					this.mode = WormBeast.Mode.Wander;
					Body body2 = (PlayerCamera.main ? PlayerCamera.main.body : null);
					if (this.Enraged && body2 && body2.alive && Worm.CanHunt(body2))
					{
						this.StartHunt(body2.transform.position, "comes back at her in its rage");
					}
				}
				break;
			default:
				if ((this.retarget -= deltaTime) <= 0f || Vector2.Distance(this.HeadPos, this.wanderTarget) < 3f)
				{
					this.retarget = Random.Range(6f, 12f);
					this.wanderTarget = this.home + Random.insideUnitCircle * 30f;
				}
				vector3 = this.wanderTarget;
				num2 = 3f;
				num3 = 60f;
				if (this.sated <= 0f && body && body.alive && Worm.CanHunt(body) && num < 5f)
				{
					this.StartHunt(vector, string.Format("feels her right by it ({0:0} away)", num));
				}
				else if (this.sated <= 0f)
				{
					Scavenger scavenger2 = WormBeast.NearestPrey(this.Maw, 10f);
					if (scavenger2 != null)
					{
						this.StartHunt(scavenger2.Chest, "feels a raider right by it");
					}
				}
				break;
			}
			Vector2 normalized = (vector3 - this.HeadPos).normalized;
			if (normalized.sqrMagnitude > 0.01f)
			{
				float num4 = Vector2.SignedAngle(this.dir, normalized);
				this.dir = Quaternion.Euler(0f, 0f, Mathf.Clamp(num4, -num3 * deltaTime, num3 * deltaTime)) * this.dir;
				this.dir.Normalize();
			}
			Vector2 vector4 = this.HeadPos + this.dir * num2 * deltaTime;
			WorldGeneration world = WorldGeneration.world;
			vector4.x = Mathf.Clamp(vector4.x, -world.halfWidth + 6f, world.halfWidth - 6f);
			vector4.y = Mathf.Clamp(vector4.y, -world.halfHeight + 6f, world.halfHeight - 6f);
			this.joints[0] = vector4;
			this.Animate(deltaTime, num2);
			this.Bait(body, deltaTime);
			this.FireScare(body, deltaTime);
			this.Bomb(deltaTime);
			this.Rumble(num, deltaTime);
			this.Hear(body, num, deltaTime);
			this.Dig(deltaTime, vector);
			if ((this.mode == WormBeast.Mode.Hunt || this.mode == WormBeast.Mode.Lunge) && (this.growlTimer -= deltaTime) <= 0f)
			{
				this.Growl(0.7f);
			}
			if (Vector2.Distance(this.HeadPos, vector) < 60f && (this.dustTimer -= deltaTime) <= 0f && world.GetBlock(world.WorldToBlockPos(this.HeadPos)) != 0)
			{
				this.dustTimer = ((this.mode == WormBeast.Mode.Wander) ? 0.5f : 0.25f);
				GameObject gameObject = Resources.Load<GameObject>("DustBig");
				if (gameObject)
				{
					Object.Destroy(Object.Instantiate<GameObject>(gameObject, this.HeadPos, Quaternion.identity), 3f);
				}
			}
		}

		// Token: 0x06000765 RID: 1893 RVA: 0x000722AD File Offset: 0x000704AD
		private bool Senses(Body b, float dist)
		{
			return b && b.alive && (dist < 5f || (Time.time - this.herNoiseTime < 3f && dist < Plugin.WormSenseRange.Value));
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x000722F0 File Offset: 0x000704F0
		public void OnNoise(Vector2 at, float radius, string what)
		{
			if (this.mode == WormBeast.Mode.Holding || this.leaving || this.mode == WormBeast.Mode.Flee || this.bombTimer >= 0f)
			{
				return;
			}
			if (Vector2.Distance(this.HeadPos, at) > radius)
			{
				return;
			}
			if (this.sated > 0f && what != "blast" && what != "fuse")
			{
				return;
			}
			if (Time.time < this.calmUntil && (what == "run" || what == "moan" || what == "drop" || what == "fall"))
			{
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			this.noiseAt = at;
			this.noiseTime = Time.time;
			if (body && Vector2.Distance(at, body.transform.position) < 3.5f)
			{
				this.herNoiseTime = Time.time;
			}
			if (this.mode == WormBeast.Mode.Wander && (!body || Worm.CanHunt(body)))
			{
				this.StartHunt(at, string.Format("hears {0} ({1:0} away)", what, Vector2.Distance(this.HeadPos, at)));
			}
		}

		// Token: 0x06000767 RID: 1895 RVA: 0x00072438 File Offset: 0x00070638
		private void Bait(Body b, float dt)
		{
			if (this.bombTimer >= 0f || this.leaving || this.mode == WormBeast.Mode.Holding || (this.baitScan -= dt) > 0f)
			{
				return;
			}
			this.baitScan = 0.4f;
			foreach (CustomItemBehaviour customItemBehaviour in Object.FindObjectsOfType<CustomItemBehaviour>())
			{
				Item item = (customItemBehaviour ? customItemBehaviour.GetComponent<Item>() : null);
				if (item && !(item.id != "dynamite") && customItemBehaviour.data != null && customItemBehaviour.data.Length != 0)
				{
					object obj = customItemBehaviour.data[0];
					if (obj is bool)
					{
						bool flag = (bool)obj;
						if (flag)
						{
							Vector2 vector = item.transform.position;
							float num = Vector2.Distance(this.Maw, vector);
							if (num <= Plugin.WormNoiseFuse.Value)
							{
								this.OnNoise(vector, Plugin.WormNoiseFuse.Value, "fuse");
								bool flag2 = !item.transform.parent || !item.transform.IsChildOf(b ? b.transform : item.transform.parent);
								if (num < 3f && flag2 && !item.ParentContainer())
								{
									customItemBehaviour.CancelInvoke("DynamiteExplode");
									Object.Destroy(item.gameObject);
									this.bombTimer = 1.2f;
									this.mode = WormBeast.Mode.Hunt;
									Sound.Play("eatFlesh", this.HeadPos, false, true, null, 1f, 0.5f, false, false);
									if (b && b.alive && Vector2.Distance(this.HeadPos, b.transform.position) < 40f)
									{
										Relief.Say(b, "worm_bomb_eaten");
									}
									Plugin.Log.LogInfo("The giant worm swallowed a lit stick of dynamite");
									return;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06000768 RID: 1896 RVA: 0x0007265C File Offset: 0x0007085C
		public void BombInside(float seconds)
		{
			this.bombTimer = Mathf.Max(0.1f, seconds);
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x00072670 File Offset: 0x00070870
		private void Bomb(float dt)
		{
			if (this.bombTimer < 0f || (this.bombTimer -= dt) > 0f)
			{
				return;
			}
			this.bombTimer = -1f;
			Plugin.Log.LogInfo("The dynamite went off inside the giant worm");
			this.Explode();
		}

		// Token: 0x0600076A RID: 1898 RVA: 0x000726C3 File Offset: 0x000708C3
		public void Explode()
		{
			if (this.bursting)
			{
				return;
			}
			this.bursting = true;
			base.StartCoroutine(this.Burst());
		}

		// Token: 0x0600076B RID: 1899 RVA: 0x000726E2 File Offset: 0x000708E2
		private IEnumerator Burst()
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Worm.Holder == this && body)
			{
				Worm.LetGo(body);
			}
			GameObject gameObject = Resources.Load("Special/ExplosionParticle") as GameObject;
			if (gameObject)
			{
				Object.Destroy(Object.Instantiate<GameObject>(gameObject, this.HeadPos, Quaternion.identity), 4f);
			}
			Sound.Play("explosion", this.HeadPos, false, true, null, 1f, 0.7f, false, false);
			PlayerCamera main = PlayerCamera.main;
			if (main != null)
			{
				main.shaker.Shake(Plugin.WormShake.Value * 1.5f);
			}
			this.Roar(1f);
			GameObject blood = Resources.Load("BloodExplosion") as GameObject;
			GameObject meat = Resources.Load("animalflesh") as GameObject;
			int num;
			for (int i = 0; i < this.parts.Count; i = num + 1)
			{
				SpriteRenderer spriteRenderer = this.parts[i];
				if (spriteRenderer)
				{
					Vector2 vector = this.joints[i];
					if (blood)
					{
						Object.Destroy(Object.Instantiate<GameObject>(blood, vector, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))), 6f);
					}
					if (i % 2 == 0)
					{
						Sound.Play("gore" + Random.Range(1, 6).ToString(), vector, false, true, null, 0.9f, Random.Range(0.6f, 0.8f), false, false);
					}
					Rigidbody2D rigidbody2D;
					if (meat && Random.value < 0.6f && Object.Instantiate<GameObject>(meat, vector, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).TryGetComponent<Rigidbody2D>(ref rigidbody2D))
					{
						rigidbody2D.velocity = Random.insideUnitCircle.normalized * Random.Range(6f, 14f) + Vector2.up * 4f;
						rigidbody2D.angularVelocity = Random.Range(-600f, 600f);
					}
					spriteRenderer.enabled = false;
					yield return new WaitForSeconds(0.06f);
				}
				num = i;
			}
			this.Die(false);
			yield break;
		}

		// Token: 0x0600076C RID: 1900 RVA: 0x000726F4 File Offset: 0x000708F4
		private void FireScare(Body b, float dt)
		{
			if (this.leaving || (this.mode != WormBeast.Mode.Hunt && this.mode != WormBeast.Mode.Lunge) || (this.fireScan -= dt) > 0f)
			{
				return;
			}
			this.fireScan = 0.25f;
			float value = Plugin.WormFireScare.Value;
			if (value <= 0f)
			{
				return;
			}
			foreach (Item item in Object.FindObjectsOfType<Item>())
			{
				if (item && (!(item.id != "torch") || !(item.id != "campfire")) && item.condition > 0f && !item.ParentContainer())
				{
					Vector2 vector = item.transform.position;
					if (Vector2.Distance(this.Maw, vector) <= value)
					{
						this.Roar(0.8f);
						if (this.Enraged)
						{
							this.Flee(1.5f);
						}
						else
						{
							this.Retreat(8f, 25f, b);
						}
						if (b && b.alive)
						{
							Relief.Say(b, "worm_fire");
						}
						Plugin.Log.LogInfo("The giant worm shies away from a " + ((item.id == "torch") ? "torch" : "campfire"));
						return;
					}
				}
			}
		}

		// Token: 0x1700011D RID: 285
		// (get) Token: 0x0600076D RID: 1901 RVA: 0x0007286F File Offset: 0x00070A6F
		// (set) Token: 0x0600076E RID: 1902 RVA: 0x00072877 File Offset: 0x00070A77
		public bool Enraged { get; private set; }

		// Token: 0x0600076F RID: 1903 RVA: 0x00072880 File Offset: 0x00070A80
		public void TakeHit(float dmg, string why)
		{
			if (dmg <= 0f || this.leaving)
			{
				return;
			}
			if (why != "a blast" && Time.time - this.lastHitTime < 0.1f && Mathf.Abs(dmg - this.lastHitDmg) < 0.5f)
			{
				return;
			}
			this.lastHitTime = Time.time;
			this.lastHitDmg = dmg;
			foreach (BuildingEntity buildingEntity in this.entities)
			{
				if (buildingEntity)
				{
					buildingEntity.health = 100000f;
				}
			}
			this.hp -= dmg;
			float now = Time.time;
			this.hits.Add(new ValueTuple<float, float>(now, dmg));
			this.hits.RemoveAll(([TupleElementNames(new string[] { "t", "dmg" })] ValueTuple<float, float> h) => now - h.Item1 > 6f);
			float num = 0f;
			foreach (ValueTuple<float, float> valueTuple in this.hits)
			{
				num += valueTuple.Item2;
			}
			Plugin.Log.LogInfo(string.Format("The giant worm takes {0:0} ({1}): {2:0} left, {3:0} in the last 6 s", new object[]
			{
				dmg,
				why,
				Mathf.Max(0f, this.hp),
				num
			}));
			if (this.hp <= 0f)
			{
				this.Die(true);
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && why != "a blast")
			{
				this.noiseAt = body.transform.position;
				this.noiseTime = (this.herNoiseTime = Time.time);
			}
			if (num >= Plugin.WormRetreatDamage.Value && this.mode != WormBeast.Mode.Holding && !this.Enraged)
			{
				this.hits.Clear();
				this.Roar(1f);
				this.Retreat(20f, 90f, body);
				if (body && body.alive)
				{
					Relief.Say(body, "worm_retreat");
				}
				Plugin.Log.LogInfo("The giant worm has had enough of it and backs off");
				return;
			}
			this.OnOutsideHit();
		}

		// Token: 0x06000770 RID: 1904 RVA: 0x00072B04 File Offset: 0x00070D04
		public string State(Body b)
		{
			return string.Format("mode {0}, maw {1:0} away, senses her {2}, last noise {3:0.0}s ago at {4}, hp {5:0}, lost for {6:0.0}s, sated {7:0}", new object[]
			{
				this.mode,
				Vector2.Distance(this.Maw, b.transform.position),
				this.Senses(b, Vector2.Distance(this.Maw, b.transform.position)),
				Time.time - this.noiseTime,
				this.noiseAt,
				this.hp,
				this.lostFor,
				this.sated
			});
		}

		// Token: 0x06000771 RID: 1905 RVA: 0x00072BC8 File Offset: 0x00070DC8
		public void StartHunt(Vector2 her, string why)
		{
			this.mode = WormBeast.Mode.Hunt;
			this.lastSeen = her;
			this.lostFor = 0f;
			this.Roar(1f);
			Sound.Play("geyserrumble", her, false, true, null, 0.8f, 0.6f, false, false);
			Plugin.Log.LogInfo("The giant worm " + why + " and hunts");
		}

		// Token: 0x06000772 RID: 1906 RVA: 0x00072C30 File Offset: 0x00070E30
		public void Roar(float vol)
		{
			if (this.roarTimer > Time.time)
			{
				return;
			}
			this.roarTimer = Time.time + 1.4f;
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body)
			{
				return;
			}
			float num = Mathf.Max(30f, Plugin.WormHearRange.Value);
			float num2 = Mathf.Clamp01(1f - Vector2.Distance(this.HeadPos, body.transform.position) / num);
			if (num2 <= 0f)
			{
				return;
			}
			Sfx.Play(body, vol * (0.3f + 0.7f * num2), new string[] { "worm_roar" });
			Sound.Play("growl", this.HeadPos, false, true, null, vol * num2, 0.3f, false, false);
			PlayerCamera.main.shaker.Shake(Plugin.WormShake.Value * 0.5f * num2);
		}

		// Token: 0x06000773 RID: 1907 RVA: 0x00072D28 File Offset: 0x00070F28
		public void Retreat(float seconds, float calm, Body b)
		{
			Vector2 vector = (b ? b.transform.position : this.HeadPos);
			Vector2 vector2 = (((this.HeadPos - vector).sqrMagnitude > 0.01f) ? (this.HeadPos - vector).normalized : (-this.dir));
			WorldGeneration world = WorldGeneration.world;
			Vector2 vector3 = this.HeadPos + vector2 * 70f;
			if (world)
			{
				vector3.x = Mathf.Clamp(vector3.x, -world.halfWidth + 15f, world.halfWidth - 15f);
				vector3.y = Mathf.Clamp(vector3.y, -world.halfHeight + 15f, world.halfHeight - 15f);
			}
			this.home = vector3;
			this.wanderTarget = vector3;
			this.retarget = 12f;
			this.leaving = false;
			this.sated = Mathf.Max(this.sated, calm);
			this.shakenUntil = Mathf.Max(this.shakenUntil, Time.time + calm);
			this.Flee(seconds);
		}

		// Token: 0x06000774 RID: 1908 RVA: 0x00072E6C File Offset: 0x0007106C
		private void GiveUp(Body b)
		{
			this.mode = WormBeast.Mode.Wander;
			if (this.Enraged)
			{
				Plugin.Log.LogInfo("The giant worm's rage is spent");
			}
			this.Enraged = false;
			this.calmUntil = Time.time + 25f;
			this.lostFor = 0f;
			this.retarget = 0f;
			Sound.Play("growl", this.HeadPos, false, true, null, 0.5f, 0.35f, false, false);
			if (b && b.alive)
			{
				Relief.Say(b, "worm_lost");
			}
			Plugin.Log.LogInfo("The giant worm heard nothing more of her and gives up");
		}

		// Token: 0x06000775 RID: 1909 RVA: 0x00072F10 File Offset: 0x00071110
		private void Hear(Body b, float dist, float dt)
		{
			float value = Plugin.WormHearRange.Value;
			if (!b || !b.alive || value <= 0f || dist > value || (this.hearTimer -= dt) > 0f)
			{
				return;
			}
			float num = 1f - dist / value;
			this.hearTimer = Mathf.Lerp(7f, 2.5f, num);
			float num2 = 0.15f + 0.65f * num * num;
			Sound.Play((Random.value < 0.25f) ? "growl" : "geyserrumble", b.transform.position, true, true, null, num2, Random.Range(0.32f, 0.45f), false, false);
			if (!Worm.HeardThisLayer && this.mode == WormBeast.Mode.Wander)
			{
				Worm.HeardThisLayer = true;
				Relief.Say(b, "worm_hear");
			}
		}

		// Token: 0x06000776 RID: 1910 RVA: 0x00072FF0 File Offset: 0x000711F0
		private void Rumble(float dist, float dt)
		{
			PlayerCamera main = PlayerCamera.main;
			float value = Plugin.WormShakeRange.Value;
			if (!main || dist > value || (this.shakeTimer -= dt) > 0f)
			{
				return;
			}
			this.shakeTimer = 0.12f;
			float num = 1f - dist / value;
			float num2 = ((this.mode == WormBeast.Mode.Lunge) ? 1.6f : ((this.mode == WormBeast.Mode.Hunt) ? 1f : ((this.mode == WormBeast.Mode.Flee) ? 0.7f : 0.4f)));
			main.shaker.Shake(Plugin.WormShake.Value * num * num * num2);
		}

		// Token: 0x06000777 RID: 1911 RVA: 0x0007309C File Offset: 0x0007129C
		private void Dig(float dt, Vector2 her)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world || Plugin.WormDigMaxHealth.Value <= 0f || (this.digTimer -= dt) > 0f)
			{
				return;
			}
			this.digTimer = 0.1f;
			if (Vector2.Distance(this.HeadPos, her) > 60f)
			{
				return;
			}
			float num = ((WormArt.Head.Length != 0) ? WormArt.Head[0].bounds.extents.y : 0.5f) * 2f * 0.9f;
			Vector2Int vector2Int = world.WorldToBlockPos(this.HeadPos + this.dir * num * 0.5f);
			int num2 = Mathf.CeilToInt(num);
			for (int i = -num2; i <= num2; i++)
			{
				for (int j = -num2; j <= num2; j++)
				{
					if ((float)(i * i + j * j) <= num * num)
					{
						Vector2Int vector2Int2;
						vector2Int2..ctor(vector2Int.x + i, vector2Int.y + j);
						ushort block = world.GetBlock(vector2Int2);
						if (block != 0)
						{
							BlockInfo blockInfo = world.GetBlockInfo(block);
							if (blockInfo != null && blockInfo.health <= Plugin.WormDigMaxHealth.Value)
							{
								world.DamageBlock(vector2Int2, blockInfo.health + 1f, false, false, true);
							}
						}
					}
				}
			}
		}

		// Token: 0x06000778 RID: 1912 RVA: 0x00073204 File Offset: 0x00071404
		private bool OutOfSight(Vector2 her)
		{
			Camera main = Camera.main;
			float num = (main ? (main.orthographicSize * Mathf.Max(1f, main.aspect)) : 15f);
			using (List<Vector2>.Enumerator enumerator = this.joints.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (Vector2.Distance(enumerator.Current, main ? main.transform.position : her) < num * 1.3f + 4f)
					{
						return false;
					}
				}
			}
			return true;
		}

		// Token: 0x06000779 RID: 1913 RVA: 0x000732B4 File Offset: 0x000714B4
		private void Growl(float vol)
		{
			this.growlTimer = Random.Range(3f, 6f);
			Sound.Play("growl", this.HeadPos, false, true, null, vol, 0.45f, false, false);
		}

		// Token: 0x0600077A RID: 1914 RVA: 0x000732F4 File Offset: 0x000714F4
		private void Animate(float dt, float speed)
		{
			for (int i = 1; i < this.joints.Count; i++)
			{
				Vector2 vector = this.joints[i] - this.joints[i - 1];
				if (vector.magnitude > this.spacing)
				{
					this.joints[i] = this.joints[i - 1] + vector.normalized * this.spacing;
				}
			}
			for (int j = 0; j < this.parts.Count; j++)
			{
				SpriteRenderer spriteRenderer = this.parts[j];
				if (spriteRenderer)
				{
					Vector2 vector2 = ((j == 0) ? this.dir : (this.joints[j - 1] - this.joints[j]).normalized);
					float num = Mathf.Sin(Time.time * 3f - (float)j * 0.8f) * ((speed > 0f) ? 0.25f : 0.08f);
					spriteRenderer.transform.position = this.joints[j] + new Vector2(-vector2.y, vector2.x) * num;
					spriteRenderer.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(vector2.y, vector2.x) * 57.29578f);
					spriteRenderer.flipY = vector2.x < 0f;
					float num2 = ((j == 0) ? 1f : Mathf.Lerp(1f, 0.7f, (float)j / (float)(this.parts.Count - 1)));
					spriteRenderer.transform.localScale = new Vector3(2f, 2f * num2, 1f);
				}
			}
			if (WormArt.Head.Length > 1)
			{
				bool flag = Time.time >= this.gulpUntil && (this.mode == WormBeast.Mode.Lunge || ((this.mode == WormBeast.Mode.Hunt || this.mode == WormBeast.Mode.Lunge) && this.preyClose) || (this.mode == WormBeast.Mode.Hunt && Vector2.Distance(this.HeadPos, PlayerCamera.main ? PlayerCamera.main.body.transform.position : this.HeadPos) < 16f));
				this.parts[0].sprite = WormArt.Head[(flag > false) ? 1 : 0];
			}
		}

		// Token: 0x0600077B RID: 1915 RVA: 0x0007358D File Offset: 0x0007178D
		public void Flee(float seconds)
		{
			this.mode = WormBeast.Mode.Flee;
			this.fleeTimer = seconds;
			this.Hidden = false;
		}

		// Token: 0x0600077C RID: 1916 RVA: 0x000735A4 File Offset: 0x000717A4
		public void DebugBeat()
		{
			this.OnOutsideHit();
		}

		// Token: 0x0600077D RID: 1917 RVA: 0x000735AC File Offset: 0x000717AC
		public void OnOutsideHit()
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			foreach (BuildingEntity buildingEntity in this.entities)
			{
				if (buildingEntity)
				{
					buildingEntity.health = 100000f;
				}
			}
			this.Roar(0.9f);
			if (!this.Enraged && Time.time < this.shakenUntil && this.mode != WormBeast.Mode.Holding && body && body.alive && !this.leaving && Worm.CanHunt(body))
			{
				this.Enraged = true;
				this.shakenUntil = 0f;
				this.sated = 0f;
				this.hits.Clear();
				this.Roar(1f);
				this.StartHunt(body.transform.position, "is hit again and flies into a rage");
				if (body.alive)
				{
					Relief.Say(body, "worm_rage");
				}
				return;
			}
			if ((this.mode == WormBeast.Mode.Wander || (this.mode == WormBeast.Mode.Flee && !this.leaving && this.sated <= 0f)) && body && body.alive && Worm.CanHunt(body))
			{
				this.sated = 0f;
				this.StartHunt(body.transform.position, "is hit");
				return;
			}
			if ((this.mode == WormBeast.Mode.Hunt || this.mode == WormBeast.Mode.Lunge) && !this.Enraged && Random.value < 0.15f)
			{
				this.Flee(3f);
				this.sated = 6f;
			}
		}

		// Token: 0x0600077E RID: 1918 RVA: 0x0007377C File Offset: 0x0007197C
		public void Die(bool chunks = true)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Worm.Holder == this && body)
			{
				Worm.LetGo(body);
			}
			this.Roar(1f);
			Sound.Play("gore", this.HeadPos, false, true, null, 1f, 0.6f, false, false);
			foreach (SpriteRenderer spriteRenderer in this.parts)
			{
				if (chunks && spriteRenderer && Random.value < 0.35f)
				{
					GameObject gameObject = Resources.Load("animalflesh") as GameObject;
					if (gameObject)
					{
						Object.Instantiate<GameObject>(gameObject, spriteRenderer.transform.position, Quaternion.identity);
					}
				}
			}
			if (body && body.alive)
			{
				Relief.Say(body, "worm_killed");
			}
			Plugin.Log.LogInfo("The giant worm is dead");
			Worm.Beasts.Remove(this);
			Object.Destroy(base.gameObject);
		}

		// Token: 0x0600077F RID: 1919 RVA: 0x000738B0 File Offset: 0x00071AB0
		public void Indigestion()
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (Worm.Holder == this && body)
			{
				Worm.LetGo(body);
			}
			this.Roar(1f);
			Sound.Play("vomit", this.HeadPos, false, true, null, 1f, 0.4f, false, false);
			this.mode = WormBeast.Mode.Flee;
			this.leaving = true;
			this.Flee(30f);
			if (body && body.alive)
			{
				Relief.Say(body, "worm_indigestion");
			}
			Plugin.Log.LogInfo("The giant worm choked on a blast in its mouth: it leaves for good");
		}

		// Token: 0x06000780 RID: 1920 RVA: 0x00073960 File Offset: 0x00071B60
		private void OnDestroy()
		{
			foreach (SpriteRenderer spriteRenderer in this.parts)
			{
				if (spriteRenderer)
				{
					Object.Destroy(spriteRenderer.gameObject);
				}
			}
			Worm.Beasts.Remove(this);
		}

		// Token: 0x0400078E RID: 1934
		public WormBeast.Mode mode;

		// Token: 0x0400078F RID: 1935
		public const float Size = 2f;

		// Token: 0x04000790 RID: 1936
		public float sated;

		// Token: 0x04000791 RID: 1937
		public Vector2 home;

		// Token: 0x04000792 RID: 1938
		public readonly List<Vector2> joints = new List<Vector2>();

		// Token: 0x04000793 RID: 1939
		public readonly List<SpriteRenderer> parts = new List<SpriteRenderer>();

		// Token: 0x04000794 RID: 1940
		public Vector2 dir = Vector2.right;

		// Token: 0x04000795 RID: 1941
		private Vector2 wanderTarget;

		// Token: 0x04000796 RID: 1942
		private float spacing;

		// Token: 0x04000797 RID: 1943
		private float growlTimer;

		// Token: 0x04000798 RID: 1944
		private float dustTimer;

		// Token: 0x04000799 RID: 1945
		private float fleeTimer;

		// Token: 0x0400079A RID: 1946
		private float retarget;

		// Token: 0x0400079B RID: 1947
		private float shakeTimer;

		// Token: 0x0400079C RID: 1948
		private float digTimer;

		// Token: 0x0400079D RID: 1949
		private float hearTimer;

		// Token: 0x0400079E RID: 1950
		private float lostFor;

		// Token: 0x0400079F RID: 1951
		private Vector2 lastSeen;

		// Token: 0x040007A0 RID: 1952
		private BuildingEntity entity;

		// Token: 0x040007A1 RID: 1953
		private Vector2 noiseAt;

		// Token: 0x040007A2 RID: 1954
		private float noiseTime = -99f;

		// Token: 0x040007A3 RID: 1955
		private float herNoiseTime = -99f;

		// Token: 0x040007A4 RID: 1956
		private float baitScan;

		// Token: 0x040007A5 RID: 1957
		private float fireScan;

		// Token: 0x040007A6 RID: 1958
		private float hp;

		// Token: 0x040007A7 RID: 1959
		private float bombTimer = -1f;

		// Token: 0x040007A8 RID: 1960
		[TupleElementNames(new string[] { "t", "dmg" })]
		private readonly List<ValueTuple<float, float>> hits = new List<ValueTuple<float, float>>();

		// Token: 0x040007A9 RID: 1961
		private readonly List<BuildingEntity> entities = new List<BuildingEntity>();

		// Token: 0x040007AA RID: 1962
		private const float TouchRange = 5f;

		// Token: 0x040007AB RID: 1963
		private float roarTimer;

		// Token: 0x040007AC RID: 1964
		private const float GiveUpSeconds = 10f;

		// Token: 0x040007AD RID: 1965
		private const float SmellRange = 10f;

		// Token: 0x040007AE RID: 1966
		public bool leaving;

		// Token: 0x040007AF RID: 1967
		private float campBite;

		// Token: 0x040007B0 RID: 1968
		private bool preyClose;

		// Token: 0x040007B1 RID: 1969
		private float gulpUntil;

		// Token: 0x040007B2 RID: 1970
		private TurretScript foeTurret;

		// Token: 0x040007B3 RID: 1971
		private float turretUntil;

		// Token: 0x040007B4 RID: 1972
		private bool bursting;

		// Token: 0x040007B5 RID: 1973
		private float lastHitTime = -99f;

		// Token: 0x040007B6 RID: 1974
		private float lastHitDmg;

		// Token: 0x040007B7 RID: 1975
		private float calmUntil;

		// Token: 0x040007B8 RID: 1976
		private float shakenUntil;

		// Token: 0x040007BA RID: 1978
		public const float Toughness = 100000f;

		// Token: 0x02000257 RID: 599
		public enum Mode
		{
			// Token: 0x04000B71 RID: 2929
			Wander,
			// Token: 0x04000B72 RID: 2930
			Hunt,
			// Token: 0x04000B73 RID: 2931
			Lunge,
			// Token: 0x04000B74 RID: 2932
			Holding,
			// Token: 0x04000B75 RID: 2933
			Flee
		}
	}
}
