using UnityEngine;

internal class CrystalHealing : CrystalEffect
{
	private bool activated;

	private Vector2 origPos;

	public override void Start()
	{
		crystal.SetColor(new Color32(219, byte.MaxValue, 89, 200));
		origPos = crystal.transform.position;
	}

	public override void Touched(GameObject touched)
	{
		if (!activated && Utils.GetBody(touched, out var body) && body.alive)
		{
			PlayerCamera.main.StartCoroutine("FlashBrief");
			crystal.SetColor(Color.black);
			body.talker.Talk(Locale.GetCharacter("magicheal"));
			body.brainHealth = Mathf.Lerp(body.brainHealth, 100f, 0.1f);
			Limb[] limbs = body.limbs;
			foreach (Limb obj in limbs)
			{
				obj.bleedAmount -= 10f;
				obj.muscleHealth += 25f;
				obj.skinHealth += 25f;
				obj.infectionAmount -= 40f;
				obj.pain = 0f;
			}
			body.happiness += 2f;
			body.internalBleeding *= 0.5f;
			body.septicShock *= 0.5f;
			body.sicknessAmount *= 0.66f;
			body.strokeAmount *= 0.1f;
			body.temperature = 37f;
			body.thirst = Mathf.Lerp(body.thirst, 125f, 0.25f);
			body.hunger = Mathf.Lerp(body.hunger, 125f, 0.25f);
			body.radiationSickness *= 0.75f;
			body.fibrillationProgress = 0f;
			body.bloodVolume = Mathf.Lerp(body.bloodVolume, 100f, 0.1f);
			Sound.Play("crystalheal", crystal.transform.position, twoDimensional: true, pitchShift: false);
			activated = true;
			crystal.SetColor(Color.black);
		}
	}

	public override void Update()
	{
		if (!activated)
		{
			crystal.transform.position = origPos + (Vector2)crystal.transform.up * (Mathf.Abs(Mathf.Sin(Time.time * 2f)) - 0.6f);
		}
	}
}
