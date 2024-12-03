using System;
using System.IO;
using BepInEx;
using Newtonsoft.Json.Linq;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class PRankMaterialChanger : MonoBehaviour
{
	// Token: 0x0600008E RID: 142 RVA: 0x00006748 File Offset: 0x00004948
	private void Start()
	{
		string text = Path.Combine(Paths.ConfigPath, "HatefulScripts" + Path.DirectorySeparatorChar.ToString() + "rankScores.json");
		bool flag = !File.Exists(text);
		if (flag)
		{
			Debug.Log("JSON file not found, returning.");
		}
		else
		{
			string text2 = File.ReadAllText(text);
			JObject jobject = JObject.Parse(text2);
			bool flag2 = false;
			bool flag3;
			if (this.isFirstLevel)
			{
				JToken jtoken = jobject["firstLevel"];
				if (jtoken == null)
				{
					flag3 = false;
				}
				else
				{
					JToken jtoken2 = jtoken["isPRanked"];
					bool? flag4 = ((jtoken2 != null) ? new bool?(jtoken2.Value<bool>()) : null);
					bool flag5 = true;
					flag3 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
				}
			}
			else
			{
				flag3 = false;
			}
			bool flag6 = flag3;
			if (flag6)
			{
				this.ChangeMaterial();
				flag2 = true;
			}
			else
			{
				bool flag7;
				if (this.isSecondLevel)
				{
					JToken jtoken3 = jobject["secondLevel"];
					if (jtoken3 == null)
					{
						flag7 = false;
					}
					else
					{
						JToken jtoken4 = jtoken3["isPRanked"];
						bool? flag4 = ((jtoken4 != null) ? new bool?(jtoken4.Value<bool>()) : null);
						bool flag5 = true;
						flag7 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
					}
				}
				else
				{
					flag7 = false;
				}
				bool flag8 = flag7;
				if (flag8)
				{
					this.ChangeMaterial();
					flag2 = true;
				}
				else
				{
					bool flag9;
					if (this.isThirdLevel)
					{
						JToken jtoken5 = jobject["thirdLevel"];
						if (jtoken5 == null)
						{
							flag9 = false;
						}
						else
						{
							JToken jtoken6 = jtoken5["isPRanked"];
							bool? flag4 = ((jtoken6 != null) ? new bool?(jtoken6.Value<bool>()) : null);
							bool flag5 = true;
							flag9 = (flag4.GetValueOrDefault() == flag5) & (flag4 != null);
						}
					}
					else
					{
						flag9 = false;
					}
					bool flag10 = flag9;
					if (flag10)
					{
						this.ChangeMaterial();
						flag2 = true;
					}
				}
			}
			bool flag11 = !flag2;
			if (flag11)
			{
				Debug.Log("No conditions met for changing material, returning.");
			}
		}
	}

	// Token: 0x0600008F RID: 143 RVA: 0x0000690C File Offset: 0x00004B0C
	private void ChangeMaterial()
	{
		Renderer component = base.GetComponent<Renderer>();
		bool flag = component != null;
		if (flag)
		{
			component.material = this.newMaterial;
			Debug.Log("Material changed to: " + this.newMaterial.name);
		}
		else
		{
			Debug.LogError("Renderer component not found on the GameObject.");
		}
	}

	// Token: 0x040000BB RID: 187
	[SerializeField]
	private bool isFirstLevel;

	// Token: 0x040000BC RID: 188
	[SerializeField]
	private bool isSecondLevel;

	// Token: 0x040000BD RID: 189
	[SerializeField]
	private bool isThirdLevel;

	// Token: 0x040000BE RID: 190
	[SerializeField]
	private Material newMaterial;
}
