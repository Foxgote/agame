using System;
using TMPro;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A7 RID: 167
	internal static class LatexVoice
	{
		// Token: 0x06000359 RID: 857 RVA: 0x0003714C File Offset: 0x0003534C
		public static void Update(Body b)
		{
			TextMeshPro textMeshPro = ((b && b.talker) ? b.talker.text : null);
			if (!textMeshPro)
			{
				return;
			}
			if (textMeshPro != LatexVoice.text)
			{
				LatexVoice.text = textMeshPro;
				LatexVoice.own = textMeshPro.color;
			}
			Color color = (Suit.Latex ? LatexVoice.Violet : LatexVoice.own);
			if (textMeshPro.color != color)
			{
				textMeshPro.color = color;
			}
		}

		// Token: 0x04000305 RID: 773
		private static readonly Color Violet = new Color(0.78f, 0.52f, 1f);

		// Token: 0x04000306 RID: 774
		private static TextMeshPro text;

		// Token: 0x04000307 RID: 775
		private static Color own;
	}
}
