using System;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200003D RID: 61
	internal static class PodRifle
	{
		// Token: 0x06000100 RID: 256 RVA: 0x0000EB5C File Offset: 0x0000CD5C
		public static void MaybePlace(Body b)
		{
			if (!b || !Plugin.GunsEnabled.Value || Random.value >= Plugin.PodRifleChance.Value)
			{
				return;
			}
			ArmDef armDef = Armory.ById("arm_mosin");
			ArmDef armDef2 = Armory.ById("arm_box762");
			if (armDef == null || armDef2 == null)
			{
				return;
			}
			float num = (b.isRight ? (-1f) : 1f);
			Vector2 vector = b.transform.position + new Vector2(num * 1.1f, 1.7f);
			GameObject gameObject = Armory.Spawn(armDef, vector, 0.75f, 1f);
			GameObject gameObject2 = Armory.Spawn(armDef2, vector + Vector2.down * 0.75f, 1f, 1f);
			AmmoScript ammoScript;
			if (gameObject2 && gameObject2.TryGetComponent<AmmoScript>(ref ammoScript))
			{
				ammoScript.rounds = armDef2.Rounds;
			}
			PodRifle.Mount(gameObject, 0f);
			PodRifle.Mount(gameObject2, 0f);
			Plugin.Log.LogInfo(string.Format("The three-line rifle hangs on the pod's wall at {0} (box of rifle rounds below it)", vector));
		}

		// Token: 0x06000101 RID: 257 RVA: 0x0000EC71 File Offset: 0x0000CE71
		private static void Mount(GameObject go, float angle)
		{
			if (!go)
			{
				return;
			}
			go.transform.rotation = Quaternion.Euler(0f, 0f, angle);
			go.AddComponent<WallMount>();
		}
	}
}
