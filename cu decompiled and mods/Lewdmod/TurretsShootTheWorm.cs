using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200013B RID: 315
	[HarmonyPatch(typeof(TurretScript), "Update")]
	internal static class TurretsShootTheWorm
	{
		// Token: 0x060007AC RID: 1964 RVA: 0x00076050 File Offset: 0x00074250
		private static void Postfix(TurretScript __instance)
		{
			if (!__instance || Worm.Beasts.Count == 0)
			{
				return;
			}
			WormBeast wormBeast;
			if (TurretsShootTheWorm.aimedAt.TryGetValue(__instance, out wormBeast) && __instance.timeSinceFired < 0.25f)
			{
				TurretsShootTheWorm.aimedAt.Remove(__instance);
				if (wormBeast)
				{
					wormBeast.Provoke(__instance);
				}
			}
			if (__instance.didBeep || !__instance.barrel)
			{
				return;
			}
			float num;
			if (TurretsShootTheWorm.scan.TryGetValue(__instance, out num) && Time.time < num)
			{
				return;
			}
			TurretsShootTheWorm.scan[__instance] = Time.time + 0.3f;
			BuildingEntity component = __instance.GetComponent<BuildingEntity>();
			if (component && component.health < 350f)
			{
				return;
			}
			Vector2 vector = __instance.barrel.position;
			Vector2 vector2 = __instance.transform.right * __instance.transform.localScale.x;
			foreach (RaycastHit2D raycastHit2D in from h in Physics2D.RaycastAll(vector, vector2, 80f, LayerMask.GetMask(new string[] { "Ground", "Descriptor" }))
				orderby h.distance
				select h)
			{
				if (raycastHit2D.transform && !(raycastHit2D.transform == __instance.transform))
				{
					if (raycastHit2D.transform.gameObject.layer == 6)
					{
						break;
					}
					WormHitRelay component2 = raycastHit2D.collider.GetComponent<WormHitRelay>();
					if (component2 && component2.worm)
					{
						Sound.Play("turretsee", __instance.transform.position, true, false, null, 1f, 1f, false, false);
						__instance.didBeep = true;
						Traverse.Create(__instance).Field("beepTime").SetValue(0f);
						TurretsShootTheWorm.aimedAt[__instance] = component2.worm;
						Plugin.Log.LogInfo(string.Format("A turret at {0} sees the giant worm across its line of fire and shoots", __instance.transform.position));
						break;
					}
				}
			}
		}

		// Token: 0x040007D9 RID: 2009
		private static readonly Dictionary<TurretScript, float> scan = new Dictionary<TurretScript, float>();

		// Token: 0x040007DA RID: 2010
		private static readonly Dictionary<TurretScript, WormBeast> aimedAt = new Dictionary<TurretScript, WormBeast>();
	}
}
