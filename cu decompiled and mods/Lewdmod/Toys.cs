using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000097 RID: 151
	internal static class Toys
	{
		// Token: 0x06000302 RID: 770 RVA: 0x00033CA8 File Offset: 0x00031EA8
		static Toys()
		{
			foreach (ValueTuple<string, string, int, int, float> valueTuple in Toys.Shapes)
			{
				foreach (ValueTuple<string, Color32> valueTuple2 in Toys.Colors)
				{
					Toys.All.Add(new ToyDef
					{
						Id = string.Concat(new string[] { "lewd_", valueTuple.Item1, "_", valueTuple.Item2, "_", valueTuple2.Item1 }),
						Kind = valueTuple.Item1,
						Size = valueTuple.Item2,
						Color = valueTuple2.Item1,
						Tint = valueTuple2.Item2,
						W = valueTuple.Item3,
						H = valueTuple.Item4,
						Boost = valueTuple.Item5
					});
				}
			}
		}

		// Token: 0x06000303 RID: 771 RVA: 0x00033FD8 File Offset: 0x000321D8
		public static ToyDef ById(string id)
		{
			if (id == null || !id.StartsWith("lewd_"))
			{
				return null;
			}
			return Toys.All.FirstOrDefault<ToyDef>((ToyDef t) => t.Id == id);
		}

		// Token: 0x06000304 RID: 772 RVA: 0x00034024 File Offset: 0x00032224
		public static string Name(ToyDef t)
		{
			string gender = t.Gender;
			return string.Format(Plugin.T("toy_" + t.Kind + "_name"), Plugin.T("toycol_" + t.Color + "_" + gender), Plugin.T("toysize_" + t.Size + "_" + gender));
		}

		// Token: 0x06000305 RID: 773 RVA: 0x00034090 File Offset: 0x00032290
		public static void Register()
		{
			ItemInfo itemInfo;
			if (Item.GlobalItems == null || !Item.GlobalItems.TryGetValue("rag", out itemInfo))
			{
				return;
			}
			foreach (ToyDef toyDef in Toys.All)
			{
				if (!Item.GlobalItems.ContainsKey(toyDef.Id))
				{
					ItemInfo itemInfo2 = (ItemInfo)AccessTools.Method(typeof(object), "MemberwiseClone", null, null).Invoke(itemInfo, null);
					itemInfo2.category = "utility";
					itemInfo2.usable = true;
					itemInfo2.usableOnLimb = false;
					itemInfo2.useLimbAction = null;
					ItemInfo itemInfo3 = itemInfo2;
					ItemInfo.Use use;
					if ((use = Toys.<>O.<0>__Use) == null)
					{
						use = (Toys.<>O.<0>__Use = new ItemInfo.Use(Toys.Use));
					}
					itemInfo3.useAction = use;
					itemInfo2.destroyAtZeroCondition = false;
					itemInfo2.combineable = false;
					itemInfo2.scaleWeightWithCondition = false;
					itemInfo2.weight = 0.15f + (float)toyDef.H * 0.03f;
					itemInfo2.value = 6 + toyDef.H;
					itemInfo2.decayMinutes = 0f;
					itemInfo2.rotSpeed = 0f;
					itemInfo2.qualities = new List<CraftingQuality>();
					itemInfo2.tags = "";
					itemInfo2.SetTags();
					itemInfo2.fullName = Toys.Name(toyDef);
					itemInfo2.description = Plugin.T("toy_" + toyDef.Kind + "_desc");
					if (toyDef.Kind == "vibe")
					{
						itemInfo2.usable = false;
						itemInfo2.useAction = null;
						itemInfo2.wearable = true;
						itemInfo2.wearSlotId = "dd_vibe";
						itemInfo2.desiredWearLimb = "DownTorso";
						itemInfo2.wearableVisualOffset = -20;
						itemInfo2.wearableArmor = 0f;
						itemInfo2.wearableIsolation = 0f;
					}
					if (toyDef.Plug)
					{
						itemInfo2.usable = false;
						itemInfo2.useAction = null;
						itemInfo2.wearable = true;
						itemInfo2.wearSlotId = "dd_plug";
						itemInfo2.desiredWearLimb = "DownTorso";
						itemInfo2.wearableVisualOffset = -20;
						itemInfo2.wearableArmor = 0f;
						itemInfo2.wearableIsolation = 0f;
						itemInfo2.description = Plugin.T((toyDef.Kind == "plug") ? "toy_plug_wear_desc" : ("toy_" + toyDef.Kind + "_desc"));
					}
					Item.GlobalItems[toyDef.Id] = itemInfo2;
				}
			}
		}

		// Token: 0x06000306 RID: 774 RVA: 0x00034314 File Offset: 0x00032514
		public static GameObject Template(ToyDef t)
		{
			if (t.Template)
			{
				return t.Template;
			}
			GameObject gameObject = Resources.Load("rag") as GameObject;
			if (!gameObject)
			{
				return null;
			}
			if (!Toys.holder)
			{
				Toys.holder = new GameObject("LewdToyTemplates");
				Toys.holder.SetActive(false);
				Object.DontDestroyOnLoad(Toys.holder);
			}
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject, Toys.holder.transform);
			gameObject2.name = t.Id;
			gameObject2.GetComponent<Item>().id = t.Id;
			Container[] componentsInChildren = gameObject2.GetComponentsInChildren<Container>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Object.DestroyImmediate(componentsInChildren[i]);
			}
			SpriteRenderer component = gameObject2.GetComponent<SpriteRenderer>();
			Sprite sprite = Resources.FindObjectsOfTypeAll<Sprite>().FirstOrDefault<Sprite>((Sprite s) => s.name == "experimentUpTorso");
			float num = (sprite ? sprite.pixelsPerUnit : ((component && component.sprite) ? component.sprite.pixelsPerUnit : 16f));
			Texture2D texture2D = Toys.Paint(t);
			Sprite sprite2 = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), num);
			sprite2.name = t.Id;
			if (component)
			{
				component.sprite = sprite2;
				component.color = Color.white;
			}
			Vector2 vector;
			vector..ctor((float)texture2D.width / num, (float)texture2D.height / num);
			foreach (Collider2D collider2D in gameObject2.GetComponents<Collider2D>())
			{
				if (!(collider2D is BoxCollider2D))
				{
					Object.DestroyImmediate(collider2D);
				}
			}
			BoxCollider2D boxCollider2D = gameObject2.GetComponent<BoxCollider2D>() ?? gameObject2.AddComponent<BoxCollider2D>();
			boxCollider2D.size = vector;
			boxCollider2D.offset = Vector2.zero;
			t.Template = gameObject2;
			return gameObject2;
		}

		// Token: 0x06000307 RID: 775 RVA: 0x00034520 File Offset: 0x00032720
		private static Texture2D Paint(ToyDef t)
		{
			int num = t.W + 2;
			int h = t.H;
			Texture2D texture2D = new Texture2D(num, h, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			Color32[] array = new Color32[num * h];
			Color32 tint = t.Tint;
			Color32 color = Toys.Mul(tint, 0.7f);
			Color32 color2 = Toys.Mul(tint, 1.25f);
			int num2 = 1;
			int num3 = 1 + t.W - 1;
			for (int i = 0; i < h; i++)
			{
				for (int j = 0; j < num; j++)
				{
					bool flag;
					if (t.Kind == "beads")
					{
						flag = ((i % 3 != 2) ? (j >= num2 && j <= num3) : (j == num / 2));
					}
					else if (t.Kind == "vibe")
					{
						flag = ((i == h - 1) ? (j > num2 && j < num3) : ((i == 0) ? (j == num / 2) : (j >= num2 && j <= num3)));
					}
					else if (t.Kind == "dildo")
					{
						if (i == 0)
						{
							flag = true;
						}
						else if (i == h - 1)
						{
							flag = (j > num2 && j < num3) || (t.W <= 2 && j >= num2 && j <= num3);
						}
						else
						{
							flag = j >= num2 && j <= num3;
						}
					}
					else
					{
						int num4 = 2;
						if (i == 0)
						{
							flag = true;
						}
						else if (i < num4)
						{
							flag = j == num / 2 || j == (num - 1) / 2;
						}
						else if (i == h - 1)
						{
							flag = j > num2 && j < num3;
						}
						else
						{
							flag = j >= num2 && j <= num3;
						}
					}
					if (flag)
					{
						Color32 color3 = tint;
						if (i == 0)
						{
							color3 = color;
						}
						else if (j == num2)
						{
							color3 = color2;
						}
						else if (j == num3)
						{
							color3 = color;
						}
						array[i * num + j] = color3;
					}
				}
			}
			texture2D.SetPixels32(array);
			texture2D.Apply();
			return texture2D;
		}

		// Token: 0x06000308 RID: 776 RVA: 0x00034750 File Offset: 0x00032950
		private static Color32 Mul(Color32 c, float k)
		{
			return new Color32((byte)Mathf.Clamp((float)c.r * k, 0f, 255f), (byte)Mathf.Clamp((float)c.g * k, 0f, 255f), (byte)Mathf.Clamp((float)c.b * k, 0f, 255f), byte.MaxValue);
		}

		// Token: 0x06000309 RID: 777 RVA: 0x000347B4 File Offset: 0x000329B4
		public static void DebugSpawn(Vector2 at)
		{
			ToyDef toyDef = Toys.All[Random.Range(0, Toys.All.Count)];
			GameObject gameObject = Toys.Template(toyDef);
			if (gameObject)
			{
				Object.Instantiate<GameObject>(gameObject, at, Quaternion.identity);
			}
			Outfits.SpawnAt(Outfits.Blanket, at + Vector2.up * 0.5f);
			GameObject gameObject2 = Suppressant.Template();
			if (gameObject2)
			{
				Object.Instantiate<GameObject>(gameObject2, at + Vector2.up, Quaternion.identity);
			}
			GameObject gameObject3 = Scalpel.Template();
			if (gameObject3)
			{
				Object.Instantiate<GameObject>(gameObject3, at + Vector2.up * 1.5f, Quaternion.identity);
			}
			Plugin.Log.LogInfo(string.Format("Debug: a {0} and a blanket at {1}", toyDef.Id, at));
		}

		// Token: 0x0600030A RID: 778 RVA: 0x0003489C File Offset: 0x00032A9C
		public static void MaybeDrop(BuildingEntity e)
		{
			if (Random.value >= Plugin.ToyCrateChance.Value || Toys.All.Count == 0)
			{
				return;
			}
			ToyDef toyDef = Toys.All[Random.Range(0, Toys.All.Count)];
			GameObject gameObject = Toys.Template(toyDef);
			if (!gameObject)
			{
				return;
			}
			Rigidbody2D rigidbody2D;
			if (Object.Instantiate<GameObject>(gameObject, e.transform.position, Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))).TryGetComponent<Rigidbody2D>(ref rigidbody2D))
			{
				rigidbody2D.velocity = new Vector2(Random.Range(-6f, 6f), Random.Range(2f, 6f));
			}
			Plugin.Log.LogInfo("Crate broke: a " + toyDef.Id + " was in it");
		}

		// Token: 0x0600030B RID: 779 RVA: 0x00034974 File Offset: 0x00032B74
		public static bool AllowPickUp(Body b, Item item)
		{
			if (item && Devices.ById(item.id) != null)
			{
				return Devices.AllowPickUp(b, item);
			}
			if (!item || !LewdSettings.On || Toys.ById(item.id) == null || !PlayerCamera.main || PlayerCamera.main.body != b)
			{
				return true;
			}
			if (Time.timeSinceLevelLoad < 5f || Toys.accepted.Contains(item.GetInstanceID()))
			{
				return true;
			}
			if (Plugin.S.lust < Plugin.ToyMinLust.Value && Plugin.S.arousal < Plugin.ToyMinLust.Value)
			{
				if (Time.time - Toys.lastRefuse > 4f)
				{
					Toys.lastRefuse = Time.time;
					Relief.Say(b, "toy_refuse");
					PlayerCamera.main.PlayUISound(6, 1f);
				}
				return false;
			}
			Toys.accepted.Add(item.GetInstanceID());
			Relief.Say(b, "toy_accept");
			Plugin.Log.LogInfo(string.Format("She took the {0} (lust {1:0})", item.id, Plugin.S.lust));
			return true;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x00034AA8 File Offset: 0x00032CA8
		private static void Use(Body body, Item item)
		{
			ToyDef toyDef = Toys.ById(item ? item.id : null);
			if (toyDef == null || !LewdSettings.On || !PlayerCamera.main || PlayerCamera.main.body != body)
			{
				return;
			}
			if (Plugin.S.lust < Plugin.ToyMinLust.Value && Plugin.S.arousal < Plugin.ToyMinLust.Value)
			{
				Relief.Say(body, "toy_nouse");
				return;
			}
			if (Devices.BeltOn(body))
			{
				Relief.Say(body, "toy_belt");
				return;
			}
			Relief.StartWithToy(body, toyDef.Boost, toyDef.Plug);
		}

		// Token: 0x040002D1 RID: 721
		[TupleElementNames(new string[] { "id", "c" })]
		private static readonly ValueTuple<string, Color32>[] Colors = new ValueTuple<string, Color32>[]
		{
			new ValueTuple<string, Color32>("pink", new Color32(236, 112, 170, byte.MaxValue)),
			new ValueTuple<string, Color32>("violet", new Color32(150, 84, 200, byte.MaxValue)),
			new ValueTuple<string, Color32>("black", new Color32(46, 40, 50, byte.MaxValue)),
			new ValueTuple<string, Color32>("blue", new Color32(70, 130, 220, byte.MaxValue)),
			new ValueTuple<string, Color32>("red", new Color32(200, 44, 60, byte.MaxValue)),
			new ValueTuple<string, Color32>("flesh", new Color32(226, 168, 140, byte.MaxValue))
		};

		// Token: 0x040002D2 RID: 722
		[TupleElementNames(new string[] { "kind", "size", "w", "h", "boost" })]
		private static readonly ValueTuple<string, string, int, int, float>[] Shapes = new ValueTuple<string, string, int, int, float>[]
		{
			new ValueTuple<string, string, int, int, float>("dildo", "s", 2, 6, 1.5f),
			new ValueTuple<string, string, int, int, float>("dildo", "m", 3, 8, 2f),
			new ValueTuple<string, string, int, int, float>("dildo", "l", 3, 11, 2.6f),
			new ValueTuple<string, string, int, int, float>("plug", "s", 3, 5, 1.6f),
			new ValueTuple<string, string, int, int, float>("plug", "l", 4, 7, 2.1f),
			new ValueTuple<string, string, int, int, float>("beads", "m", 3, 9, 2.3f),
			new ValueTuple<string, string, int, int, float>("vibe", "m", 3, 4, 1.8f),
			new ValueTuple<string, string, int, int, float>("tailplug", "m", 3, 6, 1.8f),
			new ValueTuple<string, string, int, int, float>("inflplug", "m", 4, 6, 2.2f)
		};

		// Token: 0x040002D3 RID: 723
		public static readonly List<ToyDef> All = new List<ToyDef>();

		// Token: 0x040002D4 RID: 724
		private static GameObject holder;

		// Token: 0x040002D5 RID: 725
		private static float lastRefuse = -99f;

		// Token: 0x040002D6 RID: 726
		private static readonly HashSet<int> accepted = new HashSet<int>();

		// Token: 0x020001BD RID: 445
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x04000993 RID: 2451
			public static ItemInfo.Use <0>__Use;
		}
	}
}
