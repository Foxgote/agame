using System;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x02000045 RID: 69
	internal class ScavHitRelay : MonoBehaviour
	{
		// Token: 0x0600015A RID: 346 RVA: 0x00015F41 File Offset: 0x00014141
		public void AnimalHit(float damage)
		{
			if (this.scav)
			{
				this.scav.OnHit(damage);
			}
		}

		// Token: 0x0600015B RID: 347 RVA: 0x00015F5C File Offset: 0x0001415C
		public void AnimalDeath()
		{
			if (this.scav)
			{
				this.scav.OnDeath();
			}
		}

		// Token: 0x04000177 RID: 375
		public Scavenger scav;
	}
}
