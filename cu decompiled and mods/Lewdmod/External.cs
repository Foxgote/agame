using System;
using ScavArmory;

namespace LewdStatus
{
	// Token: 0x02000086 RID: 134
	internal static class External
	{
		// Token: 0x17000046 RID: 70
		// (get) Token: 0x06000274 RID: 628 RVA: 0x0002D20B File Offset: 0x0002B40B
		public static bool ScavHolding
		{
			get
			{
				return Capture.Active;
			}
		}
	}
}
