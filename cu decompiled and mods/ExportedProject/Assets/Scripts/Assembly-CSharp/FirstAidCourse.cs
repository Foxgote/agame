using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstAidCourse : TutorialCourse
{
	public override IEnumerator Sequence()
	{
		TutorialCourse.tutorial.forcePlayerAlive = false;
		TutorialCourse.body.skills.AddExp(2, 1000f);
		StartCoroutine(PreventStroke());
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid1"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid2"));
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		yield return new WaitUntil(() => PlayerCamera.main.woundView.activeSelf || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid3"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightAnchor(0f, 1f, 0f, 1f);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightPosition(247f, -128f);
		TutorialCourse.tutorial.SetHighlightSize(320f, 28f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid4"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(355f, -153f);
		TutorialCourse.tutorial.SetHighlightSize(94f, 28f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid5"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(355f, -180f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid6"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(111f, -180f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid7"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(111f, -216f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid8"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(232f, -181f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid9"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(75f, -412f);
		TutorialCourse.tutorial.SetHighlightSize(75f, 63f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid10"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(75f, -456f);
		TutorialCourse.tutorial.SetHighlightSize(75f, 23f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid11"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(348f, -424f);
		TutorialCourse.tutorial.SetHighlightSize(82f, 88f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid12"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(94f, -498f);
		TutorialCourse.tutorial.SetHighlightSize(62f, 23f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid13"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(193f, -498f);
		TutorialCourse.tutorial.SetHighlightSize(106f, 23f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid14"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(353f, -498f);
		TutorialCourse.tutorial.SetHighlightSize(106f, 23f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid15"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(220f, -552f);
		TutorialCourse.tutorial.SetHighlightSize(361f, 75f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid16"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.SetHighlightPosition(219f, -704f);
		TutorialCourse.tutorial.SetHighlightSize(381f, 193f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid17"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.showUiHighlight = false;
		yield return new WaitForSeconds(1.5f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid18"));
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(0f, 0f, 0f, 0f);
		TutorialCourse.tutorial.SetHighlightPosition(488f, 44f);
		TutorialCourse.tutorial.SetHighlightSize(784f, 88f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid19"));
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
		TutorialCourse.body.limbs[4].bleedAmount += 25f;
		TutorialCourse.body.limbs[4].skinHealth -= 25f;
		TutorialCourse.body.limbs[4].muscleHealth -= 10f;
		TutorialCourse.body.lastTimeStepVelocity += Vector2.left * 5f;
		TutorialCourse.body.Ragdoll();
		TutorialCourse.body.shock += 25f;
		TutorialCourse.body.forceWalk = false;
		MusicManager.main.PlaySong(TutorialCourse.tutorial.tutorialMusic[1]);
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.armKnifeSpriteOverride = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid20"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid21"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return GiveItemToBody("sterilizedbandage");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid22"));
		yield return new WaitUntil(() => TutorialCourse.body.limbs[4].bleedAmount < 0.1f);
		yield return new WaitForSeconds(2f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid23"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid24"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid25"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid26"));
		float timer = 5f;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			TutorialCourse.body.bloodVolume = Mathf.MoveTowards(TutorialCourse.body.bloodVolume, 0f, Time.deltaTime * 20f);
			yield return null;
		}
		Coroutine cor = StartCoroutine(ForceConsciousness());
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid27"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid28"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid29"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid30"));
		yield return GiveItemToBody("aed");
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid31"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid32"));
		bool saidWrongDefibAed = false;
		while (true)
		{
			if ((bool)MinigameBase.main.currentMinigame && MinigameBase.main.currentMinigame is AEDMinigame aEDMinigame)
			{
				if (aEDMinigame.limb == TutorialCourse.body.limbs[1])
				{
					break;
				}
				if (!saidWrongDefibAed)
				{
					saidWrongDefibAed = true;
					TutorialCourse.tutorial.Speak(Locale.GetTutorial("aidfail1"));
				}
			}
			yield return null;
		}
		yield return new WaitUntil(() => TutorialCourse.body.fibrillationProgress < 10f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid33"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid34"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid35"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		timer = 20f;
		while (timer > 0f)
		{
			timer -= Time.deltaTime;
			TutorialCourse.body.fibrillationProgress = Mathf.PingPong(timer * 14f, 100f) * 0.95f;
			TutorialCourse.tutorial.speakerTalker.text.text = Mathf.RoundToInt(TutorialCourse.body.fibrillationProgress) + "%";
			yield return null;
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid36"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		Object.Destroy(TutorialCourse.tutorial.lastSpawnedObject);
		yield return GiveItemToBody("manualdefibrillator");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid37"));
		bool saidWrongDefib = false;
		while (true)
		{
			if ((bool)MinigameBase.main.currentMinigame && MinigameBase.main.currentMinigame is ManualDefibMinigame manualDefibMinigame)
			{
				if (manualDefibMinigame.limb == TutorialCourse.body.limbs[1])
				{
					break;
				}
				if (!saidWrongDefib)
				{
					saidWrongDefib = true;
					TutorialCourse.tutorial.Speak(Locale.GetTutorial("aidfail2"));
				}
			}
			yield return null;
		}
		yield return new WaitUntil(() => MinigameBase.main.currentMinigame);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid38"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid39"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid40"));
		TutorialCourse.body.TryStartFibrillation(forced: true);
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(0.5f, 0.5f, 0.5f, 0.5f);
		TutorialCourse.tutorial.SetHighlightSize(280f, 64f);
		TutorialCourse.tutorial.SetHighlightPosition(-127f, 30f);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid41"));
		yield return new WaitUntil(() => TutorialCourse.body.fibrillationProgress > 5f);
		if (!TutorialCourse.body.fibrillationForced)
		{
			TutorialCourse.body.TryStartFibrillation(forced: true);
		}
		yield return new WaitUntil(() => TutorialCourse.body.fibrillationProgress < 5f);
		yield return new WaitForSeconds(1f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("nice"));
		Object.Destroy(TutorialCourse.tutorial.lastSpawnedObject);
		StartCoroutine(Repressurize());
		yield return GiveItemToBody("autopump");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid42"));
		yield return new WaitUntil(() => TutorialCourse.body.HasWearable("autopump") && PlayerCamera.main.woundView.activeSelf);
		StopCoroutine(cor);
		TutorialCourse.body.TryStartFibrillation(forced: true);
		TutorialCourse.body.fibrillationProgress = 90f;
		yield return new WaitUntil(() => TutorialCourse.body.inCardiacArrest);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid43"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid44"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return GiveItemToBody("syringe");
		TutorialCourse.tutorial.lastSpawnedObject.GetComponent<WaterContainerItem>().AddLiquid("epinephrine", 20f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid45"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.lastSpawnedObject.GetComponent<Item>().condition < 0.199f);
		timer = 10f;
		while (timer > 0f && TutorialCourse.body.inCardiacArrest)
		{
			timer -= Time.deltaTime;
			yield return null;
		}
		if (TutorialCourse.body.inCardiacArrest)
		{
			TutorialCourse.body.heartRate = 240f;
		}
		TutorialCourse.body.fibrillationProgress = 40f;
		TutorialCourse.body.heartRatePressureOffset = 0f;
		CoUtils.instance.Stop("epinephrine");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid46"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid47"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid48"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid49"));
		Object.Destroy(TutorialCourse.tutorial.lastSpawnedObject);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return GiveItemToBody("streptokinase");
		TutorialCourse.body.bloodViscosity = 75f;
		TutorialCourse.body.hasPulmonaryEmbolism = true;
		TutorialCourse.body.AutoPickUpItem(TutorialCourse.tutorial.lastSpawnedObject.GetComponent<Item>());
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid50"));
		yield return new WaitUntil(() => !TutorialCourse.body.hasPulmonaryEmbolism);
		TutorialCourse.body.limbs[1].muscleHealth = 100f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid51"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return GiveItemToBody("bruisekit");
		TutorialCourse.tutorial.handPos = TutorialCourse.body.transform.position + Vector3.right * 4f;
		TutorialCourse.body.eyeScareTime = 1f;
		TutorialCourse.tutorial.armKnifeSpriteOverride = true;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.armSpeedMultiplier = 5f;
		TutorialCourse.tutorial.handPos = TutorialCourse.body.limbs[1].transform.position;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos.x += 5f;
		TutorialCourse.body.DoGoreSound();
		TutorialCourse.body.limbs[1].pain += 35f;
		TutorialCourse.body.limbs[1].bleedAmount += 3f;
		TutorialCourse.body.limbs[1].skinHealth -= 20f;
		TutorialCourse.body.limbs[1].muscleHealth -= 100f;
		TutorialCourse.body.lastTimeStepVelocity += Vector2.left * 5f;
		TutorialCourse.body.Ragdoll();
		TutorialCourse.body.shock += 25f;
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.tutorial.armKnifeSpriteOverride = false;
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid52"));
		yield return new WaitUntil(() => TutorialCourse.body.limbs[1].muscleHealth > 5f && TutorialCourse.body.respiratoryRate > 10f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid53"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid54"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid55"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return new WaitForSeconds(2f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid56"));
		yield return GiveItemToBody("waterjug");
		yield return GiveItemToBody("dogfood");
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return new WaitUntil(() => Input.GetKey(KeyBinds.GetBind("bark")));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid57"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.armSpeedMultiplier = 5f;
		TutorialCourse.tutorial.handPos = TutorialCourse.body.limbs[8].transform.position;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos.x += 5f;
		TutorialCourse.body.DoGoreSound();
		TutorialCourse.body.limbs[8].Dislocate();
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid58"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking || !TutorialCourse.body.limbs[8].dislocated);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid59"));
		yield return new WaitUntil(() => (bool)MinigameBase.main.currentMinigame || !TutorialCourse.body.limbs[8].dislocated);
		StartCoroutine(RemovePain());
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid60"));
		yield return new WaitUntil(() => !TutorialCourse.body.limbs[8].dislocated);
		yield return new WaitForSeconds(2f);
		TutorialCourse.tutorial.armSpeedMultiplier = 5f;
		TutorialCourse.tutorial.handPos = TutorialCourse.body.limbs[8].transform.position;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos.x += 5f;
		TutorialCourse.body.DoGoreSound();
		TutorialCourse.body.limbs[8].BreakBone();
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.handPos += Vector2.right * 4f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid61"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return GiveItemToBody("splint");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid62"));
		yield return new WaitUntil(() => TutorialCourse.body.limbs[8].GetComponent<SplintLimb>());
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid63"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.body.limbs[8].bleedAmount *= 0.2f;
		Limb[] limbs = TutorialCourse.body.limbs;
		foreach (Limb obj in limbs)
		{
			obj.infected = false;
			obj.infectionAmount = 0f;
			obj.bleedAmount = 0f;
			obj.muscleHealth = 100f;
			obj.skinHealth = 100f;
			obj.MendBone();
			obj.UnDislocate();
			if (obj.TryGetComponent<SplintLimb>(out var component))
			{
				component.TakeOff();
			}
		}
		yield return new WaitForSeconds(2f);
		TutorialCourse.tutorial.armSpeedMultiplier = 5f;
		TutorialCourse.tutorial.handPos = TutorialCourse.body.limbs[8].transform.position;
		yield return new WaitUntil(() => TutorialCourse.tutorial.handPos == TutorialCourse.tutorial.handPosCurrent);
		TutorialCourse.tutorial.handPos.x += 5f;
		TutorialCourse.body.DoGoreSound();
		TutorialCourse.body.limbs[5].muscleHealth = 0f;
		TutorialCourse.body.forceWalk = false;
		TutorialCourse.body.Ragdoll();
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		TutorialCourse.tutorial.handPos += Vector2.right * 4f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid64"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid65"));
		TutorialCourse.body.handSlot = 0;
		TutorialCourse.tutorial.showHealthPanelHighlight = true;
		TutorialCourse.tutorial.showUiHighlight = true;
		TutorialCourse.tutorial.SetHighlightAnchor(1f, 0f, 1f, 0f);
		TutorialCourse.tutorial.SetHighlightPosition(-36f, 300f);
		TutorialCourse.tutorial.SetHighlightSize(72f, 70f);
		yield return new WaitUntil(() => TutorialCourse.body.handSlot == 1);
		TutorialCourse.tutorial.showHealthPanelHighlight = false;
		TutorialCourse.tutorial.showUiHighlight = false;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("nice"));
		yield return new WaitForSeconds(3f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid66"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid67"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid68"));
		yield return GiveItemToBody("opium");
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid69"));
		yield return new WaitUntil(() => TutorialCourse.body.TryGetComponent<Painkillers>(out var component2) && component2.actualOpiateReception > 1f);
		yield return new WaitForSeconds(3f);
		TutorialCourse.body.limbs[0].pain = 100f;
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid70"));
		yield return new WaitUntil(() => TutorialCourse.body.limbs[0].pain < 10f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid71"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid72"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return new WaitForSeconds(2f);
		yield return new WaitForSeconds(3f);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("aid73"));
		TutorialHandler.CompleteTut(1);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("PreGen");
	}

	public override string LocaleName()
	{
		return "firstaidcourse";
	}

	public IEnumerator ForceConsciousness()
	{
		while (true)
		{
			if (TutorialCourse.body.consciousness < 35f)
			{
				TutorialCourse.body.consciousness = 35f;
			}
			if (TutorialCourse.body.fibrillationProgress > 95f)
			{
				TutorialCourse.body.fibrillationProgress = 95f;
			}
			yield return null;
		}
	}

	public IEnumerator RemovePain()
	{
		while (true)
		{
			Limb[] limbs = TutorialCourse.body.limbs;
			foreach (Limb obj in limbs)
			{
				obj.pain = Mathf.MoveTowards(obj.pain, 0f, Time.deltaTime * 6f);
			}
			yield return null;
		}
	}

	public IEnumerator Repressurize()
	{
		float timer = 20f;
		while (timer > 0f)
		{
			TutorialCourse.body.bloodVolume = Mathf.MoveTowards(TutorialCourse.body.bloodVolume, 100f, Time.deltaTime * 5f);
			timer -= Time.deltaTime;
			yield return null;
		}
	}

	public IEnumerator PreventStroke()
	{
		while (true)
		{
			TutorialCourse.body.strokeAmount = 0f;
			yield return null;
		}
	}
}
