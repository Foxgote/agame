using System;
using PoseKit;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000122 RID: 290
	internal static class WallCrawl
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060006C0 RID: 1728 RVA: 0x00069F84 File Offset: 0x00068184
		// (set) Token: 0x060006C1 RID: 1729 RVA: 0x00069F8B File Offset: 0x0006818B
		public static bool Active { get; private set; }

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060006C2 RID: 1730 RVA: 0x00069F93 File Offset: 0x00068193
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x060006C3 RID: 1731 RVA: 0x00069F9C File Offset: 0x0006819C
		private static bool Can(Body b)
		{
			return Suit.Symbiote && Suit.Stage >= 5 && b && b.alive && b.conscious && b.standing && !b.inWater && !b.currentClimbable && !Scenes.Busy && !HerPuppet.Active && !Relief.Active && LewdSettings.ParasitesOn;
		}

		// Token: 0x060006C4 RID: 1732 RVA: 0x0006A00C File Offset: 0x0006820C
		private static bool Key(string bind)
		{
			if (Time.time < WallCrawl.debugUpUntil)
			{
				return bind == "up" || (bind == "right" && WallCrawl.side > 0) || (bind == "left" && WallCrawl.side < 0);
			}
			return Keys.Held(KeyBinds.GetBind(bind));
		}

		// Token: 0x060006C5 RID: 1733 RVA: 0x0006A06C File Offset: 0x0006826C
		public static string DebugClimb(Body b, float seconds)
		{
			Vector2 vector = b.transform.position + b.col.offset;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			RaycastHit2D raycastHit2D = Physics2D.Raycast(vector, Vector2.right, 14f, mask);
			RaycastHit2D raycastHit2D2 = Physics2D.Raycast(vector, Vector2.left, 14f, mask);
			if (!raycastHit2D && !raycastHit2D2)
			{
				return "no wall near her";
			}
			int num = ((raycastHit2D && (!raycastHit2D2 || raycastHit2D.distance <= raycastHit2D2.distance)) ? 1 : (-1));
			RaycastHit2D raycastHit2D3 = ((num > 0) ? raycastHit2D : raycastHit2D2);
			float num2 = b.col.size.x * 0.5f + b.col.edgeRadius + 0.05f;
			b.transform.position += new Vector2(raycastHit2D3.point.x - (float)num * num2 - vector.x, 0.3f);
			Physics2D.SyncTransforms();
			WallCrawl.side = num;
			WallCrawl.debugUpUntil = Time.time + seconds;
			return string.Format("at the wall on her {0} ({1:0.0} away), climbing {2:0}s", (num > 0) ? "right" : "left", raycastHit2D3.distance, seconds);
		}

		// Token: 0x060006C6 RID: 1734 RVA: 0x0006A1C8 File Offset: 0x000683C8
		private static int Wall(Body b)
		{
			Vector2 vector = b.transform.position + b.col.offset;
			int mask = LayerMask.GetMask(new string[] { "Ground" });
			float num = b.col.edgeRadius + 0.14f;
			bool flag = Physics2D.BoxCast(vector, b.col.size, 0f, Vector2.right, num, mask);
			bool flag2 = Physics2D.BoxCast(vector, b.col.size, 0f, Vector2.left, num, mask);
			if (flag && flag2)
			{
				if (WallCrawl.side == 0)
				{
					return 1;
				}
				return WallCrawl.side;
			}
			else
			{
				if (flag)
				{
					return 1;
				}
				if (!flag2)
				{
					return 0;
				}
				return -1;
			}
		}

		// Token: 0x060006C7 RID: 1735 RVA: 0x0006A284 File Offset: 0x00068484
		private static bool WallAtHead(Body b, int s)
		{
			return Physics2D.Raycast(b.transform.position + b.col.offset + Vector2.up * (b.col.size.y * 0.5f + 0.2f), Vector2.right * (float)s, b.col.size.x * 0.5f + b.col.edgeRadius + 0.45f, LayerMask.GetMask(new string[] { "Ground" }));
		}

		// Token: 0x060006C8 RID: 1736 RVA: 0x0006A330 File Offset: 0x00068530
		private static bool Ceiling(Body b)
		{
			return Physics2D.BoxCast(b.transform.position + b.col.offset, new Vector2(b.col.size.x * 0.8f, b.col.size.y), 0f, Vector2.up, b.col.edgeRadius + 0.12f, LayerMask.GetMask(new string[] { "Ground" }));
		}

		// Token: 0x060006C9 RID: 1737 RVA: 0x0006A3C0 File Offset: 0x000685C0
		public static void Update(Body b, float dt)
		{
			if (b != WallCrawl.body)
			{
				WallCrawl.Active = false;
				WallCrawl.body = b;
			}
			if (WallCrawl.cooldown > 0f)
			{
				WallCrawl.cooldown -= dt;
			}
			if (!WallCrawl.Can(b))
			{
				if (WallCrawl.Active)
				{
					WallCrawl.Release(b);
				}
				return;
			}
			int num = WallCrawl.Wall(b);
			float num2 = (WallCrawl.Key("right") ? 1f : 0f) - (WallCrawl.Key("left") ? 1f : 0f);
			bool flag = WallCrawl.Key("up");
			bool flag2 = WallCrawl.Key("down");
			if (!WallCrawl.Active)
			{
				if (WallCrawl.cooldown <= 0f && num != 0 && num2 != 0f && (int)Mathf.Sign(num2) == num && (!b.grounded || flag) && b.stamina > 10f)
				{
					WallCrawl.Begin(b, num);
				}
				return;
			}
			if (num != WallCrawl.side)
			{
				if (flag && num == 0)
				{
					WallCrawl.Mantle(b);
					return;
				}
				WallCrawl.Release(b);
				return;
			}
			else
			{
				if (num2 != 0f && (int)Mathf.Sign(num2) == -WallCrawl.side)
				{
					WallCrawl.Release(b);
					WallCrawl.cooldown = 0.4f;
					return;
				}
				if (b.grounded && flag2)
				{
					WallCrawl.Release(b);
					return;
				}
				b.stamina -= ((flag || flag2) ? 3.5f : 1.1f) * dt;
				if (b.stamina < 4f)
				{
					Relief.Say(b, "sym_crawl_tired");
					SymbioteVoice.Say(b, "symv_crawl_tired", 1.2f);
					WallCrawl.Release(b);
					WallCrawl.cooldown = 2.5f;
					return;
				}
				if (flag && !WallCrawl.WallAtHead(b, WallCrawl.side))
				{
					WallCrawl.Mantle(b);
					return;
				}
				WallCrawl.vy = ((flag && !WallCrawl.Ceiling(b)) ? 2.6f : (flag2 ? (-2.6f) : 0f));
				b.rb.velocity = new Vector2((float)WallCrawl.side * 1.5f, WallCrawl.vy);
				if ((flag || flag2) && (WallCrawl.lineTimer -= dt) <= 0f)
				{
					WallCrawl.lineTimer = Random.Range(25f, 45f);
					if (Random.value < 0.6f)
					{
						Relief.Say(b, "sym_crawl");
						return;
					}
					SymbioteVoice.Say(b, "symv_crawl", 0f);
				}
				return;
			}
		}

		// Token: 0x060006CA RID: 1738 RVA: 0x0006A610 File Offset: 0x00068810
		private static void Begin(Body b, int s)
		{
			WallCrawl.Active = true;
			WallCrawl.side = s;
			WallCrawl.vy = 0f;
			b.rb.velocity = new Vector2((float)s * 1.5f, 0f);
			if (!WallCrawl.S.symbCrawled)
			{
				WallCrawl.S.symbCrawled = true;
				Relief.Say(b, "sym_crawl_first");
				SymbioteVoice.Say(b, "symv_crawl_first", 2.4f);
				Plugin.Log.LogInfo("The symbiote holds her on a wall: she crawls for the first time");
			}
		}

		// Token: 0x060006CB RID: 1739 RVA: 0x0006A694 File Offset: 0x00068894
		private static void Mantle(Body b)
		{
			int num = WallCrawl.side;
			WallCrawl.Release(b);
			b.rb.velocity = new Vector2((float)num * 3.5f, 7.5f);
			WallCrawl.cooldown = 0.5f;
			if (Random.value < 0.35f)
			{
				Relief.Say(b, "sym_crawl_ledge");
			}
		}

		// Token: 0x060006CC RID: 1740 RVA: 0x0006A6EB File Offset: 0x000688EB
		private static void Release(Body b)
		{
			WallCrawl.Active = false;
			if (b && b.rb)
			{
				b.rb.gravityScale = 1f;
			}
		}

		// Token: 0x060006CD RID: 1741 RVA: 0x0006A718 File Offset: 0x00068918
		public static void Physics(Body b)
		{
			if (!WallCrawl.Active || b != WallCrawl.body)
			{
				return;
			}
			b.rb.gravityScale = 0f;
			b.rb.velocity = new Vector2((float)WallCrawl.side * 1.5f, WallCrawl.vy);
		}

		// Token: 0x060006CE RID: 1742 RVA: 0x0006A76C File Offset: 0x0006896C
		public static void Steer(Body b)
		{
			if (!WallCrawl.Active || b != WallCrawl.body)
			{
				return;
			}
			if (Keys.Down(KeyBinds.GetBind("jump")))
			{
				WallCrawl.Release(b);
				WallCrawl.cooldown = 0.35f;
				return;
			}
			b.moveDir = new Vector2((float)WallCrawl.side, 0f);
			b.crouching = false;
		}

		// Token: 0x04000712 RID: 1810
		private const float Speed = 2.6f;

		// Token: 0x04000713 RID: 1811
		private static int side;

		// Token: 0x04000714 RID: 1812
		private static float vy;

		// Token: 0x04000715 RID: 1813
		private static float cooldown;

		// Token: 0x04000716 RID: 1814
		private static float lineTimer = 20f;

		// Token: 0x04000717 RID: 1815
		private static Body body;

		// Token: 0x04000718 RID: 1816
		private static float debugUpUntil;
	}
}
