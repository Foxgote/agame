using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200005A RID: 90
	internal static class Burning
	{
		// Token: 0x060001B3 RID: 435 RVA: 0x0002581C File Offset: 0x00023A1C
		public static FleshBurn Make(GameObject go, string what, float health, SpriteRenderer look, Action stun, Action death)
		{
			FleshBurn fleshBurn = go.AddComponent<FleshBurn>();
			fleshBurn.what = what;
			fleshBurn.hp = (fleshBurn.max = Mathf.Max(1f, health));
			fleshBurn.look = look;
			fleshBurn.stun = stun;
			fleshBurn.death = death;
			BuildingEntity component = go.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
			return fleshBurn;
		}

		// Token: 0x060001B4 RID: 436 RVA: 0x00025884 File Offset: 0x00023A84
		private static bool Lit(Item it)
		{
			return it && ((it.id == "torch" && it.condition > 0f) || (it.id == "campfire" && it.condition > 0f));
		}

		// Token: 0x060001B5 RID: 437 RVA: 0x000258E0 File Offset: 0x00023AE0
		public static bool Douse(Body b)
		{
			if (!b)
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < 2; i++)
			{
				if (b.HoldingItem(i))
				{
					Item item = b.GetItem(i);
					if (item != null && item.id == "torch" && item.condition > 0f)
					{
						item.condition = 0f;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				return false;
			}
			Sound.Play("Scald", b.transform.position, false, true, b.transform, 0.8f, 0.6f, false, false);
			Sound.Play("goo", b.transform.position, false, true, b.transform, 0.7f, 1.3f, false, false);
			if (b.alive)
			{
				Relief.Say(b, "torch_doused");
			}
			Plugin.Log.LogInfo("The slime put out the torch in her hand");
			return true;
		}

		// Token: 0x060001B6 RID: 438 RVA: 0x000259CC File Offset: 0x00023BCC
		public static bool LitTorchInHand(Body b)
		{
			Item item;
			return b && Burning.TorchInHand(b, out item);
		}

		// Token: 0x060001B7 RID: 439 RVA: 0x000259EC File Offset: 0x00023BEC
		private static bool TorchInHand(Body b, out Item torch)
		{
			torch = null;
			for (int i = 0; i < 2; i++)
			{
				if (b.HoldingItem(i))
				{
					Item item = b.GetItem(i);
					if (item != null && item.id == "torch" && item.condition > 0f)
					{
						torch = item;
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x060001B8 RID: 440 RVA: 0x00025A44 File Offset: 0x00023C44
		public static void Hit(FleshBurn f, float damage)
		{
			if (!f || f.dead)
			{
				return;
			}
			BuildingEntity component = f.GetComponent<BuildingEntity>();
			if (component)
			{
				component.health = 100000f;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			Item item;
			if (body && Burning.TorchInHand(body, out item) && Vector2.Distance(item.transform.position, f.Center) < f.Reach + 2.5f)
			{
				if (f.douses)
				{
					Burning.Douse(body);
					if (f.hitsHurt)
					{
						Burning.Damage(f, damage, "a hit", false);
					}
					return;
				}
				Burning.Damage(f, Plugin.FireTorchHit.Value * f.fireScale, "a lit torch", true);
				return;
			}
			else
			{
				if (f.hitsHurt)
				{
					Burning.Damage(f, damage, "a hit", false);
					return;
				}
				Action stun = f.stun;
				if (stun == null)
				{
					return;
				}
				stun();
				return;
			}
		}

		// Token: 0x060001B9 RID: 441 RVA: 0x00025B3C File Offset: 0x00023D3C
		public static void Damage(FleshBurn f, float amount, string why, bool fire = true)
		{
			if (!f || f.dead || amount <= 0f)
			{
				return;
			}
			f.hp -= amount;
			if (fire)
			{
				f.Scorch();
				Sound.Play("Scald", f.transform.position, false, true, null, 0.6f, Random.Range(0.8f, 1.1f), false, false);
			}
			if (f.hp > 0f)
			{
				Plugin.Log.LogInfo(string.Format("The {0} {1} ({2}, {3:0}): {4:0}/{5:0} left", new object[]
				{
					f.what,
					fire ? "burns" : "takes",
					why,
					amount,
					f.hp,
					f.max
				}));
				return;
			}
			f.dead = true;
			Plugin.Log.LogInfo(string.Concat(new string[] { "The ", f.what, " is dead (", why, ")" }));
			Sound.Play("gore", f.transform.position, false, true, null, 1f, 1f, false, false);
			Action death = f.death;
			if (death == null)
			{
				return;
			}
			death();
		}

		// Token: 0x060001BA RID: 442 RVA: 0x00025C94 File Offset: 0x00023E94
		public static void Blast(Vector2 at, float range)
		{
			float num = Mathf.Max(4f, range) + 1.5f;
			foreach (FleshBurn fleshBurn in Burning.All.ToArray())
			{
				if (fleshBurn && !fleshBurn.dead && fleshBurn.isActiveAndEnabled)
				{
					float num2 = Mathf.Max(0f, Vector2.Distance(fleshBurn.Center, at) - fleshBurn.Reach * 0.5f);
					if (num2 <= num)
					{
						Burning.Damage(fleshBurn, Plugin.FireBlast.Value * (1f - 0.6f * num2 / num), "a blast", true);
					}
				}
			}
		}

		// Token: 0x060001BB RID: 443 RVA: 0x00025D3C File Offset: 0x00023F3C
		public static void Tick(float dt)
		{
			if (Burning.All.Count == 0)
			{
				return;
			}
			if ((Burning.scanTimer -= dt) <= 0f)
			{
				Burning.scanTimer = 2f;
				Burning.fires.Clear();
				foreach (Item item in Object.FindObjectsOfType<Item>())
				{
					if (item.id == "torch" || item.id == "campfire")
					{
						Burning.fires.Add(item);
					}
				}
			}
			if ((Burning.burnTimer -= dt) > 0f)
			{
				return;
			}
			Burning.burnTimer = 0.5f;
			float value = Plugin.FireReach.Value;
			foreach (Item item2 in Burning.fires)
			{
				if (Burning.Lit(item2) && !item2.ParentContainer())
				{
					Vector2 vector = item2.transform.position;
					foreach (FleshBurn fleshBurn in Burning.All.ToArray())
					{
						if (fleshBurn && !fleshBurn.dead && fleshBurn.isActiveAndEnabled && Vector2.Distance(fleshBurn.Center, vector) < value + fleshBurn.Reach)
						{
							Burning.Damage(fleshBurn, Plugin.FireBurn.Value * 0.5f * fleshBurn.fireScale, (item2.id == "campfire") ? "a campfire" : "a torch", true);
						}
					}
				}
			}
		}

		// Token: 0x040001C7 RID: 455
		public static readonly List<FleshBurn> All = new List<FleshBurn>();

		// Token: 0x040001C8 RID: 456
		private static readonly List<Item> fires = new List<Item>();

		// Token: 0x040001C9 RID: 457
		private static float scanTimer;

		// Token: 0x040001CA RID: 458
		private static float burnTimer;
	}
}
