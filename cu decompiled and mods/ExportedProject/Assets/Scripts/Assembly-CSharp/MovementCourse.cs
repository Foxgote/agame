using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MovementCourse : TutorialCourse
{
	public int progress;

	public override IEnumerator Sequence()
	{
		TutorialCourse.tutorial.checkForEscape = false;
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		WorldGeneration.world.GenerateObjectAtPos(Vector2Int.one * (int)WorldGeneration.world.halfWidth, Resources.Load<GameObject>("Special/MovementCourseSpawn").GetComponent<Tilemap>());
		WorldGeneration.world.GenerateEntityAtPos(Vector2.one * 0.5f, Resources.Load<GameObject>("Special/MovementCourseSpawn"));
		StartCoroutine(ForceMaxStamina());
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.body.transform.position = Vector3.down * 106f + Vector3.left * 64f;
		MusicManager.main.PlaySong(TutorialCourse.tutorial.tutorialMusic[2]);
		MusicManager.main.specialLoop = true;
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove1"));
		PlayerCamera.main.transform.position = TutorialCourse.body.transform.position;
		yield return new WaitForSeconds(2f);
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.y > -73f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove2"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.y > -54f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove3"));
		progress = 1;
		while (true)
		{
			if (TutorialCourse.body.transform.position.y < -105f)
			{
				if (!(TutorialCourse.body.lastTimeStepVelocity.magnitude >= TutorialCourse.body.jumpSpeed * 2f))
				{
					break;
				}
				ResetPlayer();
			}
			yield return null;
		}
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove4"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.y > -94f && TutorialCourse.body.transform.position.x > -14f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove5"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.y > -82f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove6"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x > -8f && TutorialCourse.body.transform.position.y > -77f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove7"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x < -10f && TutorialCourse.body.transform.position.y > -72f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove8"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("minibarrel") || (TutorialCourse.body.transform.position.x > -10f && TutorialCourse.body.transform.position.y > -52f));
		Item it = null;
		if (!TutorialCourse.body.FindByIdSurface("minibarrel", out it))
		{
			TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove9"));
		}
		else
		{
			TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove10"));
		}
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x > -10f && TutorialCourse.body.transform.position.y > -52f);
		if ((bool)it)
		{
			TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove11"));
		}
		progress = 2;
		while (true)
		{
			if (TutorialCourse.body.transform.position.y < -54f)
			{
				ResetPlayer();
			}
			if (TutorialCourse.body.HoldingItem("minibarrel"))
			{
				break;
			}
			yield return null;
		}
		it = null;
		if (!TutorialCourse.body.FindByIdSurface("minibarrel", out it))
		{
			throw new Exception("Cannot find minibarrel on player..?");
		}
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove12"));
		progress = 3;
		while (true)
		{
			if (TutorialCourse.body.transform.position.y < -105f)
			{
				if (!(TutorialCourse.body.lastTimeStepVelocity.magnitude >= TutorialCourse.body.jumpSpeed * 2f))
				{
					break;
				}
				ResetPlayer();
				if (!TutorialCourse.body.HoldingItem(it))
				{
					it.transform.position = TutorialCourse.body.transform.position;
				}
			}
			if ((TutorialCourse.body.transform.position.x < 1f || TutorialCourse.body.transform.position.x > 18f) && TutorialCourse.body.transform.position.y < -54f)
			{
				ResetPlayer();
				if (!TutorialCourse.body.HoldingItem(it))
				{
					it.transform.position = TutorialCourse.body.transform.position;
				}
			}
			yield return null;
		}
		UnityEngine.Object.Destroy(it.gameObject);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove13"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove14"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x > 41f && TutorialCourse.body.transform.position.y > -95f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove15"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x > 47f && TutorialCourse.body.transform.position.y > -80f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove16"));
		yield return new WaitUntil(() => TutorialCourse.body.transform.position.x > 25f && TutorialCourse.body.transform.position.y > -68f);
		TutorialCourse.tutorial.Speak(Locale.GetOther("tutmove17"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialHandler.CompleteTut(3);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("PreGen");
	}

	public void ResetPlayer()
	{
		Vector2 vector = Vector3.down * 106f + Vector3.left * 64f;
		switch (progress)
		{
		case 1:
			vector = new Vector2(-46f, -53f);
			break;
		case 2:
			vector = new Vector2(0f, -51f);
			break;
		case 3:
			vector = new Vector2(5f, -51f);
			break;
		}
		TutorialCourse.body.shock = 0f;
		TutorialCourse.body.transform.position = vector;
		TutorialCourse.body.Stand(force: true);
		TutorialCourse.body.SetVelocity(Vector2.zero);
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny, 0.6f);
		TutorialCourse.body.Heal();
	}

	public IEnumerator ForceItemAlive(Item item)
	{
		while (true)
		{
			item.condition = 1f;
			yield return null;
		}
	}

	public IEnumerator ForceMaxStamina()
	{
		while (true)
		{
			TutorialCourse.body.stamina = 100f;
			TutorialCourse.body.energy = 100f;
			TutorialCourse.body.hunger = 100f;
			TutorialCourse.body.thirst = 100f;
			TutorialCourse.body.happiness = 0f;
			TutorialCourse.body.bloodOxygen = 100f;
			TutorialCourse.body.bloodVolume = 100f;
			TutorialCourse.body.bloodPressure = 120f;
			TutorialCourse.body.consciousness = 100f;
			TutorialCourse.body.brainHealth = 100f;
			TutorialCourse.body.temperature = 37f;
			TutorialCourse.body.weightOffset = 0f;
			TutorialCourse.body.adrenaline = 0f;
			Limb[] limbs = TutorialCourse.body.limbs;
			foreach (Limb limb in limbs)
			{
				limb.muscleHealth = 100f;
				limb.skinHealth = 100f;
				limb.infectionAmount = 0f;
				limb.pain = 0f;
				if (limb.broken)
				{
					limb.MendBone();
				}
				if (limb.dislocated)
				{
					limb.UnDislocate();
				}
			}
			yield return null;
		}
	}

	public override string LocaleName()
	{
		return "movementcourse";
	}
}
