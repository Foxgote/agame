using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using HarmonyLib;
using UnityEngine;

namespace PoseKit
{
	// Token: 0x02000007 RID: 7
	internal static class HerPuppet
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x0600000A RID: 10 RVA: 0x0000251E File Offset: 0x0000071E
		// (set) Token: 0x0600000B RID: 11 RVA: 0x00002533 File Offset: 0x00000733
		public static bool Active
		{
			get
			{
				return HerPuppet.active && HerPuppet.body;
			}
			private set
			{
				HerPuppet.active = value;
			}
		}

		// Token: 0x0600000C RID: 12 RVA: 0x0000253B File Offset: 0x0000073B
		public static bool Holds(Body b)
		{
			return HerPuppet.Active && b && b == HerPuppet.body;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x0000255C File Offset: 0x0000075C
		public static bool Begin(Body b)
		{
			if (!b || !b.bodyAnimator)
			{
				return false;
			}
			HerPuppet.body = b;
			HerPuppet.limbs.Clear();
			HerPuppet.hinge.Clear();
			foreach (Limb limb in b.limbs)
			{
				if (limb)
				{
					HerPuppet.limbs[limb.name] = limb;
					HingeJoint2D joint = limb.joint;
					if (joint && joint.connectedBody)
					{
						HerPuppet.hinge[limb.name] = new ValueTuple<string, Vector2, Vector2>(joint.connectedBody.name, joint.anchor, joint.connectedAnchor);
					}
				}
			}
			if (!b.standing)
			{
				b.Stand(true);
			}
			b.bodyAnimator.enabled = false;
			if (b.armsAnimator)
			{
				b.armsAnimator.enabled = false;
			}
			HerPuppet.savedGravity = b.rb.gravityScale;
			b.rb.gravityScale = 0f;
			Traverse.Create(b).Field("movingAllowed").SetValue(false);
			HerPuppet.Active = true;
			Kit.Log(string.Format("Her puppet on ({0} hinges, standing {1})", HerPuppet.hinge.Count, b.standing));
			return true;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000026BC File Offset: 0x000008BC
		public static void End()
		{
			Body body = HerPuppet.body;
			HerPuppet.Active = false;
			HerPuppet.poseA = (HerPuppet.poseB = null);
			if (!body)
			{
				return;
			}
			body.bodyAnimator.enabled = true;
			if (body.armsAnimator)
			{
				body.armsAnimator.enabled = true;
			}
			body.rb.gravityScale = HerPuppet.savedGravity;
			Traverse.Create(body).Field("movingAllowed").SetValue(true);
			body.bodyAnimator.Play("Grounded");
		}

		// Token: 0x0600000F RID: 15 RVA: 0x0000274C File Offset: 0x0000094C
		public static void Show(ScenePose a, ScenePose bPose, float t, Vector2 hisRoot, int hisFacing)
		{
			HerPuppet.poseA = a;
			HerPuppet.poseB = bPose;
			HerPuppet.blend = t;
			HerPuppet.facing = hisFacing;
			if (a == null)
			{
				return;
			}
			Vector2 vector = Vector2.Lerp(a.Her, bPose.Her, t);
			HerPuppet.rootWorld = hisRoot + new Vector2(vector.x * (float)hisFacing, vector.y);
		}

		// Token: 0x06000010 RID: 16 RVA: 0x000027AC File Offset: 0x000009AC
		public static void Drive(Body b)
		{
			if (!HerPuppet.Active || b != HerPuppet.body || HerPuppet.poseA == null)
			{
				return;
			}
			if (!b.standing)
			{
				b.Stand(true);
				if (!b.standing)
				{
					return;
				}
			}
			if (b.isRight != HerPuppet.facing > 0)
			{
				b.SwitchDir();
			}
			b.moveDir = Vector2.zero;
			b.rb.velocity = Vector2.zero;
			b.rb.position = HerPuppet.rootWorld;
			b.transform.position = HerPuppet.rootWorld;
			Traverse traverse = Traverse.Create(b);
			foreach (string text in new string[] { "accelRot", "attackRot", "torsoLookSmooth", "extraCrouchSmooth", "armOffset" })
			{
				if (traverse.Field(text).FieldExists())
				{
					traverse.Field(text).SetValue(0f);
				}
			}
			b.crouchAmount = 0f;
			if (traverse.Field("visualBodyOffset").FieldExists())
			{
				traverse.Field("visualBodyOffset").SetValue(Vector2.zero);
			}
			if (traverse.Field("standLerpTime").FieldExists())
			{
				traverse.Field("standLerpTime").SetValue(1f);
			}
			Vector3 vector = Vector3.Lerp(HerPuppet.poseA.Root, HerPuppet.poseB.Root, HerPuppet.blend);
			Dictionary<string, Vector2> dictionary = new Dictionary<string, Vector2>();
			Dictionary<string, float> dictionary2 = new Dictionary<string, float>();
			dictionary["DownTorso"] = Quaternion.Euler(0f, 0f, vector.z) * HerPuppet.RootPos + new Vector2(vector.x, vector.y);
			dictionary2["DownTorso"] = HerPuppet.Rest["DownTorso"] + vector.z;
			for (int j = 1; j < HerPuppet.Order.Length; j++)
			{
				string text2 = HerPuppet.Order[j];
				ValueTuple<string, Vector2, Vector2> valueTuple;
				if (HerPuppet.hinge.TryGetValue(text2, out valueTuple) && dictionary.ContainsKey(valueTuple.Item1))
				{
					float num = dictionary2[valueTuple.Item1] + (HerPuppet.Rest[text2] - HerPuppet.Rest[valueTuple.Item1]) + Poses.Lerp(HerPuppet.poseA.Limb, HerPuppet.poseB.Limb, text2, HerPuppet.blend);
					Vector2 vector2 = dictionary[valueTuple.Item1] + Quaternion.Euler(0f, 0f, dictionary2[valueTuple.Item1]) * valueTuple.Item3;
					dictionary[text2] = vector2 - Quaternion.Euler(0f, 0f, num) * valueTuple.Item2;
					dictionary2[text2] = num;
				}
			}
			Transform transform = b.bodyAnimator.transform;
			foreach (KeyValuePair<string, Vector2> keyValuePair in dictionary)
			{
				Limb limb;
				if (HerPuppet.limbs.TryGetValue(keyValuePair.Key, out limb) && limb.animLimb)
				{
					limb.animLimb.position = transform.TransformPoint(keyValuePair.Value);
					limb.animLimb.rotation = Quaternion.Euler(0f, 0f, dictionary2[keyValuePair.Key]);
				}
			}
		}

		// Token: 0x0400000C RID: 12
		private static readonly Dictionary<string, float> Rest = new Dictionary<string, float>
		{
			{ "DownTorso", 0f },
			{ "UpTorso", 0f },
			{ "Head", 0f },
			{ "UpArmF", 347.9f },
			{ "DownArmF", 11.6f },
			{ "HandF", 5.3f },
			{ "UpArmB", 2.7f },
			{ "DownArmB", 11.5f },
			{ "HandB", 25.7f },
			{ "ThighF", 354.8f },
			{ "CrusF", 287.8f },
			{ "FootF", 35.9f },
			{ "ThighB", 25f },
			{ "CrusB", 307.7f },
			{ "FootB", 49.2f }
		};

		// Token: 0x0400000D RID: 13
		private static readonly Vector2 RootPos = new Vector2(0f, 0.125f);

		// Token: 0x0400000E RID: 14
		private static readonly string[] Order = new string[]
		{
			"DownTorso", "UpTorso", "Head", "UpArmF", "DownArmF", "HandF", "UpArmB", "DownArmB", "HandB", "ThighF",
			"CrusF", "FootF", "ThighB", "CrusB", "FootB"
		};

		// Token: 0x0400000F RID: 15
		private static bool active;

		// Token: 0x04000010 RID: 16
		private static Body body;

		// Token: 0x04000011 RID: 17
		private static float savedGravity;

		// Token: 0x04000012 RID: 18
		private static readonly Dictionary<string, Limb> limbs = new Dictionary<string, Limb>();

		// Token: 0x04000013 RID: 19
		[TupleElementNames(new string[] { "parent", "anchor", "conn" })]
		private static readonly Dictionary<string, ValueTuple<string, Vector2, Vector2>> hinge = new Dictionary<string, ValueTuple<string, Vector2, Vector2>>();

		// Token: 0x04000014 RID: 20
		private static ScenePose poseA;

		// Token: 0x04000015 RID: 21
		private static ScenePose poseB;

		// Token: 0x04000016 RID: 22
		private static float blend;

		// Token: 0x04000017 RID: 23
		private static Vector2 rootWorld;

		// Token: 0x04000018 RID: 24
		private static int facing = 1;
	}
}
