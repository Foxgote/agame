using System;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000138 RID: 312
	internal class WormHitRelay : MonoBehaviour
	{
		// Token: 0x06000782 RID: 1922 RVA: 0x00073A42 File Offset: 0x00071C42
		public void AnimalHit(float damage)
		{
			if (this.worm)
			{
				this.worm.TakeHit(damage, "a hit");
			}
		}

		// Token: 0x06000783 RID: 1923 RVA: 0x00073A62 File Offset: 0x00071C62
		public void AnimalDeath()
		{
			if (this.worm)
			{
				this.worm.OnOutsideHit();
			}
		}

		// Token: 0x040007BB RID: 1979
		public WormBeast worm;
	}
}
