public class TraderItem
{
	public string id;

	public bool bought;

	public int value;

	public TraderScript.TraderItemPreference preference;

	public ItemInfo info => Item.GetItem(id);
}
