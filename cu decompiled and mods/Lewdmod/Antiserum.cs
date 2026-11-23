using System;
using System.Runtime.CompilerServices;

namespace LewdStatus
{
	// Token: 0x0200004F RID: 79
	internal static class Antiserum
	{
		// Token: 0x06000191 RID: 401 RVA: 0x0001A92C File Offset: 0x00018B2C
		public static void Ensure()
		{
			LiquidType liquidType;
			if (Antiserum.hooked || Liquids.Registry == null || !Liquids.Registry.TryGetValue("antiserum", out liquidType))
			{
				return;
			}
			Antiserum.hooked = true;
			LiquidType liquidType2 = liquidType;
			Delegate onHealthUse = liquidType2.onHealthUse;
			LiquidType.OnHealthUse onHealthUse2;
			if ((onHealthUse2 = Antiserum.<>O.<0>__Injected) == null)
			{
				onHealthUse2 = (Antiserum.<>O.<0>__Injected = new LiquidType.OnHealthUse(Antiserum.Injected));
			}
			liquidType2.onHealthUse = (LiquidType.OnHealthUse)Delegate.Combine(onHealthUse, onHealthUse2);
			Plugin.Log.LogInfo("Antiserum: it quiets the hosts' slime too");
		}

		// Token: 0x06000192 RID: 402 RVA: 0x0001A9A4 File Offset: 0x00018BA4
		private static void Injected(float ml, Limb limb)
		{
			Body body = (limb ? limb.body : null);
			if (!body || !PlayerCamera.main || PlayerCamera.main.body != body)
			{
				return;
			}
			try
			{
				Suit.TakeAntiserum(body, ml);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Antiserum: " + ex.Message);
			}
		}

		// Token: 0x04000192 RID: 402
		private static bool hooked;

		// Token: 0x02000179 RID: 377
		[CompilerGenerated]
		private static class <>O
		{
			// Token: 0x0400088F RID: 2191
			public static LiquidType.OnHealthUse <0>__Injected;
		}
	}
}
