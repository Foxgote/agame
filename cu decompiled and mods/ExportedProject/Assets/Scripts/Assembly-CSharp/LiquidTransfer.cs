using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LiquidTransfer : MonoBehaviour
{
	public Image amountBar;

	public Image amountTick;

	public TextMeshProUGUI amountText;

	public WaterContainerItem transferTo;

	public WaterContainerItem transferFrom;

	public float ml;

	public float amount;

	public static LiquidTransfer instance;

	private void Awake()
	{
		amount = 1f;
		instance = this;
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyBinds.GetBind("iteminteract")) || Input.GetKeyDown(KeyBinds.GetBind("pause")))
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		if (Input.GetKey(KeyBinds.GetBind("attack")) && (Input.mousePosition - base.transform.position).magnitude > 80f * PlayerCamera.uiScale)
		{
			amount = NormalToValue(Input.mousePosition - base.transform.position);
		}
		amountBar.fillAmount = amount;
		amountTick.transform.eulerAngles = new Vector3(0f, 0f, Mathf.Lerp(180f, 0f, amount));
		amountBar.color = transferFrom.AverageColor();
		ml = Mathf.Min(transferFrom.CurrentTotal, transferTo.SpaceLeft) * amount;
		amountText.text = ">" + Mathf.RoundToInt(ml) + "mL";
	}

	public void Finish()
	{
		PlayerCamera.main.body.CombineLiquids(transferTo, transferFrom, ml);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void OnDestroy()
	{
		instance = null;
	}

	public static float NormalToValue(Vector2 normal)
	{
		if (normal.y < 0f)
		{
			normal.y = 0f;
		}
		if (normal.sqrMagnitude > 0f)
		{
			normal.Normalize();
			float num = Mathf.Atan2(normal.y, normal.x);
			return 1f - num / MathF.PI;
		}
		return 0f;
	}
}
