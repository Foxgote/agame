using UnityEngine;

public class Skills
{
	public int STR;

	public int RES;

	public int INT;

	public float expSTR;

	public float expRES;

	public float expINT;

	public int minSTR;

	public int maxSTR;

	public int minRES;

	public int maxRES;

	public int minINT;

	public int maxINT;

	public float STRGainMult = 1f;

	public float RESGainMult = 1f;

	public float INTGainMult = 1f;

	public static float xpGainMult => WorldGeneration.GetRunSettingFloat("xpgain");

	public float STRFrom10 => (float)STR - 10f;

	public float RESFrom10 => (float)RES - 10f;

	public float INTFrom10 => (float)INT - 10f;

	public void Setup(int chr)
	{
		int[] array = BaseSkills(chr);
		STR = array[0];
		RES = array[1];
		INT = array[2];
		float[] array2 = BaseMultipliers(chr);
		STRGainMult = array2[0];
		RESGainMult = array2[1];
		INTGainMult = array2[2];
		UpdateExpBoundaries();
		expSTR = minSTR;
		expRES = minRES;
		expINT = minINT;
	}

	public void UpdateExpBoundaries()
	{
		minSTR = GetExperienceForLevel(STR);
		maxSTR = GetExperienceForLevel(STR + 1);
		minRES = GetExperienceForLevel(RES);
		maxRES = GetExperienceForLevel(RES + 1);
		minINT = GetExperienceForLevel(INT);
		maxINT = GetExperienceForLevel(INT + 1);
	}

	public float ToNextNormalized(float xp, float min, float max)
	{
		return (xp - min) / (max - min);
	}

	public void AddExp(int stat, float xp)
	{
		xp *= xpGainMult;
		switch (stat)
		{
		case 0:
		{
			int sTR = STR;
			xp *= STRGainMult;
			expSTR += xp;
			if (CheckForLevelUp(ref STR, expSTR, ref maxSTR))
			{
				PlayerCamera.main.DoAlert(Locale.GetOther("skillup").Replace("<>", Locale.GetOther("strength").ToLower()).Replace("<1>", sTR.ToString() ?? "")
					.Replace("<2>", STR.ToString() ?? ""));
				Sound.Play("music/levelup", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			}
			break;
		}
		case 1:
		{
			int rES = RES;
			xp *= RESGainMult;
			expRES += xp;
			if (CheckForLevelUp(ref RES, expRES, ref maxRES))
			{
				PlayerCamera.main.DoAlert(Locale.GetOther("skillup").Replace("<>", Locale.GetOther("resilience").ToLower()).Replace("<1>", rES.ToString() ?? "")
					.Replace("<2>", RES.ToString() ?? ""));
				Sound.Play("music/levelup", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			}
			break;
		}
		default:
		{
			int iNT = INT;
			xp *= INTGainMult;
			expINT += xp;
			if (CheckForLevelUp(ref INT, expINT, ref maxINT))
			{
				PlayerCamera.main.DoAlert(Locale.GetOther("skillup").Replace("<>", Locale.GetOther("intelligence").ToLower()).Replace("<1>", iNT.ToString() ?? "")
					.Replace("<2>", INT.ToString() ?? ""));
				Sound.Play("music/levelup", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
			}
			break;
		}
		}
	}

	public bool CheckForLevelUp(ref int level, float xp, ref int xpToLevel)
	{
		bool result = false;
		while (xp >= (float)xpToLevel)
		{
			level++;
			result = true;
			UpdateExpBoundaries();
		}
		return result;
	}

	public static int GetExperienceForLevel(int targetLevel)
	{
		if (targetLevel <= 0)
		{
			return 0;
		}
		float num = 0f;
		float num2 = 60f;
		for (int i = 1; i <= targetLevel; i++)
		{
			if (i < 10)
			{
				num += 60f;
				continue;
			}
			if (i == 10)
			{
				num2 = 100f;
				num += num2;
				continue;
			}
			num2 *= 1.18f;
			if (i > 20)
			{
				num2 *= 1.15f;
				if (i >= 30)
				{
					num2 *= 2f;
				}
			}
			num += num2;
		}
		return Mathf.RoundToInt(num);
	}

	public static int[] BaseSkills(int chr)
	{
		return chr switch
		{
			0 => new int[3] { 9, 8, 7 }, 
			1 => new int[3] { 4, 6, 12 }, 
			2 => new int[3] { 17, 18, 0 }, 
			_ => new int[3], 
		};
	}

	public static float[] BaseMultipliers(int chr)
	{
		return chr switch
		{
			0 => new float[3] { 1f, 1f, 1f }, 
			1 => new float[3] { 0.5f, 0.5f, 1f }, 
			2 => new float[3] { 1f, 1f, 0.5f }, 
			_ => new float[3], 
		};
	}
}
