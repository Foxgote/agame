using System;
using System.Reflection;
using HarmonyLib;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200002B RID: 43
	internal static class LewdLink
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000099 RID: 153 RVA: 0x0000AA00 File Offset: 0x00008C00
		public static bool Present
		{
			get
			{
				LewdLink.Look();
				return LewdLink.orgasm != null;
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x0000AA14 File Offset: 0x00008C14
		private static void Look()
		{
			if (LewdLink.looked)
			{
				return;
			}
			LewdLink.looked = true;
			Type type = AccessTools.TypeByName("LewdStatus.Plugin");
			if (type == null)
			{
				return;
			}
			LewdLink.orgasm = AccessTools.Method(type, "Orgasm", new Type[]
			{
				typeof(Body),
				typeof(bool)
			}, null);
			LewdLink.stateField = AccessTools.Field(type, "S");
			Type type2 = AccessTools.TypeByName("LewdStatus.State");
			if (type2 != null)
			{
				LewdLink.arousal = AccessTools.Field(type2, "arousal");
				LewdLink.orgasmTimer = AccessTools.Field(type2, "orgasmTimer");
			}
			LewdLink.fill = AccessTools.Method(AccessTools.TypeByName("LewdStatus.Womb"), "Fill", new Type[]
			{
				typeof(Body),
				typeof(float)
			}, null);
			LewdLink.semen = AccessTools.Method(AccessTools.TypeByName("LewdStatus.Semen"), "Add", new Type[]
			{
				typeof(Body),
				typeof(float)
			}, null);
			LewdLink.sfx = AccessTools.Method(AccessTools.TypeByName("LewdStatus.Sfx"), "Play", new Type[]
			{
				typeof(Body),
				typeof(float),
				typeof(string[])
			}, null);
			LewdLink.say = AccessTools.Method(AccessTools.TypeByName("LewdStatus.Relief"), "Say", new Type[]
			{
				typeof(Body),
				typeof(string)
			}, null);
			Plugin.Log.LogInfo("LEWD Casualties UNKNOWN " + ((LewdLink.orgasm != null) ? "found: scavengers can use her" : "not found: scavengers only rob"));
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600009B RID: 155 RVA: 0x0000ABDB File Offset: 0x00008DDB
		private static object S
		{
			get
			{
				FieldInfo fieldInfo = LewdLink.stateField;
				if (fieldInfo == null)
				{
					return null;
				}
				return fieldInfo.GetValue(null);
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x0600009C RID: 156 RVA: 0x0000ABF0 File Offset: 0x00008DF0
		// (set) Token: 0x0600009D RID: 157 RVA: 0x0000AC30 File Offset: 0x00008E30
		public static float Arousal
		{
			get
			{
				LewdLink.Look();
				object s = LewdLink.S;
				if (s == null || !(LewdLink.arousal != null))
				{
					return 0f;
				}
				return (float)LewdLink.arousal.GetValue(s);
			}
			set
			{
				LewdLink.Look();
				object s = LewdLink.S;
				if (s != null && LewdLink.arousal != null)
				{
					LewdLink.arousal.SetValue(s, Mathf.Clamp(value, 0f, 100f));
				}
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x0600009E RID: 158 RVA: 0x0000AC78 File Offset: 0x00008E78
		public static float OrgasmTimer
		{
			get
			{
				LewdLink.Look();
				object s = LewdLink.S;
				if (s == null || !(LewdLink.orgasmTimer != null))
				{
					return 0f;
				}
				return (float)LewdLink.orgasmTimer.GetValue(s);
			}
		}

		// Token: 0x0600009F RID: 159 RVA: 0x0000ACB6 File Offset: 0x00008EB6
		public static void Orgasm(Body b)
		{
			LewdLink.Look();
			MethodInfo methodInfo = LewdLink.orgasm;
			if (methodInfo == null)
			{
				return;
			}
			methodInfo.Invoke(null, new object[] { b, false });
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x0000ACE1 File Offset: 0x00008EE1
		public static void Fill(Body b, float amount)
		{
			LewdLink.Look();
			MethodInfo methodInfo = LewdLink.fill;
			if (methodInfo == null)
			{
				return;
			}
			methodInfo.Invoke(null, new object[] { b, amount });
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x0000AD0C File Offset: 0x00008F0C
		public static void Semen(Body b, float amount)
		{
			LewdLink.Look();
			MethodInfo methodInfo = LewdLink.semen;
			if (methodInfo == null)
			{
				return;
			}
			methodInfo.Invoke(null, new object[] { b, amount });
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000AD37 File Offset: 0x00008F37
		public static void Sound(Body b, float vol, params string[] sets)
		{
			LewdLink.Look();
			MethodInfo methodInfo = LewdLink.sfx;
			if (methodInfo == null)
			{
				return;
			}
			methodInfo.Invoke(null, new object[] { b, vol, sets });
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x0000AD66 File Offset: 0x00008F66
		public static void Say(Body b, string key)
		{
			LewdLink.Look();
			MethodInfo methodInfo = LewdLink.say;
			if (methodInfo == null)
			{
				return;
			}
			methodInfo.Invoke(null, new object[] { b, key });
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x0000AD8C File Offset: 0x00008F8C
		private static bool LookSuit()
		{
			if (LewdLink.suit != null)
			{
				return LewdLink.suitAttach != null;
			}
			LewdLink.suit = AccessTools.TypeByName("LewdStatus.Suit") ?? typeof(object);
			LewdLink.suitStage = AccessTools.Property(LewdLink.suit, "Stage");
			LewdLink.suitSlime = AccessTools.Property(LewdLink.suit, "Slime");
			LewdLink.suitAttach = AccessTools.Method(LewdLink.suit, "Attach", new Type[]
			{
				typeof(Body),
				typeof(int)
			}, null);
			LewdLink.suitNext = AccessTools.Method(LewdLink.suit, "ForceNextStage", new Type[] { typeof(Body) }, null);
			return LewdLink.suitAttach != null;
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x060000A5 RID: 165 RVA: 0x0000AE5F File Offset: 0x0000905F
		public static bool CanInfect
		{
			get
			{
				return LewdLink.LookSuit();
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x060000A6 RID: 166 RVA: 0x0000AE66 File Offset: 0x00009066
		public static int SuitStage
		{
			get
			{
				if (!LewdLink.LookSuit() || !(LewdLink.suitStage != null))
				{
					return 0;
				}
				return (int)LewdLink.suitStage.GetValue(null);
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000AE90 File Offset: 0x00009090
		public static LewdLink.Infection Infect(Body b)
		{
			if (!LewdSettings.ParasitesOn)
			{
				return LewdLink.Infection.Blocked;
			}
			switch (Suit.HostDose(b))
			{
			case 0:
				return LewdLink.Infection.Attached;
			case 1:
				return LewdLink.Infection.Grew;
			case 2:
				return LewdLink.Infection.Full;
			default:
				return LewdLink.Infection.Blocked;
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x0000AEC8 File Offset: 0x000090C8
		public static void Pour(Vector2 at, int cells)
		{
			SlimeFluid.Pour(at, cells, null);
		}

		// Token: 0x040000BE RID: 190
		private static bool looked;

		// Token: 0x040000BF RID: 191
		private static MethodInfo orgasm;

		// Token: 0x040000C0 RID: 192
		private static MethodInfo fill;

		// Token: 0x040000C1 RID: 193
		private static MethodInfo semen;

		// Token: 0x040000C2 RID: 194
		private static MethodInfo sfx;

		// Token: 0x040000C3 RID: 195
		private static MethodInfo say;

		// Token: 0x040000C4 RID: 196
		private static FieldInfo stateField;

		// Token: 0x040000C5 RID: 197
		private static FieldInfo arousal;

		// Token: 0x040000C6 RID: 198
		private static FieldInfo orgasmTimer;

		// Token: 0x040000C7 RID: 199
		private static Type suit;

		// Token: 0x040000C8 RID: 200
		private static PropertyInfo suitStage;

		// Token: 0x040000C9 RID: 201
		private static PropertyInfo suitSlime;

		// Token: 0x040000CA RID: 202
		private static MethodInfo suitAttach;

		// Token: 0x040000CB RID: 203
		private static MethodInfo suitNext;

		// Token: 0x02000158 RID: 344
		public enum Infection
		{
			// Token: 0x0400082A RID: 2090
			Attached,
			// Token: 0x0400082B RID: 2091
			Grew,
			// Token: 0x0400082C RID: 2092
			Full,
			// Token: 0x0400082D RID: 2093
			Blocked
		}
	}
}
