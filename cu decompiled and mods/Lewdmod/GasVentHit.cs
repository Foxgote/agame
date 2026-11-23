using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000B6 RID: 182
	internal class GasVentHit : MonoBehaviour
	{
		// Token: 0x06000415 RID: 1045 RVA: 0x000427C4 File Offset: 0x000409C4
		public void AnimalHit(float damage)
		{
			LustGas.VentHit(this.vent);
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x000427D1 File Offset: 0x000409D1
		public void AnimalDeath()
		{
			LustGas.BreakVent(this.vent, "struck down");
		}

		// Token: 0x040003C3 RID: 963
		public LustGas.Vent vent;
	}
}
