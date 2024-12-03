using System;
using System.IO;
using System.Reflection;
using UnityEngine;

// Token: 0x0200000B RID: 11
public class GameManager : MonoBehaviour
{
	// Token: 0x06000025 RID: 37 RVA: 0x0000307C File Offset: 0x0000127C
	public static string ModPath()
	{
		return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
	}

	// Token: 0x06000026 RID: 38 RVA: 0x000030B2 File Offset: 0x000012B2
	private void Start()
	{
		this.why = Assembly.GetExecutingAssembly();
		this.bundlePath = Path.Combine(GameManager.ModPath(), this.resourceName);
	}

	// Token: 0x06000027 RID: 39 RVA: 0x000030D8 File Offset: 0x000012D8
	private void OnTriggerEnter(Collider other)
	{
		bool flag = ((Component)other).gameObject.CompareTag("Player");
		if (flag)
		{
			AssetBundleLoader.LoadSceneFromAssetBundle(this.bundlePath, this.sceneName);
		}
	}

	// Token: 0x0400002A RID: 42
	public string sceneName;

	// Token: 0x0400002B RID: 43
	private AssetBundle loadedBundle;

	// Token: 0x0400002C RID: 44
	public Assembly why;

	// Token: 0x0400002D RID: 45
	public string resourceName = "hate1scene";

	// Token: 0x0400002E RID: 46
	public string bundlePath;
}
