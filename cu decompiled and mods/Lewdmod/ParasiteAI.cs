using System;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000D3 RID: 211
	internal class ParasiteAI : MonoBehaviour
	{
		// Token: 0x1700009C RID: 156
		// (get) Token: 0x06000485 RID: 1157 RVA: 0x0004940C File Offset: 0x0004760C
		public bool Mite
		{
			get
			{
				return this.color == 8;
			}
		}

		// Token: 0x1700009D RID: 157
		// (get) Token: 0x06000486 RID: 1158 RVA: 0x00049417 File Offset: 0x00047617
		public Rigidbody2D Rb
		{
			get
			{
				return this.rb;
			}
		}

		// Token: 0x1700009E RID: 158
		// (get) Token: 0x06000487 RID: 1159 RVA: 0x0004941F File Offset: 0x0004761F
		public SpriteRenderer Sr
		{
			get
			{
				return this.sr;
			}
		}

		// Token: 0x06000488 RID: 1160 RVA: 0x00049427 File Offset: 0x00047627
		public void Flee(float seconds)
		{
			this.fleeTimer = Mathf.Max(this.fleeTimer, seconds);
			this.leapCooldown = Mathf.Max(this.leapCooldown, seconds);
			this.leaping = false;
		}

		// Token: 0x1700009F RID: 159
		// (get) Token: 0x06000489 RID: 1161 RVA: 0x00049454 File Offset: 0x00047654
		private static Body Player
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

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x0600048A RID: 1162 RVA: 0x0004946E File Offset: 0x0004766E
		private static int Ground
		{
			get
			{
				if (ParasiteAI.groundMask < 0)
				{
					return ParasiteAI.groundMask = LayerMask.GetMask(new string[] { "Ground" });
				}
				return ParasiteAI.groundMask;
			}
		}

		// Token: 0x0600048B RID: 1163 RVA: 0x00049498 File Offset: 0x00047698
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody2D>();
			this.sr = base.GetComponent<SpriteRenderer>();
			this.be = base.GetComponent<BuildingEntity>();
			this.hopTimer = Random.Range(1f, 4f);
			this.leapCooldown = (this.weakened ? 6f : 1f);
			this.fleeTimer = (this.weakened ? 8f : 0f);
			this.gurgleTimer = Random.Range(3f, 9f);
			this.lastFree = base.transform.position;
			this.hasFree = !this.InRock();
			this.IgnoreHer(ParasiteAI.Player);
		}

		// Token: 0x0600048C RID: 1164 RVA: 0x00049558 File Offset: 0x00047758
		private void IgnoreHer(Body b)
		{
			if (!b || this.ignoring == b)
			{
				return;
			}
			this.ignoring = b;
			Collider2D component = base.GetComponent<Collider2D>();
			if (!component)
			{
				return;
			}
			foreach (Collider2D collider2D in b.GetComponentsInChildren<Collider2D>(true))
			{
				if (collider2D)
				{
					Physics2D.IgnoreCollision(component, collider2D, true);
				}
			}
		}

		// Token: 0x0600048D RID: 1165 RVA: 0x000495C0 File Offset: 0x000477C0
		private bool InRock()
		{
			WorldGeneration world = WorldGeneration.world;
			return world && this.sr && world.GetBlock(world.WorldToBlockPos(this.sr.bounds.center)) > 0;
		}

		// Token: 0x0600048E RID: 1166 RVA: 0x00049614 File Offset: 0x00047814
		private void Update()
		{
			this.frameTime -= Time.deltaTime * (this.leaping ? 3f : 1f);
			if (this.frameTime <= 0f)
			{
				this.frameTime = 0.35f;
				Sprite[] array = ((this.Mite && ParasiteArt.MiteFrames.Length != 0) ? ParasiteArt.MiteFrames : ParasiteArt.Frames);
				this.frame = (this.frame + 1) % array.Length;
				this.sr.sprite = array[this.frame];
			}
			Body player = ParasiteAI.Player;
			if (player == null || !player.alive)
			{
				return;
			}
			float num = Vector2.Distance(this.sr.bounds.center, player.limbs[1].transform.position);
			if (num < 14f)
			{
				this.gurgleTimer -= Time.deltaTime;
				if (this.gurgleTimer <= 0f)
				{
					this.gurgleTimer = Random.Range(4f, 9f);
					Sound.Play("goo", base.transform.position, false, true, base.transform, 0.5f, 0.7f, false, false);
				}
			}
			if (this.leaping && num < 1.8f)
			{
				this.leaping = false;
				this.Latch(player);
			}
		}

		// Token: 0x0600048F RID: 1167 RVA: 0x00049778 File Offset: 0x00047978
		private void FixedUpdate()
		{
			Body player = ParasiteAI.Player;
			if (player == null || this.rb == null)
			{
				return;
			}
			if (Time.timeScale > 5f)
			{
				return;
			}
			this.IgnoreHer(player);
			float fixedDeltaTime = Time.fixedDeltaTime;
			this.leapCooldown -= fixedDeltaTime;
			this.fleeTimer -= fixedDeltaTime;
			this.stepCooldown -= fixedDeltaTime;
			if (this.InRock())
			{
				if (this.hasFree)
				{
					this.rb.position = this.lastFree;
					this.rb.velocity = Vector2.zero;
				}
				return;
			}
			this.lastFree = this.rb.position;
			this.hasFree = true;
			if ((player.limbs[1].transform.position - this.rb.position).sqrMagnitude > 400f)
			{
				return;
			}
			if (player.alive && this.stepCooldown <= 0f && this.Wants && !Suit.CoolingDown)
			{
				Bounds bounds = this.sr.bounds;
				if (ParasiteAI.feetOf != player)
				{
					ParasiteAI.feetOf = player;
					ParasiteAI.feet[0] = player.LimbByName("FootF");
					ParasiteAI.feet[1] = player.LimbByName("FootB");
				}
				foreach (Limb limb in ParasiteAI.feet)
				{
					if (limb && !limb.dismembered)
					{
						Vector2 vector = limb.transform.position;
						if (Mathf.Abs(vector.x - bounds.center.x) < bounds.extents.x + 0.3f && vector.y > bounds.min.y - 0.2f && vector.y < bounds.max.y + 0.6f)
						{
							this.stepCooldown = 2f;
							Plugin.Log.LogInfo("She steps on a parasite");
							this.Latch(player);
							if (!this)
							{
								return;
							}
							break;
						}
					}
				}
			}
			Vector2 vector2 = this.sr.bounds.center;
			Vector2 vector3 = player.limbs[1].transform.position;
			Vector2 vector4 = vector3 - vector2;
			float magnitude = vector4.magnitude;
			int ground = ParasiteAI.Ground;
			bool flag = Physics2D.Raycast(vector2, Vector2.down, this.sr.bounds.extents.y + 0.35f, ground);
			if (this.leaping && flag && this.rb.velocity.y <= 0.1f)
			{
				this.leaping = false;
			}
			if (!flag)
			{
				return;
			}
			this.sr.flipX = vector4.x < 0f;
			bool flag2 = !Physics2D.Linecast(vector2, vector3, ground) || !Physics2D.Linecast(vector2 + Vector2.up, vector3, ground);
			bool flag3 = player.alive && magnitude < 10f && this.Wants && !Suit.CoolingDown && this.fleeTimer <= 0f && flag2;
			if (Plugin.DebugKeys.Value && magnitude < 12f && (this.debugLog -= fixedDeltaTime) <= 0f)
			{
				this.debugLog = 3f;
				Plugin.Log.LogInfo(string.Format("Parasite: dist {0:0.0}, grounded {1}, interested {2}, leaping {3}, cooldown {4:0.0}", new object[] { magnitude, flag, flag3, this.leaping, this.leapCooldown }));
			}
			if (this.fleeTimer > 0f && magnitude < 10f)
			{
				this.rb.velocity = new Vector2(-Mathf.Sign(vector4.x) * 2.5f, this.rb.velocity.y);
				return;
			}
			if (flag3 && magnitude < 4.5f && this.leapCooldown <= 0f)
			{
				this.leaping = true;
				this.leapCooldown = Random.Range(2.2f, 3.5f);
				this.rb.velocity = new Vector2(Mathf.Clamp(vector4.x * 2.6f, -9f, 9f), 6.5f + Mathf.Max(0f, vector4.y) * 1.5f);
				Sound.Play("goo", base.transform.position, false, true, base.transform, 0.8f, 1.3f, false, false);
				return;
			}
			if (!flag3)
			{
				this.hopTimer -= fixedDeltaTime;
				if (this.hopTimer <= 0f)
				{
					this.hopTimer = Random.Range(2.5f, 6f);
					this.rb.velocity = new Vector2(Random.Range(-2.5f, 2.5f), Random.Range(2f, 4f));
				}
				return;
			}
			if (Mathf.Abs(this.rb.velocity.x) < 0.3f && (this.stuckTime += fixedDeltaTime) > 0.6f)
			{
				this.stuckTime = 0f;
				this.rb.velocity = new Vector2(Mathf.Sign(vector4.x) * 2.5f, 4.5f);
				return;
			}
			this.rb.velocity = new Vector2(Mathf.Sign(vector4.x) * 1.6f, this.rb.velocity.y);
		}

		// Token: 0x06000490 RID: 1168 RVA: 0x00049D80 File Offset: 0x00047F80
		private void OnCollisionEnter2D(Collision2D c)
		{
			Body body;
			if (this.leaping && Utils.GetBody(c.gameObject, ref body) && PlayerCamera.main && body == PlayerCamera.main.body)
			{
				this.leaping = false;
				this.Latch(body);
			}
		}

		// Token: 0x170000A1 RID: 161
		// (get) Token: 0x06000491 RID: 1169 RVA: 0x00049DD0 File Offset: 0x00047FD0
		private bool Wants
		{
			get
			{
				if (!this.Mite)
				{
					return Suit.Stage == 0;
				}
				return LatexMites.Wants;
			}
		}

		// Token: 0x06000492 RID: 1170 RVA: 0x00049DE8 File Offset: 0x00047FE8
		private void Latch(Body b)
		{
			if (this.Mite)
			{
				if (b.alive)
				{
					LatexMites.OnLatch(b, this);
				}
				return;
			}
			if (Suit.Stage > 0 || !b.alive)
			{
				return;
			}
			float num = Suit.AttachChance(b);
			bool flag = Random.value < num;
			Plugin.Log.LogInfo(string.Format("Parasite leaps: chance {0:P0}, {1}", num, flag ? "latched on" : "missed"));
			if (flag)
			{
				Suit.Attach(b, this.color);
				Object.Destroy(base.gameObject);
				return;
			}
			Limb limb = (from l in b.limbs
				where l && !l.dismembered
				select l into _
				orderby Random.value
				select _).FirstOrDefault<Limb>();
			if (!limb)
			{
				return;
			}
			float armorReduction = limb.GetArmorReduction();
			limb.skinHealth -= 8f / armorReduction;
			limb.pain += 15f / armorReduction;
			Plugin.S.arousal = Mathf.Min(100f, Plugin.S.arousal + 10f);
			b.eyePanicTime = 0.4f;
			Sound.Play("goo", base.transform.position, false, true, base.transform, 1f, 1.1f, false, false);
			this.rb.velocity = new Vector2(-Mathf.Sign(b.transform.position.x - base.transform.position.x) * 4f, 3f);
			this.leapCooldown = 3f;
			Relief.Say(b, "parasite_miss");
		}

		// Token: 0x06000493 RID: 1171 RVA: 0x00049FB4 File Offset: 0x000481B4
		public void AnimalHit(float damage)
		{
			Sound.Play("goo", base.transform.position, false, true, base.transform, 1f, 1.5f, false, false);
			this.fleeTimer = Mathf.Max(this.fleeTimer, 1.2f);
		}

		// Token: 0x06000494 RID: 1172 RVA: 0x0004A008 File Offset: 0x00048208
		public void AnimalDeath()
		{
			Sound.Play("goo", base.transform.position, false, true, null, 1f, 0.6f, false, false);
			Parasites.Alive.Remove(this);
		}

		// Token: 0x06000495 RID: 1173 RVA: 0x0004A04B File Offset: 0x0004824B
		private void OnDestroy()
		{
			Parasites.Alive.Remove(this);
		}

		// Token: 0x0400044E RID: 1102
		public int color;

		// Token: 0x0400044F RID: 1103
		public bool weakened;

		// Token: 0x04000450 RID: 1104
		private Rigidbody2D rb;

		// Token: 0x04000451 RID: 1105
		private SpriteRenderer sr;

		// Token: 0x04000452 RID: 1106
		private BuildingEntity be;

		// Token: 0x04000453 RID: 1107
		private float frameTime;

		// Token: 0x04000454 RID: 1108
		private float hopTimer;

		// Token: 0x04000455 RID: 1109
		private float leapCooldown;

		// Token: 0x04000456 RID: 1110
		private float fleeTimer;

		// Token: 0x04000457 RID: 1111
		private float gurgleTimer;

		// Token: 0x04000458 RID: 1112
		private float debugLog;

		// Token: 0x04000459 RID: 1113
		private float stuckTime;

		// Token: 0x0400045A RID: 1114
		private float stepCooldown;

		// Token: 0x0400045B RID: 1115
		private bool leaping;

		// Token: 0x0400045C RID: 1116
		private int frame;

		// Token: 0x0400045D RID: 1117
		private Body ignoring;

		// Token: 0x0400045E RID: 1118
		private Vector2 lastFree;

		// Token: 0x0400045F RID: 1119
		private bool hasFree;

		// Token: 0x04000460 RID: 1120
		private const float FarSq = 400f;

		// Token: 0x04000461 RID: 1121
		private static int groundMask = -1;

		// Token: 0x04000462 RID: 1122
		private static Body feetOf;

		// Token: 0x04000463 RID: 1123
		private static readonly Limb[] feet = new Limb[2];
	}
}
