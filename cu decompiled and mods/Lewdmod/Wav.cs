using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000E1 RID: 225
	internal static class Wav
	{
		// Token: 0x0600051F RID: 1311 RVA: 0x00055ED4 File Offset: 0x000540D4
		public static AudioClip Load(string path, string name)
		{
			byte[] array = File.ReadAllBytes(path);
			if (array.Length < 44 || array[0] != 82 || array[8] != 87)
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 12;
			while (num4 + 8 <= array.Length)
			{
				string @string = Encoding.ASCII.GetString(array, num4, 4);
				int num5 = BitConverter.ToInt32(array, num4 + 4);
				int num6 = num4 + 8;
				if (@string == "fmt ")
				{
					if (BitConverter.ToInt16(array, num6) != 1)
					{
						return null;
					}
					num = (int)BitConverter.ToInt16(array, num6 + 2);
					num2 = BitConverter.ToInt32(array, num6 + 4);
					num3 = (int)BitConverter.ToInt16(array, num6 + 14);
				}
				else if (@string == "data" && num > 0)
				{
					num5 = Math.Min(num5, array.Length - num6);
					int num7 = num3 / 8;
					int num8 = num5 / num7;
					float[] array2 = new float[num8];
					for (int i = 0; i < num8; i++)
					{
						int num9 = num6 + i * num7;
						array2[i] = ((num3 == 16) ? ((float)BitConverter.ToInt16(array, num9) / 32768f) : ((num3 == 8) ? ((float)(array[num9] - 128) / 128f) : ((num3 == 24) ? ((float)((((int)array[num9] << 8) | ((int)array[num9 + 1] << 16) | ((int)array[num9 + 2] << 24)) >> 8) / 8388608f) : 0f)));
					}
					AudioClip audioClip = AudioClip.Create(name, num8 / num, num, num2, false);
					audioClip.SetData(array2, 0);
					return audioClip;
				}
				num4 = num6 + num5 + (num5 & 1);
			}
			return null;
		}
	}
}
