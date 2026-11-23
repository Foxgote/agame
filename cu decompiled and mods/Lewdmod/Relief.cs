using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x020000F1 RID: 241
	internal static class Relief
	{
		// Token: 0x170000B8 RID: 184
		// (get) Token: 0x06000550 RID: 1360 RVA: 0x00057AC4 File Offset: 0x00055CC4
		// (set) Token: 0x06000551 RID: 1361 RVA: 0x00057AEE File Offset: 0x00055CEE
		public static bool Active
		{
			get
			{
				if (Relief.active && !Relief.sessionBody)
				{
					Relief.active = false;
					Relief.toyBoost = 1f;
				}
				return Relief.active;
			}
			private set
			{
				Relief.active = value;
			}
		}

		// Token: 0x170000B9 RID: 185
		// (get) Token: 0x06000552 RID: 1362 RVA: 0x00057AF6 File Offset: 0x00055CF6
		private static State S
		{
			get
			{
				return Plugin.S;
			}
		}

		// Token: 0x06000553 RID: 1363 RVA: 0x00057B00 File Offset: 0x00055D00
		public static void Update(Body b, float dt)
		{
			Witness.Update(b);
			if (Keys.Down(Plugin.ReliefKey.Value) && Time.unscaledTime - Relief.lastToggle > 0.4f)
			{
				Relief.lastToggle = Time.unscaledTime;
				if (Puppet.Active)
				{
					Relief.Say(b, "puppet_noh");
				}
				else if (!Scenes.Busy)
				{
					if (Relief.Active)
					{
						Relief.stopRequested = true;
					}
					else
					{
						Relief.TryStart(b, false);
					}
				}
			}
			if (!Relief.Active && !Puppet.Active && Plugin.Compulsion.Value && Relief.S.corruption >= 80f && Relief.S.arousal >= 90f && Relief.S.orgasmTimer <= 0f)
			{
				Relief.compulsionTimer -= dt;
				if (Relief.compulsionTimer <= 0f)
				{
					Relief.compulsionTimer = 10f;
					string text;
					if (Random.value < 0.15f && Relief.CanStart(b, out text, true))
					{
						Relief.TryStart(b, true);
					}
				}
			}
			Relief.UpdateBlush(b, dt);
		}

		// Token: 0x06000554 RID: 1364 RVA: 0x00057C08 File Offset: 0x00055E08
		private static bool CanStart(Body b, out string reason, bool compelled = false)
		{
			reason = null;
			if (!b.conscious || b.sleeping || Scenes.Busy)
			{
				reason = "relief_cant";
			}
			else if (!b.standing || b.currentClimbable || b.inWater || b.rb.velocity.magnitude > 1f || b.exercising)
			{
				reason = "relief_notnow";
			}
			else if (Relief.Quake)
			{
				reason = "relief_quake";
			}
			else if (Relief.WaterAtHips(b))
			{
				reason = "relief_water";
			}
			else if (b.bothHandsUnusable || Devices.HandsBound(b))
			{
				reason = "relief_hands";
			}
			else if (Devices.Belted(b))
			{
				reason = "relief_belt";
			}
			else
			{
				string text;
				reason = (text = Witness.Why(b, compelled));
				if (text == null && Relief.S.arousal < 15f && Relief.S.lust < 30f)
				{
					reason = "relief_nomood";
				}
			}
			return reason == null;
		}

		// Token: 0x06000555 RID: 1365 RVA: 0x00057D10 File Offset: 0x00055F10
		private static void TryStart(Body b, bool compelled)
		{
			string text;
			if (!Relief.CanStart(b, out text, compelled))
			{
				Relief.Say(b, text);
				return;
			}
			Relief.Say(b, compelled ? "relief_compelled" : "relief_start");
			b.StartCoroutine(Relief.Session(b, compelled, false));
			TraderSuit.WatchRelief(b);
		}

		// Token: 0x06000556 RID: 1366 RVA: 0x00057D5A File Offset: 0x00055F5A
		public static void StartForced(Body b)
		{
			if (Relief.Active || !b.conscious || !b.standing || Scenes.Busy)
			{
				return;
			}
			Relief.Say(b, "puppet_use");
			b.StartCoroutine(Relief.Session(b, true, true));
		}

		// Token: 0x06000557 RID: 1367 RVA: 0x00057D98 File Offset: 0x00055F98
		public static void StartWithToy(Body b, float boost, bool plug)
		{
			if (Relief.Active)
			{
				return;
			}
			string text;
			if (!Relief.CanStart(b, out text, false))
			{
				Relief.Say(b, text);
				return;
			}
			Relief.toyBoost = Mathf.Max(1f, boost);
			Relief.Say(b, plug ? "toy_useplug" : "toy_use");
			b.StartCoroutine(Relief.Session(b, false, false));
		}

		// Token: 0x06000558 RID: 1368 RVA: 0x00057DF4 File Offset: 0x00055FF4
		public static void ForceStop()
		{
			if (Relief.Active)
			{
				Relief.stopRequested = true;
			}
		}

		// Token: 0x06000559 RID: 1369 RVA: 0x00057E03 File Offset: 0x00056003
		private static IEnumerator Session(Body b, bool compelled, bool puppet)
		{
			Relief.Active = true;
			Relief.sessionBody = b;
			Relief.stopRequested = false;
			Traverse tb = Traverse.Create(b);
			tb.Property("exercising", null).SetValue(true);
			tb.Field("movingAllowed").SetValue(false);
			b.bodyAnimator.Play("ExperimentSit");
			b.armsAnimator.Play("ArmsSit");
			float moanTimer = 2f;
			float lineTimer = 4f;
			float breathTimer = 0.5f;
			float heartTimer = 0f;
			float lastPain = b.averagePain;
			AudioSource rustle = Sfx.Loop(b, "loop_rustle");
			AudioSource wet = Sfx.Loop(b, "loop_wet");
			float fadeIn = 0f;
			string endReason = null;
			bool finished = false;
			Plugin.Log.LogInfo(string.Format("Relief started (compelled {0}), arousal {1:0}", compelled, Relief.S.arousal));
			for (;;)
			{
				float num = Mathf.Min(Time.deltaTime, 0.25f);
				float averagePain = b.averagePain;
				if (Relief.stopRequested)
				{
					endReason = (puppet ? "silent" : "stop");
				}
				else if (puppet && !Puppet.Active && !Suit.DroneMerged)
				{
					endReason = "silent";
				}
				else if (!puppet && Relief.MoveKeyDown())
				{
					endReason = "stop";
				}
				else if (!b.conscious || !b.alive)
				{
					endReason = "silent";
				}
				else if (!b.standing || b.inWater)
				{
					endReason = "relief_interrupted";
				}
				else if (Relief.Quake)
				{
					endReason = "relief_quake";
				}
				else if (Relief.WaterAtHips(b))
				{
					endReason = "relief_water";
				}
				else if (averagePain - lastPain > 8f && Relief.S.submission < 50f)
				{
					endReason = "relief_hurt";
				}
				else if (averagePain > 60f && Relief.S.submission < 50f)
				{
					endReason = "relief_toopainful";
				}
				else if (b.stamina < 3f)
				{
					endReason = "relief_tired";
				}
				else if (!puppet)
				{
					string text = Witness.Why(b, compelled);
					if (text != null && text != "relief_attacked")
					{
						endReason = "relief_spotted";
					}
				}
				if (endReason != null)
				{
					goto IL_0980;
				}
				lastPain = averagePain;
				float num2 = 3f * (0.5f + Relief.S.lust * 0.01f) * (Relief.S.inHeat ? 1.5f : 1f) * (1f + Relief.S.corruption * 0.005f) * Mathf.Clamp01(b.stamina / 30f + 0.2f) * ((Relief.S.orgasmTimer > 0f) ? 0.25f : 1f) * Relief.toyBoost * Plugs.SessionBoost(b);
				if (averagePain > 25f)
				{
					if (Relief.S.submission >= 50f)
					{
						num2 += averagePain * 0.02f * Relief.S.submission * 0.01f;
					}
					else
					{
						num2 *= 0.3f;
					}
				}
				Relief.S.arousal = Mathf.Min(100f, Relief.S.arousal + num2 * num);
				b.stamina = Mathf.Max(0f, b.stamina - 0.6f * num);
				b.energy = Mathf.Max(0f, b.energy - 0.02f * num);
				b.temperature += 0.01f * num;
				float num3 = Relief.S.arousal * 0.01f;
				fadeIn = Mathf.MoveTowards(fadeIn, 1f, num * 0.8f);
				if (rustle)
				{
					rustle.volume = Mathf.Lerp(0.15f, 0.35f, num3) * fadeIn;
					rustle.pitch = Mathf.Lerp(0.95f, 1.15f, num3);
				}
				if (wet)
				{
					wet.volume = Mathf.Clamp01((num3 - 0.35f) / 0.4f) * 0.45f * fadeIn;
					wet.pitch = Mathf.Lerp(0.9f, 1.25f, num3);
				}
				breathTimer -= num;
				if (breathTimer <= 0f)
				{
					breathTimer = Mathf.Lerp(3.5f, 1f, num3) * Random.Range(0.8f, 1.25f);
					Sfx.Play(b, Mathf.Lerp(0.3f, 0.6f, num3), new string[] { "breath" });
				}
				if (num3 >= 0.7f)
				{
					heartTimer -= num;
					if (heartTimer <= 0f)
					{
						heartTimer = Mathf.Lerp(0.9f, 0.45f, (num3 - 0.7f) / 0.3f);
						Sfx.Play(b, 0.35f, new string[] { "heartbeat" });
					}
				}
				if (Relief.S.arousal > 60f)
				{
					b.eyeCloseTime = Mathf.Max(b.eyeCloseTime, 0.1f);
				}
				moanTimer -= num;
				if (moanTimer <= 0f)
				{
					moanTimer = Mathf.Lerp(7f, 2.5f, num3) * Random.Range(0.8f, 1.2f);
					b.eatTime = Mathf.Max(b.eatTime, 0.6f);
					if (num3 < 0.6f)
					{
						Sfx.Play(b, Mathf.Lerp(0.35f, 0.6f, num3), new string[]
						{
							puppet ? "muffled" : "moan_soft",
							"moan_soft",
							"moan"
						});
					}
					else
					{
						Sfx.Play(b, Mathf.Lerp(0.6f, 0.9f, num3), new string[]
						{
							puppet ? "muffled" : "moan",
							"moan",
							"moan_soft"
						});
					}
				}
				lineTimer -= num;
				if (lineTimer <= 0f)
				{
					lineTimer = Random.Range(6f, 11f);
					if (Random.value < 0.6f)
					{
						Relief.Say(b, puppet ? "puppet_line" : ((Relief.S.arousal >= 80f) ? "relief_close" : "relief_line"));
					}
				}
				if (Relief.S.arousal >= 99f)
				{
					break;
				}
				yield return null;
			}
			finished = true;
			Plugin.Orgasm(b, !puppet);
			if (puppet)
			{
				Semen.Add(b, 35f);
			}
			Relief.Say(b, "relief_done");
			float dripAt = 0.6f;
			float t = 0f;
			while (t < 3f && b.standing && b.conscious)
			{
				if (wet)
				{
					wet.volume = Mathf.Lerp(0.45f, 0.1f, t / 3f);
				}
				if (rustle)
				{
					rustle.volume = Mathf.Lerp(0.35f, 0.05f, t / 3f);
				}
				if (t >= dripAt)
				{
					dripAt += Random.Range(0.4f, 0.9f);
					Sfx.Play(b, 0.4f, new string[] { "drip" });
				}
				yield return null;
				t += Time.deltaTime;
			}
			Sfx.Play(b, 0.6f, new string[] { "sigh" });
			IL_0980:
			if (!finished && endReason == "stop" && Relief.S.arousal >= 85f)
			{
				Relief.Edge(b);
			}
			else if (endReason != null && endReason != "stop" && endReason != "silent")
			{
				Relief.Say(b, endReason);
			}
			b.StartCoroutine(Sfx.FadeOut(rustle, 0.6f));
			b.StartCoroutine(Sfx.FadeOut(wet, 0.6f));
			tb.Property("exercising", null).SetValue(false);
			tb.Field("movingAllowed").SetValue(true);
			if (b.standing)
			{
				b.bodyAnimator.Play("Grounded");
				b.armsAnimator.Play("Grounded");
			}
			Relief.Active = false;
			Relief.toyBoost = 1f;
			Plugin.Log.LogInfo(string.Format("Relief ended ({0}), arousal {1:0}", finished ? "orgasm" : endReason, Relief.S.arousal));
			yield break;
		}

		// Token: 0x0600055A RID: 1370 RVA: 0x00057E20 File Offset: 0x00056020
		private static void Edge(Body b)
		{
			Relief.S.edges++;
			Mood.Change(b, -8f);
			Relief.S.lust = Mathf.Min(100f, Relief.S.lust + 10f);
			Relief.S.submission = Mathf.Min(100f, Relief.S.submission + 3f);
			Relief.S.frustrationTimer = 60f;
			Relief.Say(b, "relief_edge");
		}

		// Token: 0x170000BA RID: 186
		// (get) Token: 0x0600055B RID: 1371 RVA: 0x00057EAC File Offset: 0x000560AC
		private static bool Quake
		{
			get
			{
				return WorldGeneration.world && WorldGeneration.world.earthquakeTime > 0f;
			}
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00057ED0 File Offset: 0x000560D0
		private static bool WaterAtHips(Body b)
		{
			WorldGeneration world = WorldGeneration.world;
			Limb limb = b.LimbByName("DownTorso");
			if (!world || !limb || !FluidManager.main)
			{
				return false;
			}
			bool flag;
			try
			{
				flag = FluidManager.main.WaterInfo(world.WorldToBlockPos(limb.transform.position)).Item3 > 0;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x0600055D RID: 1373 RVA: 0x00057F50 File Offset: 0x00056150
		private static bool MoveKeyDown()
		{
			string[] array = new string[] { "left", "right", "up", "jump", "ragdoll", "attack" };
			for (int i = 0; i < array.Length; i++)
			{
				if (Keys.Held(KeyBinds.GetBind(array[i])))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600055E RID: 1374 RVA: 0x00057FB4 File Offset: 0x000561B4
		internal static void ResetLines()
		{
			Relief.variants.Clear();
		}

		// Token: 0x0600055F RID: 1375 RVA: 0x00057FC0 File Offset: 0x000561C0
		internal static void Say(Body b, string key)
		{
			RunStats.OnLine(key);
			if (!b || !b.talker)
			{
				return;
			}
			string[] array;
			if (!Relief.variants.TryGetValue(key, out array))
			{
				List<string> list = new List<string>();
				if (Plugin.Text.ContainsKey(key))
				{
					list.Add(key);
				}
				for (int i = 1; i < 100; i++)
				{
					if (Plugin.Text.ContainsKey(key + i.ToString()))
					{
						list.Add(key + i.ToString());
					}
					else if (i > 9)
					{
						break;
					}
				}
				array = (Relief.variants[key] = list.ToArray());
			}
			if (array.Length != 0)
			{
				b.talker.Talk(Plugin.T(array[Random.Range(0, array.Length)]), null, false, false);
			}
		}

		// Token: 0x06000560 RID: 1376 RVA: 0x00058088 File Offset: 0x00056288
		private static void UpdateBlush(Body b, float dt)
		{
			if (Relief.blushBody != b || !Relief.blush)
			{
				Relief.blushBody = b;
				Relief.blush = Relief.CreateBlush(b);
			}
			if (!Relief.blush)
			{
				return;
			}
			float num = Mathf.Clamp01((Relief.S.arousal - 40f) / 40f) * 0.85f;
			Relief.blushAlpha = Mathf.MoveTowards(Relief.blushAlpha, num, dt * 0.5f);
			Relief.blush.color = new Color(1f, 1f, 1f, Relief.blushAlpha);
			Relief.blush.enabled = Relief.blushAlpha > 0.01f;
		}

		// Token: 0x06000561 RID: 1377 RVA: 0x00058140 File Offset: 0x00056340
		private static SpriteRenderer CreateBlush(Body b)
		{
			FacialExpression componentInChildren = b.GetComponentInChildren<FacialExpression>(true);
			if (!componentInChildren || !componentInChildren.head)
			{
				return null;
			}
			Sprite sprite = (componentInChildren.defaultHead ? componentInChildren.defaultHead : componentInChildren.head.sprite);
			if (!sprite)
			{
				return null;
			}
			Relief.<>c__DisplayClass30_0 CS$<>8__locals1;
			CS$<>8__locals1.w = (int)sprite.rect.width;
			CS$<>8__locals1.h = (int)sprite.rect.height;
			CS$<>8__locals1.tex = new Texture2D(CS$<>8__locals1.w, CS$<>8__locals1.h, 4, false)
			{
				filterMode = 0,
				wrapMode = 1
			};
			Color32[] array = new Color32[CS$<>8__locals1.w * CS$<>8__locals1.h];
			CS$<>8__locals1.tex.SetPixels32(array);
			Color32 color;
			color..ctor(byte.MaxValue, 110, 150, 200);
			Color32 color2;
			color2..ctor(byte.MaxValue, 150, 180, 120);
			Relief.<CreateBlush>g__Px|30_0(13, 9, color, ref CS$<>8__locals1);
			Relief.<CreateBlush>g__Px|30_0(14, 9, color, ref CS$<>8__locals1);
			Relief.<CreateBlush>g__Px|30_0(15, 9, color2, ref CS$<>8__locals1);
			Relief.<CreateBlush>g__Px|30_0(12, 9, color2, ref CS$<>8__locals1);
			Relief.<CreateBlush>g__Px|30_0(14, 10, color2, ref CS$<>8__locals1);
			CS$<>8__locals1.tex.Apply();
			Sprite sprite2 = Sprite.Create(CS$<>8__locals1.tex, new Rect(0f, 0f, (float)CS$<>8__locals1.w, (float)CS$<>8__locals1.h), new Vector2(sprite.pivot.x / (float)CS$<>8__locals1.w, sprite.pivot.y / (float)CS$<>8__locals1.h), sprite.pixelsPerUnit);
			GameObject gameObject = new GameObject("LewdBlush", new Type[] { typeof(SpriteRenderer) });
			gameObject.transform.SetParent(componentInChildren.head.transform, false);
			SpriteRenderer component = gameObject.GetComponent<SpriteRenderer>();
			component.sprite = sprite2;
			component.sortingLayerID = componentInChildren.head.sortingLayerID;
			component.sortingOrder = componentInChildren.head.sortingOrder + 1;
			component.sharedMaterial = componentInChildren.head.sharedMaterial;
			component.enabled = false;
			return component;
		}

		// Token: 0x06000562 RID: 1378 RVA: 0x00058360 File Offset: 0x00056560
		public static void SyncFlip()
		{
			if (Relief.blush && Relief.blushBody)
			{
				SpriteRenderer spriteRenderer = (Relief.blush.transform.parent ? Relief.blush.transform.parent.GetComponent<SpriteRenderer>() : null);
				if (spriteRenderer)
				{
					Relief.blush.flipX = spriteRenderer.flipX;
					Relief.blush.flipY = spriteRenderer.flipY;
				}
			}
		}

		// Token: 0x06000564 RID: 1380 RVA: 0x00058405 File Offset: 0x00056605
		[CompilerGenerated]
		internal static void <CreateBlush>g__Px|30_0(int x, int yTop, Color32 c, ref Relief.<>c__DisplayClass30_0 A_3)
		{
			if (x >= 0 && x < A_3.w && yTop >= 0 && yTop < A_3.h)
			{
				A_3.tex.SetPixel(x, A_3.h - 1 - yTop, c);
			}
		}

		// Token: 0x04000626 RID: 1574
		private static bool active;

		// Token: 0x04000627 RID: 1575
		private static Body sessionBody;

		// Token: 0x04000628 RID: 1576
		private static bool stopRequested;

		// Token: 0x04000629 RID: 1577
		private static float lastToggle = -10f;

		// Token: 0x0400062A RID: 1578
		private static float compulsionTimer = 10f;

		// Token: 0x0400062B RID: 1579
		private static float blushAlpha;

		// Token: 0x0400062C RID: 1580
		private static SpriteRenderer blush;

		// Token: 0x0400062D RID: 1581
		private static Body blushBody;

		// Token: 0x0400062E RID: 1582
		private static float toyBoost = 1f;

		// Token: 0x0400062F RID: 1583
		private static readonly Dictionary<string, string[]> variants = new Dictionary<string, string[]>();
	}
}
