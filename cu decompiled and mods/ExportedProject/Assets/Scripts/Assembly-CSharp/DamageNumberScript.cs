using TMPro;
using UnityEngine;

public class DamageNumberScript : MonoBehaviour
{
	private void Update()
	{
		Color color = GetComponent<TextMeshPro>().color;
		color.a -= Time.deltaTime * 2f;
		GetComponent<TextMeshPro>().color = color;
	}
}
