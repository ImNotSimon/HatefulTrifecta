using System;
using System.Collections;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

// Token: 0x02000012 RID: 18
public class HatefulAbruptLevelChanger : MonoBehaviour
{
	// Token: 0x17000003 RID: 3
	// (get) Token: 0x0600004C RID: 76 RVA: 0x00004059 File Offset: 0x00002259
	// (set) Token: 0x0600004D RID: 77 RVA: 0x00004061 File Offset: 0x00002261
	public string CurrentScene { get; private set; }

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600004E RID: 78 RVA: 0x0000406A File Offset: 0x0000226A
	// (set) Token: 0x0600004F RID: 79 RVA: 0x00004072 File Offset: 0x00002272
	public string LastScene { get; private set; }

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000050 RID: 80 RVA: 0x0000407B File Offset: 0x0000227B
	// (set) Token: 0x06000051 RID: 81 RVA: 0x00004083 File Offset: 0x00002283
	public string PendingScene { get; private set; }

	// Token: 0x06000052 RID: 82 RVA: 0x0000408C File Offset: 0x0000228C
	public void AbruptChangeLevel()
	{
		bool flag = this.saveMission;
		if (flag)
		{
			MonoSingleton<PreviousMissionSaver>.Instance.previousMission = MonoSingleton<StatsManager>.Instance.levelNumber;
		}
		this.LoadSceneAsync(this.levelname, this.bundleName, false);
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000040D0 File Offset: 0x000022D0
	public string SanitizeLevelPath(string scene)
	{
		bool flag = scene.StartsWith("Assets/Scenes/");
		if (flag)
		{
			scene = scene.Substring("Assets/Scenes/".Length);
		}
		bool flag2 = scene.EndsWith(".unity");
		if (flag2)
		{
			scene = scene.Substring(0, scene.Length - ".unity".Length);
		}
		return scene;
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00004131 File Offset: 0x00002331
	public IEnumerator LoadSceneAsync(string sceneName, string assetBundlePath, bool noSplash = false)
	{
		bool flag = this.PendingScene == null;
		if (flag)
		{
			this.PendingScene = sceneName;
			sceneName = this.SanitizeLevelPath(sceneName);
			Debug.Log("(LoadSceneAsync) Loading scene " + sceneName);
			this.loadingBlocker.SetActive(!noSplash);
			yield return null;
			bool flag2 = this.CurrentScene != sceneName;
			if (flag2)
			{
				this.LastScene = this.CurrentScene;
			}
			this.CurrentScene = sceneName;
			bool flag3 = MonoSingleton<MapVarManager>.Instance != null;
			if (flag3)
			{
				MonoSingleton<MapVarManager>.Instance.ReloadMapVars();
			}
			AssetBundleLoader.LoadSceneFromAssetBundle(assetBundlePath, sceneName);
			bool flag4 = GameStateManager.Instance != null;
			if (flag4)
			{
				GameStateManager.Instance.currentCustomGame = null;
			}
			bool flag5 = this.preloadingBadge != null;
			if (flag5)
			{
				this.preloadingBadge.SetActive(false);
			}
			bool flag6 = this.loadingBlocker != null;
			if (flag6)
			{
				this.loadingBlocker.SetActive(false);
			}
			bool flag7 = this.loadingBar != null;
			if (flag7)
			{
				this.loadingBar.gameObject.SetActive(false);
			}
			this.PendingScene = null;
		}
		yield break;
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004155 File Offset: 0x00002355
	public void NormalChangeLevel()
	{
		this.LoadSceneAsync(this.levelname, this.bundleName, false);
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0000416C File Offset: 0x0000236C
	public void GoToLevel(int missionNumber)
	{
		SceneHelper.LoadScene(GetMissionName.GetSceneName(missionNumber), false);
	}

	// Token: 0x06000057 RID: 87 RVA: 0x0000417C File Offset: 0x0000237C
	public void GoToSavedLevel()
	{
		PreviousMissionSaver instance = MonoSingleton<PreviousMissionSaver>.Instance;
		bool flag = instance != null;
		if (flag)
		{
			int previousMission = instance.previousMission;
			UnityEngine.Object.Destroy(instance.gameObject);
			this.GoToLevel(instance.previousMission);
		}
		else
		{
			this.GoToLevel(GameProgressSaver.GetProgress(MonoSingleton<PrefsManager>.Instance.GetInt("difficulty", 0)));
		}
	}

	// Token: 0x04000050 RID: 80
	public bool loadingSplash;

	// Token: 0x04000051 RID: 81
	public bool saveMission;

	// Token: 0x04000055 RID: 85
	public string bundleName;

	// Token: 0x04000056 RID: 86
	public string levelname;

	// Token: 0x04000057 RID: 87
	[SerializeField]
	private AssetReference finalRoomPit;

	// Token: 0x04000058 RID: 88
	[SerializeField]
	private GameObject loadingBlocker;

	// Token: 0x04000059 RID: 89
	[SerializeField]
	private TMP_Text loadingBar;

	// Token: 0x0400005A RID: 90
	[SerializeField]
	private GameObject preloadingBadge;

	// Token: 0x0400005B RID: 91
	[SerializeField]
	private GameObject eventSystem;
}
