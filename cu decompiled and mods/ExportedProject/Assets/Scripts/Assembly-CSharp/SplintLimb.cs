using Newtonsoft.Json;
using UnityEngine;

[Saveable]
[JsonObject(MemberSerialization.OptIn)]
public class SplintLimb : MonoBehaviour
{
	[JsonProperty]
	public float condition;

	[JsonProperty]
	public float conditionLossMinute;

	private Limb limb;

	[JsonProperty]
	public string item;

	private void Start()
	{
		limb = GetComponent<Limb>();
		limb.splinted = true;
		WoundView.view.AddImageToLimb(limb, Resources.Load<Sprite>("Special/splintIcon"), flip: false, (Limb x) => !x.GetComponent<SplintLimb>());
	}

	public void TakeOff()
	{
		GameObject gameObject = Object.Instantiate(Resources.Load(item), base.transform.position, Quaternion.identity) as GameObject;
		gameObject.GetComponent<Item>().condition = condition;
		limb.body.AutoPickUpItem(gameObject.GetComponent<Item>());
		limb.splinted = false;
		Object.Destroy(this);
	}

	public void OnDismember()
	{
		TakeOff();
	}

	private void Update()
	{
		condition -= conditionLossMinute * Time.deltaTime / 60f;
		if (condition <= 0f)
		{
			TakeOff();
		}
		limb.boneHealTimer -= Limb.boneHealSpeed * Time.deltaTime * 1.5f;
		limb.dislocationTimer -= Limb.dislocationHealSpeed * Time.deltaTime;
	}
}
