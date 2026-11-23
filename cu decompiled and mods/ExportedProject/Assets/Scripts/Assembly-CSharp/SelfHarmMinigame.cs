using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelfHarmMinigame : Minigame
{
	public int cutsLeft;

	public int cutsDone;

	public Image currentCut;

	private float cutTime;

	private bool cutting;

	public int triesLeft = 15;

	public bool isEye;

	public Image eye;

	public Transform pupil;

	public override HandSpriteType HandType()
	{
		if (!cutting)
		{
			return HandSpriteType.SelfHarm;
		}
		return HandSpriteType.SelfHarmForceOn;
	}

	public override string GuideLocaleString()
	{
		return "";
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public override bool CanExit()
	{
		return false;
	}

	public SelfHarmMinigame(int cuts, bool isEye = false)
	{
		cutsLeft = cuts;
		this.isEye = isEye;
	}

	public void StartCut()
	{
		cutTime = 0.25f;
		cutting = true;
		if (!isEye)
		{
			Sound.Play("cut" + (1 + cutsDone), Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		}
		else
		{
			Sound.Play("cuteye", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true, ignoreMixer: true);
		}
		if (isEye)
		{
			eye.sprite = Resources.Load<Sprite>("Sprites/minigameeyegone");
			pupil.gameObject.SetActive(value: false);
		}
		Transform transform = Object.Instantiate(Minigame.game.spawnedMiniGame.GetChild(0), Minigame.game.spawnedMiniGame);
		transform.transform.position = Minigame.game.handTransform.position;
		transform.gameObject.SetActive(value: true);
		currentCut = transform.GetComponent<Image>();
		transform = Object.Instantiate(Minigame.game.spawnedMiniGame.GetChild(1), PlayerCamera.main.mainCanvas.transform);
		transform.SetAsFirstSibling();
		transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		transform.GetComponent<RectTransform>().sizeDelta = Vector2.one;
		transform.gameObject.SetActive(value: true);
		Object.Destroy(transform.gameObject, 0.35f);
		cutsLeft--;
		cutsDone++;
	}

	public void EndCut()
	{
		cutting = false;
		if (cutsLeft == 0)
		{
			Minigame.game.EndMinigame();
		}
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/SelfHarmMinigame");
		if (isEye)
		{
			Minigame.game.spawnedMiniGame.GetChild(2).gameObject.SetActive(value: true);
			eye = Minigame.game.spawnedMiniGame.GetChild(2).GetComponent<Image>();
			pupil = Minigame.game.spawnedMiniGame.GetChild(2).GetChild(0);
		}
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		cutTime -= Time.deltaTime;
		if (cutting)
		{
			currentCut.fillAmount = 1f - cutTime * 4f;
		}
		if (cutTime < 0f && cutting)
		{
			EndCut();
		}
		if (isEye)
		{
			pupil.localPosition = Random.insideUnitCircle * (15f - (float)triesLeft) * 0.5f;
		}
		if (Minigame.game.handStartedClicking && cutTime < -0.25f)
		{
			if (triesLeft > 0)
			{
				Minigame.game.handVelocity += Random.insideUnitCircle.normalized * (15f - (float)triesLeft + 4f) * 0.8f;
				Sound.Play("selfharmtap", Vector2.zero, twoDimensional: true, pitchShift: false, null, (15f - (float)triesLeft) / 15f, 1.5f - (15f - (float)triesLeft) / 15f, noReverb: true, ignoreMixer: true);
				triesLeft--;
			}
			else
			{
				StartCut();
			}
		}
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		if (Random.value < 0.4f)
		{
			Minigame.game.handVelocity += Random.insideUnitCircle * 1f;
		}
		if (cutting)
		{
			Minigame.game.handVelocity = Vector2.Lerp(Minigame.game.handVelocity, Vector2.down * 48f, 0.7f);
		}
		else if (isEye)
		{
			Minigame.game.handPos.x = Mathf.Lerp(Minigame.game.handPos.x, 0f, 0.3f * (15f - (float)triesLeft) / 15f);
			Minigame.game.handPos.y = Mathf.Lerp(Minigame.game.handPos.y, 360f, 0.3f * (15f - (float)triesLeft) / 15f);
		}
	}
}
