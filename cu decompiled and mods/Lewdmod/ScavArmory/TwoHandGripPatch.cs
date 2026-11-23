using System;
using HarmonyLib;
using PoseKit;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000049 RID: 73
	[HarmonyPatch(typeof(Body), "HandleVisuals")]
	internal static class TwoHandGripPatch
	{
		// Token: 0x0600016D RID: 365 RVA: 0x00018F74 File Offset: 0x00017174
		private static bool IsLongGun(Item it)
		{
			if (!it || !it.GetComponent<GunScript>())
			{
				return false;
			}
			if (Array.IndexOf<string>(TwoHandGripPatch.Long, it.id) >= 0)
			{
				return true;
			}
			ArmDef armDef = Armory.ById(it.id);
			return armDef != null && Array.IndexOf<string>(TwoHandGripPatch.Long, armDef.Base) >= 0;
		}

		// Token: 0x0600016E RID: 366 RVA: 0x00018FD4 File Offset: 0x000171D4
		private static void Postfix(Body __instance)
		{
			try
			{
				if (__instance && __instance.alive && __instance.standing && !HerPuppet.Holds(__instance) && PlayerCamera.main && !(PlayerCamera.main.body != __instance))
				{
					int num = -1;
					bool flag = ReloadAnim.Active && __instance.HoldingItem(__instance.handSlot) && __instance.GetItem(__instance.handSlot) && ReloadAnim.Gun && __instance.GetItem(__instance.handSlot).gameObject == ReloadAnim.Gun.gameObject;
					if (flag)
					{
						num = __instance.handSlot;
					}
					else
					{
						for (int i = 0; i < 2; i++)
						{
							if (__instance.HoldingItem(i) && TwoHandGripPatch.IsLongGun(__instance.GetItem(i)))
							{
								num = i;
								break;
							}
						}
					}
					if (num >= 0 && !__instance.HoldingItem(1 - num))
					{
						Item item = __instance.GetItem(num);
						GunScript component = item.GetComponent<GunScript>();
						Limb componentInParent = item.GetComponentInParent<Limb>();
						if (componentInParent)
						{
							string text = (componentInParent.name.EndsWith("F") ? "B" : "F");
							Limb limb = __instance.LimbByName("UpArm" + text);
							Limb limb2 = __instance.LimbByName("DownArm" + text);
							Limb limb3 = __instance.LimbByName("Hand" + text);
							if (limb && limb2 && limb3 && !limb.dismembered && !limb2.dismembered && !limb3.dismembered && limb.joint && limb2.joint)
							{
								if (TwoHandGripPatch.IsLongGun(item))
								{
									string text2 = (componentInParent.name.EndsWith("F") ? "F" : "B");
									Limb limb4 = __instance.LimbByName("UpArm" + text2);
									Limb limb5 = __instance.LimbByName("DownArm" + text2);
									if (limb4 && limb5 && limb4.joint && limb5.joint && !limb4.dismembered && !limb5.dismembered)
									{
										Vector2 vector = limb4.transform.TransformPoint(limb4.joint.anchor);
										float z = item.transform.eulerAngles.z;
										TwoHandGripPatch.Reach(limb4, limb5, componentInParent, vector + (componentInParent.transform.position - vector) * 0.68f);
										float num2 = Mathf.DeltaAngle(item.transform.eulerAngles.z, z);
										componentInParent.transform.RotateAround(componentInParent.transform.position, Vector3.forward, num2);
									}
								}
								Vector2 vector2 = componentInParent.transform.position;
								Vector2 vector3 = (component.barrel ? component.barrel.position : (vector2 + item.transform.right));
								Vector2 vector4 = Vector2.Lerp(vector2, vector3, (item.id == "arm_mp9") ? 0.3f : 0.33f);
								if (flag)
								{
									Limb limb6 = __instance.LimbByName("DownTorso");
									Vector2 vector5 = (limb6 ? (limb6.transform.position + Vector2.down * 0.3f) : (vector2 + Vector2.down));
									Vector2 vector6;
									float num3;
									if (!ReloadAnim.Target(vector2, vector3, vector5, limb3.transform, out vector6, out num3))
									{
										return;
									}
									if (!TwoHandGripPatch.IsLongGun(item))
									{
										vector4 = limb3.transform.position;
									}
									vector4 = Vector2.Lerp(vector4, vector6, num3);
								}
								TwoHandGripPatch.Reach(limb, limb2, limb3, vector4);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (!TwoHandGripPatch.warned)
				{
					TwoHandGripPatch.warned = true;
					Plugin.Log.LogWarning("Two-hand grip: " + ex.Message);
				}
			}
		}

		// Token: 0x0600016F RID: 367 RVA: 0x00019450 File Offset: 0x00017650
		private static void Reach(Limb up, Limb down, Limb hand, Vector2 t)
		{
			Vector2 vector = up.transform.TransformPoint(up.joint.anchor);
			Vector2 vector2 = down.transform.TransformPoint(down.joint.anchor);
			Vector2 vector3 = hand.transform.position;
			float magnitude = (vector2 - vector).magnitude;
			float magnitude2 = (vector3 - vector2).magnitude;
			if (magnitude < 0.01f || magnitude2 < 0.01f)
			{
				return;
			}
			Vector2 vector4 = t - vector;
			float num = Mathf.Clamp(vector4.magnitude, Mathf.Abs(magnitude - magnitude2) + 0.01f, magnitude + magnitude2 - 0.01f);
			float num2 = Mathf.Atan2(vector4.y, vector4.x);
			float num3 = Mathf.Acos(Mathf.Clamp((magnitude * magnitude + num * num - magnitude2 * magnitude2) / (2f * magnitude * num), -1f, 1f));
			float num4 = num2 - num3;
			float num5 = num2 + num3;
			float num6 = (((Mathf.Sin(num4) <= Mathf.Sin(num5)) ? num4 : num5) - Mathf.Atan2(vector2.y - vector.y, vector2.x - vector.x)) * 57.29578f;
			Limb[] array = new Limb[] { up, down, hand };
			for (int i = 0; i < array.Length; i++)
			{
				array[i].transform.RotateAround(vector, Vector3.forward, num6);
			}
			vector2 = down.transform.TransformPoint(down.joint.anchor);
			Vector2 vector5 = hand.transform.position;
			Vector2 vector6 = vector + vector4.normalized * num;
			float num7 = Vector2.SignedAngle(vector5 - vector2, vector6 - vector2);
			array = new Limb[] { down, hand };
			for (int i = 0; i < array.Length; i++)
			{
				array[i].transform.RotateAround(vector2, Vector3.forward, num7);
			}
		}

		// Token: 0x0400017F RID: 383
		private static readonly string[] Long = new string[] { "rifle", "makeshiftrifle", "shotgun", "arm_mp9" };

		// Token: 0x04000180 RID: 384
		private const float Shoulder = 0.68f;

		// Token: 0x04000181 RID: 385
		private static bool warned;
	}
}
