using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class AmmoScript : MonoBehaviour
{
	public enum AmmoItemType
	{
		Round = 0,
		Magazine = 1
	}

	public AmmoItemType itemType;

	public GunScript.AmmoType ammoType;

	[JsonProperty]
	public int maxRounds;

	[JsonProperty]
	public int rounds;

	public void UnloadRound()
	{
		if (rounds > 0)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load(AmmoTypeToItem(ammoType)), base.transform.position, Quaternion.identity) as GameObject;
			PlayerCamera.main.body.AutoPickUpItem(gameObject.GetComponent<Item>());
			Sound.Play("gunloadshell", base.transform.position);
			rounds--;
		}
	}

	public void LoadRound(AmmoScript ammo)
	{
		if (ammo.itemType == AmmoItemType.Round && itemType == AmmoItemType.Magazine && ammo.ammoType == ammoType && rounds < maxRounds)
		{
			rounds++;
			Sound.Play("gunloadshell", base.transform.position);
			Object.Destroy(ammo.gameObject);
		}
	}

	public static string AmmoTypeToItem(GunScript.AmmoType ammo)
	{
		return ammo switch
		{
			GunScript.AmmoType.Pistol => "9mmround", 
			GunScript.AmmoType.Rifle => "556round", 
			GunScript.AmmoType.Shotgun => "12gauge", 
			_ => "geofruit", 
		};
	}

	public static string AmmoTypeToMagazine(GunScript.AmmoType ammo)
	{
		return ammo switch
		{
			GunScript.AmmoType.Pistol => "smallmagazine", 
			GunScript.AmmoType.Rifle => "riflemagazine", 
			_ => "geofruit", 
		};
	}
}
