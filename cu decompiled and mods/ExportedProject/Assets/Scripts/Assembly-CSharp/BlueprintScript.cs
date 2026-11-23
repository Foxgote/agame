using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class BlueprintScript : MonoBehaviour
{
	[JsonProperty]
	public int recipeIndex;

	private void Awake()
	{
		recipeIndex = Recipes.RandomRecipeIndex();
	}
}
