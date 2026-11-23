using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000EA RID: 234
	internal static class Prof
	{
		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x06000537 RID: 1335 RVA: 0x00056B23 File Offset: 0x00054D23
		public static bool On
		{
			get
			{
				return Time.unscaledTime < Prof.Until;
			}
		}

		// Token: 0x06000538 RID: 1336 RVA: 0x00056B34 File Offset: 0x00054D34
		public static void Start(float seconds)
		{
			Prof.Until = Time.unscaledTime + seconds;
			if (Prof.instrumented)
			{
				return;
			}
			Prof.instrumented = true;
			Harmony harmony = new Harmony("jens.lewdstatus.prof");
			HarmonyMethod harmonyMethod = new HarmonyMethod(typeof(Prof), "T0", null);
			HarmonyMethod harmonyMethod2 = new HarmonyMethod(typeof(Prof), "T1", null);
			int num = 0;
			foreach (Type type in typeof(Prof).Assembly.GetTypes())
			{
				if (!(type == typeof(Prof)) && !type.ContainsGenericParameters)
				{
					bool flag = type.GetCustomAttributes(typeof(HarmonyPatch), true).Length != 0;
					bool flag2 = typeof(MonoBehaviour).IsAssignableFrom(type);
					foreach (MethodInfo methodInfo in type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
					{
						if (((flag && methodInfo.IsStatic && (methodInfo.Name == "Prefix" || methodInfo.Name == "Postfix")) || (flag2 && !methodInfo.IsStatic && (methodInfo.Name == "Update" || methodInfo.Name == "FixedUpdate" || methodInfo.Name == "LateUpdate"))) && !methodInfo.IsGenericMethod)
						{
							try
							{
								harmony.Patch(methodInfo, harmonyMethod, harmonyMethod2, null, null, null);
								num++;
							}
							catch (Exception ex)
							{
								Plugin.Log.LogWarning(string.Concat(new string[] { "[Prof] can't time ", type.Name, ".", methodInfo.Name, ": ", ex.Message }));
							}
						}
					}
				}
			}
			harmony.Patch(AccessTools.Method(typeof(Body), "FixedUpdate", null, null), new HarmonyMethod(typeof(Prof), "Step", null), null, null, null, null);
			Plugin.Log.LogInfo(string.Format("[Prof] timing {0} methods", num));
		}

		// Token: 0x06000539 RID: 1337 RVA: 0x00056D94 File Offset: 0x00054F94
		private static void T0(out long __state)
		{
			__state = Stopwatch.GetTimestamp();
		}

		// Token: 0x0600053A RID: 1338 RVA: 0x00056DA0 File Offset: 0x00054FA0
		private static void T1(long __state, MethodBase __originalMethod)
		{
			if (!Prof.On)
			{
				return;
			}
			double num = (double)(Stopwatch.GetTimestamp() - __state) * 1000.0 / (double)Stopwatch.Frequency;
			string text = __originalMethod.DeclaringType.Name + "." + __originalMethod.Name;
			double num2;
			Prof.spent[text] = (Prof.spent.TryGetValue(text, out num2) ? num2 : 0.0) + num;
			int num3;
			Prof.calls[text] = (Prof.calls.TryGetValue(text, out num3) ? num3 : 0) + 1;
		}

		// Token: 0x0600053B RID: 1339 RVA: 0x00056E33 File Offset: 0x00055033
		private static void Step()
		{
			Prof.fixedSteps++;
		}

		// Token: 0x0600053C RID: 1340 RVA: 0x00056E44 File Offset: 0x00055044
		public static void Begin()
		{
			if (!Prof.On)
			{
				return;
			}
			float unscaledDeltaTime = Time.unscaledDeltaTime;
			if (unscaledDeltaTime > 0.08f)
			{
				double num = Prof.spent.Values.Sum();
				string text = string.Join("; ", from kv in Prof.spent.OrderByDescending<KeyValuePair<string, double>, double>((KeyValuePair<string, double> kv) => kv.Value).Take<KeyValuePair<string, double>>(6)
					select string.Format("{0} {1:0.0}ms x{2}", kv.Key, kv.Value, Prof.calls[kv.Key]));
				Plugin.Log.LogInfo(string.Format("[Prof] hitch {0:0} ms at x{1:0.#}, {2} physics steps, mod {3:0.0} ms: {4}", new object[]
				{
					unscaledDeltaTime * 1000f,
					Time.timeScale,
					Prof.fixedSteps,
					num,
					text
				}));
			}
			Prof.spent.Clear();
			Prof.calls.Clear();
			Prof.fixedSteps = 0;
		}

		// Token: 0x0400061A RID: 1562
		public static float Until = -1f;

		// Token: 0x0400061B RID: 1563
		private static readonly Dictionary<string, double> spent = new Dictionary<string, double>();

		// Token: 0x0400061C RID: 1564
		private static readonly Dictionary<string, int> calls = new Dictionary<string, int>();

		// Token: 0x0400061D RID: 1565
		private static int fixedSteps;

		// Token: 0x0400061E RID: 1566
		private static bool instrumented;
	}
}
