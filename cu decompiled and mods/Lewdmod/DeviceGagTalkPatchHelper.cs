using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000079 RID: 121
	internal static class DeviceGagTalkPatchHelper
	{
		// Token: 0x0600024A RID: 586 RVA: 0x0002C450 File Offset: 0x0002A650
		public static void Muffle(ref string text)
		{
			string[] array2;
			if (!Plugin.Russian)
			{
				string[] array = new string[5];
				array[0] = "Mmph!";
				array[1] = "Mm-mph...";
				array[2] = "Mmmmph!";
				array[3] = "Hmph... mm...";
				array2 = array;
				array[4] = "Mmph-mph!";
			}
			else
			{
				string[] array3 = new string[5];
				array3[0] = "Ммф!";
				array3[1] = "Мм-мф...";
				array3[2] = "Ммммф!";
				array3[3] = "Хмф... мм...";
				array2 = array3;
				array3[4] = "Ммф-мф!";
			}
			string[] array4 = array2;
			text = array4[Mathf.Abs((text ?? "").GetHashCode()) % array4.Length];
		}
	}
}
