using System;
using UnityEngine;

// Token: 0x0200000E RID: 14
public class EnableOnTrigger : MonoBehaviour
{
	// Token: 0x0600003B RID: 59 RVA: 0x000033EF File Offset: 0x000015EF
	private void Start()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00003400 File Offset: 0x00001600
	private void Update()
	{
		bool flag = this.trigger != null && this.trigger.gameObject.activeSelf;
		if (flag)
		{
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x04000037 RID: 55
	public Transform trigger;
}
