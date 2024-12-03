using System;
using System.Collections;
using System.Collections.Generic;
using DoomahLevelLoader.UnityComponents;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class TriggerZoneBehavior : MonoBehaviour
{
	// Token: 0x0600001A RID: 26 RVA: 0x00002C84 File Offset: 0x00000E84
	private void Start()
	{
		Collider component = base.GetComponent<Collider>();
		bool flag = component != null;
		if (flag)
		{
			component.isTrigger = true;
		}
		this.DelayedInitialization();
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002CB8 File Offset: 0x00000EB8
	private void DelayedInitialization()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00002D1C File Offset: 0x00000F1C
	private IEnumerator ActivateChildrenWithDelay()
	{
		bool flag4;
		do
		{
			bool allChildrenActivated = true;
			foreach (object obj in base.transform)
			{
				Transform child = (Transform)obj;
				bool flag = child == null;
				if (!flag)
				{
					bool flag2 = !this.ignoreList.Contains(child) && !child.name.Contains("Gore Zone") && !child.gameObject.activeSelf;
					if (flag2)
					{
						allChildrenActivated = false;
						bool flag3 = child.name != null && (child.name != "NoPass(Clone)" || child.GetComponent<AddressableReplacer>() == null);
						if (flag3)
						{
							yield return new WaitForSeconds(this.activationDelay);
						}
						child.gameObject.SetActive(true);
						Debug.Log("Trigger Activated child:  " + child.name);
					}
					else
					{
						this.ignoreList.Add(child);
					}
					child = null;
				}
			}
			IEnumerator enumerator = null;
			flag4 = allChildrenActivated;
		}
		while (!flag4);
		this.hasActivated = true;
		yield break;
		yield break;
	}

	// Token: 0x0600001D RID: 29 RVA: 0x00002D2C File Offset: 0x00000F2C
	private void OnTriggerEnter(Collider other)
	{
		bool flag = other.gameObject.name == "Player" && !this.hasActivated;
		if (flag)
		{
			base.StartCoroutine(this.ActivateChildrenWithDelay());
		}
	}

	// Token: 0x04000020 RID: 32
	public float delay = 2f;

	// Token: 0x04000021 RID: 33
	public float activationDelay = 0.1f;

	// Token: 0x04000022 RID: 34
	private bool hasActivated = false;

	// Token: 0x04000023 RID: 35
	private List<Transform> ignoreList = new List<Transform>();
}
