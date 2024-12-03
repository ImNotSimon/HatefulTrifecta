using System;
using System.IO;
using BepInEx;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000018 RID: 24
public class RankScoreChecker : MonoBehaviour
{
	// Token: 0x06000091 RID: 145 RVA: 0x00006970 File Offset: 0x00004B70
	public static string getConfigPath()
	{
		string configPath = Paths.ConfigPath;
		bool flag = configPath == null;
		string text;
		if (flag)
		{
			Debug.LogError("Paths.ConfigPath is null.");
			text = null;
		}
		else
		{
			text = Path.Combine(new string[] { configPath + Path.DirectorySeparatorChar.ToString() + "HatefulScripts" });
		}
		return text;
	}

	// Token: 0x06000092 RID: 146 RVA: 0x000069C8 File Offset: 0x00004BC8
	private void OnTriggerEnter(Collider other)
	{
		bool flag = this.stman == null;
		if (flag)
		{
			Debug.LogError("StatsManager (stman) is not assigned.");
		}
		else
		{
			this.rankScore = this.stman.rankScore;
			this.asscon = MonoSingleton<AssistController>.Instance;
			bool flag2 = this.asscon == null;
			if (flag2)
			{
				Debug.LogError("AssistController (asscon) is not assigned.");
			}
			else
			{
				bool flag3 = this.rankScore == 12 && !this.asscon.cheatsEnabled;
				if (flag3)
				{
					string configPath = RankScoreChecker.getConfigPath();
					bool flag4 = configPath == null;
					if (flag4)
					{
						Debug.LogError("Config path is null.");
					}
					else
					{
						string text = Path.Combine(configPath, "rankScores.json");
						bool flag5 = !Directory.Exists(configPath);
						if (flag5)
						{
							Directory.CreateDirectory(configPath);
						}
						bool flag6 = File.Exists(text);
						JObject jobject;
						if (flag6)
						{
							string text2 = File.ReadAllText(text);
							jobject = JObject.Parse(text2);
						}
						else
						{
							jobject = new JObject();
						}
						bool flag7;
						if (this.isFirstLevel)
						{
							JToken jtoken = jobject["firstLevel"];
							if (jtoken == null)
							{
								flag7 = false;
							}
							else
							{
								JToken jtoken2 = jtoken["isPRanked"];
								bool? flag8 = ((jtoken2 != null) ? new bool?(jtoken2.Value<bool>()) : null);
								bool flag9 = true;
								flag7 = (flag8.GetValueOrDefault() == flag9) & (flag8 != null);
							}
						}
						else
						{
							flag7 = false;
						}
						bool flag10 = flag7;
						if (flag10)
						{
							Debug.Log("firstLevel is already PRanked, returning.");
						}
						else
						{
							bool flag11;
							if (this.isSecondLevel)
							{
								JToken jtoken3 = jobject["secondLevel"];
								if (jtoken3 == null)
								{
									flag11 = false;
								}
								else
								{
									JToken jtoken4 = jtoken3["isPRanked"];
									bool? flag8 = ((jtoken4 != null) ? new bool?(jtoken4.Value<bool>()) : null);
									bool flag9 = true;
									flag11 = (flag8.GetValueOrDefault() == flag9) & (flag8 != null);
								}
							}
							else
							{
								flag11 = false;
							}
							bool flag12 = flag11;
							if (flag12)
							{
								Debug.Log("secondLevel is already PRanked, returning.");
							}
							else
							{
								bool flag13;
								if (this.isThirdLevel)
								{
									JToken jtoken5 = jobject["thirdLevel"];
									if (jtoken5 == null)
									{
										flag13 = false;
									}
									else
									{
										JToken jtoken6 = jtoken5["isPRanked"];
										bool? flag8 = ((jtoken6 != null) ? new bool?(jtoken6.Value<bool>()) : null);
										bool flag9 = true;
										flag13 = (flag8.GetValueOrDefault() == flag9) & (flag8 != null);
									}
								}
								else
								{
									flag13 = false;
								}
								bool flag14 = flag13;
								if (flag14)
								{
									Debug.Log("thirdLevel is already PRanked, returning.");
								}
								else
								{
									bool flag15 = this.isFirstLevel;
									if (flag15)
									{
										JObject jobject2 = jobject;
										string text3 = "firstLevel";
										JObject jobject3 = new JObject();
										jobject3["isPRanked"] = true;
										jobject2[text3] = jobject3;
									}
									bool flag16 = this.isSecondLevel;
									if (flag16)
									{
										JObject jobject4 = jobject;
										string text4 = "secondLevel";
										JObject jobject5 = new JObject();
										jobject5["isPRanked"] = true;
										jobject4[text4] = jobject5;
									}
									bool flag17 = this.isThirdLevel;
									if (flag17)
									{
										JObject jobject6 = jobject;
										string text5 = "thirdLevel";
										JObject jobject7 = new JObject();
										jobject7["isPRanked"] = true;
										jobject6[text5] = jobject7;
									}
									File.WriteAllText(text, jobject.ToString());
									Debug.Log("Data saved to JSON file: " + text);
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x040000BF RID: 191
	private AssistController asscon;

	// Token: 0x040000C0 RID: 192
	[SerializeField]
	private StatsManager stman;

	// Token: 0x040000C1 RID: 193
	[SerializeField]
	private bool isFirstLevel;

	// Token: 0x040000C2 RID: 194
	[SerializeField]
	private bool isSecondLevel;

	// Token: 0x040000C3 RID: 195
	[SerializeField]
	private bool isThirdLevel;

	// Token: 0x040000C4 RID: 196
	private int rankScore;
}
