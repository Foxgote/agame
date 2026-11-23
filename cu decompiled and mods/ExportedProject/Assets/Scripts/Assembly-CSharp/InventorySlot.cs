using UnityEngine;

public class InventorySlot : MonoBehaviour
{
	public Body body;

	public Limb limb;

	public int slot;

	public bool dropWhenUnconscious;

	public int spriteSortOrder;

	public Limb[] useLimbs;

	public bool isHand;

	public bool isMainHand;

	public bool canPickUp
	{
		get
		{
			if (body.conscious)
			{
				if (limb.totalForce >= 0.1f)
				{
					if (limb.isHead)
					{
						if (!limb.dislocated)
						{
							return !body.disfigured;
						}
						return false;
					}
					return true;
				}
				return false;
			}
			return false;
		}
	}

	public float armPowerMult
	{
		get
		{
			float num = 0f;
			for (int i = 0; i < useLimbs.Length; i++)
			{
				num += body.staminaStrength.Evaluate(useLimbs[i].totalForce);
			}
			return Mathf.Min(num / (float)useLimbs.Length * body.actualStaminaStrength, (body.limbs[1].broken || body.limbs[1].dislocated || body.limbs[0].broken) ? 0.65f : 1f);
		}
	}

	private void Start()
	{
		for (int i = 0; i < body.slots.Length; i++)
		{
			if (body.slots[i] == this)
			{
				slot = i;
			}
		}
	}

	private void Update()
	{
		if (dropWhenUnconscious && (bool)limb && !canPickUp)
		{
			body.DropItem(slot);
		}
	}
}
