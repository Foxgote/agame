using System;
using System.Collections.Generic;
using System.Linq;
using LewdStatus;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000030 RID: 48
	internal static class EarPro
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x060000DA RID: 218 RVA: 0x0000D970 File Offset: 0x0000BB70
		public static bool On
		{
			get
			{
				if (EarPro.frame != Time.frameCount)
				{
					EarPro.frame = Time.frameCount;
					EarPro.on = PlayerCamera.main && Outfits.EarProOn(PlayerCamera.main.body);
				}
				return EarPro.on;
			}
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000D9B0 File Offset: 0x0000BBB0
		public static bool IsShot(AudioClip clip)
		{
			if (!clip)
			{
				return false;
			}
			if (EarPro.shotClips == null)
			{
				EarPro.shotClips = new HashSet<string> { "gunshot" };
				List<string> list = new List<string>();
				list.Add("pistol");
				list.Add("rifle");
				list.Add("shotgun");
				list.Add("makeshiftrifle");
				list.AddRange(from d in Armory.All
					where d.Kind == Kind.Gun
					select d.Id);
				foreach (string text in list)
				{
					GameObject gameObject = Resources.Load(text) as GameObject;
					GunScript gunScript = (gameObject ? gameObject.GetComponent<GunScript>() : null);
					if (gunScript && gunScript.fireSound)
					{
						EarPro.shotClips.Add(gunScript.fireSound.name);
					}
				}
				foreach (GunScript gunScript2 in Resources.FindObjectsOfTypeAll<GunScript>())
				{
					if (gunScript2 && gunScript2.fireSound)
					{
						EarPro.shotClips.Add(gunScript2.fireSound.name);
					}
				}
				Plugin.Log.LogInfo("Ear protection: gunshot sounds " + string.Join(", ", EarPro.shotClips));
			}
			return EarPro.shotClips.Contains(clip.name);
		}

		// Token: 0x040000E7 RID: 231
		private static int frame = -1;

		// Token: 0x040000E8 RID: 232
		private static bool on;

		// Token: 0x040000E9 RID: 233
		public const float ShotVolume = 0.4f;

		// Token: 0x040000EA RID: 234
		private static HashSet<string> shotClips;
	}
}
