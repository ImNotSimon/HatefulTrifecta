using System;
using UnityEngine;

namespace HatefulScripts
{
	public class HatefulSetChallengeStatus : MonoBehaviour //script name change to avoid any potential conflicts with E&S

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
		public bool Active;
	}
}
