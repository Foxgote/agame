using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class KeypadMinigame : Minigame
{
	public List<GameObject> inputButtons;

	private GameObject clearButton;

	private TextMeshProUGUI matchText;

	private TextMeshProUGUI inputText;

	public BuildingEntity toDestroy;

	public string match;

	public string current;

	public override HandSpriteType HandType()
	{
		return HandSpriteType.Point;
	}

	public override string GuideLocaleString()
	{
		return "lifepodMinigameGuide";
	}

	public override bool NeedsItem()
	{
		return false;
	}

	public KeypadMinigame(BuildingEntity build, string code)
	{
		toDestroy = build;
		match = code;
	}

	public static string AddDashes(string input)
	{
		if (string.IsNullOrEmpty(input))
		{
			return input;
		}
		return string.Join("-", input.ToCharArray());
	}

	public static string GenerateCode()
	{
		string text = "";
		int num = Random.Range(6, 13);
		for (int i = 0; i < num; i++)
		{
			text += Random.Range(0, 10);
		}
		return text;
	}

	public override void Start()
	{
		Minigame.game.CreateScreen("Special/LifepodMinigame");
		clearButton = Minigame.game.spawnedMiniGame.GetChild(2).gameObject;
		matchText = Minigame.game.spawnedMiniGame.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
		inputText = Minigame.game.spawnedMiniGame.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
		inputButtons = new List<GameObject>();
		for (int i = 3; i < 13; i++)
		{
			inputButtons.Add(Minigame.game.spawnedMiniGame.GetChild(i).gameObject);
		}
		current = "";
		matchText.text = AddDashes(match);
	}

	public void CheckForMegalovania()
	{
		if (current.Contains("2296") && !match.Contains("2296"))
		{
			Minigame.game.EndMinigame();
			WorldGeneration.CreateExplosion(new ExplosionParams
			{
				position = Minigame.game.body.transform.position,
				bleedAmount = new RangeF(0f, 2f),
				shrapnelChance = 0f,
				skinDamage = new RangeF(7f, 10f),
				muscleDamage = new RangeF(8f, 10f),
				range = 3f,
				disfigureChance = 0f,
				boneBreakChance = 0f,
				dislocationChance = 0f,
				velocity = 30f,
				bleedChance = 0.1f,
				skinDamageChance = 0.5f,
				structuralDamage = 200f
			});
		}
	}

	public void CheckCasts(List<RaycastResult> uiCasts)
	{
		foreach (RaycastResult uiCast in uiCasts)
		{
			if (inputButtons.Contains(uiCast.gameObject))
			{
				Sound.Play("beep" + uiCast.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text, Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
				current += uiCast.gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
				Minigame.game.handVelocity *= 0.3f;
				CheckForMegalovania();
				break;
			}
			if (uiCast.gameObject == clearButton)
			{
				Sound.Play("beep0", Vector2.zero, twoDimensional: true, pitchShift: false, null, 1f, 1f, noReverb: true);
				current = "";
			}
		}
	}

	public override void Update(List<RaycastResult> uiCasts)
	{
		if (!toDestroy)
		{
			Minigame.game.EndMinigame();
			return;
		}
		if (Minigame.game.handStartedClicking)
		{
			CheckCasts(uiCasts);
		}
		if (current.Length > match.Length)
		{
			current = current.Substring(0, match.Length);
		}
		if (current == match)
		{
			toDestroy.health = 0f;
			Minigame.game.EndMinigame();
		}
		inputText.text = AddDashes(current);
	}

	public override void PhysicsUpdate(float deltaTime)
	{
		Minigame.game.handVelocity *= 0.9f;
		Minigame.game.handPos = Vector2.Lerp(Minigame.game.handPos, Minigame.game.GetMousePos(), deltaTime * 10f);
	}
}
