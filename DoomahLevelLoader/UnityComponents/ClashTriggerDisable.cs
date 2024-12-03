using System;
using UnityEngine;

namespace DoomahLevelLoader.UnityComponents
{
	// Token: 0x0200002B RID: 43
	public class ClashTriggerDisable : MonoBehaviour
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x0000A2BC File Offset: 0x000084BC
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.tag == "Player";
			bool flag2 = flag;
			if (flag2)
			{
				MonoSingleton<PlayerTracker>.Instance.ChangeToFPS();
			}
		}
	}
}
