using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200006D RID: 109
	internal static class Lockpick
	{
		// Token: 0x06000235 RID: 565 RVA: 0x0002BAD0 File Offset: 0x00029CD0
		public static void Start(Body b, Item dev, Item picks)
		{
			if (MinigameBase.main == null || MinigameBase.main.currentMinigame)
			{
				return;
			}
			GameObject gameObject = new GameObject("LewdDeviceLock");
			gameObject.SetActive(false);
			Lockpick.dummy = gameObject.AddComponent<BuildingEntity>();
			Lockpick.dummy.health = 1f;
			Lockpick.device = dev;
			Lockpick.body = b;
			MinigameBase.main.StartMinigame(new LockpingMinigame(Lockpick.dummy, 12f * WorldGeneration.GetRunSettingFloat("lockpickprecision")), picks);
			Relief.Say(b, "dd_pick_start");
		}

		// Token: 0x06000236 RID: 566 RVA: 0x0002BB64 File Offset: 0x00029D64
		public static void Update()
		{
			if (!Lockpick.dummy)
			{
				return;
			}
			if (Lockpick.dummy.health <= 0f)
			{
				if (Lockpick.device && Lockpick.body)
				{
					Devices.Release(Lockpick.body, Lockpick.device, true);
					Relief.Say(Lockpick.body, "dd_pick_ok");
					Plugin.Log.LogInfo("Devices: she picked the lock of her " + Lockpick.device.id);
				}
				Object.Destroy(Lockpick.dummy.gameObject);
				Lockpick.dummy = null;
				return;
			}
			if (MinigameBase.main == null || !MinigameBase.main.currentMinigame)
			{
				Object.Destroy(Lockpick.dummy.gameObject);
				Lockpick.dummy = null;
			}
		}

		// Token: 0x04000243 RID: 579
		private static BuildingEntity dummy;

		// Token: 0x04000244 RID: 580
		private static Item device;

		// Token: 0x04000245 RID: 581
		private static Body body;
	}
}
