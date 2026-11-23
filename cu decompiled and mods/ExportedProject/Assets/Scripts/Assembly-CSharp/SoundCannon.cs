using UnityEngine;

public class SoundCannon : MonoBehaviour
{
	private bool charging;

	private bool spent;

	private float checkTime;

	public float maxDistance = 50f;

	private float chargeTime;

	private bool CanSeePos(Vector2 pos)
	{
		RaycastHit2D[] array = Physics2D.LinecastAll(base.transform.position, pos, LayerMask.GetMask("Ground"));
		bool flag = false;
		RaycastHit2D[] array2 = array;
		foreach (RaycastHit2D raycastHit2D in array2)
		{
			if (raycastHit2D.transform != base.transform)
			{
				flag = true;
				break;
			}
		}
		return !flag;
	}

	private void Update()
	{
		checkTime -= Time.deltaTime;
		if (checkTime < 0f && !charging && !spent && WorldGeneration.world.timeSinceFinishedGeneration > 5f && !WorldGeneration.world.generatingWorld)
		{
			Vector2 vector = PlayerCamera.main.body.transform.position;
			checkTime = 0.5f;
			if (Vector2.Distance(base.transform.position, vector) < maxDistance && CanSeePos(vector))
			{
				charging = true;
				Sound.Play("sonarcharge", base.transform.position, twoDimensional: true, pitchShift: false);
				PlayerCamera.main.WarnSoundCannon(base.transform.position);
			}
		}
		if (!charging)
		{
			return;
		}
		Vector2 vector2 = PlayerCamera.main.body.transform.position;
		chargeTime += Time.deltaTime;
		base.transform.GetChild(0).eulerAngles = new Vector3(0f, 0f, Mathf.LerpAngle(base.transform.GetChild(0).eulerAngles.z, Vector2.SignedAngle(Vector2.up, (vector2 - (Vector2)base.transform.position).normalized), Time.deltaTime * 4f));
		if (!(chargeTime > 5f))
		{
			return;
		}
		spent = true;
		charging = false;
		Body body = PlayerCamera.main.body;
		if (CanSeePos(vector2) && FluidManager.main.HasLiquid(WorldGeneration.world.WorldToBlockPos(base.transform.position)) == PlayerCamera.main.body.inWater)
		{
			if (body.bodyAffect.wasWater)
			{
				Sound.Play("sonarmegaouchfixed", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: false, ignoreMixer: true);
				body.hearingLoss = 100f;
				body.internalBleeding += 100f;
				body.happiness -= 10f;
				body.shock = 100f;
				body.limbs[0].pain = 100f;
				PlayerCamera.main.shaker.Shake(3000f);
				body.brainHealth -= 20f;
				PlayerCamera.main.bonusAbber = -70f;
				PlayerCamera.main.globalMuteTime += 120f;
			}
			else
			{
				Sound.Play("sonarouch", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: false, ignoreMixer: true);
				body.hearingLoss += 60f;
				body.internalBleeding += Random.Range(20f, 30f);
				body.happiness -= 2f;
				body.shock = 65f;
				body.limbs[0].pain = 80f;
				PlayerCamera.main.shaker.Shake(1000f);
				body.brainHealth -= 5f;
				PlayerCamera.main.bonusAbber = -30f;
				PlayerCamera.main.globalMuteTime += 30f;
			}
			Sound.Play("tinnitus", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: false, ignoreMixer: true);
			body.consciousness -= 69f;
			body.eyeCloseTime = 10f;
			body.eyeScareTime = 25f;
			body.adrenaline += 100f;
			body.respiratoryRate = 0f;
			Limb[] limbs = body.limbs;
			foreach (Limb obj in limbs)
			{
				obj.muscleHealth -= Random.Range(20f, 30f);
				obj.pain += 40f;
			}
			body.talker.Talk("AAAHHHHHH!!!", null, force: true, resetTalkTimer: true);
		}
		else
		{
			Sound.Play("sonarouchblocked", base.transform.position, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: false, ignoreMixer: true);
			body.consciousness -= 30f;
			body.eyeScareTime = 4f;
			PlayerCamera.main.shaker.Shake();
			PlayerCamera.main.globalMuteTime += 3f;
		}
	}
}
