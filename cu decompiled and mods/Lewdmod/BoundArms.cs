using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace LewdStatus
{
	// Token: 0x02000070 RID: 112
	internal static class BoundArms
	{
		// Token: 0x0600023B RID: 571 RVA: 0x0002BC50 File Offset: 0x00029E50
		[return: TupleElementNames(new string[] { "limb", "delta" })]
		private static ValueTuple<string, float>[] PoseFor(Body b)
		{
			if (!Devices.On || !b || !b.alive || !PlayerCamera.main || PlayerCamera.main.body != b)
			{
				return null;
			}
			if (Devices.Worn(b, Devices.Kind.Jacket))
			{
				return BoundArms.Crossed;
			}
			if (Devices.Worn(b, Devices.Kind.Wrists))
			{
				return BoundArms.Behind;
			}
			return null;
		}

		// Token: 0x0600023C RID: 572 RVA: 0x0002BCC0 File Offset: 0x00029EC0
		public static void Apply(Body b, bool physics)
		{
			ValueTuple<string, float>[] array = BoundArms.PoseFor(b);
			if (array == null)
			{
				return;
			}
			float num = (b.isRight ? 1f : (-1f));
			foreach (ValueTuple<string, float> valueTuple in array)
			{
				string item = valueTuple.Item1;
				float item2 = valueTuple.Item2;
				Limb limb = b.LimbByName(item);
				if (limb && !limb.dismembered && limb.joint && limb.joint.connectedBody)
				{
					Rigidbody2D connectedBody = limb.joint.connectedBody;
					string name = connectedBody.name;
					if (BoundArms.Rest.ContainsKey(name))
					{
						if (physics)
						{
							Rigidbody2D component = limb.GetComponent<Rigidbody2D>();
							if (component)
							{
								Vector3 lossyScale = connectedBody.transform.lossyScale;
								Vector3 lossyScale2 = limb.transform.lossyScale;
								float num2 = connectedBody.rotation + num * (BoundArms.Rest[item] - BoundArms.Rest[name] + item2);
								Vector2 vector = connectedBody.position + Quaternion.Euler(0f, 0f, connectedBody.rotation) * Vector3.Scale(limb.joint.connectedAnchor, lossyScale);
								component.rotation = num2;
								component.position = vector - Quaternion.Euler(0f, 0f, num2) * Vector3.Scale(limb.joint.anchor, lossyScale2);
								component.angularVelocity = 0f;
								component.velocity = connectedBody.velocity;
							}
						}
						else
						{
							Transform transform = connectedBody.transform;
							float num3 = transform.eulerAngles.z + num * (BoundArms.Rest[item] - BoundArms.Rest[name] + item2);
							limb.transform.eulerAngles = new Vector3(0f, 0f, num3);
							Vector2 vector2 = transform.TransformPoint(limb.joint.connectedAnchor);
							limb.transform.position += vector2 - limb.transform.TransformPoint(limb.joint.anchor);
						}
					}
				}
			}
		}

		// Token: 0x04000246 RID: 582
		private static readonly Dictionary<string, float> Rest = new Dictionary<string, float>
		{
			{ "UpTorso", 0f },
			{ "UpArmF", 347.9f },
			{ "DownArmF", 11.6f },
			{ "HandF", 5.3f },
			{ "UpArmB", 2.7f },
			{ "DownArmB", 11.5f },
			{ "HandB", 25.7f }
		};

		// Token: 0x04000247 RID: 583
		[TupleElementNames(new string[] { "limb", "delta" })]
		private static readonly ValueTuple<string, float>[] Crossed = new ValueTuple<string, float>[]
		{
			new ValueTuple<string, float>("UpArmF", 30f),
			new ValueTuple<string, float>("DownArmF", 108f),
			new ValueTuple<string, float>("HandF", 10f),
			new ValueTuple<string, float>("UpArmB", 20f),
			new ValueTuple<string, float>("DownArmB", 114f),
			new ValueTuple<string, float>("HandB", 10f)
		};

		// Token: 0x04000248 RID: 584
		[TupleElementNames(new string[] { "limb", "delta" })]
		private static readonly ValueTuple<string, float>[] Behind = new ValueTuple<string, float>[]
		{
			new ValueTuple<string, float>("UpArmF", -30f),
			new ValueTuple<string, float>("DownArmF", 28f),
			new ValueTuple<string, float>("HandF", 0f),
			new ValueTuple<string, float>("UpArmB", -24f),
			new ValueTuple<string, float>("DownArmB", 32f),
			new ValueTuple<string, float>("HandB", 0f)
		};
	}
}
