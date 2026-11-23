using UnityEngine;

public class BatteryRecharger : MonoBehaviour
{
	private Item[] items;

	private FixedJoint2D[] joints;

	public Transform[] targets;

	private bool firstTime = true;

	private bool isFull
	{
		get
		{
			if ((bool)items[0])
			{
				return items[1];
			}
			return false;
		}
	}

	private int firstAvailable
	{
		get
		{
			if (!items[0])
			{
				return 0;
			}
			return 1;
		}
	}

	private void Start()
	{
		items = new Item[2];
		joints = new FixedJoint2D[2];
	}

	private void OnDestroy()
	{
		FixedJoint2D[] array = joints;
		foreach (FixedJoint2D fixedJoint2D in array)
		{
			if ((bool)fixedJoint2D)
			{
				Object.Destroy(fixedJoint2D);
			}
		}
	}

	public void OnUse()
	{
		Item item = PlayerCamera.main.body.GetItem(PlayerCamera.main.body.handSlot);
		if ((bool)item && item.Stats is BatteryInfo && !isFull)
		{
			PlayerCamera.main.body.DropItem(item);
			item.transform.rotation = Quaternion.identity;
			item.transform.position = targets[firstAvailable].position;
			item.rb.rotation = 0f;
			item.rb.position = item.transform.position;
			item.GetComponent<SpriteRenderer>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
			joints[firstAvailable] = item.gameObject.AddComponent<FixedJoint2D>();
			joints[firstAvailable].frequency = 20f;
			joints[firstAvailable].autoConfigureConnectedAnchor = true;
			joints[firstAvailable].connectedAnchor = targets[firstAvailable].position;
			items[firstAvailable] = item;
			Sound.Play("batteryinsert", base.transform.position);
			if (firstTime)
			{
				GetComponent<Talker>().Talk(Locale.GetOther("rechargingstationcheer"));
				Item component = Utils.Create("mp3player", base.transform.position, 0f).GetComponent<Item>();
				PlayerCamera.main.body.AutoPickUpItem(component);
				component.battery.UnloadBattery(skipSpawn: true);
				firstTime = false;
			}
		}
	}

	private void Update()
	{
		for (int i = 0; i < items.Length; i++)
		{
			if (items[i] != null && items[i].Stats is BatteryInfo batteryInfo)
			{
				items[i].condition = Mathf.MoveTowards(items[i].condition, 1f, Time.deltaTime / batteryInfo.maxCharge * 1.35f);
				if ((bool)items[i].transform.parent)
				{
					Object.Destroy(joints[i]);
					items[i] = null;
					Sound.Play("batteryinsert", base.transform.position);
				}
			}
		}
	}
}
