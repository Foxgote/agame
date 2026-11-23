using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LewdStatus
{
	// Token: 0x020000A5 RID: 165
	internal static class Keys
	{
		// Token: 0x17000072 RID: 114
		// (get) Token: 0x06000351 RID: 849 RVA: 0x00036EB8 File Offset: 0x000350B8
		public static bool Typing
		{
			get
			{
				if (ConsoleScript.instance && ConsoleScript.instance.active)
				{
					return true;
				}
				EventSystem current = EventSystem.current;
				GameObject gameObject = (current ? current.currentSelectedGameObject : null);
				if (!gameObject)
				{
					return false;
				}
				TMP_InputField component = gameObject.GetComponent<TMP_InputField>();
				if (component && component.isFocused)
				{
					return true;
				}
				InputField component2 = gameObject.GetComponent<InputField>();
				return component2 && component2.isFocused;
			}
		}

		// Token: 0x06000352 RID: 850 RVA: 0x00036F31 File Offset: 0x00035131
		public static bool Down(KeyCode k)
		{
			return k != null && Input.GetKeyDown(k) && !Keys.Typing;
		}

		// Token: 0x06000353 RID: 851 RVA: 0x00036F48 File Offset: 0x00035148
		public static bool Held(KeyCode k)
		{
			return k != null && Input.GetKey(k) && !Keys.Typing;
		}
	}
}
