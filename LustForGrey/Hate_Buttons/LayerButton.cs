using System;
using System.IO;
using UnityEngine;

namespace LustForGrey.Hate_Buttons
{
	// Token: 0x02000021 RID: 33
	public class LayerButton : MonoBehaviour
	{
		// Token: 0x060000D5 RID: 213 RVA: 0x00008534 File Offset: 0x00006734
		public void OnEnable()
		{
			bool flag = !File.Exists(this.firstLevelInfoPath);
			if (flag)
			{
				base.gameObject.SetActive(false);
			}
		}

		// Token: 0x060000D6 RID: 214 RVA: 0x00008564 File Offset: 0x00006764
		public void DisableCHSelect()
		{
			GameObject gameObject = GameObject.Find("Canvas");
			bool flag = gameObject != null;
			if (flag)
			{
				Transform transform = gameObject.transform.Find("Chapter Select");
				bool flag2 = transform != null;
				if (flag2)
				{
					transform.gameObject.SetActive(false);
					bool flag3 = this.hatePanel != null;
					if (flag3)
					{
						this.hatePanel.SetActive(true);
					}
				}
			}
		}

		// Token: 0x040000FC RID: 252
		public GameObject hatePanel;

		// Token: 0x040000FD RID: 253
		private string firstLevelInfoPath = Path.Combine(GameProgressSaver.SavePath, "lvl500progress.bepis");
	}
}
