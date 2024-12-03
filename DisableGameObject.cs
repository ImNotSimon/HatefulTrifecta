using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class DisableGameObject : MonoBehaviour
{
	// Token: 0x0600000C RID: 12 RVA: 0x000022C0 File Offset: 0x000004C0
	public void DISABLEIT()
	{
		string text = "Outdoors/OOB Minefield/Plane(Clone)";
		GameObject gameObject = GameObject.Find(text);
		bool flag = gameObject != null;
		if (flag)
		{
			gameObject.gameObject.SetActive(false);
			Debug.Log("GameObject found and disabled: " + gameObject.name);
		}
		else
		{
			Debug.LogWarning("GameObject not found at path: " + text);
		}
	}
}
