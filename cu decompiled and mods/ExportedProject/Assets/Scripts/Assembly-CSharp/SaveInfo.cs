using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveInfo
{
	public Body body;

	public Limb[] limbs;

	public SavedItem[] items;

	public Dictionary<Type, Component>[] itemComponents;

	public Dictionary<Type, Component> bodyComponents;

	public Dictionary<Type, Component>[] limbComponents;

	public int biome;

	public int endlessLoops;

	public int totalTraveled;

	public float runTime;

	public int cHeight;

	public int cAge;

	public int cId;

	public int cVer;

	public SavedRecipeData savedRecipeData;

	public float[] lastHappiness;

	public float trapRarity;

	public float lootRarity;

	public int caloriesConsumed;

	public int character;

	public bool isMercRun;

	public bool cheatedSave;

	public List<(string, object)> runSettings;
}
