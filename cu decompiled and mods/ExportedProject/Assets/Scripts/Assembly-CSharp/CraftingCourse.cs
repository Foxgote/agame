using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CraftingCourse : TutorialCourse
{
	private struct SceneEntity
	{
		public bool isEntity;

		public string id;

		public Vector2 pos;

		public string entityLiquid;

		public sbyte liquidTileId;

		public SceneEntity(bool isEntity, string id, Vector2 pos)
		{
			this.isEntity = isEntity;
			this.id = id;
			this.pos = pos;
			entityLiquid = "";
			liquidTileId = -1;
		}

		public SceneEntity(string id, Vector2 pos)
		{
			isEntity = true;
			this.id = id;
			this.pos = pos;
			entityLiquid = "";
			liquidTileId = -1;
		}

		public SceneEntity(string id, Vector2 pos, string liquid)
		{
			isEntity = true;
			this.id = id;
			this.pos = pos;
			entityLiquid = liquid;
			liquidTileId = -1;
		}

		public SceneEntity(sbyte liquidTileId, Vector2 pos)
		{
			isEntity = false;
			id = "";
			this.pos = pos;
			entityLiquid = "";
			this.liquidTileId = liquidTileId;
		}
	}

	private List<GameObject> currentlySpawned;

	private List<SceneEntity> scene;

	private bool sceneBeingBuilt;

	public override IEnumerator Sequence()
	{
		currentlySpawned = new List<GameObject>();
		scene = new List<SceneEntity>();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft1"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft2"));
		TutorialCourse.tutorial.AddBlockQuery(17, -2, 3);
		TutorialCourse.tutorial.AddBlockQuery(18, -2, 3);
		TutorialCourse.tutorial.AddBlockQuery(17, -1, 3);
		TutorialCourse.tutorial.AddBlockQuery(18, -1, 3);
		TutorialCourse.tutorial.AddBlockQuery(-18, -2, 11);
		TutorialCourse.tutorial.AddBlockQuery(-17, -2, 11);
		TutorialCourse.tutorial.AddBlockQuery(-18, -1, 11);
		TutorialCourse.tutorial.AddBlockQuery(-17, -1, 11);
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft3"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		yield return PlaceObjectAt("tutorialcraftingbutton", new Vector2(-3.5f, 1.5f));
		TutorialCourse.tutorial.lastSpawnedObject.AddComponent<TutorialCraftButtonBehaviour>().onReset.AddListener(TryRestartScene);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft4"));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-19.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-18.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-17.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-16.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-15.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-14.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-13.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-12.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-11.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-10.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-9.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-8.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-7.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-6.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-5.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-4.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-3.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-2.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-19.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-18.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-17.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-16.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-15.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(7.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(8.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(9.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(10.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(11.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(12.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(13.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(14.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(15.5f, -1.5f)));
		scene.Add(new SceneEntity("glowplant", new Vector2(-18.87f, 1.35f)));
		scene.Add(new SceneEntity("glowplant", new Vector2(-14.27f, -0.3f)));
		scene.Add(new SceneEntity("geotree", new Vector2(-12.27f, 1.4f)));
		scene.Add(new SceneEntity("geotree", new Vector2(-9.27f, 1.1f)));
		scene.Add(new SceneEntity("geotree", new Vector2(-6.27f, 1.3f)));
		scene.Add(new SceneEntity("geotree", new Vector2(-4.27f, 1.2f)));
		scene.Add(new SceneEntity("hydreed", new Vector2(9.5f, 1f)));
		scene.Add(new SceneEntity("hydreed", new Vector2(13f, 0.96f)));
		yield return BuildScene();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft5"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("foliagebag"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft6"));
		TutorialCourse.body.FindByIdSurface("foliagebag", out var fbag);
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft6a"));
		yield return new WaitUntil(() => !fbag || fbag.colorSwatch == 2);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft6b"));
		yield return new WaitUntil(() => !fbag || !PlayerCamera.main.itemCustomizer.activeSelf);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft7"));
		yield return new WaitUntil(() => Input.GetKey(KeyBinds.GetBind("bark")));
		yield return RestartScene();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft8"));
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("bandage"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft9"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft10"));
		yield return new WaitUntil(() => PlayerCamera.main.pinnedRecipe.HasValue && Recipes.recipes[PlayerCamera.main.pinnedRecipe.Value].result.id == "flimsyknife");
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft11"));
		TutorialCourse.body.forceWalk = false;
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 2.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 3.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(18.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 2.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 3.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(19.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 2.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 3.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(17.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-1.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-0.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(0.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(1.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(2.5f, 4.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-1.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-0.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(0.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(1.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(2.5f, 5.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-1.5f, 6.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-0.5f, 6.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(0.5f, 6.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(1.5f, 6.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(2.5f, 6.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-1.5f, 7.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-0.5f, 7.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(0.5f, 7.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(1.5f, 7.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(2.5f, 7.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-1.5f, 8.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(-0.5f, 8.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(0.5f, 8.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(1.5f, 8.5f)));
		scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(2.5f, 8.5f)));
		yield return RestartScene();
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("flimsyknife"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft12"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft13"));
		yield return new WaitUntil(() =>
		{
			foreach (Item allItem in TutorialCourse.body.GetAllItems())
			{
				if (allItem.id == "flimsyknife" && allItem.favourited)
				{
					return true;
				}
			}
			return false;
		});
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("nice"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		StartCoroutine(EraseScene());
		scene.Clear();
		scene.Add(new SceneEntity("bigpack", new Vector2(-4f, 0f)));
		scene.Add(new SceneEntity("glowplantfruit", new Vector2(-6f, 0f)));
		scene.Add(new SceneEntity("glowplantfruit", new Vector2(-8f, 0f)));
		scene.Add(new SceneEntity("glowplantfruit", new Vector2(-9.5f, 0f)));
		scene.Add(new SceneEntity("bloodsac", new Vector2(4f, 0f)));
		scene.Add(new SceneEntity("bloodsac", new Vector2(5f, 0f)));
		scene.Add(new SceneEntity("waterbottle", new Vector2(8f, 0f)));
		scene.Add(new SceneEntity("canteen", new Vector2(10f, 0f), "soap"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft14"));
		yield return new WaitUntil(() => !sceneBeingBuilt);
		yield return BuildScene();
		TutorialCourse.body.skills.AddExp(2, 500f);
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("sterilizedbandage"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft15"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft16"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft17"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft18"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft19"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		scene.Clear();
		scene.Add(new SceneEntity("flimsyknife", new Vector2(-4f, 0f)));
		scene.Add(new SceneEntity("makeshiftwrench", new Vector2(-6f, 0f)));
		scene.Add(new SceneEntity("canteen", new Vector2(-8f, 0f), "biochem"));
		for (float num = 9.5f; num < 20.5f; num++)
		{
			for (float num2 = -1.5f; num2 < 2.5f; num2++)
			{
				scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(num, num2)));
			}
		}
		for (float num3 = 9.5f; num3 < 20.5f; num3++)
		{
			for (float num4 = 2.5f; num4 < 6.5f; num4++)
			{
				scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(num3, num4)));
			}
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft20"));
		yield return RestartScene();
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("machete"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft21"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft22"));
		scene.Clear();
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-19.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-18.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-17.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-16.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-15.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-14.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-13.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-12.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-11.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-10.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-9.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-8.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-7.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-6.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-5.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-4.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-3.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-2.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-19.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-18.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-17.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-16.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "16", new Vector2(-15.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "10", new Vector2(14.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "10", new Vector2(14.5f, -0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "10", new Vector2(14.5f, 0.5f)));
		scene.Add(new SceneEntity(isEntity: false, "10", new Vector2(14.5f, 1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "10", new Vector2(14.5f, 2.5f)));
		for (float num5 = 2.5f; num5 < 10.5f; num5++)
		{
			for (float num6 = -1.5f; num6 < 2.5f; num6++)
			{
				scene.Add(new SceneEntity(isEntity: false, "11", new Vector2(num5, num6)));
			}
		}
		for (float num7 = -19.5f; num7 < 20.5f; num7++)
		{
			scene.Add(new SceneEntity(isEntity: false, "9", new Vector2(num7, -2.5f)));
		}
		for (float num8 = 15.5f; num8 < 20.5f; num8++)
		{
			for (float num9 = -1.5f; num9 < 4.5f; num9++)
			{
				scene.Add(new SceneEntity(1, new Vector2(num8, num9)));
			}
		}
		scene.Add(new SceneEntity("drybush", new Vector2(-18f, 2f)));
		scene.Add(new SceneEntity("drybush", new Vector2(-14f, 1f)));
		scene.Add(new SceneEntity("drybush", new Vector2(-9f, 1f)));
		scene.Add(new SceneEntity("glowplant", new Vector2(-6f, 0.25f)));
		scene.Add(new SceneEntity("stick", new Vector2(0f, 0f)));
		scene.Add(new SceneEntity("stick", new Vector2(0f, 0f)));
		yield return RestartScene();
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft23"));
		yield return new WaitUntil(() =>
		{
			foreach (Item allItem2 in TutorialCourse.body.GetAllItems())
			{
				if (allItem2.TryGetComponent<WaterContainerItem>(out var component) && component.HasLiquid("water"))
				{
					return true;
				}
			}
			return false;
		});
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft24"));
		yield return EraseScene();
		scene.Clear();
		for (float num10 = -19.5f; num10 < 20.5f; num10++)
		{
			TutorialCourse.tutorial.AddBlockQuery(new Vector2(num10, -2.5f), 9);
		}
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		scene.Add(new SceneEntity("charcoal", new Vector2(-8f, -1f)));
		scene.Add(new SceneEntity("charcoal", new Vector2(-9f, -1f)));
		scene.Add(new SceneEntity("charcoal", new Vector2(-10f, -1f)));
		scene.Add(new SceneEntity("lighter", new Vector2(-12f, -1f)));
		scene.Add(new SceneEntity("animalflesh", new Vector2(3f, -1f)));
		scene.Add(new SceneEntity("experimentflesh", new Vector2(3f, -1f)));
		scene.Add(new SceneEntity("brownshroom", new Vector2(6f, -1f)));
		scene.Add(new SceneEntity("brownshroom", new Vector2(7.3f, -1f)));
		scene.Add(new SceneEntity("brownshroom", new Vector2(8.6f, -1f)));
		scene.Add(new SceneEntity("brownshroom", new Vector2(9.2f, -1f)));
		scene.Add(new SceneEntity("sickle", new Vector2(14f, 1f)));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft25"));
		yield return BuildScene();
		yield return new WaitUntil(() => TutorialCourse.body.HoldingItem("pemmican"));
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft26"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		yield return EraseScene();
		scene.Clear();
		TutorialCourse.body.skills.AddExp(2, 800f);
		for (float num11 = -19.5f; num11 < 20.5f; num11++)
		{
			scene.Add(new SceneEntity(isEntity: false, "9", new Vector2(num11, -2.5f)));
		}
		scene.Add(new SceneEntity(isEntity: false, "35", new Vector2(19.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "34", new Vector2(18.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "34", new Vector2(17.5f, -1.5f)));
		scene.Add(new SceneEntity(isEntity: false, "34", new Vector2(16.5f, -1.5f)));
		scene.Add(new SceneEntity("waterbottle", new Vector2(2f, -1f)));
		scene.Add(new SceneEntity("materialpouch", new Vector2(0f, -1f)));
		scene.Add(new SceneEntity("titaniummultitool", new Vector2(-4f, -1f)));
		scene.Add(new SceneEntity("canteen", new Vector2(-2f, 0f), "biochem"));
		scene.Add(new SceneEntity("jumppad", new Vector2(13f, -1.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(11f, -1.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(9f, -1.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(7f, -1.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(13f, -0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(11f, -0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(9f, -0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(7f, -0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(13f, 0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(11f, 0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(9f, 0.5f)));
		scene.Add(new SceneEntity("jumppad", new Vector2(7f, 0.5f)));
		for (float num12 = -19.5f; num12 < -14.5f; num12++)
		{
			for (float num13 = -1.5f; num13 < 3.5f; num13++)
			{
				scene.Add(new SceneEntity(isEntity: false, "3", new Vector2(num12, num13)));
			}
		}
		for (float num14 = -19.5f; num14 < -14.5f; num14++)
		{
			for (float num15 = 3.5f; num15 < 10.5f; num15++)
			{
				scene.Add(new SceneEntity(isEntity: false, "8", new Vector2(num14, num15)));
			}
		}
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft27"));
		yield return BuildScene();
		yield return new WaitUntil(() =>
		{
			foreach (Item allItem3 in TutorialCourse.body.GetAllItems())
			{
				if (allItem3.id == "minilaserdrill" && allItem3.condition > 0f)
				{
					return true;
				}
			}
			return false;
		});
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft28"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialCourse.tutorial.Speak(Locale.GetTutorial("craft29"));
		yield return new WaitUntil(() => TutorialCourse.tutorial.finishedTalking);
		TutorialHandler.CompleteTut(2);
		GlobalDark.main.Darken();
		yield return new WaitUntil(() => !GlobalDark.main.IsDarkening());
		SceneManager.LoadScene("PreGen");
	}

	public IEnumerator BuildScene()
	{
		if (sceneBeingBuilt)
		{
			yield break;
		}
		sceneBeingBuilt = true;
		TutorialCourse.tutorial.armSpeedMultiplier = 1.5f;
		TutorialCourse.body.thirst = 100f;
		TutorialCourse.body.hunger = 100f;
		TutorialCourse.body.stamina = 100f;
		TutorialCourse.body.bloodPressure = 120f;
		TutorialCourse.body.energy = 100f;
		TutorialCourse.body.happiness = 0f;
		TutorialCourse.body.bloodOxygen = 100f;
		TutorialCourse.body.brainHealth = 100f;
		TutorialCourse.body.dirtyness = 0f;
		foreach (SceneEntity entity in scene)
		{
			if (entity.isEntity)
			{
				yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
				yield return PlaceObjectAt(entity.id, entity.pos);
				if ((bool)TutorialCourse.tutorial.lastSpawnedObject)
				{
					currentlySpawned.Add(TutorialCourse.tutorial.lastSpawnedObject);
					if (TutorialCourse.tutorial.lastSpawnedObject.TryGetComponent<JumpPadScript>(out var component))
					{
						component.disabled = true;
					}
					if (!string.IsNullOrEmpty(entity.entityLiquid))
					{
						TutorialCourse.tutorial.lastSpawnedObject.GetComponent<WaterContainerItem>().AddLiquid(entity.entityLiquid, 9999f);
					}
				}
			}
			else if (entity.liquidTileId >= 0)
			{
				yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
				yield return PourLiquidAt((byte)entity.liquidTileId, entity.pos);
			}
			else
			{
				TutorialCourse.tutorial.AddBlockQuery(entity.pos, ushort.Parse(entity.id));
			}
		}
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		sceneBeingBuilt = false;
	}

	public void TryRestartScene()
	{
		StartCoroutine(RestartScene());
	}

	public IEnumerator EraseScene()
	{
		if (sceneBeingBuilt)
		{
			yield break;
		}
		sceneBeingBuilt = true;
		TutorialCourse.tutorial.armSpeedMultiplier = 2f;
		foreach (GameObject item in currentlySpawned)
		{
			if ((bool)item)
			{
				Object.Destroy(item);
			}
		}
		currentlySpawned.Clear();
		List<Item> list = new List<Item>();
		list.AddRange(Object.FindObjectsOfType<Item>());
		Rect rect = new Rect(Vector2.zero, new Vector2(42f, 25f))
		{
			center = Vector2.up * 9.5f
		};
		foreach (Item item2 in list)
		{
			if (rect.Contains(item2.transform.position))
			{
				Object.Destroy(item2.gameObject);
			}
		}
		FluidManager.main.Clear();
		TutorialCourse.tutorial.ClearAllPlacedBlocks();
		yield return new WaitUntil(() => TutorialCourse.tutorial.blockQueueEmpty);
		TutorialCourse.tutorial.armSpeedMultiplier = 1f;
		sceneBeingBuilt = false;
	}

	public IEnumerator RestartScene()
	{
		if (!sceneBeingBuilt)
		{
			yield return EraseScene();
			yield return BuildScene();
		}
	}

	public override string LocaleName()
	{
		return "craftingcourse";
	}
}
