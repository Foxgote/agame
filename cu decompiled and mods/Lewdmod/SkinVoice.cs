using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdStatus
{
	// Token: 0x02000100 RID: 256
	internal static class SkinVoice
	{
		// Token: 0x170000BF RID: 191
		// (get) Token: 0x06000591 RID: 1425 RVA: 0x0005A00F File Offset: 0x0005820F
		public static string SkinName
		{
			get
			{
				if (SkinVoice.skin.Count <= 0 || SkinVoice.skinName == null)
				{
					return "original";
				}
				return SkinVoice.skinName.Value;
			}
		}

		// Token: 0x06000592 RID: 1426 RVA: 0x0005A038 File Offset: 0x00058238
		public static void Init(ConfigFile config, string root)
		{
			SkinVoice.skinName = config.Bind<string>("Skin", "Skin", "female", "Folder name in skins/ to use (female, argonian), or 'none' for the original look.");
			SkinVoice.voiceName = config.Bind<string>("Voice", "Voice", "female", "Folder name in voice/ to use, or 'none' for the original voice.");
			SkinVoice.EffortOriginalEntry = config.Bind<bool>("Voice", "EffortOriginal", false, "Jumps, blows and climbing use the game's own effort sounds instead of her female ones (her pain, panting and the mod's scenes stay female). Settings -> Lewd -> \"Original effort sounds\".");
			SkinVoice.FlatBelly = config.Bind<bool>("Skin", "FlatBelly", true, "Her belly flat: the skin's lower torso has a one-pixel curve forward that read as a bump that never went away. Off: the skin as drawn. (A full womb or a brood still shows.)");
			SkinVoice.StandardTail = config.Bind<bool>("Skin", "StandardTail", false, "Her tail as the game draws it (big and fluffy). Off: the skin's slim tail with the orange-yellow tuft. Settings -> Lewd -> \"Standard tail\".");
			SkinVoice.StandardTail.SettingChanged += delegate(object s, EventArgs e)
			{
				SkinVoice.ApplyTail();
			};
			SkinVoice.VoiceVolume = config.Bind<float>("Voice", "Volume", 1f, "Loudness of her voice in the game's own sounds (pain, effort, growls, panting...), 0-2.");
			SkinVoice.LoadSkin(Path.Combine(Path.Combine(root, "skins"), SkinVoice.skinName.Value));
			SkinVoice.LoadVoice(Path.Combine(Path.Combine(root, "voice"), SkinVoice.voiceName.Value));
			if (SkinVoice.skin.Count > 0)
			{
				SceneManager.sceneLoaded += delegate(Scene s, LoadSceneMode m)
				{
					SkinVoice.ApplySkin("scene " + s.name);
				};
			}
		}

		// Token: 0x06000593 RID: 1427 RVA: 0x0005A18C File Offset: 0x0005838C
		public static void Tick(float dt)
		{
			if (SkinVoice.skin.Count == 0 || (SkinVoice.rescan -= dt) > 0f)
			{
				return;
			}
			SkinVoice.rescan = ((Time.timeSinceLevelLoad < 60f) ? 3f : 20f);
			SkinVoice.ApplySkin(null);
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x0005A1E0 File Offset: 0x000583E0
		private static void LoadSkin(string dir)
		{
			if (!Directory.Exists(dir))
			{
				Plugin.Log.LogInfo("Skin off (" + dir + " not found)");
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.png"))
			{
				SkinVoice.skin[Path.GetFileNameWithoutExtension(text)] = File.ReadAllBytes(text);
			}
			Plugin.Log.LogInfo(string.Format("Skin '{0}': {1} textures", SkinVoice.skinName.Value, SkinVoice.skin.Count));
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x0005A274 File Offset: 0x00058474
		private static void LoadVoice(string dir)
		{
			if (!Directory.Exists(dir))
			{
				Plugin.Log.LogInfo("Voice off (" + dir + " not found)");
				return;
			}
			foreach (string text in Directory.GetFiles(dir, "*.wav"))
			{
				string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);
				int num = fileNameWithoutExtension.IndexOf('_');
				string text2 = ((num > 0) ? fileNameWithoutExtension.Substring(0, num) : fileNameWithoutExtension);
				AudioClip audioClip = Wav.Load(text, fileNameWithoutExtension);
				if (audioClip == null)
				{
					Plugin.Log.LogWarning("Not a PCM wav: " + text);
				}
				else
				{
					List<AudioClip> list;
					if (!SkinVoice.Voice.TryGetValue(text2, out list))
					{
						list = (SkinVoice.Voice[text2] = new List<AudioClip>());
					}
					list.Add(audioClip);
				}
			}
			Plugin.Log.LogInfo(string.Format("Voice '{0}': {1} clips for {2} sounds", SkinVoice.voiceName.Value, SkinVoice.Voice.Sum<KeyValuePair<string, List<AudioClip>>>((KeyValuePair<string, List<AudioClip>> v) => v.Value.Count), SkinVoice.Voice.Count));
		}

		// Token: 0x06000596 RID: 1430 RVA: 0x0005A3A0 File Offset: 0x000585A0
		private static void ApplySkin(string reason)
		{
			int num = 0;
			foreach (Texture2D texture2D in Resources.FindObjectsOfTypeAll<Texture2D>())
			{
				byte[] array2;
				byte[] array3;
				if (texture2D != null && texture2D.name == "experimentTail" && !SkinVoice.done.Contains(texture2D.GetInstanceID()) && SkinVoice.skin.TryGetValue("experimentTail@slim", out array2))
				{
					SkinVoice.done.Add(texture2D.GetInstanceID());
					if (SkinVoice.tailGame == null)
					{
						try
						{
							SkinVoice.tailGame = ImageConversion.EncodeToPNG(Bust.Readable(texture2D));
						}
						catch (Exception ex)
						{
							SkinVoice.tailGame = null;
							Plugin.Log.LogWarning("Tail: the game's own could not be kept: " + ex.Message);
						}
					}
					SkinVoice.tailTex.RemoveAll((Texture2D t) => !t);
					SkinVoice.tailTex.Add(texture2D);
					SkinVoice.tailSlim = array2;
					SkinVoice.ApplyTail();
					num++;
				}
				else if (!(texture2D == null) && SkinVoice.skin.TryGetValue(texture2D.name, out array3) && !SkinVoice.done.Contains(texture2D.GetInstanceID()))
				{
					int width = texture2D.width;
					int height = texture2D.height;
					bool flag = texture2D.name == "experimentUpTorso";
					bool flag2 = texture2D.name == "experimentDownTorso" && SkinVoice.FlatBelly != null && SkinVoice.FlatBelly.Value;
					bool flag3;
					try
					{
						flag3 = ImageConversion.LoadImage(texture2D, flag2 ? SkinVoice.FlatPng(texture2D.name, array3) : array3, !flag && !flag2);
					}
					catch (Exception ex2)
					{
						flag3 = false;
						Plugin.Log.LogWarning("Skin for " + texture2D.name + ": " + ex2.Message);
					}
					if (!flag3)
					{
						Plugin.Log.LogWarning("Could not load skin for " + texture2D.name);
						SkinVoice.done.Add(texture2D.GetInstanceID());
					}
					else
					{
						if (texture2D.width != width || texture2D.height != height)
						{
							Plugin.Log.LogWarning(string.Format("{0}: skin is {1}x{2}, game expects {3}x{4}", new object[] { texture2D.name, texture2D.width, texture2D.height, width, height }));
						}
						texture2D.filterMode = 0;
						texture2D.wrapMode = 1;
						SkinVoice.done.Add(texture2D.GetInstanceID());
						num++;
						if (flag)
						{
							byte[] array4;
							Bust.RegisterChest(texture2D, array3, SkinVoice.skin.TryGetValue(texture2D.name + "@small", out array4) ? array4 : null);
						}
					}
				}
			}
			if (num > 0)
			{
				Plugin.Log.LogInfo(string.Format("Applied {0} skin textures", num) + ((reason != null) ? (" (" + reason + ")") : ""));
			}
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x0005A6D4 File Offset: 0x000588D4
		private static void ApplyTail()
		{
			byte[] array = ((SkinVoice.StandardTail != null && SkinVoice.StandardTail.Value) ? (FurRealCompat.Present ? (SkinVoice.StandardInHerColours() ?? SkinVoice.tailGame) : SkinVoice.tailGame) : SkinVoice.tailSlim);
			if (array == null)
			{
				return;
			}
			int num = 0;
			foreach (Texture2D texture2D in SkinVoice.tailTex)
			{
				if (texture2D)
				{
					try
					{
						ImageConversion.LoadImage(texture2D, array, false);
						texture2D.filterMode = 0;
						texture2D.wrapMode = 1;
						num++;
					}
					catch (Exception ex)
					{
						Plugin.Log.LogWarning("Tail: " + ex.Message);
					}
				}
			}
			if (num > 0)
			{
				Plugin.Log.LogInfo(string.Format("Her tail: {0} ({1} textures)", (array == SkinVoice.tailSlim) ? "slim" : ((array == SkinVoice.tailStandard) ? "the game's own in her colours" : "the game's own"), num));
			}
			FurRealCompat.ResetTail();
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x0005A7F8 File Offset: 0x000589F8
		private static byte[] StandardInHerColours()
		{
			if (SkinVoice.tailStandard != null)
			{
				return SkinVoice.tailStandard;
			}
			if (SkinVoice.tailGame == null)
			{
				return null;
			}
			List<Color32> list = new List<Color32>();
			foreach (string text in new string[] { "experimentThigh", "experimentUpArm", "experimentDownTorso" })
			{
				byte[] array2;
				if (SkinVoice.skin.TryGetValue(text, out array2))
				{
					Texture2D texture2D = new Texture2D(2, 2, 4, false);
					try
					{
						if (ImageConversion.LoadImage(texture2D, array2, false))
						{
							list.AddRange(from c in texture2D.GetPixels32()
								where c.a > 200
								select c);
						}
					}
					finally
					{
						Object.Destroy(texture2D);
					}
				}
			}
			if (list.Count < 4)
			{
				return null;
			}
			list.Sort((Color32 x, Color32 y) => SkinVoice.Lum(x).CompareTo(SkinVoice.Lum(y)));
			Texture2D texture2D2 = new Texture2D(2, 2, 4, false);
			try
			{
				if (!ImageConversion.LoadImage(texture2D2, SkinVoice.tailGame, false))
				{
					return null;
				}
				Color32[] pixels = texture2D2.GetPixels32();
				int width = texture2D2.width;
				List<Color32> list2 = SkinVoice.TuftPalette();
				int num = int.MaxValue;
				int num2 = -1;
				for (int j = 0; j < pixels.Length; j++)
				{
					if (pixels[j].a > 0)
					{
						num = Mathf.Min(num, j % width);
						num2 = Mathf.Max(num2, j % width);
					}
				}
				float num3 = (float)num + 0.3f * (float)(num2 - num);
				bool[] tuft = new bool[pixels.Length];
				if (list2.Count > 0)
				{
					for (int k = 0; k < pixels.Length; k++)
					{
						tuft[k] = pixels[k].a > 0 && (float)(k % width) <= num3;
					}
				}
				SkinVoice.Recolour(pixels, (int i) => !tuft[i], list, 0.2f, 0.8f);
				if (list2.Count > 0)
				{
					SkinVoice.Recolour(pixels, (int i) => tuft[i], list2, 0f, 1f);
				}
				texture2D2.SetPixels32(pixels);
				texture2D2.Apply();
				SkinVoice.tailStandard = ImageConversion.EncodeToPNG(texture2D2);
				Plugin.Log.LogInfo(string.Format("Standard tail recoloured to her fur ({0} skin pixels)", list.Count));
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Standard tail colours: " + ex.Message);
			}
			finally
			{
				Object.Destroy(texture2D2);
			}
			return SkinVoice.tailStandard;
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0005AAD4 File Offset: 0x00058CD4
		private static float Lum(Color32 c)
		{
			return 0.299f * (float)c.r + 0.587f * (float)c.g + 0.114f * (float)c.b;
		}

		// Token: 0x0600059A RID: 1434 RVA: 0x0005AB00 File Offset: 0x00058D00
		private static float Sat(Color32 c)
		{
			float num;
			float num2;
			float num3;
			Color.RGBToHSV(c, ref num, ref num2, ref num3);
			return num2;
		}

		// Token: 0x0600059B RID: 1435 RVA: 0x0005AB20 File Offset: 0x00058D20
		private static float Val(Color32 c)
		{
			float num;
			float num2;
			float num3;
			Color.RGBToHSV(c, ref num, ref num2, ref num3);
			return num3;
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x0005AB40 File Offset: 0x00058D40
		private static List<Color32> TuftPalette()
		{
			List<Color32> list = new List<Color32>();
			if (SkinVoice.tailSlim == null)
			{
				return list;
			}
			Texture2D texture2D = new Texture2D(2, 2, 4, false);
			try
			{
				if (ImageConversion.LoadImage(texture2D, SkinVoice.tailSlim, false))
				{
					list.AddRange(from c in texture2D.GetPixels32()
						where c.a > 200 && SkinVoice.Sat(c) > 0.35f && SkinVoice.Val(c) > 0.4f
						select c);
				}
			}
			finally
			{
				Object.Destroy(texture2D);
			}
			list.Sort((Color32 x, Color32 y) => SkinVoice.Lum(x).CompareTo(SkinVoice.Lum(y)));
			return list;
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0005ABE4 File Offset: 0x00058DE4
		private static void Recolour(Color32[] px, Func<int, bool> pick, List<Color32> palette, float from = 0f, float to = 1f)
		{
			float num = float.MaxValue;
			float num2 = float.MinValue;
			for (int i = 0; i < px.Length; i++)
			{
				if (px[i].a > 0 && pick(i))
				{
					float num3 = SkinVoice.Lum(px[i]);
					num = Mathf.Min(num, num3);
					num2 = Mathf.Max(num2, num3);
				}
			}
			for (int j = 0; j < px.Length; j++)
			{
				if (px[j].a != 0 && pick(j))
				{
					float num4 = ((num2 > num) ? ((SkinVoice.Lum(px[j]) - num) / (num2 - num)) : 0.5f);
					num4 = Mathf.Lerp(from, to, num4);
					Color32 color = palette[Mathf.Clamp(Mathf.RoundToInt(num4 * (float)(palette.Count - 1)), 0, palette.Count - 1)];
					px[j] = new Color32(color.r, color.g, color.b, px[j].a);
				}
			}
		}

		// Token: 0x0600059E RID: 1438 RVA: 0x0005ACFC File Offset: 0x00058EFC
		private static byte[] FlatPng(string name, byte[] png)
		{
			byte[] array;
			if (SkinVoice.flat.TryGetValue(png, out array))
			{
				return array;
			}
			byte[] array2 = png;
			Texture2D texture2D = new Texture2D(2, 2, 4, false);
			try
			{
				if (ImageConversion.LoadImage(texture2D, png, false))
				{
					SkinVoice.Flatten(texture2D);
					array2 = ImageConversion.EncodeToPNG(texture2D);
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.LogWarning("Flat belly: " + ex.Message);
			}
			finally
			{
				Object.Destroy(texture2D);
			}
			SkinVoice.flat[png] = array2;
			return array2;
		}

		// Token: 0x0600059F RID: 1439 RVA: 0x0005AD8C File Offset: 0x00058F8C
		private static void Flatten(Texture2D tex)
		{
			SkinVoice.<>c__DisplayClass30_0 CS$<>8__locals1;
			CS$<>8__locals1.tex = tex;
			CS$<>8__locals1.w = CS$<>8__locals1.tex.width;
			int height = CS$<>8__locals1.tex.height;
			int num = 0;
			int[] array = new int[height];
			for (int i = 0; i < height; i++)
			{
				array[i] = SkinVoice.<Flatten>g__Front|30_0(i, ref CS$<>8__locals1);
			}
			int key = (from f in array
				where f >= 0
				group f by f into g
				orderby g.Count<int>() descending, g.Key
				select g).First<IGrouping<int, int>>().Key;
			for (int j = 0; j < height; j++)
			{
				if (array[j] > key)
				{
					for (int k = array[j]; k > key; k--)
					{
						CS$<>8__locals1.tex.SetPixel(k - 1, j, CS$<>8__locals1.tex.GetPixel(k, j));
						CS$<>8__locals1.tex.SetPixel(k, j, Color.clear);
						num++;
					}
				}
			}
			CS$<>8__locals1.tex.Apply(false);
			if (num > 0)
			{
				Plugin.Log.LogInfo(string.Format("Flat belly: {0} px taken off the front of the skin's lower torso", num));
			}
		}

		// Token: 0x060005A0 RID: 1440 RVA: 0x0005AF10 File Offset: 0x00059110
		internal static bool IsHers(AudioClip clip)
		{
			return clip != null && SkinVoice.Voice.ContainsKey(clip.name) && (!SkinVoice.OriginalEffort || !SkinVoice.IsEffort(clip.name));
		}

		// Token: 0x060005A1 RID: 1441 RVA: 0x0005AF46 File Offset: 0x00059146
		private static bool IsEffort(string name)
		{
			return name.StartsWith("exert", StringComparison.OrdinalIgnoreCase) || name.StartsWith("exp", StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060005A2 RID: 1442 RVA: 0x0005AF64 File Offset: 0x00059164
		private static bool OriginalEffort
		{
			get
			{
				return SkinVoice.EffortOriginalEntry != null && SkinVoice.EffortOriginalEntry.Value;
			}
		}

		// Token: 0x060005A3 RID: 1443 RVA: 0x0005AF7C File Offset: 0x0005917C
		internal static AudioClip Replacement(AudioClip clip)
		{
			List<AudioClip> list;
			if (clip == null || !SkinVoice.Voice.TryGetValue(clip.name, out list) || list.Count == 0)
			{
				return clip;
			}
			return list[Random.Range(0, list.Count)];
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0005AFFC File Offset: 0x000591FC
		[CompilerGenerated]
		internal static int <Flatten>g__Front|30_0(int y, ref SkinVoice.<>c__DisplayClass30_0 A_1)
		{
			for (int i = A_1.w - 1; i >= 0; i--)
			{
				if (A_1.tex.GetPixel(i, y).a > 0.5f)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x04000642 RID: 1602
		internal static readonly Dictionary<string, List<AudioClip>> Voice = new Dictionary<string, List<AudioClip>>(StringComparer.OrdinalIgnoreCase);

		// Token: 0x04000643 RID: 1603
		private static readonly Dictionary<string, byte[]> skin = new Dictionary<string, byte[]>();

		// Token: 0x04000644 RID: 1604
		private static readonly HashSet<int> done = new HashSet<int>();

		// Token: 0x04000645 RID: 1605
		private static ConfigEntry<string> skinName;

		// Token: 0x04000646 RID: 1606
		private static ConfigEntry<string> voiceName;

		// Token: 0x04000647 RID: 1607
		public static ConfigEntry<bool> StandardTail;

		// Token: 0x04000648 RID: 1608
		private static readonly List<Texture2D> tailTex = new List<Texture2D>();

		// Token: 0x04000649 RID: 1609
		private static byte[] tailGame;

		// Token: 0x0400064A RID: 1610
		private static byte[] tailSlim;

		// Token: 0x0400064B RID: 1611
		private static byte[] tailStandard;

		// Token: 0x0400064C RID: 1612
		private const string TailName = "experimentTail";

		// Token: 0x0400064D RID: 1613
		private const string SlimSuffix = "@slim";

		// Token: 0x0400064E RID: 1614
		internal static ConfigEntry<float> VoiceVolume;

		// Token: 0x0400064F RID: 1615
		private static float rescan;

		// Token: 0x04000650 RID: 1616
		private static readonly Dictionary<byte[], byte[]> flat = new Dictionary<byte[], byte[]>();

		// Token: 0x04000651 RID: 1617
		internal static ConfigEntry<bool> FlatBelly;

		// Token: 0x04000652 RID: 1618
		internal static ConfigEntry<bool> EffortOriginalEntry;
	}
}
