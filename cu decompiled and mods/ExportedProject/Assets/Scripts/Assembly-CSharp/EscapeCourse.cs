using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeCourse : TutorialCourse
{
	public override IEnumerator Sequence()
	{
		TutorialCourse.tutorial.forcePlayerAlive = false;
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		TutorialCourse.tutorial.Speak(Locale.GetOther("escapecourse1"));
		yield return new WaitForSeconds(7.5f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetOther("escapecourse2"));
		while (TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) > 0.5f)
		{
			TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position;
			yield return null;
		}
		TurretScript[] turrets = TutorialCourse.tutorial.turrets;
		foreach (TurretScript turretScript in turrets)
		{
			if ((bool)turretScript)
			{
				turretScript.enabled = false;
			}
		}
		TutorialCourse.tutorial.Grab(TutorialCourse.body.transform);
		TutorialCourse.tutorial.handPos = Vector2.up * 16f;
		yield return new WaitForSeconds(5f);
		TutorialCourse.tutorial.doorAnimator.Play("TutorialDoorSwing");
		Sound.Play("tutorialdoorsequence", TutorialCourse.tutorial.doorAnimator.transform.position, twoDimensional: false, pitchShift: false);
		yield return new WaitForSeconds(2.06f);
		Limb limb = TutorialCourse.body.limbs.PickRandom();
		TutorialCourse.body.DoGoreSound();
		limb.skinHealth -= 16f;
		limb.pain += 18f;
		limb.bleedAmount += 4f;
		while (TutorialCourse.body.consciousness > 5f)
		{
			TutorialCourse.body.consciousness -= Time.deltaTime * (Body.consciousnessRiseRate + 12f);
			yield return null;
		}
		yield return new WaitUntil(() => !TutorialCourse.body.conscious);
		SteamManager.TryUnlockAchievement("LEVELONEBREACH", force: true);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("PreGen");
		yield return null;
	}

	public override string LocaleName()
	{
		return "escapecourse";
	}
}
