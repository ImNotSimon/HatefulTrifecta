using System;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000005 RID: 5
public class AssetBundleSceneLoader : MonoBehaviour
{
	// Token: 0x06000005 RID: 5 RVA: 0x0000218C File Offset: 0x0000038C
	public static string ModPath()
	{
		return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
	}

	// Token: 0x06000006 RID: 6 RVA: 0x000021C2 File Offset: 0x000003C2
	private void Start()
	{
		this.why = Assembly.GetExecutingAssembly();
		this.bundlePath = Path.Combine(AssetBundleSceneLoader.ModPath(), this.resourceName);
		this.loadedBundle = AssetBundle.LoadFromFile(this.bundlePath);
	}

	// Token: 0x06000007 RID: 7 RVA: 0x000021F8 File Offset: 0x000003F8
	private void OnTriggerEnter(Collider other)
	{
		bool flag = ((Component)other).gameObject.CompareTag("Player");
		if (flag)
		{
			base.StartCoroutine(this.LoadAssetBundleAndScene(this.resourceName, this.sceneNameToLoad));
			AssetBundleSceneLoader.doit = true;
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00002240 File Offset: 0x00000440
	public IEnumerator LoadAssetBundleAndScene(string resourceName, string sceneName)
	{
		this.bundlePath = Path.Combine(AssetBundleSceneLoader.ModPath(), resourceName);
		this.sceneNameToLoad = sceneName;
		AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(this.bundlePath);
		yield return bundleRequest;
		bool flag = this.loadedBundle == null;
		if (flag)
		{
			Debug.LogError("Failed to load AssetBundle from path: " + this.assetBundlePath);
			yield break;
		}
		Debug.Log("AssetBundle loaded successfully.");
		bool flag2 = !this.loadedBundle.isStreamedSceneAssetBundle;
		if (flag2)
		{
			Debug.LogError("The AssetBundle does not contain any scenes.");
			this.UnloadBundle();
			yield break;
		}
		string[] scenePaths = this.loadedBundle.GetAllScenePaths();
		string scenePathToLoad = null;
		foreach (string scenePath in scenePaths)
		{
			bool flag3 = scenePath.EndsWith(this.sceneNameToLoad + ".unity");
			if (flag3)
			{
				scenePathToLoad = scenePath;
				break;
			}
		}
		string[] array = null;
		bool flag4 = string.IsNullOrEmpty(scenePathToLoad);
		if (flag4)
		{
			Debug.LogError("Scene " + this.sceneNameToLoad + " not found in the AssetBundle.");
			this.UnloadBundle();
			yield break;
		}
		AsyncOperation sceneLoadRequest = SceneManager.LoadSceneAsync(scenePathToLoad, LoadSceneMode.Single);
		yield return sceneLoadRequest;
		AssetBundleSceneLoader.doit = true;
		bool isDone = sceneLoadRequest.isDone;
		if (isDone)
		{
			Debug.Log("Scene loaded successfully from AssetBundle: " + this.sceneNameToLoad);
		}
		else
		{
			Debug.LogError("Failed to load the scene from AssetBundle.");
		}
		yield break;
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00002260 File Offset: 0x00000460
	private void UnloadBundle()
	{
		bool flag = this.loadedBundle != null;
		if (flag)
		{
			this.loadedBundle.Unload(false);
			this.loadedBundle = null;
			Debug.Log("AssetBundle unloaded.");
		}
	}

	// Token: 0x0600000A RID: 10 RVA: 0x0000229F File Offset: 0x0000049F
	private void OnDestroy()
	{
		this.UnloadBundle();
	}

	// Token: 0x04000002 RID: 2
	[SerializeField]
	private string assetBundlePath;

	// Token: 0x04000003 RID: 3
	[SerializeField]
	public string sceneNameToLoad;

	// Token: 0x04000004 RID: 4
	private AssetBundle loadedBundle;

	// Token: 0x04000005 RID: 5
	public Assembly why;

	// Token: 0x04000006 RID: 6
	public string resourceName = "hate";

	// Token: 0x04000007 RID: 7
	public string bundlePath;

	// Token: 0x04000008 RID: 8
	public static bool doit;
}
