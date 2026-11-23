using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DuneMode : MonoBehaviour
{
	public static DuneMode dune;

	private const int objectsPerFrame = 4;

	private GameObject[] objects;

	private Material duneMat;

	private void Awake()
	{
		dune = this;
		Object.DontDestroyOnLoad(base.gameObject);
		StartCoroutine(ShuffleObjects());
		duneMat = Resources.Load<Material>("Special/DuneMaterial");
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	public IEnumerator ShuffleObjects()
	{
		while (true)
		{
			objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
			yield return new WaitForSecondsRealtime(20f);
		}
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		objects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
	}

	private void Update()
	{
		if (objects.Length == 0)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			GameObject gameObject = objects.PickRandom();
			if (!(gameObject == null))
			{
				Image component2;
				TilemapRenderer component3;
				TextMeshProUGUI component4;
				if (gameObject.TryGetComponent<SpriteRenderer>(out var component))
				{
					component.material = duneMat;
				}
				else if (gameObject.TryGetComponent<Image>(out component2))
				{
					component2.material = duneMat;
				}
				else if (gameObject.TryGetComponent<TilemapRenderer>(out component3))
				{
					component3.material = duneMat;
				}
				else if (gameObject.TryGetComponent<TextMeshProUGUI>(out component4))
				{
					component4.material = duneMat;
				}
			}
		}
	}
}
