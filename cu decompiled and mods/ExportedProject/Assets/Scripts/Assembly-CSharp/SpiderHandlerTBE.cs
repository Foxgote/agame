using UnityEngine;

public class SpiderHandlerTBE : SpiderHandler
{
	public override void DamageLimb(Limb limb)
	{
		biteCooldown = 5f;
		Limb limb2 = null;
		for (int i = 0; i < 100; i++)
		{
			Limb limb3 = limb.body.limbs[Random.Range(3, limb.body.limbs.Length)];
			if (!limb3.dismembered)
			{
				limb2 = limb3;
				break;
			}
		}
		if ((bool)limb2)
		{
			limb2.Dismember();
		}
		else if (!limb.body.disfigured)
		{
			if (WorldGeneration.GetRunSettingBool("disfigurement"))
			{
				limb.body.Disfigure();
				limb.body.RemoveEye();
				limb.body.RemoveEye();
			}
			else
			{
				limb.body.limbs[0].skinHealth = 0f;
				limb.body.limbs[0].bleedAmount = 100f;
				limb.body.limbs[0].pain = 100f;
			}
		}
		else
		{
			limb.body.brainHealth = 0f;
		}
		PlayerCamera.main.DoAchievementDelayed(600f, "TISBUTASCRATCH");
	}
}
