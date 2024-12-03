using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HatefulScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000013 RID: 19
public static class HatefulAssetBundleSceneLoader
{
	// Token: 0x06000059 RID: 89 RVA: 0x000041E8 File Offset: 0x000023E8
	public static string ModPath()
	{
		return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
	}

	// Token: 0x0600005A RID: 90 RVA: 0x0000421E File Offset: 0x0000241E
	public static IEnumerator LoadAssetBundleAndScene(string resourceName, string sceneNameToLoad)
	{
		string bundlePath = Path.Combine(HatefulAssetBundleSceneLoader.ModPath(), resourceName);
		HatefulAssetBundleSceneLoader.publicModPath = bundlePath;
		HatefulAssetBundleSceneLoader.publicSceneName = sceneNameToLoad;
		AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
		yield return bundleRequest;
		AssetBundle loadedBundle = bundleRequest.assetBundle;
		bool flag = loadedBundle == null;
		if (flag)
		{
			Debug.LogError("Failed to load AssetBundle from path: " + bundlePath);
			yield break;
		}
		Debug.Log("AssetBundle loaded successfully.");
		bool flag2 = !loadedBundle.isStreamedSceneAssetBundle;
		if (flag2)
		{
			Debug.LogError("The AssetBundle does not contain any scenes.");
			HatefulAssetBundleSceneLoader.UnloadBundle(loadedBundle);
			yield break;
		}
		string[] scenePaths = loadedBundle.GetAllScenePaths();
		string scenePathToLoad = null;
		foreach (string scenePath in scenePaths)
		{
			bool flag3 = scenePath.EndsWith(sceneNameToLoad + ".unity");
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
			Debug.LogError("Scene " + sceneNameToLoad + " not found in the AssetBundle.");
			HatefulAssetBundleSceneLoader.UnloadBundle(loadedBundle);
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
		HatefulAssetBundleSceneLoader.UnloadBundle(loadedBundle);
		yield break;
	}

	// Token: 0x0600005B RID: 91 RVA: 0x00004234 File Offset: 0x00002434
	public static AsyncOperation ReloadCurrentScene()
	{
		string name = SceneManager.GetActiveScene().name;
		AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(name);
		asyncOperation.completed += delegate(AsyncOperation operation)
		{
			SceneHelper.DismissBlockers();
			Plugin.Fixorsmth();
		};
		return asyncOperation;
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00004284 File Offset: 0x00002484
	public static void UnloadAssetBundle(string bundlePath)
	{
		AssetBundle assetBundle;
		bool flag = HatefulAssetBundleSceneLoader.loadedBundles.TryGetValue(bundlePath, out assetBundle);
		if (flag)
		{
			assetBundle.Unload(true);
			HatefulAssetBundleSceneLoader.loadedBundles.Remove(bundlePath);
			Debug.Log("AssetBundle unloaded: " + bundlePath);
		}
	}

	// Token: 0x0600005D RID: 93 RVA: 0x000042CC File Offset: 0x000024CC
	private static void UnloadBundle(AssetBundle bundle)
	{
		bool flag = bundle != null;
		if (flag)
		{
			bundle.Unload(false);
			Debug.Log("AssetBundle unloaded.");
		}
	}

	// Token: 0x0400005C RID: 92
	public static string publicModPath;

	// Token: 0x0400005D RID: 93
	public static string publicSceneName;

	// Token: 0x0400005E RID: 94
	private static Dictionary<string, AssetBundle> loadedBundles = new Dictionary<string, AssetBundle>();
}
