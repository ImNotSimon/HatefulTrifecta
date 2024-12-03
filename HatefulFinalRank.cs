using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using HatefulScripts;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000015 RID: 21
public class HatefulFinalRank : MonoBehaviour
{
	// Token: 0x06000065 RID: 101 RVA: 0x00004A27 File Offset: 0x00002C27
	private void Start()
	{
		Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
	}

    public void LoadLevel(string bundlename, string lvlname)
    {
        // Create a new GameObject and add HABundle as a component
        GameObject habundleObject = new GameObject("HABundleObject");
        HABundle habundle = habundleObject.AddComponent<HABundle>();

        // Log what you're attempting to load for debugging purposes
        UnityEngine.Debug.Log($"Loading via HABundle: {bundlename}, Scene: {lvlname}");

        // Start the coroutine on this new HABundle instance
        StartCoroutine(habundle.LoadAssetBundleAndScene(bundlename, lvlname));
    }

    // Token: 0x06000066 RID: 102 RVA: 0x00004A40 File Offset: 0x00002C40
    private void Update()
	{
		bool flag = this.countTime;
		if (flag)
		{
			bool flag2 = this.time == null;
			if (flag2)
			{
				Debug.LogError("Time component is null!");
				return;
			}
			bool flag3 = this.savedTime >= this.checkedSeconds;
			if (flag3)
			{
				bool flag4 = this.savedTime > this.checkedSeconds;
				if (flag4)
				{
					float num = this.savedTime - this.checkedSeconds;
					this.checkedSeconds += Time.deltaTime * 20f + Time.deltaTime * num * 1.5f;
					this.seconds += Time.deltaTime * 20f + Time.deltaTime * num * 1.5f;
				}
				bool flag5 = this.checkedSeconds >= this.savedTime || this.skipping;
				if (flag5)
				{
					this.checkedSeconds = this.savedTime;
					this.seconds = this.savedTime;
					this.minutes = 0;
					while (this.seconds >= 60f)
					{
						this.seconds -= 60f;
						this.minutes++;
					}
					this.countTime = false;
					bool flag6 = this.time.GetComponent<AudioSource>() != null;
					if (flag6)
					{
						this.time.GetComponent<AudioSource>().Stop();
					}
					else
					{
						Debug.LogError("AudioSource on time is null!");
					}
					base.Invoke("Appear", this.timeBetween * 2f);
				}
				bool flag7 = this.seconds >= 60f;
				if (flag7)
				{
					this.seconds -= 60f;
					this.minutes++;
				}
				this.time.text = this.minutes.ToString() + ":" + this.seconds.ToString("00.000");
			}
		}
		else
		{
			bool flag8 = this.countKills;
			if (flag8)
			{
				bool flag9 = this.kills == null;
				if (flag9)
				{
					Debug.LogError("Kills component is null!");
					return;
				}
				bool flag10 = (float)this.savedKills >= this.checkedKills;
				if (flag10)
				{
					bool flag11 = (float)this.savedKills > this.checkedKills;
					if (flag11)
					{
						this.checkedKills += Time.deltaTime * 45f;
					}
					bool flag12 = this.checkedKills >= (float)this.savedKills || this.skipping;
					if (flag12)
					{
						this.checkedKills = (float)this.savedKills;
						this.countKills = false;
						bool flag13 = this.kills.GetComponent<AudioSource>() != null;
						if (flag13)
						{
							this.kills.GetComponent<AudioSource>().Stop();
						}
						else
						{
							Debug.LogError("AudioSource on kills is null!");
						}
						base.Invoke("Appear", this.timeBetween * 2f);
					}
					this.kills.text = this.checkedKills.ToString("0");
				}
			}
			else
			{
				bool flag14 = this.countStyle && (float)this.savedStyle >= this.checkedStyle;
				if (flag14)
				{
					bool flag15 = this.style == null;
					if (flag15)
					{
						Debug.LogError("Style component is null!");
						return;
					}
					float num2 = this.checkedStyle;
					bool flag16 = (float)this.savedStyle > this.checkedStyle;
					if (flag16)
					{
						this.checkedStyle += Time.deltaTime * 4500f;
					}
					bool flag17 = this.checkedStyle >= (float)this.savedStyle || this.skipping;
					if (flag17)
					{
						this.checkedStyle = (float)this.savedStyle;
						this.countStyle = false;
						bool flag18 = this.style.GetComponent<AudioSource>() != null;
						if (flag18)
						{
							this.style.GetComponent<AudioSource>().Stop();
						}
						else
						{
							Debug.LogError("AudioSource on style is null!");
						}
						base.Invoke("Appear", this.timeBetween * 2f);
						this.totalPoints += this.savedStyle;
						this.PointsShow();
					}
					else
					{
						int i = this.totalPoints + Mathf.RoundToInt(this.checkedStyle);
						int num3 = 0;
						while (i >= 1000)
						{
							num3++;
							i -= 1000;
						}
						bool flag19 = num3 > 0;
						if (flag19)
						{
							bool flag20 = this.pointsText == null;
							if (flag20)
							{
								Debug.LogError("pointsText component is null!");
								return;
							}
							bool flag21 = i < 10;
							if (flag21)
							{
								this.pointsText.text = string.Concat(new string[]
								{
									"+",
									num3.ToString(),
									",00",
									i.ToString(),
									"<color=orange>P</color>"
								});
							}
							else
							{
								bool flag22 = i < 100;
								if (flag22)
								{
									this.pointsText.text = string.Concat(new string[]
									{
										"+",
										num3.ToString(),
										",0",
										i.ToString(),
										"<color=orange>P</color>"
									});
								}
								else
								{
									this.pointsText.text = string.Concat(new string[]
									{
										"+",
										num3.ToString(),
										",",
										i.ToString(),
										"<color=orange>P</color>"
									});
								}
							}
						}
						else
						{
							this.pointsText.text = "+" + i.ToString() + "<color=orange>P</color>";
						}
					}
                    LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
				}
			}
		}
		bool flag23 = this.flashFade;
		if (flag23)
		{
			bool flag24 = this.flashPanel == null;
			if (flag24)
			{
				Debug.LogError("flashPanel component is null!");
				return;
			}
			this.flashColor.a = this.flashColor.a - Time.deltaTime * this.flashMultiplier;
			this.flashPanel.color = this.flashColor;
			bool flag25 = this.flashColor.a <= 0f;
			if (flag25)
			{
				this.flashFade = false;
			}
		}
		InputManager instance = MonoSingleton<InputManager>.Instance;
		bool flag26 = instance == null;
		if (flag26)
		{
			Debug.LogError("InputManager instance is null!");
		}
		else
		{
			bool flag27 = !instance.PerformingCheatMenuCombo() && instance.InputSource.Fire1.WasPerformedThisFrame && this.complete && Time.timeScale != 0f && this.reachedSecondPit;
			if (flag27)
			{
				this.LevelChange(false);
			}
			else
			{
				bool flag28 = !instance.PerformingCheatMenuCombo() && instance.InputSource.Fire1.WasPerformedThisFrame && !this.complete && Time.timeScale != 0f;
				if (flag28)
				{
					this.skipping = true;
					this.timeBetween = 0.01f;
				}
				else
				{
					bool flag29 = this.rankless && this.asyncLoad != null && this.asyncLoad.progress >= 0.9f;
					if (flag29)
					{
						this.LevelChange(false);
					}
				}
			}
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x000051E0 File Offset: 0x000033E0
	public void SetTime(float seconds, string rank)
	{
		this.savedTime = seconds;
		this.timeRank.text = rank;
		SceneManager.GetSceneByName(this.targetLevelName);
	}

	// Token: 0x06000068 RID: 104 RVA: 0x00005203 File Offset: 0x00003403
	public void SetKills(int killAmount, string rank)
	{
		this.savedKills = killAmount;
		this.killsRank.text = rank;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0000521A File Offset: 0x0000341A
	public void SetStyle(int styleAmount, string rank)
	{
		this.savedStyle = styleAmount;
		this.styleRank.text = rank;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x00005234 File Offset: 0x00003434
	public void SetInfo(int restarts, bool damage, bool majorUsed, bool cheatsUsed)
	{
		this.extraInfo.text = "";
		int num = 1;
		bool flag = !damage;
		if (flag)
		{
			num++;
		}
		if (majorUsed)
		{
			num++;
		}
		if (cheatsUsed)
		{
			num++;
		}
		if (cheatsUsed)
		{
			TMP_Text tmp_Text = this.extraInfo;
			tmp_Text.text += "- <color=#44FF45>CHEATS USED</color>\n";
		}
		if (majorUsed)
		{
			bool flag2 = !MonoSingleton<AssistController>.Instance.hidePopup;
			if (flag2)
			{
				TMP_Text tmp_Text2 = this.extraInfo;
				tmp_Text2.text += "- <color=#4C99E6>MAJOR ASSISTS USED</color>\n";
			}
			this.majorAssists = true;
		}
		bool flag3 = restarts == 0;
		if (flag3)
		{
			bool flag4 = num >= 3;
			if (flag4)
			{
				TMP_Text tmp_Text3 = this.extraInfo;
				tmp_Text3.text += "+ NO RESTARTS\n";
			}
			else
			{
				TMP_Text tmp_Text4 = this.extraInfo;
				tmp_Text4.text += "+ NO RESTARTS\n  (+500<color=orange>P</color>)\n";
			}
			this.noRestarts = true;
		}
		else
		{
			TMP_Text tmp_Text5 = this.extraInfo;
			tmp_Text5.text = tmp_Text5.text + "- <color=red>" + restarts.ToString() + "</color> RESTARTS\n";
		}
		bool flag5 = !damage;
		if (flag5)
		{
			bool flag6 = num >= 3;
			if (flag6)
			{
				TMP_Text tmp_Text6 = this.extraInfo;
				tmp_Text6.text += "+ <color=orange>NO DAMAGE</color>\n";
			}
			else
			{
				TMP_Text tmp_Text7 = this.extraInfo;
				tmp_Text7.text += "+ <color=orange>NO DAMAGE\n  (</color>+5,000<color=orange>P)</color>\n";
			}
			this.noDamage = true;
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x000053CE File Offset: 0x000035CE
	public void SetRank(string rank)
	{
		this.totalRank.text = rank;
	}

	// Token: 0x0600006C RID: 108 RVA: 0x000053E0 File Offset: 0x000035E0
	public void SetSecrets(int secretsAmount, int maxSecrets)
	{
		this.secrets.text = 0.ToString() + " / " + maxSecrets.ToString();
		this.allSecrets = maxSecrets;
		this.secretsFound = secretsAmount;
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00005424 File Offset: 0x00003624
	public void Appear()
	{
		bool flag = this.i < this.toAppear.Length;
		if (flag)
		{
			bool flag2 = !this.casual;
			if (flag2)
			{
				bool flag3 = this.skipping;
				if (flag3)
				{
					HudOpenEffect component = this.toAppear[this.i].GetComponent<HudOpenEffect>();
					bool flag4 = component != null;
					if (flag4)
					{
						component.skip = true;
					}
				}
				bool flag5 = this.toAppear[this.i] == this.time.gameObject;
				if (flag5)
				{
					bool flag6 = this.skipping;
					if (flag6)
					{
						this.checkedSeconds = this.savedTime;
						this.seconds = this.savedTime;
						this.minutes = 0;
						while (this.seconds >= 60f)
						{
							this.seconds -= 60f;
							this.minutes++;
						}
						this.time.GetComponent<AudioSource>().playOnAwake = false;
						base.Invoke("Appear", this.timeBetween * 2f);
						this.time.text = this.minutes.ToString() + ":" + this.seconds.ToString("00.000");
					}
					else
					{
						this.countTime = true;
					}
				}
				else
				{
					bool flag7 = this.toAppear[this.i] == this.kills.gameObject;
					if (flag7)
					{
						bool flag8 = this.skipping;
						if (flag8)
						{
							this.checkedKills = (float)this.savedKills;
							this.kills.GetComponent<AudioSource>().playOnAwake = false;
							base.Invoke("Appear", this.timeBetween * 2f);
							this.kills.text = this.checkedKills.ToString("0");
						}
						else
						{
							this.countKills = true;
						}
					}
					else
					{
						bool flag9 = this.toAppear[this.i] == this.style.gameObject;
						if (flag9)
						{
							bool flag10 = this.skipping;
							if (flag10)
							{
								this.checkedStyle = (float)this.savedStyle;
								this.style.text = this.checkedStyle.ToString("0");
								this.style.GetComponent<AudioSource>().playOnAwake = false;
								base.Invoke("Appear", this.timeBetween * 2f);
								this.totalPoints += this.savedStyle;
								this.PointsShow();
							}
							else
							{
								this.countStyle = true;
							}
						}
						else
						{
							bool flag11 = this.toAppear[this.i] == this.secrets.gameObject;
							if (flag11)
							{
								bool flag12 = this.prevSecrets.Count > 0;
								if (flag12)
								{
									foreach (int num in this.prevSecrets)
									{
										this.secretsInfo[num].color = new Color(0.5f, 0.5f, 0.5f);
										this.checkedSecrets++;
										this.secrets.text = this.checkedSecrets.ToString() + " / " + this.levelSecrets.Length.ToString();
									}
								}
								this.toAppear[this.i].gameObject.SetActive(true);
								base.Invoke("CountSecrets", this.timeBetween);
							}
							else
							{
								bool flag13 = this.toAppear[this.i] == this.timeRank.gameObject || this.toAppear[this.i] == this.killsRank.gameObject || this.toAppear[this.i] == this.styleRank.gameObject;
								if (flag13)
								{
									string text = this.toAppear[this.i].GetComponent<TMP_Text>().text;
									string text2 = text;
									if (!(text2 == "<color=#0094FF>D</color>"))
									{
										if (!(text2 == "<color=#4CFF00>C</color>"))
										{
											if (!(text2 == "<color=#FFD800>B</color>"))
											{
												if (!(text2 == "<color=#FF6A00>A</color>"))
												{
													if (text2 == "<color=#FF0000>S</color>")
													{
														this.AddPoints(2500);
													}
												}
												else
												{
													this.AddPoints(2000);
												}
											}
											else
											{
												this.AddPoints(1500);
											}
										}
										else
										{
											this.AddPoints(1000);
										}
									}
									else
									{
										this.AddPoints(500);
									}
                                    LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
                                    this.FlashPanel(this.toAppear[this.i].transform.parent.GetChild(1).gameObject);
									base.Invoke("Appear", this.timeBetween * 2f);
									bool flag14 = this.skipping;
									if (flag14)
									{
										this.toAppear[this.i].GetComponentInChildren<AudioSource>().playOnAwake = false;
									}
								}
								else
								{
									bool flag15 = this.toAppear[this.i] == this.totalRank.gameObject;
									if (flag15)
									{
										this.FlashPanel(this.toAppear[this.i].transform.parent.GetChild(1).gameObject);
										this.flashMultiplier = 0.5f;
										base.Invoke("Appear", this.timeBetween * 4f);
									}
									else
									{
										bool flag16 = this.toAppear[this.i] == this.extraInfo.gameObject;
										if (flag16)
										{
											bool flag17 = this.noRestarts;
											if (flag17)
											{
												this.AddPoints(500);
											}
											bool flag18 = this.noDamage;
											if (flag18)
											{
												this.AddPoints(5000);
											}
											base.Invoke("Appear", this.timeBetween);
										}
										else
										{
											base.Invoke("Appear", this.timeBetween);
										}
									}
								}
							}
						}
					}
				}
			}
			else
			{
				base.Invoke("Appear", this.timeBetween);
			}
			this.toAppear[this.i].gameObject.SetActive(true);
			this.i++;
			bool flag19 = !this.toAppear[0].gameObject.activeSelf;
			if (flag19)
			{
				this.toAppear[0].gameObject.SetActive(true);
			}
			bool flag20 = this.i >= this.toAppear.Length && !this.complete;
			if (flag20)
			{
				this.complete = true;
				GameProgressSaver.AddMoney(this.totalPoints);
                LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
            }
		}
		else
		{
			bool flag21 = !this.complete;
			if (flag21)
			{
				this.complete = true;
				GameProgressSaver.AddMoney(this.totalPoints);
			}
		}
        LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
    }

	// Token: 0x0600006E RID: 110 RVA: 0x00005BB8 File Offset: 0x00003DB8
	public void FlashPanel(GameObject panel)
	{
		bool flag = this.flashFade;
		if (flag)
		{
			this.flashColor.a = 0f;
			this.flashPanel.color = this.flashColor;
		}
		this.flashPanel = panel.GetComponent<Image>();
		this.flashColor = this.flashPanel.color;
		this.flashColor.a = 1f;
		this.flashPanel.color = this.flashColor;
		this.flashFade = true;
        LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
    }

	// Token: 0x0600006F RID: 111 RVA: 0x00005C64 File Offset: 0x00003E64
	private void CountSecrets()
	{
		bool flag = this.levelSecrets.Length != 0;
		if (flag)
		{
			bool flag2 = this.levelSecrets[this.secretsCheckProgress] == null && !this.prevSecrets.Contains(this.secretsCheckProgress);
			if (flag2)
			{
				this.checkedSecrets++;
				this.secrets.text = this.checkedSecrets.ToString() + " / " + this.levelSecrets.Length.ToString();
				this.secrets.GetComponent<AudioSource>().Play();
				this.secretsInfo[this.secretsCheckProgress].color = Color.white;
				this.secretsCheckProgress++;
				this.AddPoints(1000);
				bool flag3 = this.secretsCheckProgress < this.levelSecrets.Length;
				if (flag3)
				{
					base.Invoke("CountSecrets", this.timeBetween);
				}
				else
				{
					base.Invoke("Appear", this.timeBetween);
				}
			}
			else
			{
				bool flag4 = this.secretsCheckProgress < this.levelSecrets.Length - 1;
				if (flag4)
				{
					this.secretsCheckProgress++;
					this.CountSecrets();
				}
				else
				{
					this.secretsCheckProgress++;
					base.Invoke("Appear", this.timeBetween);
				}
			}
		}
		else
		{
			base.Invoke("Appear", this.timeBetween);
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x00005DE8 File Offset: 0x00003FE8
	public void NotRanklessNextLevel(string bundlename, string lvlname)
	{
		bool flag = lvlname != "";
		if (flag)
		{
            LoadLevel(bundlename, lvlname);
        }
	}

	// Token: 0x06000071 RID: 113 RVA: 0x00005E44 File Offset: 0x00004044
	public void LevelChange(bool force = false)
	{
		bool isPlayingCustom = SceneHelper.IsPlayingCustom;
		if (isPlayingCustom)
		{
			if (force)
			{
				MonoSingleton<OptionsManager>.Instance.QuitMission();
			}
			else
			{
				bool flag = MonoSingleton<AdditionalMapDetails>.Instance && MonoSingleton<AdditionalMapDetails>.Instance.hasAuthorLinks;
				if (flag)
				{
					base.gameObject.SetActive(false);
					MonoSingleton<WorkshopMapEndLinks>.Instance.Show();
				}
				else
				{
					bool flag2 = GameStateManager.Instance.currentCustomGame != null && GameStateManager.Instance.currentCustomGame.workshopId != null;
					if (flag2)
					{
						MonoSingleton<WorkshopMapEndRating>.Instance.enabled = true;
						base.gameObject.SetActive(false);
					}
					else
					{
						MonoSingleton<OptionsManager>.Instance.QuitMission();
					}
				}
			}
			bool isCustomLevel = Plugin.IsCustomLevel;
			if (isCustomLevel)
			{
                LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
            }
            LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
        }
		bool flag3 = this.playerPosInfo != null;
		if (flag3)
		{
			bool flag4 = this.ppiObject == null;
			if (flag4)
			{
				this.ppiObject = UnityEngine.Object.Instantiate<GameObject>(this.playerPosInfo);
			}
			PlayerPosInfo component = this.ppiObject.GetComponent<PlayerPosInfo>();
			Rigidbody component2 = MonoSingleton<NewMovement>.Instance.gameObject.GetComponent<Rigidbody>();
			component.velocity = component2.velocity;
			component.position = component2.transform.position - this.finalPitPos;
			component.position = new Vector3(component.position.x, component.position.y, component.position.z - 990f);
			component.wooshTime = component2.GetComponentInChildren<WallCheck>().GetComponent<AudioSource>().time;
			bool flag5 = this.dontSavePos || this.targetLevelName == "Main Menu";
			if (flag5)
			{
				component.noPosition = true;
			}
		}
		base.gameObject.SetActive(false);
        LoadLevel(this.gbundlename, this.glvlname);
    }

	// Token: 0x06000072 RID: 114 RVA: 0x0000609A File Offset: 0x0000429A
	public void AddPoints(int points)
	{
		this.totalPoints += points;
		this.PointsShow();
	}

	// Token: 0x06000073 RID: 115 RVA: 0x000060B4 File Offset: 0x000042B4
	private void PointsShow()
	{
        LoadLevel(HatefulAssetBundleSceneLoader.publicModPath, HatefulAssetBundleSceneLoader.publicSceneName);
        int i = this.totalPoints;
		int num = 0;
		while (i >= 1000)
		{
			num++;
			i -= 1000;
		}
		bool flag = num > 0;
		if (flag)
		{
			bool flag2 = i < 10;
			if (flag2)
			{
				this.pointsText.text = string.Concat(new string[]
				{
					"+",
					num.ToString(),
					",00",
					i.ToString(),
					"<color=orange>P</color>"
				});
			}
			else
			{
				bool flag3 = i < 100;
				if (flag3)
				{
					this.pointsText.text = string.Concat(new string[]
					{
						"+",
						num.ToString(),
						",0",
						i.ToString(),
						"<color=orange>P</color>"
					});
				}
				else
				{
					this.pointsText.text = string.Concat(new string[]
					{
						"+",
						num.ToString(),
						",",
						i.ToString(),
						"<color=orange>P</color>"
					});
				}
			}
		}
		else
		{
			this.pointsText.text = "+" + i.ToString() + "<color=orange>P</color>";
		}
	}

	// Token: 0x0400006E RID: 110
	public bool casual;

	// Token: 0x0400006F RID: 111
	public bool dontSavePos;

	// Token: 0x04000070 RID: 112
	public bool reachedSecondPit;

	// Token: 0x04000071 RID: 113
	public TMP_Text time;

	// Token: 0x04000072 RID: 114
	private float savedTime;

	// Token: 0x04000073 RID: 115
	public TMP_Text timeRank;

	// Token: 0x04000074 RID: 116
	private bool countTime;

	// Token: 0x04000075 RID: 117
	private int minutes;

	// Token: 0x04000076 RID: 118
	private float seconds;

	// Token: 0x04000077 RID: 119
	private float checkedSeconds;

	// Token: 0x04000078 RID: 120
	public TMP_Text kills;

	// Token: 0x04000079 RID: 121
	private int savedKills;

	// Token: 0x0400007A RID: 122
	public TMP_Text killsRank;

	// Token: 0x0400007B RID: 123
	private bool countKills;

	// Token: 0x0400007C RID: 124
	private float checkedKills;

	// Token: 0x0400007D RID: 125
	public TMP_Text style;

	// Token: 0x0400007E RID: 126
	private int savedStyle;

	// Token: 0x0400007F RID: 127
	public TMP_Text styleRank;

	// Token: 0x04000080 RID: 128
	private bool countStyle;

	// Token: 0x04000081 RID: 129
	private float checkedStyle;

	// Token: 0x04000082 RID: 130
	public TMP_Text extraInfo;

	// Token: 0x04000083 RID: 131
	public TMP_Text totalRank;

	// Token: 0x04000084 RID: 132
	public TMP_Text secrets;

	// Token: 0x04000085 RID: 133
	public Image[] secretsInfo;

	// Token: 0x04000086 RID: 134
	private int secretsFound;

	// Token: 0x04000087 RID: 135
	public GameObject[] levelSecrets;

	// Token: 0x04000088 RID: 136
	private int checkedSecrets;

	// Token: 0x04000089 RID: 137
	private int secretsCheckProgress;

	// Token: 0x0400008A RID: 138
	private int allSecrets;

	// Token: 0x0400008B RID: 139
	public List<int> prevSecrets;

	// Token: 0x0400008C RID: 140
	public Image[] challenges;

	// Token: 0x0400008D RID: 141
	public GameObject[] toAppear;

	// Token: 0x0400008E RID: 142
	private int i;

	// Token: 0x0400008F RID: 143
	private bool flashFade;

	// Token: 0x04000090 RID: 144
	private Image flashPanel;

	// Token: 0x04000091 RID: 145
	private Color flashColor;

	// Token: 0x04000092 RID: 146
	private float flashMultiplier = 1f;

	// Token: 0x04000093 RID: 147
	private Vector3 maxPos;

	// Token: 0x04000094 RID: 148
	private Vector3 startingPos;

	// Token: 0x04000095 RID: 149
	private Vector3 goalPos;

	// Token: 0x04000096 RID: 150
	public bool complete;

	// Token: 0x04000097 RID: 151
	public GameObject playerPosInfo;

	// Token: 0x04000098 RID: 152
	public Vector3 finalPitPos;

	// Token: 0x04000099 RID: 153
	private AsyncOperation asyncLoad;

	// Token: 0x0400009A RID: 154
	private string oldBundle;

	// Token: 0x0400009B RID: 155
	private bool rankless;

	// Token: 0x0400009C RID: 156
	public GameObject ppiObject;

	// Token: 0x0400009D RID: 157
	public string targetLevelName;

	// Token: 0x0400009E RID: 158
	public TMP_Text pointsText;

	// Token: 0x0400009F RID: 159
	public int totalPoints;

	// Token: 0x040000A0 RID: 160
	private bool loadAndActivateScene;

	// Token: 0x040000A1 RID: 161
	public bool dependenciesLoaded;

	// Token: 0x040000A2 RID: 162
	private bool sceneBundleLoaded;

	// Token: 0x040000A3 RID: 163
	private bool skipping;

	// Token: 0x040000A4 RID: 164
	private float timeBetween = 0.25f;

	// Token: 0x040000A5 RID: 165
	private bool noRestarts;

	// Token: 0x040000A6 RID: 166
	private bool noDamage;

	// Token: 0x040000A7 RID: 167
	private bool majorAssists;

	// Token: 0x040000A8 RID: 168
	public string gbundlename;

	// Token: 0x040000A9 RID: 169
	public string glvlname;
}
