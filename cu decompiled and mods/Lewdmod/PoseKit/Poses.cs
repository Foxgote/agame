using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PoseKit
{
	// Token: 0x02000006 RID: 6
	internal static class Poses
	{
		// Token: 0x06000005 RID: 5 RVA: 0x000020BC File Offset: 0x000002BC
		public static void Load(string file)
		{
			if (!File.Exists(file))
			{
				Kit.Warn("No poses: " + file);
				return;
			}
			ScenePose scenePose = null;
			int num = 0;
			foreach (string text in File.ReadAllLines(file))
			{
				num++;
				try
				{
					string[] array2 = text.Split(new char[] { '#' })[0].Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
					if (array2.Length != 0)
					{
						string text2 = array2[0];
						if (text2 != null)
						{
							switch (text2.Length)
							{
							case 3:
							{
								char c = text2[1];
								if (c != 'e')
								{
									if (c == 'i')
									{
										if (text2 == "him")
										{
											scenePose.Him[array2[1]] = Poses.<Load>g__F|1_0(array2[2]);
											goto IL_02E3;
										}
									}
								}
								else if (text2 == "her")
								{
									scenePose.Her = new Vector2(Poses.<Load>g__F|1_0(array2[1]), Poses.<Load>g__F|1_0(array2[2]));
									goto IL_02E3;
								}
								break;
							}
							case 4:
							{
								char c = text2[0];
								if (c != 'l')
								{
									if (c == 'r')
									{
										if (text2 == "root")
										{
											scenePose.Root = new Vector3(Poses.<Load>g__F|1_0(array2[1]), Poses.<Load>g__F|1_0(array2[2]), Poses.<Load>g__F|1_0(array2[3]));
											goto IL_02E3;
										}
									}
								}
								else if (text2 == "loop")
								{
									scenePose.Loop = Poses.<Load>g__F|1_0(array2[1]);
									goto IL_02E3;
								}
								break;
							}
							case 5:
							{
								char c = text2[0];
								if (c != 'n')
								{
									if (c == 's')
									{
										if (text2 == "scene")
										{
											scenePose = (Poses.All[array2[1]] = new ScenePose());
											goto IL_02E3;
										}
									}
								}
								else if (text2 == "nohim")
								{
									goto IL_02E3;
								}
								break;
							}
							case 7:
							{
								char c = text2[3];
								if (c != 'b')
								{
									if (c == 'l')
									{
										if (text2 == "himlean")
										{
											scenePose.HimLean = Poses.<Load>g__F|1_0(array2[1]);
											goto IL_02E3;
										}
									}
								}
								else if (text2 == "himbody")
								{
									scenePose.HimBody = new Vector2(Poses.<Load>g__F|1_0(array2[1]), Poses.<Load>g__F|1_0(array2[2]));
									goto IL_02E3;
								}
								break;
							}
							}
						}
						float num2;
						if (scenePose != null && array2.Length >= 2 && float.TryParse(array2[1], NumberStyles.Float, CultureInfo.InvariantCulture, out num2))
						{
							scenePose.Limb[array2[0]] = num2;
						}
						else
						{
							Kit.Warn("Poses: line not understood: " + text.Trim());
						}
						IL_02E3:;
					}
				}
				catch (Exception ex)
				{
					Kit.Warn(string.Format("Poses: line {0} skipped ({1}): {2}", num, ex.GetType().Name, text.Trim()));
				}
			}
			Kit.Log(string.Format("Poses: {0} ({1})", Poses.All.Count, string.Join(", ", Poses.All.Keys)));
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002440 File Offset: 0x00000640
		[return: TupleElementNames(new string[] { "a", "b", "t" })]
		public static ValueTuple<ScenePose, ScenePose, float> At(string scene, float time, float speed = 1f)
		{
			ScenePose scenePose;
			if (!Poses.All.TryGetValue(scene, out scenePose))
			{
				return new ValueTuple<ScenePose, ScenePose, float>(null, null, 0f);
			}
			ScenePose scenePose2;
			if (!Poses.All.TryGetValue(scene + "_b", out scenePose2))
			{
				return new ValueTuple<ScenePose, ScenePose, float>(scenePose, scenePose, 0f);
			}
			float num = (Mathf.Sin(time * speed * 3.1415927f * 2f / Mathf.Max(0.1f, scenePose.Loop)) + 1f) * 0.5f;
			return new ValueTuple<ScenePose, ScenePose, float>(scenePose, scenePose2, num);
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000024CC File Offset: 0x000006CC
		public static float Lerp(Dictionary<string, float> a, Dictionary<string, float> b, string k, float t)
		{
			float num;
			float num2;
			return Mathf.Lerp(a.TryGetValue(k, out num) ? num : 0f, b.TryGetValue(k, out num2) ? num2 : 0f, t);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x00002511 File Offset: 0x00000711
		[CompilerGenerated]
		internal static float <Load>g__F|1_0(string v)
		{
			return float.Parse(v, CultureInfo.InvariantCulture);
		}

		// Token: 0x0400000B RID: 11
		public static readonly Dictionary<string, ScenePose> All = new Dictionary<string, ScenePose>();
	}
}
