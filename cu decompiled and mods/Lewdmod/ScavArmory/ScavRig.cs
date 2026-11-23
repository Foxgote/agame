using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace ScavArmory
{
	// Token: 0x02000041 RID: 65
	internal class ScavRig
	{
		// Token: 0x0600010B RID: 267 RVA: 0x0000F2A0 File Offset: 0x0000D4A0
		public ScavRig(Transform root, string skin, string overall, int orderBase)
		{
			this.Root = root;
			this.Body = new GameObject("Body").transform;
			this.Body.SetParent(root, false);
			foreach (KeyValuePair<string, Vector2> keyValuePair in ScavRig.Joint)
			{
				Transform transform = new GameObject(keyValuePair.Key).transform;
				this.J[keyValuePair.Key] = transform;
			}
			foreach (KeyValuePair<string, Vector2> keyValuePair2 in ScavRig.Joint)
			{
				string text;
				Transform transform2 = (ScavRig.JointParent.TryGetValue(keyValuePair2.Key, out text) ? this.J[text] : this.Body);
				Vector2 vector = (ScavRig.JointParent.TryGetValue(keyValuePair2.Key, out text) ? ScavRig.Joint[text] : Vector2.zero);
				this.J[keyValuePair2.Key].SetParent(transform2, false);
				this.J[keyValuePair2.Key].localPosition = keyValuePair2.Value - vector;
			}
			Dictionary<string, Sprite> dictionary2;
			Dictionary<string, Sprite> dictionary = (ScavArt.Skins.TryGetValue(skin, out dictionary2) ? dictionary2 : ScavArt.Skins.Values.FirstOrDefault<Dictionary<string, Sprite>>());
			Dictionary<string, Sprite> dictionary4;
			Dictionary<string, Sprite> dictionary3 = ((overall != null && ScavArt.Overalls.TryGetValue(overall, out dictionary4)) ? dictionary4 : null);
			Material material = (WorldGeneration.world ? WorldGeneration.world.defaultMat : null);
			foreach (ValueTuple<string, string, float, float, float, int> valueTuple in ScavRig.Rest)
			{
				string text2;
				Transform transform3 = (ScavRig.PartJoint.TryGetValue(valueTuple.Item1, out text2) ? this.J[text2] : this.Body);
				Vector2 vector2 = (ScavRig.PartJoint.TryGetValue(valueTuple.Item1, out text2) ? ScavRig.Joint[text2] : Vector2.zero);
				GameObject gameObject = new GameObject(valueTuple.Item1);
				gameObject.transform.SetParent(transform3, false);
				gameObject.transform.localPosition = new Vector2(valueTuple.Item3, valueTuple.Item4) - vector2;
				gameObject.transform.localRotation = Quaternion.Euler(0f, 0f, valueTuple.Item5);
				SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
				Sprite sprite;
				spriteRenderer.sprite = ((dictionary != null && dictionary.TryGetValue(valueTuple.Item2, out sprite)) ? sprite : null);
				if (material)
				{
					spriteRenderer.sharedMaterial = material;
				}
				spriteRenderer.sortingOrder = orderBase + valueTuple.Item6;
				this.Parts[valueTuple.Item1] = spriteRenderer;
				Sprite sprite2;
				if (dictionary3 != null && dictionary3.TryGetValue(valueTuple.Item2, out sprite2))
				{
					SpriteRenderer spriteRenderer2 = new GameObject("Cloth").AddComponent<SpriteRenderer>();
					spriteRenderer2.transform.SetParent(gameObject.transform, false);
					spriteRenderer2.sprite = sprite2;
					if (material)
					{
						spriteRenderer2.sharedMaterial = material;
					}
					spriteRenderer2.sortingOrder = spriteRenderer.sortingOrder + 1;
				}
			}
			this.EyesSr = this.Parts["Eyes"];
			this.eyeOpen = this.EyesSr.sprite;
			Sprite sprite3;
			this.eyeClosed = ((dictionary != null && dictionary.TryGetValue("EyeClosed", out sprite3)) ? sprite3 : this.eyeOpen);
			GameObject gameObject2 = new GameObject("Weapon");
			gameObject2.transform.SetParent(this.Body, false);
			this.Weapon = gameObject2.AddComponent<SpriteRenderer>();
			if (material)
			{
				this.Weapon.sharedMaterial = material;
			}
			this.Weapon.sortingOrder = orderBase + 165;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000F6D8 File Offset: 0x0000D8D8
		public void Blink(bool closed)
		{
			this.EyesSr.sprite = (closed ? this.eyeClosed : this.eyeOpen);
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000F6F8 File Offset: 0x0000D8F8
		public void DyeCloth(Color c)
		{
			foreach (SpriteRenderer spriteRenderer in this.Root.GetComponentsInChildren<SpriteRenderer>(true))
			{
				if (spriteRenderer.name == "Cloth")
				{
					spriteRenderer.color = c;
				}
			}
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000F740 File Offset: 0x0000D940
		public void Slime(bool ownSkin)
		{
			if (!ownSkin)
			{
				SpriteRenderer[] componentsInChildren = this.Root.GetComponentsInChildren<SpriteRenderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].color = new Color(0.45f, 0.95f, 0.5f, 0.9f);
				}
				this.EyesSr.color = new Color(0.7f, 1f, 0.6f, 1f);
			}
			Light2D light2D = new GameObject("Glow").AddComponent<Light2D>();
			light2D.transform.SetParent(this.Body, false);
			light2D.transform.localPosition = new Vector3(0f, 1f, 0f);
			light2D.color = (ownSkin ? new Color(0.55f, 1f, 0.2f) : new Color(0.4f, 1f, 0.45f));
			light2D.intensity = (ownSkin ? 0.3f : 0.5f);
			light2D.pointLightOuterRadius = 4f;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000F842 File Offset: 0x0000DA42
		public void Set(string joint, float angle)
		{
			this.J[joint].localRotation = Quaternion.Euler(0f, 0f, angle);
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000F865 File Offset: 0x0000DA65
		public static Vector2 JointAt(string joint)
		{
			return ScavRig.Joint[joint];
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000F874 File Offset: 0x0000DA74
		public static bool TwoBone(string shoulderJ, string elbowJ, Vector2 target, out float shoulder, out float elbow)
		{
			Vector2 vector = ScavRig.Joint[shoulderJ];
			Vector2 vector2 = ScavRig.Joint[elbowJ];
			Vector2 vector3 = (shoulderJ.EndsWith("F") ? ScavRig.HandFRest : ScavRig.HandBRest);
			float magnitude = (vector2 - vector).magnitude;
			float magnitude2 = (vector3 - vector2).magnitude;
			float num = Mathf.Atan2(vector2.y - vector.y, vector2.x - vector.x);
			float num2 = Mathf.Atan2(vector3.y - vector2.y, vector3.x - vector2.x);
			Vector2 vector4 = target - vector;
			float num3 = Mathf.Clamp(vector4.magnitude, Mathf.Abs(magnitude - magnitude2) + 0.01f, magnitude + magnitude2 - 0.01f);
			float num4 = Mathf.Atan2(vector4.y, vector4.x);
			float num5 = Mathf.Acos(Mathf.Clamp((magnitude * magnitude + num3 * num3 - magnitude2 * magnitude2) / (2f * magnitude * num3), -1f, 1f));
			float num6 = num4 - num5;
			float num7 = num4 + num5;
			float num8 = ((Mathf.Sin(num6) <= Mathf.Sin(num7)) ? num6 : num7);
			Vector2 vector5 = vector + new Vector2(Mathf.Cos(num8), Mathf.Sin(num8)) * magnitude;
			Vector2 vector6 = vector + vector4.normalized * num3;
			float num9 = Mathf.Atan2(vector6.y - vector5.y, vector6.x - vector5.x);
			shoulder = (num8 - num) * 57.29578f;
			elbow = (num9 - num2) * 57.29578f - shoulder;
			return true;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000FA2C File Offset: 0x0000DC2C
		public Vector2 HandAt(float shoulder)
		{
			Vector2 vector = ScavRig.Joint["ShoulderF"];
			Vector2 vector2 = new Vector2(0.019f, -1.296f) - vector;
			return vector + Quaternion.Euler(0f, 0f, shoulder) * vector2;
		}

		// Token: 0x04000113 RID: 275
		[TupleElementNames(new string[] { "part", "tex", "x", "y", "rot", "order" })]
		private static readonly ValueTuple<string, string, float, float, float, int>[] Rest = new ValueTuple<string, string, float, float, float, int>[]
		{
			new ValueTuple<string, string, float, float, float, int>("UpArmB", "UpArm", -0.054f, 0.768f, 2.4f, -50),
			new ValueTuple<string, string, float, float, float, int>("DownArmB", "DownArm", 0.108f, -0.465f, 11f, -60),
			new ValueTuple<string, string, float, float, float, int>("HandB", "HandB", 0.312f, -1.301f, 24.4f, -40),
			new ValueTuple<string, string, float, float, float, int>("ThighB", "Thigh", 0.098f, -0.617f, 24.8f, -10),
			new ValueTuple<string, string, float, float, float, int>("CrusB", "Crus", 0.006f, -1.28f, 308.5f, -20),
			new ValueTuple<string, string, float, float, float, int>("FootB", "Foot", 0.012f, -2.073f, 48.3f, -30),
			new ValueTuple<string, string, float, float, float, int>("Tail", "Tail", -0.015f, -0.067f, 20.5f, 0),
			new ValueTuple<string, string, float, float, float, int>("DownTorso", "DownTorso", 0f, 0.143f, 357.5f, 10),
			new ValueTuple<string, string, float, float, float, int>("UpTorso", "UpTorso", 0.043f, 1.141f, 357.5f, 20),
			new ValueTuple<string, string, float, float, float, int>("Head", "HeadBack", 0.201f, 2.373f, 349.6f, 50),
			new ValueTuple<string, string, float, float, float, int>("Eyes", "EyeOpen", 0.201f, 2.373f, 349.6f, 53),
			new ValueTuple<string, string, float, float, float, int>("FootF", "Foot", -0.753f, -2.034f, 35.5f, 80),
			new ValueTuple<string, string, float, float, float, int>("CrusF", "Crus", -0.558f, -1.291f, 289f, 90),
			new ValueTuple<string, string, float, float, float, int>("ThighF", "Thigh", -0.17f, -0.673f, 354.5f, 100),
			new ValueTuple<string, string, float, float, float, int>("UpArmF", "UpArm", -0.176f, 0.781f, 348.5f, 150),
			new ValueTuple<string, string, float, float, float, int>("DownArmF", "DownArm", -0.126f, -0.442f, 11.3f, 160),
			new ValueTuple<string, string, float, float, float, int>("HandF", "HandF", 0.019f, -1.296f, 4.9f, 170)
		};

		// Token: 0x04000114 RID: 276
		private static readonly Dictionary<string, Vector2> Joint = new Dictionary<string, Vector2>
		{
			{
				"Neck",
				new Vector2(0.12f, 1.85f)
			},
			{
				"ShoulderF",
				new Vector2(-0.12f, 1.45f)
			},
			{
				"ElbowF",
				new Vector2(-0.15f, 0.17f)
			},
			{
				"ShoulderB",
				new Vector2(0f, 1.43f)
			},
			{
				"ElbowB",
				new Vector2(0.06f, 0.15f)
			},
			{
				"HipF",
				new Vector2(-0.06f, -0.08f)
			},
			{
				"KneeF",
				new Vector2(-0.36f, -0.98f)
			},
			{
				"HipB",
				new Vector2(0.08f, -0.06f)
			},
			{
				"KneeB",
				new Vector2(0.05f, -0.95f)
			}
		};

		// Token: 0x04000115 RID: 277
		private static readonly Dictionary<string, string> PartJoint = new Dictionary<string, string>
		{
			{ "Head", "Neck" },
			{ "Eyes", "Neck" },
			{ "UpArmF", "ShoulderF" },
			{ "DownArmF", "ElbowF" },
			{ "HandF", "ElbowF" },
			{ "UpArmB", "ShoulderB" },
			{ "DownArmB", "ElbowB" },
			{ "HandB", "ElbowB" },
			{ "ThighF", "HipF" },
			{ "CrusF", "KneeF" },
			{ "FootF", "KneeF" },
			{ "ThighB", "HipB" },
			{ "CrusB", "KneeB" },
			{ "FootB", "KneeB" }
		};

		// Token: 0x04000116 RID: 278
		private static readonly Dictionary<string, string> JointParent = new Dictionary<string, string>
		{
			{ "ElbowF", "ShoulderF" },
			{ "ElbowB", "ShoulderB" },
			{ "KneeF", "HipF" },
			{ "KneeB", "HipB" }
		};

		// Token: 0x04000117 RID: 279
		public const float RootHeight = 2.55f;

		// Token: 0x04000118 RID: 280
		public readonly Transform Root;

		// Token: 0x04000119 RID: 281
		public readonly Transform Body;

		// Token: 0x0400011A RID: 282
		public readonly Dictionary<string, Transform> J = new Dictionary<string, Transform>();

		// Token: 0x0400011B RID: 283
		public readonly Dictionary<string, SpriteRenderer> Parts = new Dictionary<string, SpriteRenderer>();

		// Token: 0x0400011C RID: 284
		public SpriteRenderer Weapon;

		// Token: 0x0400011D RID: 285
		public SpriteRenderer EyesSr;

		// Token: 0x0400011E RID: 286
		private Sprite eyeOpen;

		// Token: 0x0400011F RID: 287
		private Sprite eyeClosed;

		// Token: 0x04000120 RID: 288
		private static readonly Vector2 HandBRest = new Vector2(0.312f, -1.301f);

		// Token: 0x04000121 RID: 289
		private static readonly Vector2 HandFRest = new Vector2(0.019f, -1.296f);
	}
}
