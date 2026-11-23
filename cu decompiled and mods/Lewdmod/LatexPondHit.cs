using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000AD RID: 173
	internal class LatexPondHit : MonoBehaviour
	{
		// Token: 0x060003C6 RID: 966 RVA: 0x0003E756 File Offset: 0x0003C956
		public void AnimalHit(float damage)
		{
			LatexPond.OnHit(this.pond);
		}

		// Token: 0x060003C7 RID: 967 RVA: 0x0003E763 File Offset: 0x0003C963
		public void AnimalDeath()
		{
		}

		// Token: 0x04000379 RID: 889
		public LatexPond.Pond pond;
	}
}
