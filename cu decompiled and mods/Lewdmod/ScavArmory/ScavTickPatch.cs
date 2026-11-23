using System;
using System.Linq;
using HarmonyLib;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000047 RID: 71
	[HarmonyPatch(typeof(Body), "Update")]
	internal static class ScavTickPatch
	{
		// Token: 0x0600016B RID: 363 RVA: 0x00018C24 File Offset: 0x00016E24
		private static void Postfix(Body __instance)
		{
			if ((!Plugin.ScavEnabled.Value && !Plugin.HostsEnabled.Value) || !PlayerCamera.main || PlayerCamera.main.body != __instance)
			{
				return;
			}
			Scavengers.Update(__instance);
			Camps.Update(__instance, Time.deltaTime);
			Ultimatum.Tick(__instance);
			if (Plugin.DebugOn && Plugin.ScavEnabled.Value && DebugBinds.Down(DebugBinds.RaiderNearby))
			{
				Scavengers.DebugSpawn(__instance);
			}
			if (Plugin.DebugOn && Plugin.HostsEnabled.Value && DebugBinds.Down(DebugBinds.SlimeHosts))
			{
				Vector2 vector = __instance.transform.position + new Vector2((__instance.isRight ? 1f : (-1f)) * 7f, -2.5f);
				Scavengers.HostPack(vector);
				Plugin.Log.LogInfo(string.Format("Debug: slime host at {0}", vector));
			}
			if (Plugin.DebugOn && Time.time > ScavTickPatch.nextTrace && Scavengers.All.Count > 0 && Camera.main)
			{
				ScavTickPatch.nextTrace = Time.time + 2f;
				Scavenger scavenger = (from x in Scavengers.All
					where x
					orderby Vector2.Distance(x.pos, __instance.transform.position)
					select x).FirstOrDefault<Scavenger>();
				if (scavenger && Vector2.Distance(scavenger.pos, __instance.transform.position) < 30f)
				{
					Vector3 vector2 = Camera.main.WorldToScreenPoint(scavenger.transform.position);
					Plugin.Log.LogInfo(string.Format("Trace: scavenger {0} {1} hp {2:0} at screen {3:0},{4:0}, her screen {5:0}", new object[]
					{
						scavenger.mode,
						scavenger.weapon.Id,
						scavenger.hit ? scavenger.hit.health : 0f,
						vector2.x,
						(float)Screen.height - vector2.y,
						Camera.main.WorldToScreenPoint(__instance.transform.position).x
					}));
				}
			}
		}

		// Token: 0x0400017E RID: 382
		private static float nextTrace;
	}
}
