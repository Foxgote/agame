using System;
using System.Collections.Generic;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000C4 RID: 196
	internal class OutfitDef
	{
		// Token: 0x17000095 RID: 149
		// (get) Token: 0x0600044A RID: 1098 RVA: 0x000452D8 File Offset: 0x000434D8
		public string Name
		{
			get
			{
				if (this.ColorKey != null)
				{
					return string.Format(Plugin.T(this.NameKey + "_name"), Plugin.T("ddcol_" + this.ColorKey));
				}
				return Plugin.T(this.NameKey + "_name");
			}
		}

		// Token: 0x17000096 RID: 150
		// (get) Token: 0x0600044B RID: 1099 RVA: 0x00045332 File Offset: 0x00043532
		public string Desc
		{
			get
			{
				return Plugin.T(this.NameKey + "_desc");
			}
		}

		// Token: 0x040003E0 RID: 992
		public string Id;

		// Token: 0x040003E1 RID: 993
		public string TexturePrefix;

		// Token: 0x040003E2 RID: 994
		public string NameKey;

		// Token: 0x040003E3 RID: 995
		public float Armor;

		// Token: 0x040003E4 RID: 996
		public float Isolation;

		// Token: 0x040003E5 RID: 997
		public float Weight;

		// Token: 0x040003E6 RID: 998
		public int Value;

		// Token: 0x040003E7 RID: 999
		public string Slot = "bodysuit";

		// Token: 0x040003E8 RID: 1000
		public string MainLimb = "UpTorso";

		// Token: 0x040003E9 RID: 1001
		public string MainTexture = "experimentUpTorso";

		// Token: 0x040003EA RID: 1002
		public int VisualOffset = 3;

		// Token: 0x040003EB RID: 1003
		public float MinCondition = 1f;

		// Token: 0x040003EC RID: 1004
		public float MaxCondition = 1f;

		// Token: 0x040003ED RID: 1005
		public bool Lingerie;

		// Token: 0x040003EE RID: 1006
		public bool Thermal;

		// Token: 0x040003EF RID: 1007
		public bool IsLatex;

		// Token: 0x040003F0 RID: 1008
		public bool IsSpacesuit;

		// Token: 0x040003F1 RID: 1009
		public string Shoes;

		// Token: 0x040003F2 RID: 1010
		public string Folder;

		// Token: 0x040003F3 RID: 1011
		public string ColorKey;

		// Token: 0x040003F4 RID: 1012
		public bool IsDevice;

		// Token: 0x040003F5 RID: 1013
		public bool Locks;

		// Token: 0x040003F6 RID: 1014
		public Devices.Kind Kind;

		// Token: 0x040003F7 RID: 1015
		public bool CanBeHeld;

		// Token: 0x040003F8 RID: 1016
		public int Pouches;

		// Token: 0x040003F9 RID: 1017
		public bool EarPro;

		// Token: 0x040003FA RID: 1018
		public bool NotWorn;

		// Token: 0x040003FB RID: 1019
		public float Rarity = 1f;

		// Token: 0x040003FC RID: 1020
		public float CorpseRarity;

		// Token: 0x040003FD RID: 1021
		public int MinDepth;

		// Token: 0x040003FE RID: 1022
		public bool InLoot;

		// Token: 0x040003FF RID: 1023
		public readonly Dictionary<string, Texture2D> Textures = new Dictionary<string, Texture2D>();

		// Token: 0x04000400 RID: 1024
		public GameObject Template;
	}
}
