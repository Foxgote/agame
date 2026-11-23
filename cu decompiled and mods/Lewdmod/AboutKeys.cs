using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x0200004D RID: 77
	internal class AboutKeys : MonoBehaviour
	{
		// Token: 0x0600018E RID: 398 RVA: 0x0001A87D File Offset: 0x00018A7D
		private void Update()
		{
			if (!Input.GetKeyDown(27))
			{
				return;
			}
			if (AboutMod.WindowOpen)
			{
				AboutMod.HideWindow();
				return;
			}
			AboutMod.HideNotice();
		}
	}
}
