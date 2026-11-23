using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
	private struct BlockPlaceQuery
	{
		public Vector2 position;

		public ushort blockType;
	}

	public static TutorialHandler main;

	public IKHandle clawArm;

	public TutorialCourse activeCourse;

	public Vector2 handPos;

	public TextMeshProUGUI speakerUI;

	public TextMeshPro courseText;

	public Talker speakerTalker;

	public Vector2 handPosCurrent;

	public AudioSource clawArmSound;

	public Material clawArmOpen;

	public Material clawArmClosed;

	public Material clawArmPlace;

	public Material clawArmKnife;

	private List<BlockPlaceQuery> blockPlaceQueue;

	private List<Vector2> placedBlockHistory;

	public bool armKnifeSpriteOverride;

	public float armSpeedMultiplier = 1f;

	public TurretScript[] turrets;

	private bool escapeSequenceActive;

	public (Transform grabbed, int typeOfGrab) grabInfo;

	public string objectToCreate;

	public Animator doorAnimator;

	private Coroutine sequenceCoroutine;

	public GameObject courseSelectScreen;

	public GameObject courseTextPrefab;

	private List<TextMeshProUGUI> courseTexts;

	public int selectedCourse;

	public float selectedCourseSmooth;

	public Button leftButton;

	public Button rightButton;

	public Image healthPanelHighlight;

	public Image customHighlight;

	public bool showHealthPanelHighlight;

	public bool showUiHighlight;

	public bool forcePlayerAlive = true;

	public GameObject lastSpawnedObject;

	public AudioClip[] tutorialMusic;

	private Rect tutorialBounds;

	public TextMeshProUGUI unlockRequirement;

	public static List<(string localeName, int depthReq, Type type)> availableCourses = new List<(string, int, Type)>
	{
		("basicscourse", 0, typeof(BasicCourse)),
		("firstaidcourse", 1, typeof(FirstAidCourse)),
		("craftingcourse", 2, typeof(CraftingCourse)),
		("movementcourse", 2, typeof(MovementCourse)),
		("sandbox", 0, typeof(SandboxCourse))
	};

	public bool checkForEscape = true;

	private Body body => PlayerCamera.main.body;

	public bool finishedTalking => speakerTalker.FinishedTalking();

	public bool blockQueueEmpty => blockPlaceQueue.Count == 0;

	private void Awake()
	{
		main = this;
	}

	public float ClawDistance(Vector2 pos)
	{
		return Vector2.Distance(handPosCurrent, pos);
	}

	public bool ClawAt(Vector2 pos)
	{
		return handPosCurrent == pos;
	}

	private void Start()
	{
		body.forceWalk = true;
		handPos = Vector2.zero + Vector2.right * 4f;
		blockPlaceQueue = new List<BlockPlaceQuery>();
		placedBlockHistory = new List<Vector2>();
		tutorialBounds = new Rect(Vector2.zero, new Vector2(46f, 28f));
		tutorialBounds.center = Vector2.up * 9.5f;
	}

	public static void CompleteTut(int index)
	{
		PlayerPrefs.SetInt("tutcomplete" + index, 1);
		int num = 4;
		for (int i = 0; i < num; i++)
		{
			if (PlayerPrefs.GetInt("tutcomplete" + i) == 0)
			{
				return;
			}
		}
		SteamManager.TryUnlockAchievement("GRADUATE", force: true);
	}

	public void StartCourse<T>() where T : TutorialCourse
	{
		if (!activeCourse)
		{
			GameObject gameObject = new GameObject(typeof(T).Name);
			activeCourse = gameObject.AddComponent<T>();
			sequenceCoroutine = StartCoroutine(activeCourse.Sequence());
		}
	}

	public void StartCourse(Type type)
	{
		if (!activeCourse)
		{
			if (!typeof(TutorialCourse).IsAssignableFrom(type) || type.IsAbstract)
			{
				Debug.LogError($"Invalid tutorial course type: {type}");
				return;
			}
			GameObject gameObject = new GameObject(type.Name);
			activeCourse = (TutorialCourse)gameObject.AddComponent(type);
			sequenceCoroutine = StartCoroutine(activeCourse.Sequence());
		}
	}

	public void EndCourse()
	{
		if ((bool)activeCourse)
		{
			activeCourse.StopAllCoroutines();
			StopCoroutine(sequenceCoroutine);
			UnityEngine.Object.Destroy(activeCourse.gameObject);
			activeCourse = null;
		}
	}

	public void Speak(string text)
	{
		speakerTalker.Talk(text);
	}

	public void StartSelectedCourse()
	{
		if (PlayerPrefs.GetInt("deepestlayer") < availableCourses[selectedCourse].depthReq)
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
			return;
		}
		if (body.charType != 0 && availableCourses[selectedCourse].localeName != "sandbox")
		{
			PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Deny);
			return;
		}
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.Click);
		StartCourse(availableCourses[selectedCourse].type);
		courseSelectScreen.SetActive(value: false);
	}

	public void HandleCourseSelect()
	{
		if (!courseSelectScreen.activeSelf)
		{
			return;
		}
		if (Input.GetKeyDown(KeyBinds.GetBind("pause")))
		{
			PlayerCamera.main.ToMainMenu();
			return;
		}
		int num = PlayerPrefs.GetInt("deepestlayer");
		if (courseTexts == null)
		{
			courseTexts = new List<TextMeshProUGUI>();
			foreach (var availableCourse in availableCourses)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(courseTextPrefab);
				gameObject.SetActive(value: true);
				gameObject.transform.SetParent(courseSelectScreen.transform.GetChild(0));
				gameObject.GetComponent<TextMeshProUGUI>().text = Locale.GetOther(availableCourse.localeName);
				courseTexts.Add(gameObject.GetComponent<TextMeshProUGUI>());
			}
		}
		selectedCourseSmooth = Mathf.Lerp(selectedCourseSmooth, selectedCourse, Time.deltaTime * 8f);
		for (int i = 0; i < courseTexts.Count; i++)
		{
			float num2 = (float)i * 450f - selectedCourseSmooth * 450f;
			courseTexts[i].rectTransform.anchoredPosition = Vector2.right * num2;
			courseTexts[i].rectTransform.localScale = Vector3.one / (1f + Mathf.Abs(num2) / 450f);
			float num3 = ((num < availableCourses[i].depthReq || (body.charType != 0 && availableCourses[i].localeName != "sandbox")) ? 0f : 1f);
			float num4 = ((PlayerPrefs.GetInt("tutcomplete" + i) > 0) ? 0.2f : 1f);
			courseTexts[i].color = new Color(1f, num3, num3, courseTexts[i].rectTransform.localScale.x * num4);
		}
		string text = "";
		if (PlayerPrefs.GetInt("deepestlayer") < availableCourses[selectedCourse].depthReq)
		{
			text = Locale.GetOther(availableCourses[selectedCourse].localeName + "unlock");
		}
		if (body.charType != 0 && availableCourses[selectedCourse].localeName != "sandbox")
		{
			text = Locale.GetOther("tutorialspecieslock");
		}
		unlockRequirement.text = text;
	}

	public void ChangeSelectedCourse(int change)
	{
		PlayerCamera.main.PlayUISound(PlayerCamera.UISoundType.MiniClick);
		selectedCourse += change;
		if (selectedCourse < 0)
		{
			selectedCourse = availableCourses.Count - 1;
		}
		if (selectedCourse >= availableCourses.Count)
		{
			selectedCourse = 0;
		}
	}

	private void Update()
	{
		if (Application.isEditor && Input.GetKeyDown(KeyCode.B))
		{
			speakerTalker.Skip();
		}
		if (WorldGeneration.world.generatingWorld)
		{
			return;
		}
		if (!WorldGeneration.world.worldExists)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		HandleCourseSelect();
		UpdateHighlights();
		if (!escapeSequenceActive && checkForEscape)
		{
			TurretScript[] array = turrets;
			foreach (TurretScript turretScript in array)
			{
				if (!turretScript || turretScript.timeSinceFired < 5f)
				{
					escapeSequenceActive = true;
					EndCourse();
					StartCourse<EscapeCourse>();
					break;
				}
			}
			if ((bool)activeCourse && !tutorialBounds.Contains(body.transform.position))
			{
				escapeSequenceActive = true;
				EndCourse();
				StartCourse<EscapeCourse>();
			}
		}
		if ((bool)activeCourse)
		{
			courseText.text = Locale.GetOther(activeCourse.LocaleName());
		}
		float num = 20f * armSpeedMultiplier;
		if ((bool)grabInfo.grabbed && grabInfo.typeOfGrab == 0)
		{
			num *= 0.5f;
		}
		handPosCurrent = Vector2.MoveTowards(handPosCurrent, handPos, Time.deltaTime * num);
		clawArm.targetPos = handPosCurrent;
		clawArmSound.transform.position = (handPosCurrent + clawArm.rootPos) / 2f;
		clawArmSound.volume = Mathf.MoveTowards(clawArmSound.volume, (handPosCurrent == handPos) ? 0f : 1f, Time.deltaTime * 4f);
		clawArmSound.pitch = Mathf.MoveTowards(clawArmSound.pitch, (handPosCurrent == handPos) ? 0.7f : 1f, Time.deltaTime * 2.5f);
		if (!string.IsNullOrEmpty(speakerTalker.text.text))
		{
			speakerUI.text = speakerTalker.text.text;
		}
		if (blockPlaceQueue.Count > 0)
		{
			handPos = blockPlaceQueue[0].position;
			if (handPosCurrent == blockPlaceQueue[0].position)
			{
				WorldGeneration.world.SetBlock(WorldGeneration.world.WorldToBlockPos(handPosCurrent), blockPlaceQueue[0].blockType);
				if (blockPlaceQueue[0].blockType > 0)
				{
					placedBlockHistory.Add(handPosCurrent);
					Sound.Play("tutorialblockplace", handPos, twoDimensional: false, pitchShift: false);
				}
				else
				{
					Sound.Play("tutorialblockremove", handPos, twoDimensional: false, pitchShift: false);
				}
				blockPlaceQueue.RemoveAt(0);
			}
		}
		if (!string.IsNullOrEmpty(objectToCreate))
		{
			handPos = new Vector2(10f, 27f);
			if (handPos == handPosCurrent)
			{
				GameObject gameObject = Utils.Create(objectToCreate, handPos, 0f);
				Grab(gameObject.transform);
				objectToCreate = "";
				Sound.Play("tutorialdispense", handPos, twoDimensional: false, pitchShift: false);
				if (gameObject.TryGetComponent<Item>(out var component))
				{
					component.rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
				}
				lastSpawnedObject = gameObject;
			}
		}
		if ((bool)grabInfo.grabbed)
		{
			clawArm.lineRenderer.material = clawArmClosed;
		}
		else if (armKnifeSpriteOverride)
		{
			clawArm.lineRenderer.material = clawArmKnife;
		}
		else if (!blockQueueEmpty)
		{
			clawArm.lineRenderer.material = clawArmPlace;
		}
		else
		{
			clawArm.lineRenderer.material = clawArmOpen;
		}
		if (forcePlayerAlive)
		{
			PreventFromDying();
		}
	}

	public void SetHighlightPosition(Vector2 pos)
	{
		customHighlight.rectTransform.anchoredPosition = pos;
	}

	public void SetHighlightPosition(float x, float y)
	{
		customHighlight.rectTransform.anchoredPosition = new Vector2(x, y);
	}

	public void SetHighlightSize(float width, float height)
	{
		customHighlight.rectTransform.sizeDelta = new Vector2(width, height);
	}

	public void SetHighlightAnchor(float min1, float min2, float max1, float max2)
	{
		customHighlight.rectTransform.anchorMin = new Vector2(min1, min2);
		customHighlight.rectTransform.anchorMax = new Vector2(max1, max2);
	}

	private void UpdateHighlights()
	{
		Color color = Color.white.WithAlpha(Mathf.PingPong(Time.unscaledTime * 3f, 1f));
		if ((!PlayerCamera.main.woundView.activeSelf && showHealthPanelHighlight) || (PlayerCamera.main.woundView.activeSelf && !showHealthPanelHighlight))
		{
			healthPanelHighlight.color = color;
		}
		else
		{
			healthPanelHighlight.color = Color.clear;
		}
		if (showUiHighlight)
		{
			customHighlight.color = color;
		}
		else
		{
			customHighlight.color = Color.clear;
		}
	}

	private void FixedUpdate()
	{
		if ((bool)grabInfo.grabbed)
		{
			if (grabInfo.typeOfGrab == 0)
			{
				body.shock = 30f;
				body.Ragdoll();
				body.limbs[2].rb.MovePosition(handPosCurrent);
			}
			else if (grabInfo.typeOfGrab == 1)
			{
				grabInfo.grabbed.GetComponent<Rigidbody2D>().MovePosition(handPosCurrent);
				grabInfo.grabbed.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
				grabInfo.grabbed.GetComponent<Rigidbody2D>().angularVelocity = 0f;
			}
			else if (grabInfo.typeOfGrab == 2)
			{
				grabInfo.grabbed.position = handPosCurrent;
			}
		}
	}

	public void Grab(Transform transform)
	{
		if (!grabInfo.grabbed)
		{
			grabInfo.grabbed = transform;
			if (transform == body.transform)
			{
				grabInfo.typeOfGrab = 0;
			}
			if ((bool)transform.GetComponent<Item>())
			{
				grabInfo.typeOfGrab = 1;
			}
			if ((bool)transform.GetComponent<BuildingEntity>())
			{
				grabInfo.typeOfGrab = 2;
			}
		}
	}

	public void Ungrab()
	{
		if ((bool)grabInfo.grabbed)
		{
			if (grabInfo.typeOfGrab > 0 && (grabInfo.typeOfGrab != 1 || (!body.HoldingItem(grabInfo.grabbed.GetComponent<Item>()) && (!grabInfo.grabbed.GetComponent<Item>().Stats.wearable || !body.HasWearable(grabInfo.grabbed.GetComponent<Item>())))))
			{
				grabInfo.grabbed.transform.position = handPos;
			}
			grabInfo.grabbed = null;
		}
		if (grabInfo.typeOfGrab == 0)
		{
			body.shock = 0f;
			body.Stand();
		}
	}

	public void PreventFromDying()
	{
		if (body.thirst < 60f)
		{
			body.thirst = 60f;
		}
		if (body.hunger < 60f)
		{
			body.hunger = 60f;
		}
		if (body.bloodPressure < 61f)
		{
			body.bloodPressure = 61f;
		}
		if (body.fibrillationProgress > 50f)
		{
			body.fibrillationProgress = 50f;
		}
		if (body.brainHealth < 96f)
		{
			body.brainHealth = 96f;
		}
		if (body.happiness < -15f)
		{
			body.happiness = -15f;
		}
	}

	public void ClearAllPlacedBlocks()
	{
		foreach (Vector2 item in placedBlockHistory)
		{
			AddBlockQuery(item, 0);
		}
		placedBlockHistory.Clear();
		WorldGeneration.world.ClearBlockDamages();
	}

	public void AddBlockQuery(Vector2 pos, ushort blockType)
	{
		blockPlaceQueue.Add(new BlockPlaceQuery
		{
			position = pos,
			blockType = blockType
		});
	}

	public void AddBlockQuery(float x, float y, ushort blockType)
	{
		blockPlaceQueue.Add(new BlockPlaceQuery
		{
			position = new Vector2(x, y),
			blockType = blockType
		});
	}

	public void AddBlockQuery(int x, int y, ushort blockType)
	{
		blockPlaceQueue.Add(new BlockPlaceQuery
		{
			position = new Vector2((float)x + 0.5f, (float)y + 0.5f),
			blockType = blockType
		});
	}
}
