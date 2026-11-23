public class CraftingQuality
{
	public string id;

	public float amount;

	public string LocaleName => Locale.GetOther("cq" + id);

	public CraftingQuality(string id, float amount)
	{
		this.id = id;
		this.amount = amount;
	}

	public CraftingQuality(string id)
	{
		this.id = id;
		amount = 1f;
	}

	public static implicit operator CraftingQuality(string str)
	{
		return new CraftingQuality(str);
	}
}
