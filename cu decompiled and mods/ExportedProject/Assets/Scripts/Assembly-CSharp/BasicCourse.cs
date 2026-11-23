using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicCourse : TutorialCourse
{
	public override IEnumerator Sequence()
	{
		TutorialCourse.tutorial.handPos = (Vector2)TutorialCourse.body.transform.position + Vector2.up * 5f + Vector2.right * 6f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic1"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic2"));
		TutorialCourse.tutorial.handPos += Vector2.left * 4f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic3"));
		TutorialCourse.tutorial.handPos = Vector2.left * 16f;
		yield return new WaitForSeconds(1f);
		if (TutorialCourse.body.transform.position.x < -12f)
		{
			while (TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) > 0.5f)
			{
				TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position;
				yield return null;
			}
			TutorialCourse.tutorial.Grab(TutorialCourse.body.transform);
			TutorialCourse.tutorial.handPos = Vector2.zero + Vector2.up * 2f;
			yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
			TutorialCourse.tutorial.Ungrab();
			TutorialCourse.tutorial.handPos = Vector2.left * 16f;
			yield return new WaitForSeconds(2f);
		}
		yield return new WaitUntil(() => TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) < 2f);
		TutorialCourse.tutorial.handPos = Vector2.right * 16f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic4"));
		yield return new WaitForSeconds(2f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) < 2f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic5"));
		TutorialCourse.tutorial.AddBlockQuery(11, -1, 21);
		TutorialCourse.tutorial.AddBlockQuery(11, 1, 21);
		TutorialCourse.tutorial.AddBlockQuery(11, 3, 21);
		TutorialCourse.tutorial.AddBlockQuery(11, 5, 10);
		TutorialCourse.tutorial.AddBlockQuery(10, 5, 10);
		TutorialCourse.tutorial.AddBlockQuery(9, 5, 10);
		TutorialCourse.tutorial.AddBlockQuery(8, 5, 10);
		TutorialCourse.tutorial.AddBlockQuery(-14, 5, 21);
		TutorialCourse.tutorial.AddBlockQuery(-14, 7, 21);
		TutorialCourse.tutorial.AddBlockQuery(-14, 9, 21);
		TutorialCourse.tutorial.AddBlockQuery(-14, 11, 21);
		TutorialCourse.tutorial.AddBlockQuery(-14, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-13, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-12, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-11, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-10, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-9, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-8, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-7, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-6, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-5, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-4, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 13, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 14, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 15, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 16, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 17, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 18, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 19, 10);
		TutorialCourse.tutorial.AddBlockQuery(-3, 20, 10);
		yield return new WaitForSeconds(1f);
		TutorialCourse.body.forceWalk = false;
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic6"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.handPos = new Vector2(-6f, 16f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic7"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) < 1.5f);
		TutorialCourse.body.forceWalk = true;
		while (TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) > 0.5f)
		{
			TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position;
			yield return null;
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic8"));
		TutorialCourse.tutorial.Grab(TutorialCourse.body.transform);
		TutorialCourse.tutorial.handPos = new Vector2(-17f, 17f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos = new Vector2(-17f, 1f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos = new Vector2(-2f, 2f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic9"));
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic10"));
		TutorialCourse.tutorial.objectToCreate = "nutrientbar";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		Item item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		TutorialCourse.body.hunger = 60f;
		TutorialCourse.tutorial.handPos.y -= 16f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic11"));
		yield return new WaitUntil(() => !item || TutorialCourse.body.HoldingItem(item));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic12"));
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightPosition(Vector2.zero);
		TutorialCourse.tutorial.SetHighlightSize(200f, 200f);
		yield return new WaitUntil(() => !item || item.condition < 0.99f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic13"));
		if (!item)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic13fail"));
			yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		}
		else
		{
			yield return new WaitUntil(() => !item);
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic14"));
		TutorialCourse.tutorial.showUiHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic15"));
		TutorialCourse.tutorial.objectToCreate = "rake";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		Coroutine cor = StartCoroutine(ForceItemAlive(item));
		TutorialCourse.tutorial.handPos.y -= 16f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position + Vector3.up * 6f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.Ungrab();
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic16"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightPosition(Vector2.up * 103f);
		TutorialCourse.tutorial.SetHighlightSize(519f, 165f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic17"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem(0) || TutorialCourse.body.HoldingItem(1));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic18"));
		TutorialCourse.tutorial.showUiHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.body.attackCooldown > 0.21f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic19"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic20"));
		TutorialCourse.tutorial.AddBlockQuery(13, 15, 21);
		TutorialCourse.tutorial.AddBlockQuery(14, 15, 21);
		TutorialCourse.tutorial.AddBlockQuery(13, 16, 21);
		TutorialCourse.tutorial.AddBlockQuery(14, 16, 21);
		yield return new WaitUntil(() => Input.GetKeyDown(KeyBinds.GetBind("switchhands")) && (TutorialCourse.body.HoldingItem(0) || TutorialCourse.body.HoldingItem(1)));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic21"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.handPos.x -= 16f;
		yield return new WaitUntil(() => Vector2.Distance(item.transform.position, new Vector2(14f, 15f)) < 5f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic22"));
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		if (!TutorialCourse.body.HoldingItem(item))
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic23"));
		}
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem(item));
		while (TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) > 0.5f)
		{
			TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position;
			yield return null;
		}
		TutorialCourse.tutorial.Grab(TutorialCourse.body.transform);
		TutorialCourse.tutorial.handPos = new Vector2(17f, 2f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.AddBlockQuery(-10, -2, 16);
		TutorialCourse.tutorial.AddBlockQuery(-10, -1, 16);
		TutorialCourse.tutorial.AddBlockQuery(-10, 0, 16);
		TutorialCourse.tutorial.AddBlockQuery(-10, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-4, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-4, 3, 10);
		TutorialCourse.tutorial.AddBlockQuery(-4, 5, 10);
		TutorialCourse.tutorial.AddBlockQuery(-4, 7, 10);
		TutorialCourse.tutorial.AddBlockQuery(-9, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-8, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-7, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-6, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-5, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-11, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-12, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-13, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-14, 1, 10);
		TutorialCourse.tutorial.AddBlockQuery(-11, 0, 16);
		TutorialCourse.tutorial.AddBlockQuery(-12, 0, 16);
		TutorialCourse.tutorial.AddBlockQuery(-13, 0, 16);
		TutorialCourse.tutorial.AddBlockQuery(-14, 0, 16);
		TutorialCourse.tutorial.AddBlockQuery(-11, -1, 16);
		TutorialCourse.tutorial.AddBlockQuery(-12, -1, 16);
		TutorialCourse.tutorial.AddBlockQuery(-13, -1, 16);
		TutorialCourse.tutorial.AddBlockQuery(-14, -1, 16);
		TutorialCourse.tutorial.AddBlockQuery(-11, -2, 16);
		TutorialCourse.tutorial.AddBlockQuery(-12, -2, 16);
		TutorialCourse.tutorial.AddBlockQuery(-13, -2, 16);
		TutorialCourse.tutorial.AddBlockQuery(-14, -2, 16);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic24"));
		TutorialCourse.tutorial.objectToCreate = "barbedwirefence";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		TutorialCourse.tutorial.handPos = new Vector2(-15f, 2.9f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.handPos.x -= 3f;
		TutorialCourse.tutorial.handPos.y -= 2f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic25"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.ClawDistance(TutorialCourse.body.transform.position) < 1.5f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic26"));
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(1f, 0.5f, 1f, 0.5f);
		TutorialCourse.tutorial.SetHighlightPosition(new Vector2(-77f, -512f));
		TutorialCourse.tutorial.SetHighlightSize(155f, 50f);
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		while (Time.timeScale <= 1.1f)
		{
			TutorialCourse.body.stamina = 30f;
			TutorialCourse.body.energy = 100f;
			yield return null;
		}
		TutorialCourse.tutorial.showUiHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.body.stamina > 70f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic27"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic28"));
		TutorialCourse.tutorial.armKnifeSpriteOverride = true;
		while (!TutorialCourse.tutorial.finishedTalking)
		{
			TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position + Vector3.right * 4f;
			TutorialCourse.body.eyeScareTime = 1f;
			yield return null;
		}
		TutorialCourse.tutorial.armSpeedMultiplier = 5f;
		TutorialCourse.tutorial.handPos = TutorialCourse.body.limbs[4].transform.position;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos.x += 5f;
		TutorialCourse.body.DoGoreSound();
		TutorialCourse.body.limbs[4].pain += 35f;
		TutorialCourse.body.limbs[4].bleedAmount += 12f;
		TutorialCourse.body.limbs[4].skinHealth -= 30f;
		TutorialCourse.body.limbs[4].muscleHealth -= 10f;
		TutorialCourse.body.lastTimeStepVelocity += Vector2.left * 5f;
		TutorialCourse.body.Ragdoll();
		TutorialCourse.body.shock += 25f;
		TutorialCourse.body.forceWalk = false;
		MusicManager.main.PlaySong(TutorialCourse.tutorial.tutorialMusic[0]);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.armKnifeSpriteOverride = false;
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic29"));
		yield return new WaitUntil(() => PlayerCamera.main.woundView.activeSelf);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		TutorialCourse.tutorial.SetHighlightPosition(new Vector2(-192f, 130f));
		TutorialCourse.tutorial.SetHighlightSize(125f, 48f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic30"));
		yield return new WaitUntil(() => WoundView.view.limbLookingAt == 4);
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic31"));
		TutorialCourse.tutorial.objectToCreate = "bandage";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		StopCoroutine(cor);
		item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		cor = StartCoroutine(ForceItemAlive(item));
		TutorialCourse.tutorial.handPos.y -= 25f;
		TutorialCourse.tutorial.showHealthPanelHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
		if (PlayerCamera.main.woundView.activeSelf)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("closehealthpanel"));
			yield return new WaitUntil(() => !PlayerCamera.main.woundView.activeSelf || TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
			yield return new WaitForSeconds(1f);
		}
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic32"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic33"));
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		yield return new WaitUntil(() => (PlayerCamera.main.woundView.activeSelf && TutorialCourse.body.HoldingItem(item)) || TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic34"));
		yield return new WaitUntil(() => (bool)MinigameBase.main.currentMinigame || TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic35"));
		yield return new WaitUntil(() => TutorialCourse.body.limbs[4].bleedAmount <= 0.1f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic36"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		if (!PlayerCamera.main.woundView.activeSelf)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("openhealthpanel"));
			yield return new WaitUntil(() => PlayerCamera.main.woundView.activeSelf);
		}
		TutorialCourse.body.limbs[4].infected = true;
		TutorialCourse.body.limbs[4].infectionAmount = 30f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic37"));
		TutorialCourse.tutorial.objectToCreate = "disinfectant";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		StopCoroutine(cor);
		item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		TutorialCourse.tutorial.handPos.y -= 25f;
		TutorialCourse.tutorial.showHealthPanelHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		if (PlayerCamera.main.woundView.activeSelf)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("closehealthpanel"));
			yield return new WaitUntil(() => !PlayerCamera.main.woundView.activeSelf);
			yield return new WaitForSeconds(1f);
		}
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic38"));
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		yield return new WaitUntil(() => PlayerCamera.main.woundView.activeSelf && TutorialCourse.body.HoldingItem(item));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic39"));
		while (TutorialCourse.body.limbs[4].disinfectionTime <= 0f)
		{
			if (item.condition <= 0f)
			{
				TutorialCourse.tutorial.Speak(Locale.GetTutorial("basicfail1"));
				yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
				TutorialCourse.tutorial.Speak(Locale.GetTutorial("basicfail2"));
				yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
				TutorialCourse.body.limbs[4].SetDisinfect(300f);
			}
			yield return null;
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic40"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic41"));
		TutorialCourse.tutorial.showHealthPanelHighlight = false;
		if (PlayerCamera.main.woundView.activeSelf)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("closehealthpanel"));
			yield return new WaitUntil(() => !PlayerCamera.main.woundView.activeSelf);
			yield return new WaitForSeconds(1f);
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic42"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.objectToCreate = "trashbag";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		TutorialCourse.tutorial.handPos.y -= 25f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic43"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Ungrab();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic44"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic45"));
		yield return new WaitUntil(() => !item || item.container.itemCount > 0);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic46"));
		yield return new WaitUntil(() => !item || item.container.itemCount == 0);
		if (!item)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic46fail1"));
			yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic46fail2"));
			yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		}
		if ((bool)item)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic47"));
		}
		TutorialCourse.tutorial.objectToCreate = "smallpack";
		yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
		item = TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>();
		TutorialCourse.tutorial.handPos.y -= 25f;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic48"));
		TutorialCourse.tutorial.Ungrab();
		yield return new WaitUntil(() => !item || TutorialCourse.body.HasWearable(item.id));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic49"));
		yield return new WaitUntil(() => !item || TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic50"));
		while (true)
		{
			bool flag = false;
			if (!item)
			{
				TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic46fail3"));
				yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
				break;
			}
			foreach (Transform item2 in item.transform)
			{
				if ((bool)item2.GetComponent<Container>())
				{
					flag = true;
				}
			}
			if (flag)
			{
				break;
			}
			yield return null;
		}
		if ((bool)item)
		{
			TutorialCourse.tutorial.Speak(Locale.GetTutorial("nice"));
		}
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic51"));
		Coroutine foliageCoroutine = StartCoroutine(GiveFoliage());
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(0f, 0f, 0f, 0f);
		TutorialCourse.tutorial.SetHighlightPosition(new Vector2(36f, 145f));
		TutorialCourse.tutorial.SetHighlightSize(72f, 72f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic52"));
		yield return new WaitUntil(() => PlayerCamera.main.craftingPanel.activeSelf);
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic53"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		TutorialCourse.tutorial.SetHighlightPosition(new Vector2(297f, 327f));
		TutorialCourse.tutorial.SetHighlightSize(150f, 150f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic54"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("rope"));
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic55"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("canvas"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic56"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("foliagebag"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic57"));
		StopCoroutine(foliageCoroutine);
		TutorialCourse.tutorial.objectToCreate = "";
		TutorialCourse.tutorial.Ungrab();
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic58"));
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		yield return new WaitUntil(() => PlayerCamera.main.woundView.activeSelf);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic59"));
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(1f, 1f, 1f, 1f);
		TutorialCourse.tutorial.SetHighlightPosition(new Vector2(-216f, -121f));
		TutorialCourse.tutorial.SetHighlightSize(400f, 215f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic60"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic61"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic62"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic63"));
		TutorialCourse.tutorial.showUiHighlight = false;
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic64"));
		yield return new WaitUntil(() => TutorialCourse.body.exercising);
		TutorialCourse.tutorial.AddBlockQuery(-20, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-19, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-18, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-17, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-16, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-15, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-14, -2, 8);
		TutorialCourse.tutorial.AddBlockQuery(-13, -2, 6);
		TutorialCourse.tutorial.AddBlockQuery(-13, -1, 6);
		yield return new WaitForSeconds(3f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic65"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.body.energy = 30f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic66"));
		PlayerPrefs.SetInt("didbasiccourse", 1);
		yield return new WaitUntil(() => TutorialCourse.body.energy > 50f && TutorialCourse.body.conscious);
		TutorialCourse.tutorial.showHealthPanelHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic67"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		yield return new WaitForSeconds(3f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("basic68"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialHandler.CompleteTut(0);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("PreGen");
	}

	public IEnumerator GiveFoliage()
	{
		List<Item> items = new List<Item>();
		while (true)
		{
			items.RemoveAll((Item x) => !x);
			if (items.Count < 3)
			{
				TutorialCourse.tutorial.objectToCreate = "dryfoliage";
				yield return new WaitUntil(() => TutorialCourse.tutorial.grabInfo.grabbed);
				items.Add(TutorialCourse.tutorial.grabInfo.grabbed.GetComponent<Item>());
				TutorialCourse.tutorial.handPos.y -= 12f;
				yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
				TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position + Vector3.up * 3f;
				yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
				TutorialCourse.tutorial.Ungrab();
			}
			yield return null;
		}
	}

	public IEnumerator ForceItemAlive(Item item)
	{
		while (true)
		{
			item.condition = 1f;
			yield return null;
		}
	}

	public override string LocaleName()
	{
		return "basicscourse";
	}
}
