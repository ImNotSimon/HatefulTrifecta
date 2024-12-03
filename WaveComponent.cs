using System;
using System.Collections;
using System.Collections.Generic;
using DoomahLevelLoader.UnityComponents;
using UnityEngine;

// Token: 0x0200000A RID: 10
public class WaveComponent : MonoBehaviour
{
	// Token: 0x0600001F RID: 31 RVA: 0x00002DA1 File Offset: 0x00000FA1
	private void Start()
	{
		this.DisableAllChildren();
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002DAC File Offset: 0x00000FAC
	private void DisableAllChildren()
	{
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			transform.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000021 RID: 33 RVA: 0x00002E10 File Offset: 0x00001010
	private void Update()
	{
		this.timer += Time.deltaTime;
		bool flag = this.timer >= this.checkInterval;
		if (flag)
		{
			this.timer = 0f;
			this.CheckChildren();
		}
	}

	// Token: 0x06000022 RID: 34 RVA: 0x00002E59 File Offset: 0x00001059
	private IEnumerator ActivateChildrenWithDelay(Transform[] childrenToActivate)
	{
		foreach (Transform child in childrenToActivate)
		{
			bool flag = child.name != "NoPass(Clone)" || child.GetComponent<AddressableReplacer>() == null;
			if (flag)
			{
				child.gameObject.SetActive(true);
				yield return new WaitForSeconds(this.activationDelay);
			}
			else
			{
				child.gameObject.SetActive(true);
			}
		}
		Transform[] array = null;
		yield break;
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00002E70 File Offset: 0x00001070
	private void CheckChildren()
	{
		GameObject gameObject = base.transform.parent.gameObject;
		List<GameObject> list = new List<GameObject>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			list.Add(transform.gameObject);
		}
		List<Transform> list2 = new List<Transform>();
		list2.AddRange(gameObject.GetComponentsInChildren<Transform>());
		list2.Remove(base.transform);
		List<Transform> list3 = new List<Transform>();
		foreach (Transform transform2 in list2)
		{
			EnemyIdentifier component = transform2.GetComponent<EnemyIdentifier>();
			bool flag = component != null && !component.dead;
			if (flag)
			{
				list3.Add(transform2);
			}
			else
			{
				bool flag2 = component != null && component.dead && list3.Contains(transform2);
				if (flag2)
				{
					list3.Remove(transform2);
				}
			}
		}
		Transform[] array = new Transform[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			array[i] = list[i].transform;
		}
		bool flag3 = list3.Count == 0 && !this.hasActivated;
		if (flag3)
		{
			this.hasActivated = true;
			base.StartCoroutine(this.ActivateChildrenWithDelay(array));
		}
	}

	// Token: 0x04000024 RID: 36
	public float checkInterval = 1f;

	// Token: 0x04000025 RID: 37
	private float timer = 0f;

	// Token: 0x04000026 RID: 38
	private float activationDelay = 0.1f;

	// Token: 0x04000027 RID: 39
	private bool hasActivated = false;

	// Token: 0x04000028 RID: 40
	private List<Transform> activatedChildren = new List<Transform>();

	// Token: 0x04000029 RID: 41
	private List<Transform> ignoreList = new List<Transform>();
}
