using System;
using UnityEngine;

namespace ScavArmory
{
	// Token: 0x0200003E RID: 62
	internal class WallMount : MonoBehaviour
	{
		// Token: 0x06000102 RID: 258 RVA: 0x0000ECA0 File Offset: 0x0000CEA0
		private void Start()
		{
			this.rb = base.GetComponent<Rigidbody2D>();
			this.pos = base.transform.position;
			this.rot = base.transform.rotation;
			if (this.rb)
			{
				this.rb.bodyType = 1;
				this.rb.velocity = Vector2.zero;
				this.rb.angularVelocity = 0f;
			}
		}

		// Token: 0x06000103 RID: 259 RVA: 0x0000ED14 File Offset: 0x0000CF14
		private void Update()
		{
			if (base.transform.parent != null)
			{
				if (this.rb)
				{
					this.rb.bodyType = 0;
				}
				Object.Destroy(this);
				return;
			}
			if (this.rb && this.rb.bodyType == 1)
			{
				base.transform.position = this.pos;
				base.transform.rotation = this.rot;
				this.rb.velocity = Vector2.zero;
				this.rb.angularVelocity = 0f;
			}
		}

		// Token: 0x040000FF RID: 255
		private Rigidbody2D rb;

		// Token: 0x04000100 RID: 256
		private Vector3 pos;

		// Token: 0x04000101 RID: 257
		private Quaternion rot;
	}
}
