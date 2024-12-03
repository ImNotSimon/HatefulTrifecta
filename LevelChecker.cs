using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x02000007 RID: 7
public class LevelChecker : MonoBehaviour
{
	// Token: 0x0600000E RID: 14 RVA: 0x00002329 File Offset: 0x00000529
	private void Start()
	{
		this.CheckFileAndDisableObjects();
	}

	// Token: 0x0600000F RID: 15 RVA: 0x00002334 File Offset: 0x00000534
	private void CheckFileAndDisableObjects()
	{
		if (!this.isFirstLevel)
		{
			if (!this.isSecondLevel)
			{
				if (this.isThirdLevel)
				{
					this.filePath = this.thirdLevelInfoPath;
				}
			}
			else
			{
				this.filePath = this.secondLevelInfoPath;
			}
		}
		else
		{
			this.filePath = this.firstLevelInfoPath;
		}
		bool flag = !File.Exists(this.filePath);
		if (flag)
		{
			foreach (GameObject gameObject in this.objectsToDisable)
			{
				bool flag2 = gameObject != null;
				if (flag2)
				{
					gameObject.SetActive(false);
				}
			}
		}
	}

	// Token: 0x04000009 RID: 9
	private string filePath;

	// Token: 0x0400000A RID: 10
	private string firstLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl500progress.bepis");

	// Token: 0x0400000B RID: 11
	private string secondLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl501progress.bepis");

	// Token: 0x0400000C RID: 12
	private string thirdLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl502progress.bepis");

	// Token: 0x0400000D RID: 13
	public bool isFirstLevel;

	// Token: 0x0400000E RID: 14
	public bool isSecondLevel;

	// Token: 0x0400000F RID: 15
	public bool isThirdLevel;

	// Token: 0x04000010 RID: 16
	[Header("Objects to Disable")]
	[Tooltip("List of GameObjects to disable if the level's saved data does not exist.")]
	public List<GameObject> objectsToDisable;
}
