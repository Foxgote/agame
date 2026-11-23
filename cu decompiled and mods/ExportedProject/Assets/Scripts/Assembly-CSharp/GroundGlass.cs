using UnityEngine;

public class GroundGlass : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (!collision.gameObject.TryGetComponent<Body>(out var component) || !(base.transform.position.y < component.transform.position.y))
		{
			return;
		}
		Item wearableBySlotID = component.GetWearableBySlotID("feet");
		if ((bool)wearableBySlotID)
		{
			wearableBySlotID.SetCondition(wearableBySlotID.condition - 0.1f * wearableBySlotID.Stats.wearableHitDurabilityLossMultiplier);
			Sound.Play("glassshard", base.transform.position);
			Sound.Play("glass", base.transform.position);
			Object.Destroy(base.gameObject);
			return;
		}
		Limb limb = ((Random.value > 0.5f) ? component.limbs[11] : component.limbs[14]);
		while (limb.dismembered)
		{
			limb = limb.connectedLimbs[0];
		}
		limb.skinHealth -= Random.Range(25f, 40f);
		limb.muscleHealth -= 20f;
		limb.bleedAmount += Random.Range(5f, 20f);
		limb.pain += 80f;
		limb.shrapnel = 5;
		limb.body.adrenaline = 70f;
		limb.CreateTemporarySprite(Resources.Load<Sprite>("Special/footglass"), 0f, null, scaleLimb: false, 60f, (Limb x) => !x.hasShrapnel);
		WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/footglasshealthpanel"), flip: false, (Limb x) => !x.hasShrapnel || x.dismembered);
		Sound.Play("glassshard", base.transform.position);
		component.DoGoreSound();
		component.talker.Invoke("TalkGlass", 30f);
		Object.Destroy(base.gameObject);
	}
}
