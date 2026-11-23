using System.Collections.Generic;

public class LiquidItemInfo : ItemInfo
{
	public float capacity;

	public bool autoFill = true;

	public List<LiquidStack> defaultContents;
}
