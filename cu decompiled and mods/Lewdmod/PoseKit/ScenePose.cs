using System;
using System.Collections.Generic;
using UnityEngine;

namespace PoseKit
{
	// Token: 0x02000005 RID: 5
	internal class ScenePose
	{
		// Token: 0x04000004 RID: 4
		public Vector2 Her;

		// Token: 0x04000005 RID: 5
		public Vector3 Root;

		// Token: 0x04000006 RID: 6
		public readonly Dictionary<string, float> Limb = new Dictionary<string, float>();

		// Token: 0x04000007 RID: 7
		public readonly Dictionary<string, float> Him = new Dictionary<string, float>();

		// Token: 0x04000008 RID: 8
		public float HimLean;

		// Token: 0x04000009 RID: 9
		public Vector2 HimBody;

		// Token: 0x0400000A RID: 10
		public float Loop = 1f;
	}
}
