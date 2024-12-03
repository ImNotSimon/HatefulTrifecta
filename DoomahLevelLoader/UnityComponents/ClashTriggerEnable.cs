using System;
using UnityEngine;

namespace DoomahLevelLoader.UnityComponents
{
	// Token: 0x0200002C RID: 44
	public class ClashTriggerEnable : MonoBehaviour
	{
		// Token: 0x060000FA RID: 250 RVA: 0x0000A2FC File Offset: 0x000084FC
		private void OnTriggerEnter(Collider other)
		{
			bool flag = other.gameObject.tag == "Player" && !this.hasenabled;
			bool flag2 = flag;
			if (flag2)
			{
				MonoSingleton<PlayerTracker>.Instance.ChangeToPlatformer();
				bool onlyOnce = this.OnlyOnce;
				bool flag3 = onlyOnce;
				if (flag3)
				{
					this.hasenabled = true;
				}
			}
		}

		// Token: 0x04000139 RID: 313
		private bool hasenabled = false;

		// Token: 0x0400013A RID: 314
		public bool OnlyOnce = false;
	}
}
