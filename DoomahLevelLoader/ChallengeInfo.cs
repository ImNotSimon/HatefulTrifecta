using System.Collections;
using HatefulScripts;
using TMPro;
using UnityEngine;

namespace HatefulScripts
{
    public class HatefulChallengeInfo : MonoBehaviour
    {
        public void Awake()
        {
            StartCoroutine(FindAndInitialize());
        }

        private IEnumerator FindAndInitialize()
        {
            while (this.ChallengeText == null)
            {
                this.ChallengeText = Plugin.FindObjectEvenIfDisabled("Player", "Main Camera/HUD Camera/HUD/FinishCanvas/Panel/Challenge/ChallengeText", 0, false);
                if (this.ChallengeText == null)
                {
                    Debug.LogWarning("ChallengeText not found. Retrying...");
                    yield return new WaitForSeconds(0.5f); // Wait before retrying
                }
            }

            var textMeshPro = this.ChallengeText.GetComponent<TextMeshProUGUI>();
            while (textMeshPro == null)
            {
                Debug.LogWarning("TextMeshProUGUI component not found. Retrying...");
                yield return new WaitForSeconds(0.5f);
                textMeshPro = this.ChallengeText.GetComponent<TextMeshProUGUI>();
            }

            textMeshPro.text = this.Challenge;

            while (MonoSingleton<ChallengeManager>.Instance == null)
            {
                Debug.LogWarning("ChallengeManager instance is null. Retrying...");
                yield return new WaitForSeconds(0.5f);
            }

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

            Debug.Log("HatefulChallengeInfo initialized successfully.");
        }

        public string Challenge;

        public bool ActiveByDefault;

        [HideInInspector]
        public GameObject ChallengeText;
    }
}
