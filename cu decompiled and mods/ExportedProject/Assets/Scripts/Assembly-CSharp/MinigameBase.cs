using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MinigameBase : MonoBehaviour
{
	public static MinigameBase main;

	public RectTransform minigameScreen;

	public Transform spawnedMiniGame;

	public Minigame currentMinigame;

	public RectTransform handTransform;

	private Image handSprite;

	public Item currentItem;

	public Vector2 handPos;

	public Vector2 handVelocity;

	public Sprite[] handSprites;

	public Sprite[] handSpritesFurless;

	public bool handClicking;

	public bool handStartedClicking;

	public bool handStoppedClicking;

	public const float windowHalfSize = 524f;

	public float physicsUpdateTime;

	public static float PHYSICS_UPDATE_RATE = 60f;

	public GameObject exitTip;

	public List<GameObject> textBackgrounds;

	public TextMeshProUGUI itemText;

	public TextMeshProUGUI guideText;

	public Image itemImage;

	private bool forceUngrab;

	private bool firstFrameForceUngrab;

	public float handShakeForce = 1f;

	public Body body => PlayerCamera.main.body;

	public void ForceUngrab()
	{
		if (handClicking)
		{
			forceUngrab = true;
			firstFrameForceUngrab = true;
		}
	}

	public void UpdateHandSprite(bool reset = false)
	{
		if (!currentMinigame)
		{
			return;
		}
		handSprite.color = new Color(1f, 1f - body.averagePain * 0.01f, 1f - body.averagePain * 0.01f, 210f / 255f);
		handSprite.rectTransform.localScale = new Vector3((body.handSlot == 0) ? 1f : (-1f), 1f, 1f);
		if (body.limbs[(body.handSlot == 0) ? 5 : 8].dismembered)
		{
			handSprite.sprite = handSprites[10];
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Grasp)
		{
			if (handStartedClicking)
			{
				handSprite.sprite = handSprites[2];
			}
			if (handStoppedClicking | reset)
			{
				handSprite.sprite = handSprites[Random.Range(0, 2)];
			}
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Point)
		{
			if (handStartedClicking)
			{
				handSprite.sprite = handSprites[5];
			}
			if (handStoppedClicking | reset)
			{
				handSprite.sprite = handSprites[Random.Range(3, 5)];
			}
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Tweezers)
		{
			if (handStartedClicking)
			{
				handSprite.sprite = handSprites[7];
			}
			if (handStoppedClicking | reset)
			{
				handSprite.sprite = handSprites[6];
			}
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Wrench)
		{
			if (handStartedClicking)
			{
				handSprite.sprite = handSprites[9];
			}
			if (handStoppedClicking | reset)
			{
				handSprite.sprite = handSprites[8];
			}
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Lockpick)
		{
			if (handStartedClicking)
			{
				handSprite.sprite = handSprites[15];
			}
			if (handStoppedClicking | reset)
			{
				handSprite.sprite = handSprites[14];
			}
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.SelfHarm)
		{
			handSprite.sprite = handSprites[11];
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.SelfHarmForceOn)
		{
			handSprite.sprite = handSprites[12];
		}
		else if (currentMinigame.HandType() == Minigame.HandSpriteType.Blade)
		{
			handSprite.sprite = handSprites[13];
		}
	}

	public void UpdateHandPhysics()
	{
		float num = 1f / PHYSICS_UPDATE_RATE;
		Vector2 mousePos = GetMousePos();
		if (mousePos.x > 524f)
		{
			mousePos.x = 524f;
		}
		if (mousePos.y > 524f)
		{
			mousePos.y = 524f;
		}
		if (mousePos.x < -524f)
		{
			mousePos.x = -524f;
		}
		if (mousePos.y < -524f)
		{
			mousePos.y = -524f;
		}
		float maxLength = 75f;
		float num2 = 0.25f;
		float num3 = 4f + body.skills.STRFrom10 * 0.3f;
		float num4 = 1.5f;
		num3 *= body.consciousness * 0.01f;
		num2 *= body.consciousness * 0.01f;
		Vector2 b = Vector2.ClampMagnitude((mousePos - handPos) * num2, maxLength);
		handVelocity = Vector2.Lerp(handVelocity, b, num3 * num);
		handVelocity = Vector2.Lerp(handVelocity, Vector2.zero, num * num4);
		handPos += handVelocity * num * 120f;
	}

	public Vector2 GetMousePos()
	{
		return ((Vector2)Input.mousePosition - (Vector2)minigameScreen.transform.position) / PlayerCamera.uiScale + new Vector2(Mathf.PerlinNoise(Time.time * 2.9f, Time.time * 2.7f) - 0.5f, Mathf.PerlinNoise(Time.time * 2.86f, Time.time * 2.2f) - 0.5f) * body.averagePain * Mathf.Clamp01(1f - body.skills.RESFrom10 * 0.06f) * handShakeForce;
	}

	public void UpdateHandVisuals()
	{
		handClicking = Input.GetKey(KeyBinds.GetBind("attack")) && !forceUngrab;
		handStartedClicking = Input.GetKeyDown(KeyBinds.GetBind("attack"));
		handStoppedClicking = Input.GetKeyUp(KeyBinds.GetBind("attack")) || firstFrameForceUngrab;
		if (handStoppedClicking && !firstFrameForceUngrab)
		{
			forceUngrab = false;
		}
		firstFrameForceUngrab = false;
		handTransform.anchoredPosition = handPos;
		handTransform.eulerAngles = new Vector3(0f, 0f, handVelocity.x * 0.5f + currentMinigame.HandRotOffset());
		if (handStartedClicking)
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Click);
		}
		if (handStoppedClicking)
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.MiniClick);
		}
		UpdateHandSprite();
	}

	private void Update()
	{
		if ((bool)currentMinigame)
		{
			ActiveMinigameUpdate();
		}
		else
		{
			minigameScreen.anchoredPosition = Vector2.Lerp(minigameScreen.anchoredPosition, Vector2.down * 1200f, Time.deltaTime * 10f);
		}
	}

	private void ActiveMinigameUpdate()
	{
		exitTip.SetActive(currentMinigame.CanExit());
		foreach (GameObject textBackground in textBackgrounds)
		{
			textBackground.SetActive(currentMinigame.CanExit());
		}
		if ((!currentItem && currentMinigame.NeedsItem()) || !body.conscious)
		{
			EndMinigame();
			return;
		}
		physicsUpdateTime += Time.deltaTime * PHYSICS_UPDATE_RATE;
		while (physicsUpdateTime > 1f)
		{
			UpdateHandPhysics();
			currentMinigame.PhysicsUpdate(1f / PHYSICS_UPDATE_RATE);
			physicsUpdateTime--;
		}
		UpdateHandVisuals();
		currentMinigame.Update(UIUtil.GetEventSystemRaycastResults(handSprite.transform.position));
		minigameScreen.anchoredPosition = Vector2.Lerp(minigameScreen.anchoredPosition, Vector2.zero, Time.deltaTime * 10f);
		if ((bool)currentItem)
		{
			itemText.text = (currentItem.Stats.rec.recognizable ? currentItem.fullName : Locale.GetOther("unknownobject"));
		}
		else
		{
			itemText.text = "";
		}
	}

	private void Start()
	{
		main = this;
		handSprite = handTransform.GetComponent<Image>();
		if (!PlayerCamera.main.body.hasFur)
		{
			handSprites = handSpritesFurless;
		}
	}

	public void StartMinigame(Minigame minigame, Item item)
	{
		if (!currentMinigame)
		{
			handShakeForce = WorldGeneration.GetRunSettingFloat("minigamehandshake");
			if ((bool)item)
			{
				itemImage.enabled = true;
				currentItem = item;
				itemImage.sprite = item.GetComponent<SpriteRenderer>().sprite;
				itemImage.rectTransform.sizeDelta = PlayerCamera.ImageSizeDelta(itemImage.sprite.texture, 8f, 40f);
			}
			else
			{
				itemImage.enabled = false;
			}
			currentMinigame = minigame;
			currentMinigame.Start();
			guideText.text = Locale.GetOther(minigame.GuideLocaleString());
			UpdateHandSprite(reset: true);
		}
	}

	public void EndMinigame()
	{
		if ((bool)currentMinigame)
		{
			if ((bool)spawnedMiniGame)
			{
				Object.Destroy(spawnedMiniGame.gameObject);
			}
			currentItem = null;
			currentMinigame = null;
		}
	}

	public void CreateScreen(string resourceId)
	{
		GameObject gameObject = Utils.Create(resourceId, minigameScreen);
		gameObject.transform.SetSiblingIndex(0);
		spawnedMiniGame = gameObject.transform;
	}
}
