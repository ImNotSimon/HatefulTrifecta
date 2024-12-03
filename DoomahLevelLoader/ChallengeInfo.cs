using System;
using HatefulScripts;
using TMPro;
using UnityEngine;

namespace DoomahLevelLoader
{
	public class ChallengeInfo : MonoBehaviour
	{
		public void Awake()
		{
			this.ChallengeText = Plugin.FindObjectEvenIfDisabled("Player", "Main Camera/HUD Camera/HUD/FinishCanvas/Panel/Challenge/ChallengeText", 0, false);
			this.ChallengeText.GetComponent<TextMeshProUGUI>().text = this.Challenge;
			bool activeByDefault = this.ActiveByDefault;
			if (activeByDefault)
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

		// Token: 0x04000126 RID: 294
		public string Challenge;

		// Token: 0x04000127 RID: 295
		public bool ActiveByDefault;

		// Token: 0x04000128 RID: 296
		[HideInInspector]
		public GameObject ChallengeText;
	}
}
