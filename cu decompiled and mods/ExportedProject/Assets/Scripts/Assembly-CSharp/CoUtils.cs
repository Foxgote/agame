using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CoUtils : MonoBehaviour
{
	private class TimedOpState
	{
		public float remaining;

		public float maxHad;
	}

	private static CoUtils _inst;

	private Dictionary<string, TimedOpState> activeOps = new Dictionary<string, TimedOpState>();

	public static CoUtils instance
	{
		get
		{
			if (_inst == null)
			{
				GameObject obj = new GameObject("CoUtilsInstance");
				_inst = obj.AddComponent<CoUtils>();
				UnityEngine.Object.DontDestroyOnLoad(obj);
			}
			return _inst;
		}
	}

	private void Awake()
	{
		SceneManager.activeSceneChanged += OnSceneChanged;
	}

	private void OnDestroy()
	{
		SceneManager.activeSceneChanged -= OnSceneChanged;
	}

	private void OnSceneChanged(Scene oldScene, Scene newScene)
	{
		CancelAll();
	}

	public void CancelAll()
	{
		StopAllCoroutines();
		activeOps.Clear();
	}

	public void DoTimedOp(string id, Action operation, float duration)
	{
		if (activeOps.TryGetValue(id, out var value))
		{
			value.remaining += duration;
			if (value.remaining > value.maxHad)
			{
				value.maxHad = value.remaining;
			}
		}
		else
		{
			value = new TimedOpState
			{
				remaining = duration,
				maxHad = duration
			};
			activeOps.Add(id, value);
			StartCoroutine(TimedOperationCoroutine(id, operation, value));
		}
	}

	public bool AddDuration(string id, float delta)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return false;
		}
		value.remaining += delta;
		if (value.remaining > value.maxHad)
		{
			value.maxHad = value.remaining;
		}
		return true;
	}

	public bool SetDuration(string id, float duration)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return false;
		}
		value.remaining = duration;
		if (value.remaining > value.maxHad)
		{
			value.maxHad = value.remaining;
		}
		return true;
	}

	public bool ReduceDuration(string id, float amount)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return false;
		}
		value.remaining = Mathf.Max(0f, value.remaining - amount);
		if (value.remaining > value.maxHad)
		{
			value.maxHad = value.remaining;
		}
		return true;
	}

	public bool Stop(string id)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return false;
		}
		value.remaining = 0f;
		return true;
	}

	public float DurationOf(string id)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return 0f;
		}
		return value.remaining;
	}

	public float HighestDurationOf(string id)
	{
		if (!activeOps.TryGetValue(id, out var value))
		{
			return 0f;
		}
		return value.maxHad;
	}

	private IEnumerator TimedOperationCoroutine(string id, Action operation, TimedOpState state)
	{
		while (state.remaining > 0f)
		{
			operation?.Invoke();
			yield return new WaitForSeconds(1f);
			state.remaining--;
		}
		activeOps.Remove(id);
	}
}
