using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200007C RID: 124
	internal static class TraderKeys
	{
		// Token: 0x0600024D RID: 589 RVA: 0x0002C544 File Offset: 0x0002A744
		public static void Meet(TraderScript t)
		{
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!t || !body || !Devices.On || !LewdSettings.On)
			{
				return;
			}
			int count = Devices.LockedOn(body).Count;
			if (count == 0)
			{
				return;
			}
			t.StartCoroutine(TraderKeys.Talk(t, body, count));
		}

		// Token: 0x0600024E RID: 590 RVA: 0x0002C5A9 File Offset: 0x0002A7A9
		private static IEnumerator Talk(TraderScript t, Body b, int n)
		{
			float w = 0f;
			while (t && t.items == null && w < 5f)
			{
				yield return null;
				w += Time.deltaTime;
			}
			if (!t)
			{
				yield break;
			}
			if (t.items == null)
			{
				t.items = new List<TraderItem>();
			}
			for (int j = 0; j < n; j++)
			{
				t.items.Add(new TraderItem
				{
					id = Devices.Key.Id,
					preference = 1,
					value = Mathf.Max(1, Devices.TraderKeyValue.Value)
				});
			}
			t.items = t.items.OrderBy<TraderItem, int>((TraderItem x) => x.preference).ToList<TraderItem>();
			Plugin.Log.LogInfo(string.Format("A trader sees her locked in {0} restraints: {1} keys for sale", n, n));
			yield return new WaitForSeconds(3f);
			int i = 0;
			Func<bool> <>9__2;
			while (i < 4 && t && t.talker)
			{
				Func<bool> func;
				if ((func = <>9__2) == null)
				{
					func = (<>9__2 = () => !t || !t.talker || t.talker.FinishedTalking());
				}
				yield return new WaitUntil(func);
				yield return new WaitForSeconds(0.6f);
				int num = i;
				i = num + 1;
			}
			if (!t || !t.talker || !b)
			{
				yield break;
			}
			t.talker.Talk(Plugin.T(string.Format("tdd_t{0}", Mathf.Clamp(t.character, 0, 2))), null, false, false);
			yield return new WaitUntil(() => !t || !t.talker || t.talker.FinishedTalking());
			yield return new WaitForSeconds(0.4f);
			if (b && b.alive)
			{
				Relief.Say(b, "tdd_her");
			}
			yield break;
		}
	}
}
