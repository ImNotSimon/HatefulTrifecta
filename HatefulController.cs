using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using HatefulScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000010 RID: 16
public class HatefulController : MonoBehaviour
{
	// Token: 0x06000040 RID: 64 RVA: 0x00003458 File Offset: 0x00001658
	private void Start()
	{
		bool flag = this.loadLevelButton != null;
		if (flag)
		{
			this.loadLevelButton.onClick.AddListener(delegate
			{
				this.LoadLevel();
			});
		}
		else
		{
			Debug.LogWarning("LoadLevel button is not assigned in the inspector.");
		}
		SceneManager.sceneUnloaded += this.OnSceneUnloaded;
	}

	// Token: 0x06000041 RID: 65 RVA: 0x000034B6 File Offset: 0x000016B6
	private void OnDestroy()
	{
		SceneManager.sceneUnloaded -= this.OnSceneUnloaded;
	}

	// Token: 0x06000042 RID: 66 RVA: 0x000034CB File Offset: 0x000016CB
	private void OnSceneUnloaded(Scene scene)
	{
		this.UnloadAllBundles();
	}

	// Token: 0x06000043 RID: 67 RVA: 0x000034D8 File Offset: 0x000016D8
	public string ModPath()
	{
		return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
	}

	// Token: 0x06000044 RID: 68 RVA: 0x0000350E File Offset: 0x0000170E
	public void LoadLevel()
	{
		base.StartCoroutine(this.LoadAssetBundleAndScene(this.BundleName, this.sceneNameToLoad));
	}

	// Token: 0x06000045 RID: 69 RVA: 0x0000352A File Offset: 0x0000172A
	private IEnumerator LoadAssetBundleAndScene(string resourceName, string sceneNameToLoad)
	{
		string bundlePath = Path.Combine(this.ModPath(), resourceName);
		foreach (AssetBundle bundle in this.loadedBundles)
		{
			bool flag = bundle != null && bundle.name == resourceName;
			if (flag)
			{
				Debug.LogError("AssetBundle '" + resourceName + "' is already loaded.");
				yield break;
			}
		}
		AssetBundle[] array = null;
		bool flag2 = this.currentBundle != null;
		if (flag2)
		{
			this.currentBundle.Unload(true);
			Debug.Log("Unloaded previous AssetBundle.");
		}
		AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
		yield return bundleRequest;
		AssetBundle loadedBundle = bundleRequest.assetBundle;
		bool flag3 = loadedBundle == null;
		if (flag3)
		{
			Debug.LogError("Failed to load AssetBundle from path: " + bundlePath);
			yield break;
		}
		Debug.Log("AssetBundle loaded successfully.");
		this.currentBundle = loadedBundle;
		this.loadedBundles[this.bundleIndex] = loadedBundle;
		this.bundleIndex = (this.bundleIndex + 1) % this.loadedBundles.Length;
		this.AddBundleToGlobalList(loadedBundle);
		bool flag4 = !loadedBundle.isStreamedSceneAssetBundle;
		if (flag4)
		{
			Debug.LogError("The AssetBundle does not contain any scenes.");
			yield break;
		}
		string currentSceneName = SceneManager.GetActiveScene().name;
		bool flag5 = loadedBundle.GetAllScenePaths().Any((string path) => path.EndsWith(currentSceneName + ".unity"));
		if (flag5)
		{
			AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentSceneName);
			yield return unloadOperation;
			Debug.Log("Unloaded current scene: " + currentSceneName);
			unloadOperation = null;
		}
		string[] scenePaths = loadedBundle.GetAllScenePaths();
		string scenePathToLoad = scenePaths.FirstOrDefault((string path) => path.EndsWith(sceneNameToLoad + ".unity"));
		bool flag6 = string.IsNullOrEmpty(scenePathToLoad);
		if (flag6)
		{
			Debug.LogError("Scene " + sceneNameToLoad + " not found in the AssetBundle.");
			yield break;
		}
		AsyncOperation sceneLoadRequest = SceneManager.LoadSceneAsync(scenePathToLoad, LoadSceneMode.Single);
		yield return sceneLoadRequest;
		bool isDone = sceneLoadRequest.isDone;
		if (isDone)
		{
			Debug.Log("Scene loaded successfully from AssetBundle: " + sceneNameToLoad);
			Plugin.Fixorsmth();
		}
		else
		{
			Debug.LogError("Failed to load the scene from AssetBundle.");
		}
		yield break;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x00003548 File Offset: 0x00001748
	private void AddBundleToGlobalList(AssetBundle bundle)
	{
		Plugin instance = Plugin.Instance;
		bool flag = instance == null;
		if (flag)
		{
			Debug.LogError("Plugin instance not found.");
		}
		else
		{
			for (int i = 0; i < instance.loadedBundlesList.Length; i++)
			{
				bool flag2 = instance.loadedBundlesList[i] == null;
				if (flag2)
				{
					instance.loadedBundlesList[i] = bundle;
					Debug.Log(string.Format("Added AssetBundle '{0}' to global list at index {1}.", bundle.name, i));
					return;
				}
			}
			Debug.LogWarning("Global loaded bundles list is full; unable to add the AssetBundle.");
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x000035D8 File Offset: 0x000017D8
	public void UnloadAllBundles()
	{
		for (int i = 0; i < this.loadedBundles.Length; i++)
		{
			bool flag = this.loadedBundles[i] != null;
			if (flag)
			{
				this.loadedBundles[i].Unload(true);
				this.loadedBundles[i] = null;
				Debug.Log("Unloaded AssetBundle at index " + i.ToString());
			}
		}
		this.currentBundle = null;
	}

	// Token: 0x0400003A RID: 58
	public string sceneNameToLoad;

	// Token: 0x0400003B RID: 59
	public string BundleName;

	// Token: 0x0400003C RID: 60
	private AssetBundle[] loadedBundles = new AssetBundle[3];

	// Token: 0x0400003D RID: 61
	private int bundleIndex = 0;

	// Token: 0x0400003E RID: 62
	private AssetBundle currentBundle;

	// Token: 0x0400003F RID: 63
	[SerializeField]
	private Button loadLevelButton;
}
