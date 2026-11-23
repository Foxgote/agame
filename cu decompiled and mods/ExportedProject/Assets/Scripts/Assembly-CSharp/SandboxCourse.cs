using System.Collections;
using UnityEngine;

public class SandboxCourse : TutorialCourse
{
	public override IEnumerator Sequence()
	{
		TutorialCourse.tutorial.checkForEscape = false;
		TutorialCourse.tutorial.forcePlayerAlive = false;
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.body.transform.position = Vector3.down * 106f;
		PlayerCamera.main.transform.position = TutorialCourse.body.transform.position;
		TutorialCourse.tutorial.armSpeedMultiplier = 3f;
		TutorialCourse.tutorial.objectToCreate = "terminal";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		TutorialCourse.tutorial.handPos = new Vector2(-4f, -107.5f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		yield return new WaitForSeconds(1f);
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.handPos = Vector2.zero;
		yield return null;
	}

	public override string LocaleName()
	{
		return "sandbox";
	}
}
