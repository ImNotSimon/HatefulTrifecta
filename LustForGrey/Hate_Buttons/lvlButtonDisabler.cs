using System;
using System.IO;
using UnityEngine;

namespace LustForGrey.Hate_Buttons
{
	// Token: 0x02000022 RID: 34
	public class lvlButtonDisabler : MonoBehaviour
	{
		// Token: 0x060000D8 RID: 216 RVA: 0x000085F4 File Offset: 0x000067F4
		public void OnEnable()
		{
			string text = null;
			if (!this.isFirstLevel)
			{
				if (!this.isSecondLevel)
				{
					if (this.isThirdLevel)
					{
						text = this.thirdLevelInfoPath;
					}
				}
				else
				{
					text = this.secondLevelInfoPath;
				}
			}
			else
			{
				text = this.firstLevelInfoPath;
			}
			bool flag = text != null && !File.Exists(text);
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x040000FE RID: 254
		public bool isFirstLevel;

		// Token: 0x040000FF RID: 255
		public bool isSecondLevel;

		// Token: 0x04000100 RID: 256
		public bool isThirdLevel;

		// Token: 0x04000101 RID: 257
		private string firstLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl500progress.bepis");

		// Token: 0x04000102 RID: 258
		private string secondLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl501progress.bepis");

		// Token: 0x04000103 RID: 259
		private string thirdLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl502progress.bepis");
	}
}
