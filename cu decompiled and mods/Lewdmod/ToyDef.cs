using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000096 RID: 150
	internal class ToyDef
	{
		// Token: 0x17000060 RID: 96
		// (get) Token: 0x060002FF RID: 767 RVA: 0x00033C11 File Offset: 0x00031E11
		public bool Plug
		{
			get
			{
				return this.Kind == "plug" || this.Kind == "tailplug" || this.Kind == "inflplug";
			}
		}

		// Token: 0x17000061 RID: 97
		// (get) Token: 0x06000300 RID: 768 RVA: 0x00033C4C File Offset: 0x00031E4C
		public string Gender
		{
			get
			{
				if (this.Kind == "dildo" || this.Kind == "vibe")
				{
					return "m";
				}
				if (!(this.Kind == "beads"))
				{
					return "f";
				}
				return "p";
			}
		}

		// Token: 0x040002C8 RID: 712
		public string Id;

		// Token: 0x040002C9 RID: 713
		public string Kind;

		// Token: 0x040002CA RID: 714
		public string Color;

		// Token: 0x040002CB RID: 715
		public string Size;

		// Token: 0x040002CC RID: 716
		public Color32 Tint;

		// Token: 0x040002CD RID: 717
		public int W;

		// Token: 0x040002CE RID: 718
		public int H;

		// Token: 0x040002CF RID: 719
		public float Boost;

		// Token: 0x040002D0 RID: 720
		public GameObject Template;
	}
}
