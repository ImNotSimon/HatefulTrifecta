//using System;
using System.Collections;
using System.IO;
using System.Linq;
using HatefulScripts;
using Logic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

// Token: 0x02000016 RID: 22
public class HatefulSceneHelper : MonoBehaviour
{
	// Token: 0x17000006 RID: 6
	// (get) Token: 0x06000075 RID: 117 RVA: 0x00006252 File Offset: 0x00004452
	public bool IsPlayingCustom
	{
		get
		{
			return GameStateManager.Instance.currentCustomGame != null;
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x06000076 RID: 118 RVA: 0x00006261 File Offset: 0x00004461
	public bool IsSceneRankless
	{
		get
		{
			return this.embeddedSceneInfo.ranklessScenes.Contains(this.CurrentScene);
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000077 RID: 119 RVA: 0x0000627C File Offset: 0x0000447C
	public int CurrentLevelNumber
	{
		get
		{
			bool flag = !this.IsPlayingCustom;
			int num;
			if (flag)
			{
				num = MonoSingleton<StatsManager>.Instance.levelNumber;
			}
			else
			{
				num = GameStateManager.Instance.currentCustomGame.levelNumber;
			}
			return num;
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000078 RID: 120 RVA: 0x000062B8 File Offset: 0x000044B8
	// (set) Token: 0x06000079 RID: 121 RVA: 0x000062C0 File Offset: 0x000044C0
	public string CurrentScene { get; private set; }

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600007A RID: 122 RVA: 0x000062C9 File Offset: 0x000044C9
	// (set) Token: 0x0600007B RID: 123 RVA: 0x000062D1 File Offset: 0x000044D1
	public string LastScene { get; private set; }

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600007C RID: 124 RVA: 0x000062DA File Offset: 0x000044DA
	// (set) Token: 0x0600007D RID: 125 RVA: 0x000062E2 File Offset: 0x000044E2
	public string PendingScene { get; private set; }

	// Token: 0x0600007E RID: 126 RVA: 0x000062EB File Offset: 0x000044EB
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x0600007F RID: 127 RVA: 0x000062FC File Offset: 0x000044FC
	private void OnEnable()
	{
		SceneManager.sceneLoaded += this.OnSceneLoaded;
		this.OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		bool flag = string.IsNullOrEmpty(this.CurrentScene);
		if (flag)
		{
			this.CurrentScene = SceneManager.GetActiveScene().name;
		}
	}

	// Token: 0x06000080 RID: 128 RVA: 0x0000634E File Offset: 0x0000454E
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= this.OnSceneLoaded;
	}

	// Token: 0x06000081 RID: 129 RVA: 0x00006364 File Offset: 0x00004564
	public bool IsSceneSpecial(string sceneName)
	{
		sceneName = this.SanitizeLevelPath(sceneName);
		bool flag = this.embeddedSceneInfo == null;
		return !flag && this.embeddedSceneInfo.specialScenes.Contains(sceneName);
	}

	// Token: 0x06000082 RID: 130 RVA: 0x000063A8 File Offset: 0x000045A8
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		bool flag = EventSystem.current != null;
		if (flag)
		{
			Object.Destroy(EventSystem.current.gameObject);
		}
		Object.Instantiate<GameObject>(this.eventSystem);
		bool flag2 = mode == LoadSceneMode.Single;
		if (flag2)
		{
			GameStateManager.Instance.ResetGravity();
		}
	}

	// Token: 0x06000083 RID: 131 RVA: 0x000063F8 File Offset: 0x000045F8
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

	// Token: 0x06000084 RID: 132 RVA: 0x00006459 File Offset: 0x00004659
	public void ShowLoadingBlocker()
	{
		this.loadingBlocker.SetActive(true);
	}

	// Token: 0x06000085 RID: 133 RVA: 0x00006469 File Offset: 0x00004669
	public void DismissBlockers()
	{
		this.loadingBlocker.SetActive(false);
		this.loadingBar.gameObject.SetActive(false);
	}

	// Token: 0x06000086 RID: 134 RVA: 0x0000648B File Offset: 0x0000468B
	public void LoadScene(string sceneName, string bundleName, bool noBlocker = false)
	{
		base.StartCoroutine(this.LoadSceneAsync(sceneName, Path.Combine(Plugin.ModPath(), bundleName), noBlocker));
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000064A8 File Offset: 0x000046A8
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

	// Token: 0x06000088 RID: 136 RVA: 0x000064CC File Offset: 0x000046CC
	public void RestartScene()
	{
		MonoBehaviour[] array = Object.FindObjectsOfType<MonoBehaviour>();
		foreach (MonoBehaviour monoBehaviour in array)
		{
			bool flag = monoBehaviour != null && monoBehaviour.gameObject.scene.name != "DontDestroyOnLoad";
			if (flag)
			{
				monoBehaviour.CancelInvoke();
				monoBehaviour.enabled = false;
			}
		}
		bool flag2 = string.IsNullOrEmpty(this.CurrentScene);
		if (flag2)
		{
			this.CurrentScene = SceneManager.GetActiveScene().name;
		}
		AssetBundleLoader.LoadSceneFromAssetBundle(this.assetBundlePath, this.CurrentScene);
		bool flag3 = MonoSingleton<MapVarManager>.Instance != null;
		if (flag3)
		{
			MonoSingleton<MapVarManager>.Instance.ReloadMapVars();
		}
	}

	// Token: 0x06000089 RID: 137 RVA: 0x00006590 File Offset: 0x00004790
	public void LoadPreviousScene()
	{
		string text = this.LastScene;
		bool flag = string.IsNullOrEmpty(text);
		if (flag)
		{
			text = "Main Menu";
		}
		SceneHelper.LoadScene(text, false);
	}

	// Token: 0x0600008A RID: 138 RVA: 0x000065C0 File Offset: 0x000047C0
	public void SpawnFinalPitAndFinish()
	{
		FinalRoom finalRoom = Object.FindObjectOfType<FinalRoom>();
		bool flag = finalRoom != null;
		if (flag)
		{
			bool flag2 = finalRoom.doorOpener != null;
			if (flag2)
			{
				finalRoom.doorOpener.SetActive(true);
			}
			MonoSingleton<NewMovement>.Instance.transform.position = finalRoom.dropPoint.position;
		}
		else
		{
			GameObject gameObject = Object.Instantiate<GameObject>(AssetHelper.LoadPrefab(this.finalRoomPit));
			finalRoom = gameObject.GetComponent<FinalRoom>();
			gameObject.transform.position = new Vector3(50000f, -1000f, 50000f);
			MonoSingleton<NewMovement>.Instance.transform.position = finalRoom.dropPoint.position;
		}
	}

	// Token: 0x0600008B RID: 139 RVA: 0x00006674 File Offset: 0x00004874
	public void SetLoadingSubtext(string text)
	{
		bool flag = this.loadingBlocker != null;
		if (flag)
		{
			this.loadingBar.gameObject.SetActive(true);
			this.loadingBar.text = text;
		}
	}

	// Token: 0x0600008C RID: 140 RVA: 0x000066B4 File Offset: 0x000048B4
	public int? GetLevelIndexAfterIntermission(string intermissionScene)
	{
		bool flag = this.embeddedSceneInfo == null;
		int? num;
		if (flag)
		{
			num = null;
		}
		else
		{
			foreach (IntermissionRelation intermissionRelation in this.embeddedSceneInfo.intermissions)
			{
				bool flag2 = intermissionRelation.intermissionScene == intermissionScene;
				if (flag2)
				{
					return new int?(intermissionRelation.nextLevelIndex);
				}
			}
			num = null;
		}
		return num;
	}

	// Token: 0x040000AA RID: 170
	[SerializeField]
	private AssetReference finalRoomPit;

	// Token: 0x040000AB RID: 171
	[SerializeField]
	private GameObject loadingBlocker;

	// Token: 0x040000AC RID: 172
	[SerializeField]
	private TMP_Text loadingBar;

	// Token: 0x040000AD RID: 173
	[SerializeField]
	private GameObject preloadingBadge;

	// Token: 0x040000AE RID: 174
	[SerializeField]
	private GameObject eventSystem;

	// Token: 0x040000AF RID: 175
	[Space]
	[SerializeField]
	private AudioMixerGroup masterMixer;

	// Token: 0x040000B0 RID: 176
	[SerializeField]
	private AudioMixerGroup musicMixer;

	// Token: 0x040000B1 RID: 177
	[SerializeField]
	private AudioMixer allSound;

	// Token: 0x040000B2 RID: 178
	[SerializeField]
	private AudioMixer goreSound;

	// Token: 0x040000B3 RID: 179
	[SerializeField]
	private AudioMixer musicSound;

	// Token: 0x040000B4 RID: 180
	[SerializeField]
	private AudioMixer doorSound;

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	private AudioMixer unfreezeableSound;

	// Token: 0x040000B6 RID: 182
	[Space]
	[SerializeField]
	private EmbeddedSceneInfo embeddedSceneInfo;

	// Token: 0x040000B7 RID: 183
	[SerializeField]
	private string assetBundlePath;
}
