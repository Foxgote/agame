using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Logging;
using LewdStatus;
using PoseKit;
using TMPro;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000042 RID: 66
	internal class Scavenger : MonoBehaviour
	{
		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000114 RID: 276 RVA: 0x0000FF98 File Offset: 0x0000E198
		private bool gentleNow
		{
			get
			{
				return !Diff.LethalScenes;
			}
		}

		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000115 RID: 277 RVA: 0x0000FFA2 File Offset: 0x0000E1A2
		private bool Hurt
		{
			get
			{
				return this.hit && this.hit.health < this.maxHealth * 0.4f;
			}
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000FFCC File Offset: 0x0000E1CC
		public static Scavenger MakeHost(Vector2 at, bool fromHer = false, Scavenger pack = null)
		{
			bool flag = fromHer || Random.value < 0.5f;
			string text = (flag ? "slimehost_f" : "slimehost_m");
			bool flag2 = ScavArt.Skins.ContainsKey(text);
			Scavenger scavenger = Scavenger.Make(at, Scavenger.Bare, flag, flag2 ? text : null, flag2, null);
			scavenger.slimeHost = true;
			scavenger.grabber = true;
			scavenger.lecher = false;
			scavenger.hit.health *= 0.7f;
			scavenger.rig.Weapon.enabled = false;
			scavenger.rig.Slime(flag2);
			scavenger.hit.id = "arm_slimehost";
			scavenger.pack = pack;
			if (fromHer)
			{
				scavenger.stayNear = true;
				scavenger.sated = 9999f;
			}
			return scavenger;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x00010094 File Offset: 0x0000E294
		public static Scavenger Make(Vector2 at, WeaponDef w, bool female, string skin = null, bool bare = false, string wear = null)
		{
			GameObject gameObject = new GameObject("Scavenger");
			Scavenger scavenger = gameObject.AddComponent<Scavenger>();
			scavenger.female = female;
			scavenger.skin = skin ?? ScavArt.RandomSkin(female);
			scavenger.overall = wear ?? ((!bare && ScavArt.Overalls.ContainsKey("minersuit")) ? "minersuit" : null);
			scavenger.weapon = w;
			at = Scavengers.GroundNear(at).GetValueOrDefault(at);
			scavenger.pos = (scavenger.home = (scavenger.wanderTo = at));
			scavenger.facing = ((Random.value < 0.5f) ? 1 : (-1));
			Scavenger.orderSeed = (Scavenger.orderSeed + 1) % 6;
			scavenger.rig = new ScavRig(gameObject.transform, scavenger.skin, scavenger.overall, -400 + Scavenger.orderSeed * 230);
			if (scavenger.overall == "minersuit")
			{
				scavenger.rig.DyeCloth(ScavArt.RaiderDyes[Random.Range(0, ScavArt.RaiderDyes.Length)]);
			}
			scavenger.Arm();
			scavenger.BuildHitbox();
			scavenger.rounds = w.Mag;
			scavenger.grabber = Random.value < (w.Gun ? Plugin.GrabberShare.Value : Plugin.MeleeGrabberShare.Value);
			scavenger.lecher = scavenger.grabber && Random.value < Plugin.LecherShare.Value;
			Scavengers.All.Add(scavenger);
			return scavenger;
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0001021C File Offset: 0x0000E41C
		private void FindGrip(Sprite sp)
		{
			this.grip = Vector2.zero;
			this.gripTurn = 0f;
			this.twoHandedMelee = this.weapon.Id == "pickaxe" || this.weapon.Id == "shovel" || this.weapon.Id == "sledgehammer" || this.weapon.Id == "scraptube";
			if (!sp)
			{
				return;
			}
			try
			{
				Texture2D texture2D = Bust.Readable(sp.texture);
				Rect textureRect = sp.textureRect;
				int num = (int)textureRect.width;
				int num2 = (int)textureRect.height;
				int num3 = num;
				int num4 = -1;
				int num5 = num2;
				int num6 = -1;
				for (int i = 0; i < num2; i++)
				{
					for (int j = 0; j < num; j++)
					{
						if (texture2D.GetPixel((int)textureRect.x + j, (int)textureRect.y + i).a > 0.3f)
						{
							num3 = Mathf.Min(num3, j);
							num4 = Mathf.Max(num4, j);
							num5 = Mathf.Min(num5, i);
							num6 = Mathf.Max(num6, i);
						}
					}
				}
				if (num4 >= 0)
				{
					float pixelsPerUnit = sp.pixelsPerUnit;
					if (num4 - num3 > num6 - num5)
					{
						this.gripTurn = -90f;
						this.grip = new Vector2((float)num3 + 0.25f * (float)(num4 - num3) - sp.pivot.x, (float)(num5 + num6) * 0.5f - sp.pivot.y) / pixelsPerUnit;
					}
					else
					{
						this.grip = new Vector2((float)(num3 + num4) * 0.5f - sp.pivot.x, (float)num5 + 0.15f * (float)(num6 - num5) - sp.pivot.y) / pixelsPerUnit;
					}
				}
			}
			catch
			{
				this.grip = Vector2.zero;
			}
		}

		// Token: 0x06000119 RID: 281 RVA: 0x00010434 File Offset: 0x0000E634
		private void Arm()
		{
			GameObject gameObject = Resources.Load(this.weapon.Id) as GameObject;
			SpriteRenderer spriteRenderer;
			if (gameObject && gameObject.TryGetComponent<SpriteRenderer>(ref spriteRenderer))
			{
				this.rig.Weapon.sprite = spriteRenderer.sprite;
			}
			if (!this.weapon.Gun)
			{
				this.FindGrip(this.rig.Weapon.sprite);
			}
			GunScript gunScript;
			if (gameObject && gameObject.TryGetComponent<GunScript>(ref gunScript))
			{
				this.fireSound = gunScript.fireSound;
			}
		}

		// Token: 0x0600011A RID: 282 RVA: 0x000104C0 File Offset: 0x0000E6C0
		private void BuildHitbox()
		{
			GameObject gameObject = new GameObject("ScavHit");
			gameObject.transform.SetParent(base.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.layer = LayerMask.NameToLayer("Descriptor");
			Rigidbody2D rigidbody2D = gameObject.AddComponent<Rigidbody2D>();
			rigidbody2D.bodyType = 1;
			rigidbody2D.useFullKinematicContacts = false;
			BoxCollider2D boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
			boxCollider2D.isTrigger = false;
			boxCollider2D.size = new Vector2(1.3f, 4.6f);
			boxCollider2D.offset = new Vector2(0f, 0f);
			this.hit = gameObject.AddComponent<BuildingEntity>();
			this.hit.animal = true;
			this.hit.health = (this.maxHealth = Plugin.ScavHealth.Value * (this.female ? 0.9f : 1.05f));
			this.hit.id = "arm_scav";
			this.hit.ignoreBodyOptimize = true;
			this.hit.itemsDropOnDestroy = new ItemDrop[0];
			this.hit.alwaysDrop = new ItemDrop[0];
			this.hit.itemCategoriesToAdd = new string[0];
			gameObject.AddComponent<ScavHitRelay>().scav = this;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600011B RID: 283 RVA: 0x000105F9 File Offset: 0x0000E7F9
		private Body Player
		{
			get
			{
				if (!PlayerCamera.main)
				{
					return null;
				}
				return PlayerCamera.main.body;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600011C RID: 284 RVA: 0x00010613 File Offset: 0x0000E813
		private Vector2 Eye
		{
			get
			{
				return this.pos + Vector2.up * 4.6f;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600011D RID: 285 RVA: 0x0001062F File Offset: 0x0000E82F
		internal Vector2 Chest
		{
			get
			{
				return this.pos + Vector2.up * 3.4f;
			}
		}

		// Token: 0x0600011E RID: 286 RVA: 0x0001064C File Offset: 0x0000E84C
		private bool CanSee(Body b, float range)
		{
			if (!b || !b.alive || this.sated > 0f)
			{
				return false;
			}
			if (this.slimeHost && !Capture.HostWantsHer)
			{
				return false;
			}
			Vector2 vector = b.limbs[1].transform.position;
			float num = Vector2.Distance(this.Eye, vector);
			return num <= range * WardrobeEffects.SightScale(b) && (Mathf.Sign(vector.x - this.pos.x) == (float)this.facing || num < 5f) && !Physics2D.Linecast(this.Eye, vector, LayerMask.GetMask(new string[] { "Ground" }));
		}

		// Token: 0x0600011F RID: 287 RVA: 0x00010714 File Offset: 0x0000E914
		private void Update()
		{
			float num = Mathf.Min(Time.deltaTime, 0.05f);
			if (num <= 0f)
			{
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (this.mode == Scavenger.Mode.Dead)
			{
				this.UpdateDead(num);
				return;
			}
			if (body && Vector2.Distance(this.pos, body.transform.position) > 160f)
			{
				return;
			}
			this.sated = Mathf.Max(0f, this.sated - num);
			if (this.stunned > 0f)
			{
				this.stunned -= num;
				this.stagger = Mathf.Max(this.stagger, 0.1f);
			}
			if (this.Holding && Capture.Holder != this)
			{
				this.Holding = false;
			}
			if (this.Holding)
			{
				this.vel = Vector2.zero;
				this.Step(num, 0f);
				this.AnimateHolding(num);
				this.UpdateBark(num);
				return;
			}
			this.think -= num;
			this.stagger = Mathf.Max(0f, this.stagger - num);
			this.flinch = Mathf.Max(0f, this.flinch - num * 4f);
			this.rattled = Mathf.Max(0f, this.rattled - num / 3f);
			if (this.warnHold > 0f && this.mode == Scavenger.Mode.Fight)
			{
				this.warnHold -= num;
			}
			this.Mutter(body, num);
			if ((this.thrownScan -= num) <= 0f)
			{
				this.thrownScan = 0.05f;
				this.ThrownAtIt();
			}
			if (Camps.IsCampSide(this) && !Deal.IsFor(this))
			{
				this.GuardUpdate(body, num);
				return;
			}
			if (this.ShooTick(body, num))
			{
				return;
			}
			if (Deal.Offerer == this && !Deal.Accepted && body)
			{
				this.WaitForAnswer(body, num);
				return;
			}
			if (Plugin.Feud.Value && this.Feud(body, num))
			{
				return;
			}
			Vector2 vector = (body ? body.limbs[1].transform.position : this.pos);
			float num2 = Vector2.Distance(this.Chest, vector);
			bool flag = this.CanSee(body, ((this.mode == Scavenger.Mode.Wander) ? Plugin.ScavSight.Value : (Plugin.ScavSight.Value * 1.5f)) * (this.slimeHost ? 0.7f : 1f));
			if (flag && this.mode == Scavenger.Mode.Wander && Deal.TryOffer(this, body, false))
			{
				this.lastSeen = vector;
				this.WaitForAnswer(body, num);
				return;
			}
			if (flag)
			{
				this.lastSeen = vector;
				if (this.mode == Scavenger.Mode.Wander)
				{
					this.mode = Scavenger.Mode.Alert;
					this.aim = 0f;
					if (this.weapon.Gun && !this.slimeHost && !this.gentleNow)
					{
						this.warnHold = Random.Range(1.2f, 1.5f);
						this.Bark("scav_warn", true);
					}
					else
					{
						this.Bark(this.slimeHost ? "host_spot" : "scav_spot", this.slimeHost);
					}
				}
				this.alertTimer = 14f;
				if (this.mode == Scavenger.Mode.Alert)
				{
					this.mode = Scavenger.Mode.Fight;
				}
			}
			else if (this.mode == Scavenger.Mode.Fight || this.mode == Scavenger.Mode.Alert)
			{
				if ((this.alertTimer -= num) <= 0f)
				{
					this.mode = Scavenger.Mode.Wander;
					this.Bark("scav_lost");
				}
				else
				{
					this.mode = Scavenger.Mode.Alert;
				}
			}
			if (this.hit && this.hit.health < this.maxHealth * 0.22f && this.mode == Scavenger.Mode.Fight && Random.value < num * 0.3f)
			{
				this.mode = Scavenger.Mode.Flee;
				this.Bark("scav_flee");
			}
			float num3 = 0f;
			float num4 = 4.2f;
			if (this.slimeHost && Random.value < num * 0.25f)
			{
				this.Sfx("slime_bubble", 0.25f);
			}
			switch (this.mode)
			{
			case Scavenger.Mode.Wander:
				if (this.think <= 0f)
				{
					this.think = Random.Range(2.5f, 6f);
					if (this.pack && this.pack != this && this.pack.mode != Scavenger.Mode.Dead)
					{
						this.home = this.pack.pos;
					}
					float num5 = (this.stayNear ? 5f : (this.lurk ? 2.5f : ((this.pack && this.pack != this) ? 4f : 18f)));
					this.wanderTo = ((Random.value < 0.35f) ? this.pos : (this.home + new Vector2(Random.Range(-num5, num5), 0f)));
				}
				if (Mathf.Abs(this.wanderTo.x - this.pos.x) > 0.8f)
				{
					num3 = Mathf.Sign(this.wanderTo.x - this.pos.x);
				}
				num4 = 2.4f;
				break;
			case Scavenger.Mode.Alert:
				if (Mathf.Abs(this.lastSeen.x - this.pos.x) > 1.5f)
				{
					num3 = Mathf.Sign(this.lastSeen.x - this.pos.x);
				}
				num4 = 3.6f;
				break;
			case Scavenger.Mode.Fight:
				this.facing = ((vector.x >= this.pos.x) ? 1 : (-1));
				if (this.weapon.Gun)
				{
					if ((this.repick -= num) <= 0f)
					{
						this.repick = Random.Range(2.5f, 5f);
						this.prefDist = Random.Range(6.5f, 12f);
						this.shuffle = ((Random.value < 0.45f) ? ((Random.value < 0.5f) ? 1 : (-1)) : 0);
						this.shuffleTime = Random.Range(0.4f, 1.1f);
					}
					num4 = 3.2f;
					if (this.reload > 0f)
					{
						num3 = (float)(-(float)this.facing);
						num4 = 3.8f;
					}
					else if (num2 > this.prefDist + 1.5f)
					{
						num3 = (float)this.facing;
					}
					else if (num2 < this.prefDist - 1.5f)
					{
						num3 = (float)(-(float)this.facing);
					}
					else if (this.shuffle != 0 && (this.shuffleTime -= num) > 0f)
					{
						num3 = (float)this.shuffle;
						num4 = 2f;
					}
				}
				else if (num2 > this.weapon.Range * 0.8f)
				{
					num3 = (float)this.facing;
					num4 = 6.5f;
				}
				break;
			case Scavenger.Mode.Flee:
				num3 = ((body && body.transform.position.x > this.pos.x) ? (-1f) : 1f);
				num4 = 5.2f;
				if (!flag && Random.value < num * 0.1f)
				{
					this.mode = Scavenger.Mode.Wander;
				}
				break;
			}
			bool flag2 = body && (!body.standing || body.consciousness < 45f || body.shock > 20f);
			bool flag3 = !Diff.LethalScenes && !this.slimeHost;
			bool flag4 = this.grabber || flag3 || flag2;
			if (body && (this.comingFor || (flag4 && this.mode == Scavenger.Mode.Fight && (flag2 || !this.weapon.Gun || flag3))) && Capture.CanGrab(body) && (!Capture.Active || !(Capture.Holder != this)) && Mathf.Abs(vector.x - this.pos.x) > 0.6f)
			{
				num3 = Mathf.Sign(vector.x - this.pos.x);
				num4 = Mathf.Max(num4, this.weapon.Gun ? 4.6f : 6.2f);
			}
			if (this.stagger > 0f)
			{
				num3 = 0f;
			}
			if (this.slimeHost)
			{
				num4 *= 0.55f;
			}
			if (this.Hurt)
			{
				num4 *= 0.7f;
			}
			if (num3 != 0f && (this.mode != Scavenger.Mode.Fight || !this.weapon.Gun || this.aim < 0.3f || this.reload > 0f))
			{
				this.facing = ((num3 > 0f) ? 1 : (-1));
			}
			float num6 = num3 * num4;
			this.vel.x = Mathf.MoveTowards(this.vel.x, num6, (this.grounded ? 30f : 8f) * num);
			this.Step(num, num3);
			if (Capture.Active && Capture.Holder != this && body && (this.mode == Scavenger.Mode.Fight || this.mode == Scavenger.Mode.Alert) && num2 < 25f)
			{
				this.facing = ((vector.x >= this.pos.x) ? 1 : (-1));
				float num7 = ((num2 > 6f) ? ((float)this.facing) : ((num2 < 3f) ? ((float)(-(float)this.facing)) : 0f));
				this.vel.x = Mathf.MoveTowards(this.vel.x, num7 * 2.6f, 20f * num);
				this.aim = Mathf.MoveTowards(this.aim, 0.3f, num);
				if (Random.value < num * 0.08f)
				{
					this.Bark("scav_wait", true);
				}
				this.alertTimer = 14f;
				this.Animate(num, body, vector);
				this.UpdateBark(num);
				return;
			}
			if (body && (this.comingFor || (flag4 && this.mode == Scavenger.Mode.Fight && (flag2 || !this.weapon.Gun || flag3))) && Capture.CanGrab(body) && this.stagger <= 0f)
			{
				if (num2 > 1.9f)
				{
					this.facing = ((vector.x >= this.pos.x) ? 1 : (-1));
					this.vel.x = Mathf.MoveTowards(this.vel.x, (float)this.facing * (this.weapon.Gun ? 4.6f : 6.2f) * (this.Hurt ? 0.7f : 1f), 30f * num);
				}
				else if (this.comingFor || flag2 || Random.value < num * (this.slimeHost ? 2f : 1.2f))
				{
					if (this.comingFor || flag2 || Random.value < (this.slimeHost ? 0.8f : 0.65f))
					{
						this.comingFor = false;
						Capture.Grab(body, this);
					}
					else
					{
						this.Bark("scav_grab_miss", true);
						this.stagger = 0.6f;
					}
				}
				if (this.comingFor)
				{
					this.Animate(num, body, vector);
					this.UpdateBark(num);
					return;
				}
			}
			if (this.mode == Scavenger.Mode.Fight && body && body.alive && this.stagger <= 0f && !this.comingFor && !this.slimeHost && !Capture.Surrendered && !flag3 && !flag2 && (!this.weapon.Gun || (this.warnHold <= 0f && !Ultimatum.HoldFire(this, body))))
			{
				if (this.weapon.Gun)
				{
					this.Shoot(body, vector, num);
				}
				else
				{
					this.Melee(body, vector, num2, num);
				}
			}
			else
			{
				this.aim = Mathf.MoveTowards(this.aim, 0f, num * 2f);
				if (this.swing >= 0f)
				{
					this.swing = -1f;
				}
			}
			this.Animate(num, body, vector);
			this.UpdateBark(num);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x000113BC File Offset: 0x0000F5BC
		private static bool SolidUnder(WorldGeneration w, Vector2Int block)
		{
			Vector2 vector = w.BlockToWorldPos(block);
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			return !body || Vector2.Distance(vector, body.transform.position) > 60f || Physics2D.OverlapBox(vector, new Vector2(0.8f, 0.8f), 0f, LayerMask.GetMask(new string[] { "Ground" }));
		}

		// Token: 0x06000121 RID: 289 RVA: 0x00011444 File Offset: 0x0000F644
		private void Step(float dt, float want)
		{
			WorldGeneration world = WorldGeneration.world;
			if (!world)
			{
				return;
			}
			this.vel.y = this.vel.y - 26f * dt;
			Vector2 vector = this.pos + this.vel * dt;
			float num = Mathf.Sign((this.vel.x == 0f) ? ((float)this.facing) : this.vel.x);
			bool flag = world.GetBlock(world.WorldToBlockPos(new Vector2(vector.x + num * 0.7f, this.pos.y + 0.5f))) > 0;
			bool flag2 = world.GetBlock(world.WorldToBlockPos(new Vector2(vector.x + num * 0.7f, this.pos.y + 2.5f))) != 0 || world.GetBlock(world.WorldToBlockPos(new Vector2(vector.x + num * 0.7f, this.pos.y + 4.2f))) > 0;
			if (this.vel.x != 0f && (flag || flag2))
			{
				if (this.grounded && flag && !flag2 && want != 0f)
				{
					this.vel.y = 9.5f;
					this.grounded = false;
				}
				else
				{
					vector.x = this.pos.x;
					this.vel.x = 0f;
					if (this.mode == Scavenger.Mode.Wander)
					{
						this.wanderTo = this.pos;
					}
				}
			}
			if (this.vel.y > 0f && world.GetBlock(world.WorldToBlockPos(vector + Vector2.up * 4.9f)) != 0)
			{
				this.vel.y = 0f;
			}
			Vector2Int vector2Int = world.WorldToBlockPos(vector);
			if (this.vel.y <= 0f && world.GetBlock(vector2Int) != 0 && !Scavenger.SolidUnder(world, vector2Int))
			{
				if (!this.warnedFloat)
				{
					this.warnedFloat = true;
					Plugin.Log.LogWarning(string.Format("Scavenger ({0}) at {1}: block {2} under it has no collider, it falls through", this.skin, this.pos, world.GetBlock(vector2Int)));
				}
			}
			else if (this.vel.y <= 0f && world.GetBlock(vector2Int) != 0)
			{
				vector.y = world.BlockToWorldPos(vector2Int).y + 0.5f;
				if (!this.grounded && this.vel.y < -12f)
				{
					this.stagger = 0.4f;
				}
				this.vel.y = 0f;
				this.grounded = true;
			}
			else if (world.GetBlock(world.WorldToBlockPos(vector + Vector2.down * 0.6f)) == 0)
			{
				this.grounded = false;
			}
			if (this.grounded && this.mode == Scavenger.Mode.Wander && want != 0f)
			{
				bool flag3 = false;
				int num2 = 1;
				while (num2 <= 6 && !flag3)
				{
					flag3 = world.GetBlock(world.WorldToBlockPos(new Vector2(vector.x + num * 1.2f, vector.y - (float)num2))) > 0;
					num2++;
				}
				if (!flag3)
				{
					vector.x = this.pos.x;
					this.vel.x = 0f;
					this.wanderTo = this.pos;
				}
			}
			vector.x = Mathf.Clamp(vector.x, -world.halfWidth + 4f, world.halfWidth - 4f);
			vector.y = Mathf.Clamp(vector.y, -world.halfHeight + 4f, world.halfHeight - 4f);
			this.pos = vector;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00011828 File Offset: 0x0000FA28
		private float AimAngle(Vector2 target)
		{
			Vector2 vector = base.transform.position + new Vector2(-0.12f * (float)this.facing, 1.45f);
			Vector2 vector2 = target - vector;
			return Mathf.Atan2(vector2.y, Mathf.Abs(vector2.x)) * 57.29578f;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x00011888 File Offset: 0x0000FA88
		private void Shoot(Body b, Vector2 target, float dt)
		{
			if (this.reload > 0f)
			{
				if ((this.reload -= dt) <= 0f)
				{
					this.rounds = this.weapon.Mag;
					Sound.Play("gunloadmag", base.transform.position, false, true, null, 1f, 1f, false, false);
				}
				return;
			}
			this.aim = Mathf.MoveTowards(this.aim, 1f, dt * 1.6f);
			if (this.aim < 1f)
			{
				return;
			}
			if ((this.shotTimer -= dt) > 0f)
			{
				return;
			}
			if (this.rounds <= 0)
			{
				this.reload = 2.4f;
				Sound.Play("gununloadmag", base.transform.position, false, true, null, 1f, 1f, false, false);
				this.Bark("scav_reload");
				return;
			}
			this.rounds--;
			if (this.burstLeft <= 0)
			{
				this.burstLeft = this.weapon.Burst;
			}
			this.burstLeft--;
			this.shotTimer = ((this.burstLeft > 0) ? 0.11f : (this.weapon.Cooldown * Random.Range(0.85f, 1.3f)));
			float num = this.AimAngle(target);
			Vector2 vector = base.transform.position + this.Mirror(this.rig.HandAt(num + 90f) + Quaternion.Euler(0f, 0f, num) * Vector2.right * 1.1f);
			Vector2 vector2 = target - vector;
			float num2 = Mathf.Atan2(vector2.y, Mathf.Abs(vector2.x)) * 57.29578f;
			float num3 = this.weapon.Spread * ((Mathf.Abs(this.vel.x) > 0.6f) ? 1.5f : 1f) * (1f + this.rattled * 2f) * (this.Hurt ? 1.4f : 1f);
			for (int i = 0; i < this.weapon.Pellets; i++)
			{
				float num4 = (num2 + Random.Range(-num3, num3)) * 0.017453292f;
				Vector2 vector3;
				vector3..ctor(Mathf.Cos(num4) * (float)this.facing, Mathf.Sin(num4));
				TurretScript.Shoot(new FireInfo
				{
					pos = vector,
					dir = vector3,
					ignoreTrans = (this.hit ? this.hit.transform : null),
					ignoreBody = false,
					doTinnitus = (b && Vector2.Distance(vector, b.transform.position) < 9f),
					structureDamage = this.weapon.Damage * 0.3f,
					animalDamage = this.weapon.Damage,
					playerDamageMultiplier = (Camps.IsCampSide(this) ? 0f : (this.weapon.PlayerMult * Plugin.ScavDamage.Value))
				});
			}
			if (Plugin.LogShots.Value)
			{
				Plugin.Log.LogInfo(string.Format("Scavenger fires {0} at {1} ({2} left)", this.weapon.Id, Camps.IsCampSide(this) ? "a threat to the camp" : (this.foe ? "the camp or a foe" : "her"), this.rounds));
			}
			if (this.fireSound)
			{
				Sound.Play(this.fireSound, vector, false, true, null, 0.9f, 1f, false, false);
			}
			else
			{
				Sound.Play("gunshot", vector, false, true, null, 1f, 1f, false, false);
			}
			this.flinch = 1f;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x06000124 RID: 292 RVA: 0x00011C98 File Offset: 0x0000FE98
		public bool Tending
		{
			get
			{
				return this.tendFor >= 0f;
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00011CAA File Offset: 0x0000FEAA
		public void Tend(float seconds, Action done)
		{
			this.tendFor = seconds;
			this.tendAnim = 0f;
			this.tendDone = done;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00011CC8 File Offset: 0x0000FEC8
		public void StopTending()
		{
			this.tendFor = -1f;
			this.tendDone = null;
			if (this.rig != null && this.rig.Weapon)
			{
				this.rig.Weapon.gameObject.SetActive(true);
			}
			if (this.rig != null)
			{
				this.rig.Body.localRotation = Quaternion.identity;
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00011D34 File Offset: 0x0000FF34
		private bool TendUpdate(Body b, float dt)
		{
			if (!b || !b.alive)
			{
				this.StopTending();
				return false;
			}
			Vector2 vector = b.limbs[1].transform.position;
			float num = vector.x - this.pos.x;
			if (this.tendAnim <= 0f && Mathf.Abs(num) > 1.1f)
			{
				if (Mathf.Abs(num) > 10f)
				{
					this.StopTending();
					return false;
				}
				this.mode = Scavenger.Mode.Wander;
				float num2 = Mathf.Sign(num);
				this.facing = ((num2 > 0f) ? 1 : (-1));
				this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 2.6f, (this.grounded ? 30f : 8f) * dt);
				this.Step(dt, num2);
				this.Animate(dt, b, vector);
				return true;
			}
			else
			{
				if (Mathf.Abs(num) > 2.8f)
				{
					this.StopTending();
					return false;
				}
				this.facing = ((num >= 0f) ? 1 : (-1));
				this.vel.x = Mathf.MoveTowards(this.vel.x, 0f, 30f * dt);
				this.Step(dt, 0f);
				if (this.tendAnim <= 0f)
				{
					Sound.Play("ripclothing", this.pos, false, true, null, 0.6f, 1.2f, false, false);
				}
				this.tendAnim += dt;
				this.AnimateTend(dt);
				if (this.tendAnim >= this.tendFor)
				{
					Action action = this.tendDone;
					this.StopTending();
					if (action != null)
					{
						action();
					}
				}
				return true;
			}
		}

		// Token: 0x06000128 RID: 296 RVA: 0x00011EE8 File Offset: 0x000100E8
		private void AnimateTend(float dt)
		{
			base.transform.position = this.pos + Vector2.up * 2.55f;
			base.transform.localScale = new Vector3((float)this.facing, 1f, 1f);
			float num = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(this.tendAnim / 0.45f));
			float num2 = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01((this.tendAnim - (this.tendFor - 0.35f)) / 0.35f));
			num *= 1f - num2;
			float num3 = Mathf.Sin(this.tendAnim * 7f);
			this.rig.Body.localPosition = new Vector2(0.12f * num, -0.6f * num);
			this.rig.Body.localRotation = Quaternion.Euler(0f, 0f, -16f * num);
			this.rig.Set("HipF", 85f * num);
			this.rig.Set("KneeF", -95f * num);
			this.rig.Set("HipB", 10f * num);
			this.rig.Set("KneeB", -105f * num);
			this.rig.Set("Neck", -22f * num);
			this.rig.Set("ShoulderF", (70f + 16f * num3) * num);
			this.rig.Set("ElbowF", (-35f - 22f * num3) * num);
			this.rig.Set("ShoulderB", (60f - 16f * num3) * num);
			this.rig.Set("ElbowB", (-45f + 22f * num3) * num);
			if (this.rig.Weapon)
			{
				this.rig.Weapon.gameObject.SetActive(num < 0.2f);
			}
			if ((this.blink -= dt) <= 0f)
			{
				this.blink = Random.Range(2f, 5f);
				this.rig.Blink(true);
				return;
			}
			if (this.blink < 4.85f)
			{
				this.rig.Blink(false);
			}
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0001216C File Offset: 0x0001036C
		public void SwallowedByWorm()
		{
			if (this.mode == Scavenger.Mode.Dead)
			{
				return;
			}
			if (this.Holding && this.Player)
			{
				Capture.Release(this.Player, false, false);
			}
			this.Bark(null);
			this.mode = Scavenger.Mode.Dead;
			Scavengers.All.Remove(this);
			if (Camps.IsTraderBody(this))
			{
				Camps.TraderSwallowed();
			}
			Plugin.Log.LogInfo(string.Format("Scavenger ({0}, {1}) swallowed whole by the giant worm at {2}", this.skin, this.weapon.Id, this.pos));
			Object.Destroy(base.gameObject);
		}

		// Token: 0x0600012A RID: 298 RVA: 0x00012208 File Offset: 0x00010408
		public void MakeGuard(Vector2 camp)
		{
			this.wanderTo = camp;
			this.home = camp;
			this.grabber = (this.lecher = false);
			this.offered = true;
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0001223C File Offset: 0x0001043C
		private void GuardUpdate(Body b, float dt)
		{
			if ((this.guardScan -= dt) <= 0f)
			{
				this.guardScan = 0.4f;
				Object @object = this.guardFoe ?? this.guardBeast;
				this.GuardScan(b);
				Object object2 = this.guardFoe ?? this.guardBeast;
				if (object2 && object2 != @object)
				{
					if (!Camps.IsKeeper(this))
					{
						this.Bark("camp_foe", true);
					}
					ManualLogSource log = Plugin.Log;
					object obj;
					if (!this.guardFoe)
					{
						string text = "Camp guard targets {0} ({1}, {2:0} hp) at {3}; components: {4}";
						object[] array = new object[5];
						array[0] = this.guardBeast.id;
						array[1] = this.guardBeast.name;
						array[2] = this.guardBeast.health;
						array[3] = Scavenger.BeastAt(this.guardBeast);
						array[4] = string.Join(", ", from m in this.guardBeast.GetComponents<MonoBehaviour>()
							select m.GetType().Name);
						obj = string.Format(text, array);
					}
					else
					{
						obj = string.Format("Camp guard targets a {0} at {1}", this.guardFoe.slimeHost ? "slime host" : "raider", this.guardFoe.pos);
					}
					log.LogInfo(obj);
				}
			}
			Vector2? vector = ((this.guardFoe && this.guardFoe.mode != Scavenger.Mode.Dead) ? new Vector2?(this.guardFoe.Chest) : ((this.guardBeast && this.guardBeast.health > 0f) ? new Vector2?(Scavenger.BeastAt(this.guardBeast)) : null));
			if (this.Tending)
			{
				if (vector != null)
				{
					this.StopTending();
				}
				else if (this.TendUpdate(b, dt))
				{
					this.UpdateBark(dt);
					return;
				}
			}
			bool flag = Camps.IsKeeper(this);
			float num = (flag ? 6f : 12f);
			float num2 = 0f;
			if (vector != null)
			{
				Vector2 valueOrDefault = vector.GetValueOrDefault();
				this.mode = Scavenger.Mode.Fight;
				this.facing = ((valueOrDefault.x >= this.pos.x) ? 1 : (-1));
				if (Vector2.Distance(this.Chest, valueOrDefault) > 13f)
				{
					num2 = (float)this.facing;
				}
				if (Mathf.Abs(this.pos.x + num2 - this.home.x) > num && Mathf.Sign(num2) == Mathf.Sign(this.pos.x - this.home.x))
				{
					num2 = 0f;
				}
				this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 3.4f, (this.grounded ? 30f : 8f) * dt);
				this.Step(dt, num2);
				if (this.stagger <= 0f)
				{
					Scavenger.Striker = this;
					Camps.GuardFired();
					this.Shoot(b, valueOrDefault, dt);
					Scavenger.Striker = null;
				}
				this.Animate(dt, b, valueOrDefault);
			}
			else
			{
				this.mode = Scavenger.Mode.Wander;
				if (this.reload <= 0f)
				{
					this.rounds = this.weapon.Mag;
				}
				if (this.think <= 0f)
				{
					this.think = Random.Range(3f, 7f);
					this.wanderTo = ((Random.value < 0.4f) ? this.pos : (this.home + new Vector2(Random.Range(-num + 1f, num - 1f), 0f)));
				}
				if (Mathf.Abs(this.wanderTo.x - this.pos.x) > 0.8f)
				{
					num2 = Mathf.Sign(this.wanderTo.x - this.pos.x);
				}
				bool flag2 = b && Vector2.Distance(this.pos, b.transform.position) < 10f;
				if (flag && b && b.alive && Vector2.Distance(this.pos, b.transform.position) < 4.5f)
				{
					num2 = 0f;
				}
				if (num2 != 0f)
				{
					this.facing = ((num2 > 0f) ? 1 : (-1));
				}
				else if (flag2)
				{
					this.facing = ((b.transform.position.x >= this.pos.x) ? 1 : (-1));
				}
				this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 2f, (this.grounded ? 30f : 8f) * dt);
				this.Step(dt, num2);
				this.aim = Mathf.MoveTowards(this.aim, 0f, dt * 2f);
				this.Animate(dt, b, flag2 ? b.limbs[1].transform.position : (this.pos + new Vector2((float)this.facing * 5f, 3.4f)));
				if (flag2 && b.alive && !flag && Time.time > this.greetAt)
				{
					this.greetAt = Time.time + 90f;
					this.Bark("camp_greet", true);
				}
			}
			this.UpdateBark(dt);
		}

		// Token: 0x0600012C RID: 300 RVA: 0x000127D0 File Offset: 0x000109D0
		private static Vector2 BeastAt(BuildingEntity be)
		{
			Collider2D component = be.GetComponent<Collider2D>();
			if (!component)
			{
				return be.transform.position;
			}
			return component.bounds.center;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00012810 File Offset: 0x00010A10
		private void GuardScan(Body b)
		{
			this.guardFoe = null;
			this.guardBeast = null;
			float num = 16f;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			foreach (Scavenger scavenger in Scavengers.All)
			{
				if (scavenger && !(scavenger == this) && scavenger.mode != Scavenger.Mode.Dead && !Camps.IsCampSide(scavenger) && Vector2.Distance(scavenger.pos, this.home) <= 20f)
				{
					bool flag = scavenger.slimeHost || scavenger.Holding || !Deal.IsFor(scavenger);
					float num2 = Vector2.Distance(this.Chest, scavenger.Chest);
					if (flag && num2 < num && !Physics2D.Linecast(this.Eye, scavenger.Chest, mask))
					{
						num = num2;
						this.guardFoe = scavenger;
					}
				}
			}
			if (this.guardFoe)
			{
				return;
			}
			Camp current = Camps.Current;
			TraderScript traderScript = ((current != null) ? current.Trader : null);
			foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(this.home + Vector2.up * 2f, 16f, LayerMask.GetMask(new string[] { "Descriptor" })))
			{
				BuildingEntity componentInParent = collider2D.GetComponentInParent<BuildingEntity>();
				if (componentInParent && componentInParent.animal && componentInParent.health > 0f && !(componentInParent == this.hit) && !componentInParent.GetComponent<ScavHitRelay>() && (!traderScript || !componentInParent.transform.IsChildOf(traderScript.transform)) && !componentInParent.GetComponentInParent<ScavHitRelay>())
				{
					Vector2 vector = collider2D.bounds.center;
					float num3 = Vector2.Distance(this.Chest, vector);
					if (num3 < num && !Physics2D.Linecast(this.Eye, vector, mask))
					{
						num = num3;
						this.guardBeast = componentInParent;
					}
				}
			}
		}

		// Token: 0x0600012E RID: 302 RVA: 0x00012A74 File Offset: 0x00010C74
		private bool Hostile(Scavenger f)
		{
			return f && f != this && f.mode != Scavenger.Mode.Dead && f.riseTimer < 0f && (f.slimeHost != this.slimeHost || (!this.slimeHost && !f.slimeHost && Camps.IsCampSide(f) != Camps.IsCampSide(this) && !Deal.IsFor(this) && !Deal.IsFor(f)));
		}

		// Token: 0x0600012F RID: 303 RVA: 0x00012AEC File Offset: 0x00010CEC
		private Scavenger FindFoe()
		{
			Scavenger scavenger = null;
			float num = Plugin.ScavSight.Value;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			foreach (Scavenger scavenger2 in Scavengers.All)
			{
				if (this.Hostile(scavenger2))
				{
					float num2 = Vector2.Distance(this.Chest, scavenger2.Chest);
					if (num2 < num && !Physics2D.Linecast(this.Eye, scavenger2.Chest, mask))
					{
						scavenger = scavenger2;
						num = num2;
					}
				}
			}
			return scavenger;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x00012BA0 File Offset: 0x00010DA0
		private bool Feud(Body b, float dt)
		{
			if ((this.foeScan -= dt) <= 0f)
			{
				this.foeScan = 0.4f;
				if (!this.Hostile(this.foe) || Vector2.Distance(this.Chest, this.foe.Chest) > Plugin.ScavSight.Value * 1.4f)
				{
					Scavenger scavenger = this.foe;
					this.foe = this.FindFoe();
					if (this.foe && this.foe != scavenger)
					{
						this.Bark(this.slimeHost ? "host_foe" : "scav_foe", true);
					}
				}
			}
			if (!this.Hostile(this.foe))
			{
				this.foe = null;
				return this.slimeHost && this.TraderFeud(b, dt);
			}
			Vector2 chest = this.foe.Chest;
			float num = Vector2.Distance(this.Chest, chest);
			if (b && b.alive && (!this.slimeHost || Capture.HostWantsHer) && this.sated <= 0f && Vector2.Distance(this.Chest, b.limbs[1].transform.position) < num * 0.5f && this.CanSee(b, Plugin.ScavSight.Value))
			{
				return false;
			}
			this.mode = Scavenger.Mode.Fight;
			this.alertTimer = 14f;
			this.comingFor = false;
			this.facing = ((chest.x >= this.pos.x) ? 1 : (-1));
			float num2 = 0f;
			float num3 = 4.2f;
			if (this.weapon.Gun)
			{
				num3 = 3.2f;
				if (this.reload > 0f)
				{
					num2 = (float)(-(float)this.facing);
					num3 = 3.8f;
				}
				else if (num > 11f)
				{
					num2 = (float)this.facing;
				}
				else if (num < 4.5f)
				{
					num2 = (float)(-(float)this.facing);
				}
			}
			else if (num > (this.slimeHost ? 1.5f : (this.weapon.Range * 0.8f)))
			{
				num2 = (float)this.facing;
				num3 = 5f;
			}
			if (this.stagger > 0f)
			{
				num2 = 0f;
			}
			if (this.slimeHost)
			{
				num3 *= 0.55f;
			}
			this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * num3, (this.grounded ? 30f : 8f) * dt);
			this.Step(dt, num2);
			if (this.stagger <= 0f)
			{
				if (this.slimeHost)
				{
					this.Engulf(num, dt);
				}
				else if (this.weapon.Gun)
				{
					if (b)
					{
						Scavenger.Striker = this;
						this.Shoot(b, chest, dt);
						Scavenger.Striker = null;
					}
				}
				else
				{
					this.MeleeFoe(num, dt);
				}
			}
			this.Animate(dt, b, chest);
			this.UpdateBark(dt);
			return true;
		}

		// Token: 0x06000131 RID: 305 RVA: 0x00012E90 File Offset: 0x00011090
		private bool TraderFeud(Body b, float dt)
		{
			Vector2 vector;
			if (!Camps.TraderTarget(out vector))
			{
				return false;
			}
			float num = Vector2.Distance(this.Chest, vector);
			if (num > Plugin.ScavSight.Value || Physics2D.Linecast(this.Eye, vector, LayerMask.GetMask(new string[] { "Ground" })))
			{
				return false;
			}
			if (b && b.alive && Capture.HostWantsHer && this.sated <= 0f && Vector2.Distance(this.Chest, b.limbs[1].transform.position) < num * 0.5f && this.CanSee(b, Plugin.ScavSight.Value))
			{
				return false;
			}
			this.mode = Scavenger.Mode.Fight;
			this.alertTimer = 14f;
			this.facing = ((vector.x >= this.pos.x) ? 1 : (-1));
			float num2 = ((num > 1.5f && this.stagger <= 0f) ? ((float)this.facing) : 0f);
			this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 4.2f * 0.55f, (this.grounded ? 30f : 8f) * dt);
			this.Step(dt, num2);
			if (num <= 2.2f && this.stagger <= 0f && (this.engulf -= dt) <= 0f)
			{
				this.engulf = Random.Range(0.9f, 1.4f);
				this.Thrust = 0.4f;
				this.Sfx((Random.value < 0.5f) ? "slime_gloop" : "slime_squelch", 0.6f);
				float num3 = Plugin.FeudHostDamage.Value * Random.Range(0.8f, 1.2f);
				Camp current = Camps.Current;
				Scavenger scavenger = ((current != null) ? current.Keeper : null);
				if (scavenger && scavenger.mode != Scavenger.Mode.Dead)
				{
					this.DamageFoe(scavenger, num3);
				}
				else
				{
					Camps.HurtTrader(num3);
				}
			}
			this.Animate(dt, b, vector);
			this.UpdateBark(dt);
			return true;
		}

		// Token: 0x06000132 RID: 306 RVA: 0x000130C8 File Offset: 0x000112C8
		private void Engulf(float fd, float dt)
		{
			if (fd > 2f)
			{
				this.engulf = 0.4f;
				return;
			}
			if ((this.engulf -= dt) > 0f)
			{
				return;
			}
			this.engulf = Random.Range(0.9f, 1.4f);
			this.Thrust = 0.4f;
			this.Sfx((Random.value < 0.5f) ? "slime_gloop" : "slime_squelch", 0.6f);
			this.DamageFoe(this.foe, Plugin.FeudHostDamage.Value * Random.Range(0.8f, 1.2f));
			if (this.foe)
			{
				this.foe.stagger = Mathf.Max(this.foe.stagger, 0.35f);
			}
		}

		// Token: 0x06000133 RID: 307 RVA: 0x00013198 File Offset: 0x00011398
		private void MeleeFoe(float fd, float dt)
		{
			this.swingCooldown -= dt;
			if (this.swing < 0f)
			{
				if (fd < this.weapon.Range && this.swingCooldown <= 0f)
				{
					this.swing = 0f;
					Sound.Play("BSSwing" + Random.Range(1, 5).ToString(), base.transform.position, false, true, null, 0.7f, 1f, false, false);
				}
				return;
			}
			float num = this.swing;
			this.swing += dt / 0.55f;
			if (num < 0.65f && this.swing >= 0.65f && fd < this.weapon.Range + 0.4f)
			{
				this.DamageFoe(this.foe, this.weapon.Damage * (this.weapon.Cutting ? 1.3f : 1f) * Random.Range(0.85f, 1.15f));
			}
			if (this.swing >= 1f)
			{
				this.swing = -1f;
				this.swingCooldown = this.weapon.Cooldown * Random.Range(1.1f, 1.6f);
			}
		}

		// Token: 0x06000134 RID: 308 RVA: 0x000132E4 File Offset: 0x000114E4
		private void DamageFoe(Scavenger f, float dmg)
		{
			if (!f || !f.hit)
			{
				return;
			}
			Scavenger.Striker = this;
			f.hit.health -= dmg;
			f.hit.gameObject.SendMessage("AnimalHit", dmg, 1);
			Scavenger.Striker = null;
			Sound.Play("gore" + Random.Range(1, 6).ToString(), f.Chest, false, true, null, 1f, 1f, false, false);
		}

		// Token: 0x06000135 RID: 309 RVA: 0x00013378 File Offset: 0x00011578
		private void Melee(Body b, Vector2 target, float dist, float dt)
		{
			this.swingCooldown -= dt;
			if (this.swing < 0f)
			{
				if (dist < this.weapon.Range && this.swingCooldown <= 0f)
				{
					this.swing = 0f;
					Sound.Play("BSSwing" + Random.Range(1, 5).ToString(), base.transform.position, false, true, null, 0.7f, 1f, false, false);
				}
				return;
			}
			float num = this.swing;
			this.swing += dt / 0.55f;
			if (num < 0.65f && this.swing >= 0.65f && dist < this.weapon.Range + 0.4f)
			{
				this.Strike(b);
			}
			if (this.swing >= 1f)
			{
				this.swing = -1f;
				this.swingCooldown = this.weapon.Cooldown * Random.Range(1.2f, 1.9f);
			}
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0001348C File Offset: 0x0001168C
		private void Strike(Body b)
		{
			string[] array = new string[] { "UpTorso", "UpTorso", "DownTorso", "Head", "UpArmF", "UpArmB", "ThighF" };
			Limb limb = b.LimbByName(array[Random.Range(0, array.Length)]) ?? b.limbs[1];
			float num = Mathf.Max(0.5f, limb.GetArmorReduction());
			float num2 = Plugin.ScavDamage.Value * this.weapon.Damage / 30f;
			string id = this.weapon.Id;
			float num3 = ((id == "flimsyknife") ? 1.3f : ((id == "machete" || id == "crudecleaver") ? 1.6f : ((id == "pickaxe") ? 1.4f : 1f)));
			float num4 = ((id == "sledgehammer") ? 0.15f : ((id == "shovel") ? 0.08f : ((id == "wrench") ? 0.05f : ((id == "scraptube") ? 0.04f : (0.03f * num2)))));
			bool flag = id == "sledgehammer" || id == "shovel";
			if (this.weapon.Cutting)
			{
				limb.skinHealth -= 16f * num2 / num;
				limb.muscleHealth -= 7f * num2 / num;
				limb.bleedAmount += 2.5f * num2 * num3 / num;
			}
			else if (id == "pickaxe")
			{
				limb.skinHealth -= 10f * num2 / num;
				limb.muscleHealth -= 16f * num2 / num;
				limb.bleedAmount += 1.5f * num2 * num3 / num;
			}
			else
			{
				limb.skinHealth -= 4f * num2 / num;
				limb.muscleHealth -= 12f * num2 / num;
				if (Random.value < num4)
				{
					limb.BreakBone();
				}
			}
			if (flag)
			{
				b.rb.velocity += new Vector2((float)this.facing * 6f, 3f);
			}
			limb.pain += 16f * num2 / num;
			limb.DamageWearables(0.6f);
			b.adrenaline = Mathf.Max(b.adrenaline, 60f);
			b.rb.velocity += new Vector2((float)this.facing * 6f, 2f);
			Sound.Play("gore" + Random.Range(1, 6).ToString(), limb.transform.position, false, true, null, 1f, 1f, false, false);
			Plugin.Log.LogInfo("Scavenger hits her " + limb.name + " with " + this.weapon.Id);
		}

		// Token: 0x06000137 RID: 311 RVA: 0x000137E4 File Offset: 0x000119E4
		private void WaitForAnswer(Body b, float dt)
		{
			this.mode = Scavenger.Mode.Alert;
			this.alertTimer = 14f;
			Vector2 vector = b.limbs[1].transform.position;
			float num = vector.x - this.pos.x;
			this.facing = ((num >= 0f) ? 1 : (-1));
			float num2 = ((Mathf.Abs(num) > 6f) ? ((float)this.facing) : ((Mathf.Abs(num) < 3.5f) ? ((float)(-(float)this.facing)) : 0f));
			this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 2.4f, (this.grounded ? 30f : 8f) * dt);
			this.aim = Mathf.MoveTowards(this.aim, 0f, dt * 2f);
			if (this.swing >= 0f)
			{
				this.swing = -1f;
			}
			this.Step(dt, num2);
			this.facing = ((num >= 0f) ? 1 : (-1));
			this.Animate(dt, b, vector);
			this.UpdateBark(dt);
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0001390C File Offset: 0x00011B0C
		public void TurnOn(Body b)
		{
			this.sated = 0f;
			this.comingFor = false;
			this.mode = Scavenger.Mode.Fight;
			this.alertTimer = 14f;
			if (b)
			{
				this.lastSeen = b.limbs[1].transform.position;
			}
		}

		// Token: 0x06000139 RID: 313 RVA: 0x00013964 File Offset: 0x00011B64
		public void ComeFor(Body b)
		{
			this.comingFor = true;
			this.sated = 0f;
			this.mode = Scavenger.Mode.Fight;
			this.alertTimer = 20f;
			this.lastSeen = b.limbs[1].transform.position;
		}

		// Token: 0x0600013A RID: 314 RVA: 0x000139B2 File Offset: 0x00011BB2
		public void Stun(float t)
		{
			this.stunned = t;
			this.vel = new Vector2((float)(-(float)this.facing) * 4f, 3f);
			this.grounded = false;
		}

		// Token: 0x0600013B RID: 315 RVA: 0x000139E0 File Offset: 0x00011BE0
		public void Stash(Item it)
		{
			if (!it)
			{
				return;
			}
			if (it.rb)
			{
				it.rb.simulated = false;
			}
			it.transform.SetParent(base.transform, false);
			it.gameObject.SetActive(false);
			this.stolen.Add(it);
		}

		// Token: 0x0600013C RID: 316 RVA: 0x00013A3C File Offset: 0x00011C3C
		public void Lurk()
		{
			this.lurk = true;
			this.home = (this.wanderTo = this.pos);
		}

		// Token: 0x0600013D RID: 317 RVA: 0x00013A68 File Offset: 0x00011C68
		public void StayNear(Vector2 at)
		{
			this.stayNear = true;
			this.sated = 9999f;
			this.comingFor = false;
			this.mode = Scavenger.Mode.Wander;
			this.home = at;
			this.wanderTo = at + new Vector2(Random.Range(-3f, 3f), 0f);
			this.think = Random.Range(1f, 3f);
		}

		// Token: 0x0600013E RID: 318 RVA: 0x00013AD8 File Offset: 0x00011CD8
		public void LetGo(bool escaped = false)
		{
			this.rig.Body.localRotation = Quaternion.identity;
			if (this.spent)
			{
				LewdLink.Pour(this.pos + Vector2.up * 0.5f, 8);
				LewdLink.Sound(this.Player, 0.8f, new string[] { "slime_gloop" });
				this.mode = Scavenger.Mode.Dead;
				this.deadTimer = Plugin.ScavCorpseSeconds.Value - 8f;
				if (this.hit)
				{
					this.hit.gameObject.SetActive(false);
				}
				this.Bark("host_melt", true);
				return;
			}
			this.comingFor = false;
			if (escaped)
			{
				this.mode = Scavenger.Mode.Fight;
				this.sated = 0f;
				this.alertTimer = 14f;
				Body player = this.Player;
				if (player)
				{
					this.lastSeen = player.limbs[1].transform.position;
				}
				return;
			}
			this.mode = Scavenger.Mode.Wander;
			this.sated = 60f;
			this.home = this.pos;
			this.wanderTo = this.pos + new Vector2((float)this.facing * -12f, 0f);
			this.think = 6f;
		}

		// Token: 0x0600013F RID: 319 RVA: 0x00013C30 File Offset: 0x00011E30
		private void AnimateHolding(float dt)
		{
			base.transform.position = this.pos + Vector2.up * 2.55f;
			base.transform.localScale = new Vector3((float)this.facing, 1f, 1f);
			ValueTuple<ScenePose, ScenePose, float> scene = Capture.Scene;
			if (scene.Item1 != null)
			{
				float num = Mathf.Lerp(scene.Item1.HimLean, scene.Item2.HimLean, scene.Item3);
				this.rig.Body.localRotation = Quaternion.Euler(0f, 0f, num);
				this.rig.Body.localPosition = Vector2.Lerp(scene.Item1.HimBody, scene.Item2.HimBody, scene.Item3);
				foreach (string text in new string[] { "Neck", "ShoulderF", "ElbowF", "ShoulderB", "ElbowB", "HipF", "KneeF", "HipB", "KneeB" })
				{
					float num2 = Poses.Lerp(scene.Item1.Him, scene.Item2.Him, text, scene.Item3);
					if (text == "HipF" || text == "HipB")
					{
						num2 -= num;
					}
					this.rig.Set(text, num2);
				}
				float num3 = Poses.Lerp(scene.Item1.Him, scene.Item2.Him, "ShoulderF", scene.Item3);
				this.rig.Weapon.transform.localPosition = this.rig.HandAt(num3);
				this.rig.Weapon.transform.localRotation = Quaternion.Euler(0f, 0f, -80f);
				return;
			}
			this.rig.Set("HipF", 8f);
			this.rig.Set("HipB", -8f);
			this.rig.Set("KneeF", -12f);
			this.rig.Set("KneeB", -12f);
			this.rig.Body.localPosition = new Vector2(this.Thrust * 0.22f, -0.12f);
			this.rig.Set("Neck", -10f);
			float num4 = ((this.Thrust > 0f) ? 55f : 70f);
			this.rig.Set("ShoulderF", num4);
			this.rig.Set("ShoulderB", num4 - 10f);
			this.rig.Set("ElbowF", -15f);
			this.rig.Set("ElbowB", -15f);
			this.rig.Weapon.transform.localPosition = this.rig.HandAt(num4);
			this.rig.Weapon.transform.localRotation = Quaternion.Euler(0f, 0f, -80f);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x00013F98 File Offset: 0x00012198
		public void OnHit(float damage)
		{
			if (Plugin.LogShots.Value)
			{
				Plugin.Log.LogInfo(string.Format("Scavenger hit for {0:0}, {1:0} left", damage, this.hit ? this.hit.health : 0f));
			}
			float num = damage / Mathf.Max(1f, this.maxHealth);
			this.stagger = Mathf.Max(this.stagger, Mathf.Clamp(0.3f + num * 3f, 0.3f, 1.2f));
			this.aim = 0f;
			this.swing = -1f;
			this.rattled = Mathf.Min(1f, this.rattled + 0.5f + num * 2f);
			this.flinch = 1f;
			Scavenger striker = Scavenger.Striker;
			this.killedBy = striker;
			if (Scavenger.CreatureHit)
			{
				this.killedByCreature = true;
				this.vel.x = this.vel.x - (float)this.facing * 2f;
				if (Random.value < 0.3f)
				{
					this.Bark("scav_hurt");
				}
				return;
			}
			if (striker && striker != this)
			{
				if (striker.slimeHost != this.slimeHost)
				{
					this.foe = striker;
				}
				this.vel.x = this.vel.x - (float)this.facing * 1.5f;
				if (Random.value < 0.2f)
				{
					this.Bark(this.slimeHost ? "host_foe" : "scav_hurt");
				}
				if (this.Holding && this.Player && Random.value < 0.25f)
				{
					Capture.Release(this.Player, false, false);
				}
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (body && !striker)
			{
				Ultimatum.HerAttack(body);
			}
			if (body && Camps.IsCampSide(this))
			{
				if (Vector2.Distance(body.transform.position, this.pos) > 25f)
				{
					return;
				}
				Camps.Betray(body, Camps.IsKeeper(this) ? "she hit the trader" : "she hit the guard");
			}
			if (body)
			{
				this.lastSeen = body.limbs[1].transform.position;
				this.facing = ((body.transform.position.x >= this.pos.x) ? 1 : (-1));
				this.sated = 0f;
				this.comingFor = false;
				this.shoo = 0;
				Capture.BreakSurrender();
				Deal.Cancel(this);
				if (this.mode == Scavenger.Mode.Wander)
				{
					this.mode = Scavenger.Mode.Fight;
					this.Bark("scav_hurt");
				}
				this.alertTimer = 14f;
			}
			this.vel.x = this.vel.x - (float)this.facing * 2.5f;
			if (Random.value < 0.25f)
			{
				this.Bark("scav_hurt");
			}
			if (this.Holding && Random.value < 0.35f && body)
			{
				Plugin.Log.LogInfo("She hit the " + (this.slimeHost ? "slime host" : "raider") + " holding her: it lets go and comes after her");
				Capture.Release(body, false, true);
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000141 RID: 321 RVA: 0x000142F9 File Offset: 0x000124F9
		public bool Dealer
		{
			get
			{
				return this.lecher && !this.offered && !this.slimeHost && !Camps.IsCampSide(this) && Plugin.DealEnabled.Value;
			}
		}

		// Token: 0x06000142 RID: 322 RVA: 0x00014328 File Offset: 0x00012528
		private void Mutter(Body b, float dt)
		{
			if (this.rig != null && this.rig.Weapon)
			{
				this.rig.Weapon.enabled = (!this.Dealer || this.mode != Scavenger.Mode.Wander) && this.mode != Scavenger.Mode.Dead;
			}
			if (!this.Dealer || this.mode != Scavenger.Mode.Wander || !b || Vector2.Distance(this.pos, b.transform.position) > 28f)
			{
				return;
			}
			if ((this.mutterTimer -= dt) > 0f)
			{
				return;
			}
			this.mutterTimer = Random.Range(8f, 14f);
			this.Bark("scav_mutter", true);
		}

		// Token: 0x06000143 RID: 323 RVA: 0x000143F4 File Offset: 0x000125F4
		private void ThrownAtIt()
		{
			if (!this.hit || this.mode == Scavenger.Mode.Dead)
			{
				return;
			}
			BoxCollider2D component = this.hit.GetComponent<BoxCollider2D>();
			if (!component)
			{
				return;
			}
			Vector2 vector = this.hit.transform.position + component.offset;
			float time = Time.time;
			foreach (Collider2D collider2D in Physics2D.OverlapBoxAll(vector, component.size * 1.1f, 0f, LayerMask.GetMask(new string[] { "Item" })))
			{
				Item item = (collider2D ? collider2D.GetComponentInParent<Item>() : null);
				if (item && item.rb && !item.transform.parent && ThrownByHer.Recently(item))
				{
					float magnitude = (item.rb.velocity - this.vel).magnitude;
					if (magnitude >= 5f)
					{
						int instanceID = item.GetInstanceID();
						float num;
						if (!this.struckBy.TryGetValue(instanceID, out num) || time - num >= 1f)
						{
							this.struckBy[instanceID] = time;
							bool flag;
							if (item.Stats != null && item.Stats.qualities != null)
							{
								flag = item.Stats.qualities.Exists((CraftingQuality q) => q.id == "cutting");
							}
							else
							{
								flag = false;
							}
							bool flag2 = flag;
							float num2 = magnitude * Mathf.Clamp(item.rb.mass, 0.2f, 4f) * (flag2 ? 1.5f : 1f) * Plugin.ThrownDamage.Value;
							Scavenger.Striker = null;
							this.hit.health -= num2;
							Sound.Play(flag2 ? "gore3" : "boneHit", item.transform.position, false, true, null, 0.9f, 1f, false, false);
							item.rb.velocity = -item.rb.velocity * 0.3f + Vector2.up * 2f;
							Plugin.Log.LogInfo(string.Format("A thrown {0} hits the {1}: {2:0} ({3:0.0} m/s), {4:0} left", new object[]
							{
								item.id,
								this.slimeHost ? "slime host" : "raider",
								num2,
								magnitude,
								this.hit.health
							}));
							this.OnHit(num2);
						}
					}
				}
			}
			if (this.struckBy.Count > 20)
			{
				this.struckBy.Clear();
			}
		}

		// Token: 0x06000144 RID: 324 RVA: 0x000146EC File Offset: 0x000128EC
		public void Shoo()
		{
			if (this.slimeHost)
			{
				return;
			}
			this.shoo = 1;
			this.shooTimer = 10f;
			this.Bark("scav_shoo", true);
			if (this.Player)
			{
				LewdLink.Say(this.Player, "scav_after");
			}
			Plugin.Log.LogInfo("The raider tells her to get lost (10 s)");
		}

		// Token: 0x06000145 RID: 325 RVA: 0x0001474C File Offset: 0x0001294C
		private bool ShooTick(Body b, float dt)
		{
			if (this.shoo == 0)
			{
				return false;
			}
			if (!b || !b.alive || Capture.Active || this.mode == Scavenger.Mode.Dead)
			{
				this.shoo = 0;
				return false;
			}
			Vector2 vector = b.limbs[1].transform.position;
			float num = Vector2.Distance(this.pos, vector);
			if (num > ((this.shoo == 3) ? 12f : 10f))
			{
				if (this.shoo >= 2)
				{
					this.Bark("scav_shoo_done", true);
				}
				Plugin.Log.LogInfo(string.Format("She got away from the raider ({0}, {1:0.0} m)", (this.shoo == 3) ? "after a beating" : ((this.shoo == 2) ? "after his threats" : "when told"), num));
				this.EndShoo();
				return false;
			}
			this.facing = ((vector.x >= this.pos.x) ? 1 : (-1));
			float num2 = 0f;
			this.shooTimer -= dt;
			if (this.shoo == 1)
			{
				this.aim = Mathf.MoveTowards(this.aim, 0f, dt * 2f);
				if (this.shooTimer <= 0f)
				{
					this.shoo = 2;
					this.shooTimer = 5f;
					this.Bark("scav_shoo_threat", true);
					Plugin.Log.LogInfo("She is still here: the raider threatens her");
				}
			}
			else if (this.shoo == 2)
			{
				this.aim = Mathf.MoveTowards(this.aim, this.weapon.Gun ? 1f : 0f, dt * 2f);
				if (this.shooTimer <= 0f)
				{
					this.shoo = 3;
					this.beatsLeft = Random.Range(3, 5);
					this.beatCooldown = 0.3f;
					this.Bark("scav_shoo_beat", true);
					Plugin.Log.LogInfo("She would not go: the raider beats her (" + (Diff.LethalScenes ? "his weapon" : "no deadly scenes: bruises only") + ")");
				}
			}
			else
			{
				this.aim = Mathf.MoveTowards(this.aim, 0f, dt * 3f);
				if (num > 1.6f)
				{
					num2 = (float)this.facing;
				}
				this.beatCooldown -= dt;
				if (this.swing < 0f && this.beatCooldown <= 0f && num < 2.4f)
				{
					this.swing = 0f;
					Sound.Play("BSSwing" + Random.Range(1, 5).ToString(), base.transform.position, false, true, null, 0.7f, 1f, false, false);
				}
				if (this.swing >= 0f)
				{
					float num3 = this.swing;
					this.swing += dt / 0.55f;
					if (num3 < 0.65f && this.swing >= 0.65f && num < 2.8f)
					{
						if (Diff.LethalScenes)
						{
							this.Strike(b);
						}
						else
						{
							this.Bruise(b);
						}
						this.beatsLeft--;
					}
					if (this.swing >= 1f)
					{
						this.swing = -1f;
						this.beatCooldown = 1.2f;
					}
				}
				if (this.beatsLeft <= 0 && this.swing < 0f)
				{
					this.Bark("scav_shoo_done", true);
					Plugin.Log.LogInfo("The raider is done beating her and walks off");
					this.EndShoo();
					return false;
				}
			}
			this.vel.x = Mathf.MoveTowards(this.vel.x, num2 * 4f, (this.grounded ? 30f : 8f) * dt);
			this.Step(dt, num2);
			this.Animate(dt, b, vector);
			this.UpdateBark(dt);
			return true;
		}

		// Token: 0x06000146 RID: 326 RVA: 0x00014B24 File Offset: 0x00012D24
		private void EndShoo()
		{
			this.shoo = 0;
			this.swing = -1f;
			this.mode = Scavenger.Mode.Wander;
			this.sated = Mathf.Max(this.sated, 30f);
			this.home = this.pos;
			this.wanderTo = this.pos + new Vector2((float)(-(float)this.facing) * 12f, 0f);
			this.think = 6f;
		}

		// Token: 0x06000147 RID: 327 RVA: 0x00014BA0 File Offset: 0x00012DA0
		private void Bruise(Body b)
		{
			string[] array = new string[] { "UpTorso", "DownTorso", "UpArmF", "UpArmB", "ThighF" };
			Limb limb = b.LimbByName(array[Random.Range(0, array.Length)]) ?? b.limbs[1];
			float num = Mathf.Max(0.5f, limb.GetArmorReduction());
			limb.pain = Mathf.Min(100f, limb.pain + 14f / num);
			limb.skinHealth = Mathf.Max(60f, limb.skinHealth - 2f / num);
			b.rb.velocity += new Vector2((float)this.facing * 5f, 2f);
			Sound.Play("BSFootstep" + Random.Range(1, 5).ToString(), limb.transform.position, false, true, null, 0.9f, 1f, false, false);
			Plugin.Log.LogInfo("The raider hits her " + limb.name + " (bruise only)");
		}

		// Token: 0x06000148 RID: 328 RVA: 0x00014CD4 File Offset: 0x00012ED4
		public void OnDeath()
		{
			if (this.mode == Scavenger.Mode.Dead)
			{
				return;
			}
			this.mode = Scavenger.Mode.Dead;
			this.deadTimer = 0f;
			this.Bark(null);
			Object.Instantiate(Resources.Load("BloodExplosion"), this.Chest, Quaternion.identity);
			Sound.Play("gore" + Random.Range(1, 6).ToString(), this.Chest, false, true, null, 1f, 1f, false, false);
			if (this.slimeHost)
			{
				LewdLink.Pour(this.pos + Vector2.up * 0.5f, 10);
			}
			else if (Camps.IsTraderBody(this))
			{
				Camps.KeeperDied(this.killedBy == null && !this.killedByCreature);
			}
			else
			{
				Scavengers.DropLoot(this);
			}
			if (!this.slimeHost && this.killedBy && this.killedBy.slimeHost && Plugin.HostsEnabled.Value && Random.value < Plugin.FeudRiseChance.Value)
			{
				this.riseTimer = Random.Range(4f, 7f);
				Plugin.Log.LogInfo("A raider killed by a slime host will get up as one");
			}
			if (this.Holding && this.Player)
			{
				Capture.Release(this.Player, false, false);
			}
			foreach (Item item in this.stolen)
			{
				if (item)
				{
					item.transform.SetParent(null, true);
					item.transform.position = this.pos + Vector2.up * 1.5f;
					item.gameObject.SetActive(true);
					if (item.rb)
					{
						item.rb.simulated = true;
						item.rb.velocity = new Vector2(Random.Range(-4f, 4f), Random.Range(2f, 6f));
					}
				}
			}
			this.stolen.Clear();
			Plugin.Log.LogInfo(string.Format("Scavenger ({0}, {1}) killed at {2}{3}", new object[]
			{
				this.skin,
				this.weapon.Id,
				this.pos,
				this.killedBy ? (" by " + (this.killedBy.slimeHost ? "a slime host" : "a raider")) : ""
			}));
		}

		// Token: 0x06000149 RID: 329 RVA: 0x00014F98 File Offset: 0x00013198
		private void UpdateDead(float dt)
		{
			this.deadTimer += dt;
			if (this.riseTimer >= 0f && (this.riseTimer -= dt) < 0f)
			{
				Scavenger.MakeHost(this.pos, false, (this.killedBy && this.killedBy.pack) ? this.killedBy.pack : null).Bark("host_risen", true);
				this.Sfx("slime_gloop", 0.8f);
				Plugin.Log.LogInfo(string.Format("A dead raider gets up as a slime host at {0}", this.pos));
				Object.Destroy(base.gameObject);
				return;
			}
			float num = Mathf.Clamp01(this.deadTimer / 0.45f);
			this.Step(dt, 0f);
			this.vel.x = Mathf.MoveTowards(this.vel.x, 0f, 10f * dt);
			base.transform.position = this.pos + Vector2.up * Mathf.Lerp(2.55f, 0.6f, num);
			base.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Lerp(0f, 90f * (float)this.facing, num * num));
			this.rig.Set("HipF", Mathf.Lerp(0f, 20f, num));
			this.rig.Set("HipB", Mathf.Lerp(0f, -15f, num));
			this.rig.Set("ShoulderF", Mathf.Lerp(0f, 70f, num));
			this.rig.Set("ShoulderB", Mathf.Lerp(0f, -40f, num));
			this.rig.Weapon.enabled = false;
			this.rig.Blink(true);
			if (this.deadTimer > Plugin.ScavCorpseSeconds.Value)
			{
				Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x0600014A RID: 330 RVA: 0x000151C4 File Offset: 0x000133C4
		private void Animate(float dt, Body b, Vector2 target)
		{
			base.transform.position = this.pos + Vector2.up * 2.55f;
			base.transform.localScale = new Vector3((float)this.facing, 1f, 1f);
			float num = Mathf.Clamp01(Mathf.Abs(this.vel.x) / 4f);
			float num2 = this.phase;
			this.phase += dt * (2.2f + 2.4f * num) * (this.grounded ? 1f : 0.3f) * ((num > 0.05f) ? 1f : 0f);
			float num3 = Mathf.Sin(this.phase);
			float num4 = Mathf.Cos(this.phase);
			if (num > 0.05f && this.grounded && Mathf.Floor(num2 / 3.1415927f) != Mathf.Floor(this.phase / 3.1415927f))
			{
				Sound.Play("BSFootstep" + Random.Range(1, 5).ToString(), this.pos, false, true, null, 0.35f, 1f, false, false);
			}
			float num5 = 28f * num;
			if (!this.grounded)
			{
				this.rig.Set("HipF", 20f);
				this.rig.Set("HipB", -12f);
				this.rig.Set("KneeF", -25f);
				this.rig.Set("KneeB", -30f);
			}
			else
			{
				this.rig.Set("HipF", num3 * num5);
				this.rig.Set("HipB", -num3 * num5);
				this.rig.Set("KneeF", Mathf.Min(0f, num4) * 30f * num);
				this.rig.Set("KneeB", Mathf.Min(0f, -num4) * 30f * num);
			}
			this.rig.Body.localPosition = new Vector2(0f, -Mathf.Abs(num3) * 0.08f * num - this.flinch * 0.05f);
			this.rig.Set("Neck", -this.flinch * 6f);
			float num6 = num3 * 18f * num;
			float num7 = -num6;
			float num8 = num6;
			float num9 = 0f;
			float num10 = 0f;
			float num14;
			if (this.weapon.Gun)
			{
				float num11 = ((this.mode == Scavenger.Mode.Fight) ? this.AimAngle(target) : (-55f));
				float num12 = ((this.mode == Scavenger.Mode.Fight && this.reload <= 0f) ? this.aim : ((this.reload > 0f) ? 0.4f : 0.15f));
				float num13 = num11 + 90f + this.flinch * 8f;
				num7 = Mathf.Lerp(num7, num13, num12);
				if (this.weapon.TwoHanded)
				{
					num8 = Mathf.Lerp(num8, num13 - 6f, num12);
				}
				num14 = Mathf.Lerp(-70f, num11 + this.flinch * 10f, num12);
			}
			else
			{
				float num15 = ((this.mode == Scavenger.Mode.Fight) ? 60f : (-num6));
				if (this.swing >= 0f)
				{
					float num16 = ((this.swing < 0.6f) ? Mathf.SmoothStep(0f, 1f, this.swing / 0.6f) : (1f - Mathf.SmoothStep(0f, 1f, (this.swing - 0.6f) / 0.4f)));
					num7 = ((this.swing < 0.6f) ? Mathf.Lerp(num15, 170f, num16) : Mathf.Lerp(30f, 170f, num16));
					if (this.swing >= 0.6f && this.swing < 0.75f)
					{
						num7 = Mathf.Lerp(170f, 30f, (this.swing - 0.6f) / 0.15f);
					}
				}
				else
				{
					num7 = num15;
				}
				num9 = ((this.mode == Scavenger.Mode.Fight) ? (-20f) : 0f);
				num14 = num7 - 50f + this.gripTurn;
				if (this.twoHandedMelee)
				{
					num8 = num7 - 8f;
					num10 = num9;
				}
			}
			this.rig.Set("ShoulderF", num7);
			this.rig.Set("ShoulderB", num8);
			this.rig.Set("ElbowF", num9);
			this.rig.Set("ElbowB", num10);
			Vector2 vector = this.rig.HandAt(num7);
			if (this.weapon.Gun && this.weapon.TwoHanded)
			{
				Vector2 vector2 = ScavRig.JointAt("ShoulderF");
				Vector2 vector3 = vector2 + (vector - vector2) * 0.68f;
				float num17;
				float num18;
				if (ScavRig.TwoBone("ShoulderF", "ElbowF", vector3, out num17, out num18))
				{
					this.rig.Set("ShoulderF", num17);
					this.rig.Set("ElbowF", num18);
					vector = vector3;
				}
			}
			Vector2 vector4 = (this.weapon.Gun ? Vector2.zero : (Quaternion.Euler(0f, 0f, num14) * this.grip));
			this.rig.Weapon.transform.localPosition = vector - vector4;
			this.rig.Weapon.transform.localRotation = Quaternion.Euler(0f, 0f, num14);
			if (this.weapon.Gun && this.weapon.TwoHanded)
			{
				this.SupportHand(vector, num14);
			}
			if ((this.blink -= dt) <= 0f)
			{
				this.blink = Random.Range(2f, 5f);
				this.rig.Blink(true);
				return;
			}
			if (this.blink < 4.85f)
			{
				this.rig.Blink(false);
			}
		}

		// Token: 0x0600014B RID: 331 RVA: 0x00015820 File Offset: 0x00013A20
		private void SupportHand(Vector2 grip, float gunAngle)
		{
			float num = ((this.weapon.Id == "arm_mp9") ? 0.4f : 0.7f);
			Vector2 vector = grip + Quaternion.Euler(0f, 0f, gunAngle) * Vector2.right * num;
			float num2;
			float num3;
			if (ScavRig.TwoBone("ShoulderB", "ElbowB", vector, out num2, out num3))
			{
				this.rig.Set("ShoulderB", num2);
				this.rig.Set("ElbowB", num3);
			}
		}

		// Token: 0x0600014C RID: 332 RVA: 0x000158BA File Offset: 0x00013ABA
		private void Sfx(string set, float vol)
		{
			LewdLink.Sound(this.Player, vol, new string[] { set });
		}

		// Token: 0x0600014D RID: 333 RVA: 0x000158D2 File Offset: 0x00013AD2
		private Vector2 Mirror(Vector2 local)
		{
			return new Vector2(local.x * (float)this.facing, local.y + 2.55f);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x000158F4 File Offset: 0x00013AF4
		public void Bark(string key)
		{
			this.Bark(key, key != null && key.StartsWith("scav_") && !new string[] { "scav_spot", "scav_lost", "scav_hurt", "scav_flee", "scav_reload" }.Contains(key));
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00015954 File Offset: 0x00013B54
		public void Bark(string key, bool force)
		{
			if (key == null)
			{
				if (this.bark)
				{
					Object.Destroy(this.bark.gameObject);
				}
				return;
			}
			if (this.slimeHost && key.StartsWith("scav_") && !Scavenger.HostLine.TryGetValue(key, out key))
			{
				return;
			}
			if (!force && ((this.barkTimer > Time.time && key != "scav_spot") || Random.value < 0.25f))
			{
				return;
			}
			string text = Scavengers.Line(key, this.female);
			if (text == null)
			{
				return;
			}
			this.barkTimer = Time.time + 4f;
			if (!this.bark)
			{
				this.bark = Scavenger.MakeBarkText();
				if (!this.bark)
				{
					return;
				}
			}
			this.bark.text = text;
			this.bark.color = (this.slimeHost ? new Color(0.62f, 1f, 0.35f) : new Color(1f, 0.85f, 0.7f));
			this.barkLife = 2.8f;
		}

		// Token: 0x06000150 RID: 336 RVA: 0x00015A70 File Offset: 0x00013C70
		private static TMP_Text MakeBarkText()
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Talker talker = (body ? body.talker : null);
			TMP_Text tmp_Text = null;
			if (talker && talker.textPrefab)
			{
				GameObject gameObject = Object.Instantiate<GameObject>(talker.textPrefab);
				gameObject.name = "Bark";
				tmp_Text = gameObject.GetComponent<TMP_Text>();
				foreach (MonoBehaviour monoBehaviour in gameObject.GetComponents<MonoBehaviour>())
				{
					if (!(monoBehaviour is TMP_Text))
					{
						Object.Destroy(monoBehaviour);
					}
				}
			}
			if (!tmp_Text)
			{
				TextMeshPro textMeshPro = new GameObject("Bark").AddComponent<TextMeshPro>();
				if (talker && talker.text)
				{
					textMeshPro.font = talker.text.font;
					textMeshPro.fontSize = talker.text.fontSize;
				}
				else
				{
					textMeshPro.fontSize = 5f;
				}
				tmp_Text = textMeshPro;
			}
			tmp_Text.alignment = 1026;
			tmp_Text.enableWordWrapping = false;
			tmp_Text.text = "";
			MeshRenderer component = tmp_Text.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.sortingOrder = 32000;
			}
			return tmp_Text;
		}

		// Token: 0x06000151 RID: 337 RVA: 0x00015BA4 File Offset: 0x00013DA4
		public void BarkText(string line, float life)
		{
			if (line == null)
			{
				if (this.bark)
				{
					Object.Destroy(this.bark.gameObject);
					this.bark = null;
				}
				return;
			}
			if (!this.bark)
			{
				this.bark = Scavenger.MakeBarkText();
				if (!this.bark)
				{
					return;
				}
			}
			this.bark.text = line;
			this.bark.color = new Color(1f, 0.85f, 0.7f);
			this.barkLife = Mathf.Max(2.8f, life);
			this.barkTimer = Time.time + 4f;
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00015C4C File Offset: 0x00013E4C
		private void UpdateBark(float dt)
		{
			if (!this.bark)
			{
				return;
			}
			this.bark.transform.position = this.pos + Vector2.up * 5.6f;
			this.barkLife -= dt;
			Color color = this.bark.color;
			color.a = Mathf.Clamp01(this.barkLife / 0.5f);
			this.bark.color = color;
			if (this.barkLife <= 0f)
			{
				Object.Destroy(this.bark.gameObject);
				this.bark = null;
			}
		}

		// Token: 0x06000153 RID: 339 RVA: 0x00015CF8 File Offset: 0x00013EF8
		private void OnDestroy()
		{
			Scavengers.All.Remove(this);
			if (this.bark)
			{
				Object.Destroy(this.bark.gameObject);
			}
		}

		// Token: 0x04000122 RID: 290
		public Scavenger.Mode mode;

		// Token: 0x04000123 RID: 291
		public Vector2 pos;

		// Token: 0x04000124 RID: 292
		public Vector2 vel;

		// Token: 0x04000125 RID: 293
		public int facing = 1;

		// Token: 0x04000126 RID: 294
		public bool female;

		// Token: 0x04000127 RID: 295
		public bool grounded;

		// Token: 0x04000128 RID: 296
		public string skin;

		// Token: 0x04000129 RID: 297
		public string overall;

		// Token: 0x0400012A RID: 298
		public WeaponDef weapon;

		// Token: 0x0400012B RID: 299
		public BuildingEntity hit;

		// Token: 0x0400012C RID: 300
		public bool grabber;

		// Token: 0x0400012D RID: 301
		public bool lecher;

		// Token: 0x0400012E RID: 302
		public bool hasKey;

		// Token: 0x0400012F RID: 303
		public bool slimeHost;

		// Token: 0x04000130 RID: 304
		public bool spent;

		// Token: 0x04000131 RID: 305
		public Scavenger pack;

		// Token: 0x04000132 RID: 306
		private bool stayNear;

		// Token: 0x04000133 RID: 307
		private bool lurk;

		// Token: 0x04000134 RID: 308
		public bool Holding;

		// Token: 0x04000135 RID: 309
		public float Thrust;

		// Token: 0x04000136 RID: 310
		private bool comingFor;

		// Token: 0x04000137 RID: 311
		public bool offered;

		// Token: 0x04000138 RID: 312
		public float refusedUntil;

		// Token: 0x04000139 RID: 313
		private float sated;

		// Token: 0x0400013A RID: 314
		private float stunned;

		// Token: 0x0400013B RID: 315
		private float warnHold;

		// Token: 0x0400013C RID: 316
		private float mutterTimer;

		// Token: 0x0400013D RID: 317
		private float rattled;

		// Token: 0x0400013E RID: 318
		private float thrownScan;

		// Token: 0x0400013F RID: 319
		private readonly Dictionary<int, float> struckBy = new Dictionary<int, float>();

		// Token: 0x04000140 RID: 320
		private readonly List<Item> stolen = new List<Item>();

		// Token: 0x04000141 RID: 321
		private ScavRig rig;

		// Token: 0x04000142 RID: 322
		private Vector2 home;

		// Token: 0x04000143 RID: 323
		private Vector2 wanderTo;

		// Token: 0x04000144 RID: 324
		private Vector2 lastSeen;

		// Token: 0x04000145 RID: 325
		private float phase;

		// Token: 0x04000146 RID: 326
		private float think;

		// Token: 0x04000147 RID: 327
		private float alertTimer;

		// Token: 0x04000148 RID: 328
		private float aim;

		// Token: 0x04000149 RID: 329
		private float shotTimer;

		// Token: 0x0400014A RID: 330
		private float reload;

		// Token: 0x0400014B RID: 331
		private float swing = -1f;

		// Token: 0x0400014C RID: 332
		private float swingCooldown;

		// Token: 0x0400014D RID: 333
		private float blink;

		// Token: 0x0400014E RID: 334
		private float barkTimer;

		// Token: 0x0400014F RID: 335
		private float deadTimer;

		// Token: 0x04000150 RID: 336
		private float stagger;

		// Token: 0x04000151 RID: 337
		private float flinch;

		// Token: 0x04000152 RID: 338
		private int rounds;

		// Token: 0x04000153 RID: 339
		private int burstLeft;

		// Token: 0x04000154 RID: 340
		private float maxHealth;

		// Token: 0x04000155 RID: 341
		private TMP_Text bark;

		// Token: 0x04000156 RID: 342
		private float barkLife;

		// Token: 0x04000157 RID: 343
		private AudioClip fireSound;

		// Token: 0x04000158 RID: 344
		private static int orderSeed;

		// Token: 0x04000159 RID: 345
		public static readonly WeaponDef Bare = new WeaponDef
		{
			Id = "",
			Range = 1.9f,
			Cooldown = 1f
		};

		// Token: 0x0400015A RID: 346
		private Vector2 grip;

		// Token: 0x0400015B RID: 347
		private float gripTurn;

		// Token: 0x0400015C RID: 348
		private bool twoHandedMelee;

		// Token: 0x0400015D RID: 349
		private float repick;

		// Token: 0x0400015E RID: 350
		private float prefDist = 9f;

		// Token: 0x0400015F RID: 351
		private float shuffleTime;

		// Token: 0x04000160 RID: 352
		public static Scavenger Striker;

		// Token: 0x04000161 RID: 353
		public static bool CreatureHit;

		// Token: 0x04000162 RID: 354
		private bool killedByCreature;

		// Token: 0x04000163 RID: 355
		private Scavenger foe;

		// Token: 0x04000164 RID: 356
		private Scavenger killedBy;

		// Token: 0x04000165 RID: 357
		private float foeScan;

		// Token: 0x04000166 RID: 358
		private float riseTimer = -1f;

		// Token: 0x04000167 RID: 359
		private float engulf;

		// Token: 0x04000168 RID: 360
		private int shuffle;

		// Token: 0x04000169 RID: 361
		private bool warnedFloat;

		// Token: 0x0400016A RID: 362
		private Scavenger guardFoe;

		// Token: 0x0400016B RID: 363
		private BuildingEntity guardBeast;

		// Token: 0x0400016C RID: 364
		private float guardScan;

		// Token: 0x0400016D RID: 365
		private float greetAt;

		// Token: 0x0400016E RID: 366
		private float tendFor = -1f;

		// Token: 0x0400016F RID: 367
		private float tendAnim;

		// Token: 0x04000170 RID: 368
		private Action tendDone;

		// Token: 0x04000171 RID: 369
		private int shoo;

		// Token: 0x04000172 RID: 370
		private float shooTimer;

		// Token: 0x04000173 RID: 371
		private float beatCooldown;

		// Token: 0x04000174 RID: 372
		private int beatsLeft;

		// Token: 0x04000175 RID: 373
		private static readonly Dictionary<string, string> HostLine = new Dictionary<string, string>
		{
			{ "scav_spot", "host_spot" },
			{ "scav_grab", "host_grab" },
			{ "scav_take_surrendered", "host_grab" },
			{ "scav_surrender_seen", "host_grab" },
			{ "scav_release", "host_release" },
			{ "scav_escaped", "host_escaped" },
			{ "scav_hit_her", "host_hit" },
			{ "scav_wait", "host_wait" },
			{ "scav_lost", "host_lost" },
			{ "scav_hurt", "host_hurt" },
			{ "scav_grab_miss", "host_miss" },
			{ "scav_foe", "host_foe" }
		};

		// Token: 0x0200016A RID: 362
		public enum Mode
		{
			// Token: 0x04000869 RID: 2153
			Wander,
			// Token: 0x0400086A RID: 2154
			Alert,
			// Token: 0x0400086B RID: 2155
			Fight,
			// Token: 0x0400086C RID: 2156
			Flee,
			// Token: 0x0400086D RID: 2157
			Dead
		}
	}
}
