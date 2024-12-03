using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000004 RID: 4
public static class AssetBundleLoader
{
	// Token: 0x06000003 RID: 3 RVA: 0x0000206C File Offset: 0x0000026C
	public static void LoadSceneFromAssetBundle(string assetBundlePath, string sceneName)
	{
		AssetBundle assetBundle = AssetBundle.LoadFromFile(assetBundlePath);
		bool flag = assetBundle == null;
		if (flag)
		{
			Debug.LogError("Failed to load AssetBundle from path: " + assetBundlePath);
		}
		else
		{
			bool isStreamedSceneAssetBundle = assetBundle.isStreamedSceneAssetBundle;
			if (isStreamedSceneAssetBundle)
			{
				string[] allScenePaths = assetBundle.GetAllScenePaths();
				string scenePath = AssetBundleLoader.GetScenePath(allScenePaths, sceneName);
				bool flag2 = !string.IsNullOrEmpty(scenePath);
				if (flag2)
				{
					AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(scenePath));
					asyncOperation.completed += delegate(AsyncOperation operation)
					{
						Debug.Log("Scene loaded successfully.");
						assetBundle.Unload(false);
					};
				}
				else
				{
					Debug.LogError("Scene not found in AssetBundle.");
					assetBundle.Unload(false);
				}
			}
			else
			{
				Debug.LogError("AssetBundle does not contain scenes.");
				assetBundle.Unload(false);
			}
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002148 File Offset: 0x00000348
	private static string GetScenePath(string[] scenePaths, string sceneName)
	{
		foreach (string text in scenePaths)
		{
			bool flag = Path.GetFileNameWithoutExtension(text) == sceneName;
			if (flag)
			{
				return text;
			}
		}
		return null;
	}
}
