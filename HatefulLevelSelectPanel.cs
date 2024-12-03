using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000011 RID: 17
public class HatefulLevelSelectPanel : MonoBehaviour
{
	// Token: 0x0600004A RID: 74 RVA: 0x00003670 File Offset: 0x00001870
	public void CheckScore()
	{
		this.rectTransform = base.GetComponent<RectTransform>();
		bool flag = this.origSprite == null;
		if (flag)
		{
			this.origSprite = base.transform.Find("Image").GetComponent<Image>().sprite;
		}
		bool flag2 = this.levelNumber == 666;
		if (flag2)
		{
			this.tempInt = GameProgressSaver.GetPrime(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0), this.levelNumberInLayer);
		}
		bool flag3 = this.levelNumber >= 500 && this.levelNumber < 666;
		if (flag3)
		{
			this.tempInt = GameProgressSaver.GetPrime(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0), this.levelNumberInLayer);
		}
		else
		{
			this.tempInt = GameProgressSaver.GetProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0));
		}
		int num = this.levelNumber;
		bool flag4 = this.levelNumber == 666;
		if (flag4)
		{
			num += this.levelNumberInLayer - 1;
		}
		bool flag5 = this.levelNumber >= 500 && this.levelNumber < 666;
		if (flag5)
		{
			num += this.levelNumberInLayer - 1;
		}
		this.origName = GetMissionName.GetMission(num);
		bool flag6 = (this.levelNumber == 666 && this.tempInt == 0) || (this.levelNumber != 666 && this.tempInt < this.levelNumber) || this.forceOff;
		if (flag6)
		{
			string text = "10";
			base.transform.Find("Name").GetComponent<TMP_Text>().text = text + "-" + this.levelNumberInLayer.ToString() + ": ???";
			base.transform.Find("Image").GetComponent<Image>().sprite = this.lockedSprite;
			base.GetComponent<Button>().enabled = false;
			this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, this.collapsedHeight);
			this.leaderboardPanel.SetActive(false);
		}
		else
		{
			bool flag7 = this.tempInt == this.levelNumber || (this.levelNumber == 666 && this.tempInt == 1);
			bool flag8;
			if (flag7)
			{
				flag8 = false;
				base.transform.Find("Image").GetComponent<Image>().sprite = this.unlockedSprite;
			}
			else
			{
				flag8 = true;
				base.transform.Find("Image").GetComponent<Image>().sprite = this.origSprite;
			}
			base.transform.Find("Name").GetComponent<TMP_Text>().text = this.origName;
			base.GetComponent<Button>().enabled = true;
			bool flag9 = this.challengeIcon != null;
			if (flag9)
			{
				bool flag10 = this.challengeChecker == null;
				if (flag10)
				{
					this.challengeChecker = this.challengeIcon.transform.Find("EventTrigger").gameObject;
				}
				bool flag11 = this.tempInt > this.levelNumber;
				if (flag11)
				{
					this.challengeChecker.SetActive(true);
				}
			}
			bool flag12 = MonoSingleton<PrefsManager>.Instance.GetBool("levelLeaderboards", false) && flag8;
			if (flag12)
			{
				this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, this.expandedHeight);
				this.leaderboardPanel.SetActive(true);
			}
			else
			{
				this.rectTransform.sizeDelta = new Vector2(this.rectTransform.sizeDelta.x, this.collapsedHeight);
				this.leaderboardPanel.SetActive(false);
			}
		}
		RankData rank = GameProgressSaver.GetRank(num, false);
		bool flag13 = rank != null;
		if (flag13)
		{
			int @int = MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0);
			bool flag14 = rank.levelNumber == this.levelNumber || (this.levelNumber == 666 && rank.levelNumber == this.levelNumber + this.levelNumberInLayer - 1) || (this.levelNumber >= 500 && this.levelNumber < 666 && rank.levelNumber == this.levelNumber + this.levelNumberInLayer - 1);
			if (flag14)
			{
				TMP_Text componentInChildren = base.transform.Find("Stats").Find("Rank").GetComponentInChildren<TMP_Text>();
				bool flag15 = rank.ranks[@int] == 12 && (rank.majorAssists == null || !rank.majorAssists[@int]);
				if (flag15)
				{
					componentInChildren.text = "<color=#FFFFFF>P</color>";
					Image component = componentInChildren.transform.parent.GetComponent<Image>();
					component.color = new Color(1f, 0.686f, 0f, 1f);
					component.fillCenter = true;
				}
				else
				{
					bool flag16 = rank.majorAssists != null && rank.majorAssists[@int];
					if (flag16)
					{
						bool flag17 = rank.ranks[@int] < 0;
						if (flag17)
						{
							componentInChildren.text = "";
						}
						else
						{
							Image component2 = componentInChildren.transform.parent.GetComponent<Image>();
							component2.color = new Color(0.3f, 0.6f, 0.9f, 1f);
							component2.fillCenter = true;
						}
					}
					else
					{
						bool flag18 = rank.ranks[@int] < 0;
						if (flag18)
						{
							componentInChildren.text = "";
							Image component3 = componentInChildren.transform.parent.GetComponent<Image>();
							component3.color = Color.white;
							component3.fillCenter = false;
						}
						else
						{
							Image component4 = componentInChildren.transform.parent.GetComponent<Image>();
							component4.color = Color.white;
							component4.fillCenter = false;
						}
					}
				}
				bool flag19 = rank.secretsAmount > 0;
				if (flag19)
				{
					this.allSecrets = true;
					for (int i = 0; i < 5; i++)
					{
						bool flag20 = i < rank.secretsAmount && rank.secretsFound[i];
						if (flag20)
						{
							this.secretIcons[i].fillCenter = true;
						}
						else
						{
							bool flag21 = i < rank.secretsAmount;
							if (flag21)
							{
								this.allSecrets = false;
								this.secretIcons[i].fillCenter = false;
							}
							else
							{
								bool flag22 = i >= rank.secretsAmount;
								if (flag22)
								{
									this.secretIcons[i].enabled = false;
								}
							}
						}
					}
				}
				else
				{
					Image[] array = this.secretIcons;
					for (int j = 0; j < array.Length; j++)
					{
						array[j].enabled = false;
					}
				}
				bool flag23 = this.challengeIcon;
				if (flag23)
				{
					bool challenge = rank.challenge;
					if (challenge)
					{
						this.challengeIcon.fillCenter = true;
						TMP_Text componentInChildren2 = this.challengeIcon.GetComponentInChildren<TMP_Text>();
						componentInChildren2.text = "C O M P L E T E";
						bool flag24 = rank.ranks[@int] == 12 && (this.allSecrets || rank.secretsAmount == 0);
						if (flag24)
						{
							componentInChildren2.color = new Color(0.6f, 0.4f, 0f, 1f);
						}
						else
						{
							componentInChildren2.color = Color.black;
						}
					}
					else
					{
						this.challengeIcon.fillCenter = false;
						TMP_Text componentInChildren3 = this.challengeIcon.GetComponentInChildren<TMP_Text>();
						componentInChildren3.text = "C H A L L E N G E";
						componentInChildren3.color = Color.white;
					}
				}
			}
			else
			{
				Debug.Log("Error in finding " + this.levelNumber.ToString() + " Data");
				Image component5 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
				component5.color = Color.white;
				component5.fillCenter = false;
				component5.GetComponentInChildren<TMP_Text>().text = "";
				this.allSecrets = false;
				Image[] array2 = this.secretIcons;
				foreach (Image image in array2)
				{
					image.enabled = true;
					image.fillCenter = false;
				}
			}
			bool flag25 = (rank.challenge || !this.challengeIcon) && rank.ranks[@int] == 12 && (this.allSecrets || rank.secretsAmount == 0);
			if (flag25)
			{
				base.GetComponent<Image>().color = new Color(1f, 0.686f, 0f, 0.75f);
			}
			else
			{
				base.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0.75f);
			}
		}
		else
		{
			Debug.Log("Didn't Find Level " + this.levelNumber.ToString() + " Data");
			Image component6 = base.transform.Find("Stats").Find("Rank").GetComponent<Image>();
			component6.color = Color.white;
			component6.fillCenter = false;
			component6.GetComponentInChildren<TMP_Text>().text = "";
			this.allSecrets = false;
			Image[] array4 = this.secretIcons;
			foreach (Image image2 in array4)
			{
				image2.enabled = true;
				image2.fillCenter = false;
			}
		}
	}

	// Token: 0x04000040 RID: 64
	public float collapsedHeight = 260f;

	// Token: 0x04000041 RID: 65
	public float expandedHeight = 400f;

	// Token: 0x04000042 RID: 66
	public GameObject leaderboardPanel;

	// Token: 0x04000043 RID: 67
	private RectTransform rectTransform;

	// Token: 0x04000044 RID: 68
	public int levelNumber;

	// Token: 0x04000045 RID: 69
	public int levelNumberInLayer;

	// Token: 0x04000046 RID: 70
	private bool allSecrets;

	// Token: 0x04000047 RID: 71
	public Sprite lockedSprite;

	// Token: 0x04000048 RID: 72
	public Sprite unlockedSprite;

	// Token: 0x04000049 RID: 73
	private Sprite origSprite;

	// Token: 0x0400004A RID: 74
	public Image[] secretIcons;

	// Token: 0x0400004B RID: 75
	public Image challengeIcon;

	// Token: 0x0400004C RID: 76
	private int tempInt;

	// Token: 0x0400004D RID: 77
	private string origName;

	// Token: 0x0400004E RID: 78
	private GameObject challengeChecker;

	// Token: 0x0400004F RID: 79
	public bool forceOff;
}
