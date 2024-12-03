using System;
using UnityEngine;

namespace DoomahLevelLoader
{
	// Token: 0x02000028 RID: 40
	public class SetChallengeStatus : MonoBehaviour
	{
		// Token: 0x060000F0 RID: 240 RVA: 0x00009E58 File Offset: 0x00008058
		public void Awake()
		{
			bool active = this.Active;
			if (active)
			{
				MonoSingleton<ChallengeManager>.Instance.challengeFailed = false;
				MonoSingleton<ChallengeManager>.Instance.challengeDone = true;
			}
			else
			{
				MonoSingleton<ChallengeManager>.Instance.challengeFailed = true;
				MonoSingleton<ChallengeManager>.Instance.challengeDone = false;
			}
		}

		// Token: 0x04000129 RID: 297
		public bool Active;
	}
}
