using System;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000A1 RID: 161
	[HarmonyPatch(typeof(WorldGeneration), "CreateExplosion")]
	internal static class BlastFreesHerPatch
	{
		// Token: 0x06000320 RID: 800 RVA: 0x0003554C File Offset: 0x0003374C
		private static void Prefix(ExplosionParams param)
		{
			if (param == null || !Worm.Inside)
			{
				return;
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body || Vector2.Distance(param.position, body.limbs[1].transform.position) > Mathf.Max(param.range, 4f))
			{
				return;
			}
			param.muscleDamage = new RangeF(0f, 0f);
			param.skinDamage = new RangeF(0f, 0f);
			param.skinDamageChance = 0f;
			param.boneBreakChance = 0f;
			param.dislocationChance = 0f;
			param.disfigureChance = 0f;
			param.bleedChance = 0f;
			param.bleedAmount = new RangeF(0f, 0f);
			param.shrapnelChance = 0f;
			param.structuralDamage = 0f;
			param.velocity = 0f;
			Plugin.Log.LogInfo("Dynamite went off inside the giant worm with her in it: the worm takes the blast, she is unhurt");
		}

		// Token: 0x06000321 RID: 801 RVA: 0x00035664 File Offset: 0x00033864
		private static void Postfix(ExplosionParams param)
		{
			if (param == null)
			{
				return;
			}
			try
			{
				LustGas.Blast(param.position, param.range);
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Blast and geysers: " + ex.Message);
			}
			try
			{
				Burning.Blast(param.position, param.range);
			}
			catch (Exception ex2)
			{
				Plugin.Log.LogWarning("Blast and flesh: " + ex2.Message);
			}
			try
			{
				Worm.Blast(param.position, param.range);
			}
			catch (Exception ex3)
			{
				Plugin.Log.LogWarning("Blast and worm: " + ex3.Message);
			}
			Body body = (PlayerCamera.main ? PlayerCamera.main.body : null);
			if (!body || !body.alive || !Scenes.Held)
			{
				return;
			}
			float num = Mathf.Max(4f, param.range * 0.75f);
			if (Vector2.Distance(param.position, body.limbs[1].transform.position) > num)
			{
				return;
			}
			Heart.ReleaseAll(body, "a blast close by");
			Relief.Say(body, "blast_free");
		}
	}
}
