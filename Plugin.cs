using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BepInEx;
using HarmonyLib;
using LustForGrey.Hate_Buttons;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Configgy;

namespace HatefulScripts
{
	[BepInPlugin("ImNotSimon.HatefulScripts", "Scripts used in the HATE layer's custom campaign", "0.5.1")]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance
		{
			get
			{
				return Plugin._instance;
			}
		}

		private ConfigBuilder config;
		[Configgable("Optionals", "Enable Wretches in Cybergrind (may require game restart)")]
		private static ConfigToggle cgWretch = new ConfigToggle(true);
		
		private static AssetBundle uibundle = Plugin.uibundle;
		private void OnButtonClicked()
		{
			bool flag = this.chapterSelectTransform == null;
			if (flag)
			{
				Debug.LogWarning("chapterSelectTransform is null! Cannot disable Chapter Select.");
			}
			else
			{
				this.chapterSelectTransform.gameObject.SetActive(false);
				this.instantiatedPanel.SetActive(true);
			}
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000783C File Offset: 0x00005A3C
		public static string getCatalog()
		{
			string streamingAssetsPath = Application.streamingAssetsPath;
			bool flag = streamingAssetsPath == null;
			string text;
			if (flag)
			{
				Debug.LogError("Application.streamingAssetsPath is null.");
				text = null;
			}
			else
			{
				text = Path.Combine(streamingAssetsPath, "aa", "catalog.json");
			}
			return text;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000787C File Offset: 0x00005A7C
		public static GameObject FindObjectEvenIfDisabled(string rootName, string objPath = null, int childNum = 0, bool useChildNum = false)
		{
			GameObject gameObject = null;
			GameObject[] rootGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
			bool flag = false;
			foreach (GameObject gameObject2 in rootGameObjects)
			{
				bool flag2 = gameObject2.name == rootName;
				if (flag2)
				{
					gameObject = gameObject2;
					flag = true;
				}
			}
			bool flag3 = !flag;
			if (!flag3)
			{
				GameObject gameObject3 = gameObject;
				bool flag4 = objPath != null;
				if (flag4)
				{
					gameObject3 = gameObject.transform.Find(objPath).gameObject;
					bool flag5 = !useChildNum;
					if (flag5)
					{
						gameObject = gameObject3;
					}
				}
				if (useChildNum)
				{
					GameObject gameObject4 = gameObject3.transform.GetChild(childNum).gameObject;
					gameObject = gameObject4;
				}
			}
			return gameObject;
		}

		public async void InstantiateMenuStuff()
		{
			bool flag = this.isMenuStuffInstantiated;
			if (!flag)
			{
				this.isMenuStuffInstantiated = true;
				GameObject HatefulController = new GameObject();
				HatefulController.AddComponent<HatefulController>();
				GameObject gameObject = await this.FindCanvasAsync();
				GameObject canvas = gameObject;
				gameObject = null;
				if (canvas == null)
				{
					Debug.LogError("Canvas not found, aborting instantiation.");
				}
				else
				{
					Debug.Log("Canvas found.");
					Transform transform = await this.FindChildTransformAsync(canvas.transform, "Chapter Select");
					this.chapterSelectTransform = transform as RectTransform;
					transform = null;
					if (this.chapterSelectTransform == null)
					{
						Debug.LogError("Chapter Select not found, aborting instantiation.");
					}
					else
					{
						Debug.Log("Chapter Select found.");
						string resourceName = "hate_ui";
						string bundlePath = Path.Combine(Plugin.ModPath(), resourceName);
						if (!File.Exists(bundlePath))
						{
							Debug.LogError("AssetBundle file not found at path: " + bundlePath);
						}
						else
						{
							AssetBundle assetBundle = Plugin.uibundle;
							if (assetBundle != null)
							{
								assetBundle.Unload(false);
							}
							Plugin.uibundle = AssetBundle.LoadFromFile(bundlePath);
							if (Plugin.uibundle == null)
							{
								Debug.LogError("Failed to load AssetBundle. The bundle is null.");
							}
							else
							{
								Debug.Log("AssetBundle loaded successfully.");
								GameObject uistuff = Plugin.uibundle.LoadAsset<GameObject>("HateButton");
								if (uistuff == null)
								{
									Debug.LogError("Failed to load 'Level Select (Hate)' asset from AssetBundle.");
								}
								else
								{
									Debug.Log("Panel loaded successfully.");
									UnityEngine.Object.Instantiate<GameObject>(uistuff, this.chapterSelectTransform);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000BF RID: 191 RVA: 0x00007979 File Offset: 0x00005B79
		private void TogglePanels()
		{
			this.chapterSelectTransform.gameObject.SetActive(false);
			this.instantiatedPanel.SetActive(true);
		}

        // Token: 0x060000C0 RID: 192 RVA: 0x0000799C File Offset: 0x00005B9C
        private async Task<GameObject> FindCanvasAsync()
        {
            GameObject canvas = null;
            while (canvas == null)
            {
                GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
                foreach (GameObject root in rootObjects)
                {
                    if (root.name == "Canvas")
                    {
                        canvas = root;
                        break;
                    }
                }
                if (canvas == null)
                {
                    Debug.Log("Waiting for Canvas to be available in scene root...");
                    await Task.Delay(100);
                }
            }
            return canvas;
        }


        // Token: 0x060000C1 RID: 193 RVA: 0x000079E0 File Offset: 0x00005BE0
        private async Task<Transform> FindChildTransformAsync(Transform parent, string childName)
		{
			Transform childTransform = this.FindInChildren(parent, childName);
			while (childTransform == null)
			{
				Debug.Log("Waiting for '" + childName + "' to be available...");
				await Task.Delay(25);
				childTransform = this.FindInChildren(parent, childName);
			}
			return childTransform;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x00007A34 File Offset: 0x00005C34
		private Transform FindInChildren(Transform parent, string childName)
		{
			foreach (object obj in parent)
			{
				Transform transform = (Transform)obj;
				bool flag = transform.name == childName;
				if (flag)
				{
					return transform;
				}
				Transform transform2 = this.FindInChildren(transform, childName);
				bool flag2 = transform2 != null;
				if (flag2)
				{
					return transform2;
				}
			}
			return null;
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x00007AC4 File Offset: 0x00005CC4
		private void AssignLayerButton(GameObject instantiatedButton, GameObject instantiatedPanel)
		{
			LayerButton component = instantiatedButton.GetComponent<LayerButton>();
			bool flag = component != null;
			if (flag)
			{
				component.hatePanel = instantiatedPanel;
				Debug.Log("LayerButton's hatePanel assigned.");
			}
			else
			{
				Debug.LogError("LayerButton component is missing on the HateButton.");
			}
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x00007B08 File Offset: 0x00005D08
		private void AssignMenuEsc(GameObject instantiatedPanel, Transform chapterSelectTransform)
		{
			MenuEsc component = instantiatedPanel.GetComponent<MenuEsc>();
			bool flag = component != null;
			if (flag)
			{
				component.previousPage = chapterSelectTransform.gameObject;
				Debug.Log("MenuEsc's previousPage assigned.");
			}
			else
			{
				Debug.LogError("MenuEsc component is missing on the panel.");
			}
		}

        private bool hasReloadedSceneOnce = false;  // Flag to track scene reload status
        private string previousScene = "";  // Variable to store the previous scene name

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Reset the flag when transitioning to a different scene (not on every load)
            if (scene.name != previousScene)
            {
                hasReloadedSceneOnce = false;  // Reset the flag when a new scene is loaded
                previousScene = scene.name;  // Update the previous scene
            }

            this.isMenuStuffInstantiated = false;
            bool flag = SceneHelper.CurrentScene != "Bootstrap" && SceneHelper.CurrentScene != "Intro";
            bool flag2 = SceneHelper.CurrentScene == "Main Menu";
            Plugin.allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>();
            bool flag3 = flag2 && SceneManager.GetActiveScene().name == "b3e7f2f8052488a45b35549efb98d902";
            if (flag3)
            {
                this.InstantiateMenuStuff();
            }

            bool flag4 = scene.name == "HateFirst";
            if (flag4)
            {
                Plugin.IsCustomLevel = true;
                Plugin.Fixorsmth();
            }

            bool doit = AssetBundleSceneLoader.doit;
            if (doit)
            {
                Plugin.Fixorsmth();
            }

            bool flag5 = ShaderManager.shaderDictionary.Count <= 0;
            if (flag5)
            {
                base.StartCoroutine(ShaderManager.LoadShadersAsync());
            }

            bool flag6 = SceneHelper.CurrentScene == "uk_construct";
            if (flag6)
            {
                // No specific action here yet
            }

            bool flag7 = SceneHelper.CurrentScene == "Main Menu" && SceneManager.GetActiveScene().name == "b3e7f2f8052488a45b35549efb98d902";
            if (flag7)
            {
                ShaderManager.CreateShaderDictionary();
                this.InstantiateMenuStuff();
            }

            bool flag8 = scene.name == "HateFirst" || scene.name == "HateSecond" || scene.name == "HateThird" || scene.name == "HateSecret" || scene.name == "HatePrime" || scene.name == "HateLobby";

            // Check if it's part of the hate campaign
            if (flag8)
            {
                // Scene reload only happens once per level in the campaign
                if (!hasReloadedSceneOnce)
                {
                    hasReloadedSceneOnce = true;  // Ensure reload happens only once per scene load

                    SceneHelper.CurrentScene = SceneManager.GetActiveScene().name;
                    Camera main = Camera.main;
                    Plugin.IsCustomLevel = true;
                    Debug.LogError("yooo!! hate campaign detected!!!");
                    HatefulAssetBundleSceneLoader.ReloadCurrentScene();
                    this.DisableHellMap();
                    MusicMan_Patch.startFix(MusicManager.Instance);
                    ShaderManager.ApplyShadersAsyncContinuously();
                    base.StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());

                    bool flag9 = main != null;
                    if (flag9)
                    {
                        main.clearFlags = CameraClearFlags.Skybox;
                    }
                    else
                    {
                        Debug.LogWarning("Main camera not found in the scene.");
                    }

                    base.StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());
                }

                else
                {
                    SceneHelper.CurrentScene = SceneManager.GetActiveScene().name;
                    Camera main = Camera.main;
                    Plugin.IsCustomLevel = true;
                    Debug.LogError("yooo!! hate campaign detected!!!");
                    this.DisableHellMap();
                    MusicMan_Patch.startFix(MusicManager.Instance);
                    ShaderManager.ApplyShadersAsyncContinuously();
                    base.StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());

                    bool flag9 = main != null;
                    if (flag9)
                    {
                        main.clearFlags = CameraClearFlags.Skybox;
                    }
                    else
                    {
                        Debug.LogWarning("Main camera not found in the scene.");
                    }

                    base.StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());
                }
            }
            else
            {
                Plugin.IsCustomLevel = false;
            }

            SpawnableObjectsDatabase[] array = this.FindAllInstances<SpawnableObjectsDatabase>();
			PrefabDatabase[] cgEnemies = this.FindAllInstances<PrefabDatabase>();
            bool flag10 = this.wretchSO == null;
            if (flag10)
            {
                this.LoadSOStuff();
            }

            foreach (SpawnableObjectsDatabase spawnableObjectsDatabase in array)
            {
                bool flag11 = spawnableObjectsDatabase.enemies != null && !spawnableObjectsDatabase.enemies.Contains(this.wretchSO);
                if (flag11)
                {
                    List<SpawnableObject> list = spawnableObjectsDatabase.enemies.ToList<SpawnableObject>();
                    list.Add(this.wretchSO);
                    spawnableObjectsDatabase.enemies = list.ToArray();
                }
            }
			if(cgWretch.Value == true)
			{
			foreach (PrefabDatabase cyber in cgEnemies)
             {
				bool flag11 = cyber.meleeEnemies != null && !cyber.meleeEnemies.ToList().Contains(cgwretch);
                if (flag11)
                {
					List<EndlessEnemy> list = cyber.meleeEnemies.ToList();
                    list.Add(cgwretch);
                    cyber.meleeEnemies = list.ToArray();
                }
           	 }
			}

            this.unloadShit();
        }


        // Token: 0x060000C6 RID: 198 RVA: 0x00007E1C File Offset: 0x0000601C
        private bool hellMapDisabled = false;

		private async Task DisableHellMap()
		{
    		HellMap hellMapInstance = null;
    		while (hellMapInstance == null)
    		{
        		HellMap[] allHellMaps = Resources.FindObjectsOfTypeAll<HellMap>();
        		if (allHellMaps.Length > 0)
        		{
            		hellMapInstance = allHellMaps[0];
        		}
        		await Task.Delay(50);
    		}

    		hellMapInstance.gameObject.SetActive(false);
    		Debug.Log("HellMap instance found and disabled.");
		}

		private void OnSceneUnloaded(Scene scene)
		{
			bool flag = SceneHelper.CurrentScene == "Main Menu" && SceneManager.GetActiveScene().name == "b3e7f2f8052488a45b35549efb98d902";
			if (flag)
			{
				ShaderManager.CreateShaderDictionary();
				this.InstantiateMenuStuff();
			}
			SpawnableObjectsDatabase[] array = this.FindAllInstances<SpawnableObjectsDatabase>();
			foreach (SpawnableObjectsDatabase spawnableObjectsDatabase in array)
			{
				bool flag2 = spawnableObjectsDatabase.enemies != null && spawnableObjectsDatabase.enemies.Contains(this.wretchSO);
				if (flag2)
				{
					List<SpawnableObject> list = spawnableObjectsDatabase.enemies.ToList<SpawnableObject>();
					list.Remove(this.wretchSO);
					spawnableObjectsDatabase.enemies = list.ToArray();
				}
			}
		}

		// Token: 0x060000C8 RID: 200 RVA: 0x00007F20 File Offset: 0x00006120
		public static string ModPath()
		{
			return Assembly.GetExecutingAssembly().Location.Substring(0, Assembly.GetExecutingAssembly().Location.LastIndexOf(Path.DirectorySeparatorChar));
		}

		// Token: 0x060000C9 RID: 201 RVA: 0x00007F58 File Offset: 0x00006158
		private void Awake()
		{
			config = new ConfigBuilder("ImNotSimon.HatefulTrifecta");
	        config.BuildAll();
			Debug.Log("don't you dare edit the .json to cheat :)");
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Plugin._instance = this;
			string text = "hateres";
			string text2 = Path.Combine(Plugin.ModPath(), text);
			bool flag = File.Exists(text2);
			if (flag)
			{
				Plugin.resbundle = AssetBundle.LoadFromFile(text2);
				bool flag2 = Plugin.resbundle == null;
				if (flag2)
				{
					Debug.LogError("Failed to load AssetBundle. The bundle is null.");
				}
				else
				{
					Debug.Log("AssetBundle loaded successfully.");
				}
			}
			else
			{
				Debug.LogError("AssetBundle file not found at path: " + text2);
			}
			SceneManager.sceneLoaded += this.OnSceneLoaded;
			SceneManager.sceneUnloaded += this.OnSceneUnloaded;
			this.harmony = new Harmony("simon.hate");
			this.harmony.PatchAll();
		}

		// Token: 0x060000CA RID: 202 RVA: 0x00008028 File Offset: 0x00006228
		public static string GetLevelName()
		{
			StockMapInfo stockMapInfo = UnityEngine.Object.FindObjectOfType<StockMapInfo>();
			bool flag = stockMapInfo != null;
			string text;
			if (flag)
			{
				SerializedActivityAssets assets = stockMapInfo.assets;
				string largeText = assets.LargeText;
				text = largeText;
			}
			else
			{
				Debug.LogError("StockMapInfo not found in the scene!");
				text = null;
			}
			return text;
		}

		// Token: 0x060000CB RID: 203 RVA: 0x00008070 File Offset: 0x00006270
		public static void Fixorsmth()
		{
			SceneHelper.CurrentScene = SceneManager.GetActiveScene().name;
			Camera main = Camera.main;
			Plugin.IsCustomLevel = true;
			main.clearFlags = CameraClearFlags.Skybox;
			Plugin._instance.StartCoroutine(ShaderManager.ApplyShadersAsyncContinuously());
			MusicMan_Patch.startFix(MusicManager.Instance);
        }

		// Token: 0x060000CC RID: 204 RVA: 0x000080B8 File Offset: 0x000062B8
		private void LoadSOStuff()
		{
			string text = Path.Combine(Plugin.ModPath(), "hate_enres");
			this.loadedAssetBundle = AssetBundle.LoadFromFile(text);
			bool flag = this.loadedAssetBundle == null;
			if (flag)
			{
				Debug.LogError("Failed to load AssetBundle!");
			}
			else
			{
				SpawnableObject spawnableObject = this.loadedAssetBundle.LoadAsset<SpawnableObject>("Wretch_SO");
				EndlessEnemy wretch = this.loadedAssetBundle.LoadAsset<EndlessEnemy>("WretchEndlessData");
				if (spawnableObject.name == "Wretch_SO")
				{
					this.wretchSO = spawnableObject;
				}
				bool flag3 = this.wretchSO == null;
				if (flag3)
				{
					Debug.LogError("'Wretch_SO' not found in bundle.");
				}
				if (wretch.name == "WretchEndlessData")
				{
					this.cgwretch = wretch;
				}
				else{
					Debug.LogError("WretchEndlessData not found in bundle.");
				}
			}
		}

		// Token: 0x060000CD RID: 205 RVA: 0x00008150 File Offset: 0x00006350
		private void UnloadAssetBundle()
		{
			bool flag = this.loadedAssetBundle != null;
			if (flag)
			{
				this.loadedAssetBundle.Unload(true);
				this.loadedAssetBundle = null;
				this.wretchSO = null;
			}
		}

		// Token: 0x060000CE RID: 206 RVA: 0x0000818C File Offset: 0x0000638C
		private void AddWretchToSpawnableDatabases()
		{
			SpawnableObjectsDatabase[] array = UnityEngine.Object.FindObjectsOfType<SpawnableObjectsDatabase>();
			foreach (SpawnableObjectsDatabase spawnableObjectsDatabase in array)
			{
				bool flag = spawnableObjectsDatabase.enemies == null;
				if (flag)
				{
					Debug.LogError("Enemies list is not initialized in SpawnableObjectsDatabase.");
				}
				else
				{
					List<SpawnableObject> list = new List<SpawnableObject>(spawnableObjectsDatabase.enemies);
					bool flag2 = !list.Contains(this.wretchSO);
					if (flag2)
					{
						list.Add(this.wretchSO);
						spawnableObjectsDatabase.enemies = list.ToArray();
					}
				}
			}
		}

		// Token: 0x060000CF RID: 207 RVA: 0x00008214 File Offset: 0x00006414
		private void RemoveWretchFromSpawnableDatabases()
		{
			SpawnableObjectsDatabase[] array = UnityEngine.Object.FindObjectsOfType<SpawnableObjectsDatabase>();
			foreach (SpawnableObjectsDatabase spawnableObjectsDatabase in array)
			{
				bool flag = spawnableObjectsDatabase.enemies == null;
				if (!flag)
				{
					List<SpawnableObject> list = new List<SpawnableObject>(spawnableObjectsDatabase.enemies);
					list.RemoveAll((SpawnableObject enemy) => enemy != null && enemy.name == "Wretch_SO");
					spawnableObjectsDatabase.enemies = list.ToArray();
				}
			}
		}

		// Token: 0x060000D0 RID: 208 RVA: 0x00008294 File Offset: 0x00006494
		private T[] FindAllInstances<T>() where T : UnityEngine.Object
		{
			return Resources.FindObjectsOfTypeAll<T>();
		}

		// Token: 0x060000D1 RID: 209 RVA: 0x000082AC File Offset: 0x000064AC
		private void LogCurrentListeners(Button button)
		{
			FieldInfo field = typeof(Button).GetField("m_OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
			bool flag = field != null;
			if (flag)
			{
				UnityEvent unityEvent = field.GetValue(button) as UnityEvent;
				bool flag2 = unityEvent != null;
				if (flag2)
				{
					int persistentEventCount = unityEvent.GetPersistentEventCount();
					Debug.Log(string.Format("Current number of listeners: {0}", persistentEventCount));
					for (int i = 0; i < persistentEventCount; i++)
					{
						UnityEngine.Object persistentTarget = unityEvent.GetPersistentTarget(i);
						string persistentMethodName = unityEvent.GetPersistentMethodName(i);
						Debug.Log(string.Format("Listener {0}: Target = {1}, Method = {2}", i + 1, persistentTarget, persistentMethodName));
					}
				}
				else
				{
					Debug.LogError("Failed to get UnityEvent from button.");
				}
			}
			else
			{
				Debug.LogError("Failed to find m_OnClick field.");
			}
		}

		// Token: 0x060000D2 RID: 210 RVA: 0x00008380 File Offset: 0x00006580
		private bool IsListenerAdded(Button button)
		{
			FieldInfo field = typeof(Button).GetField("m_OnClick", BindingFlags.Instance | BindingFlags.NonPublic);
			bool flag = field != null;
			if (flag)
			{
				UnityEvent unityEvent = field.GetValue(button) as UnityEvent;
				bool flag2 = unityEvent != null;
				if (flag2)
				{
					for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
					{
						UnityEngine.Object persistentTarget = unityEvent.GetPersistentTarget(i);
						string persistentMethodName = unityEvent.GetPersistentMethodName(i);
						bool flag3 = persistentMethodName == "OnButtonClicked";
						if (flag3)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

        private void unloadShit()
        {
            // Get the name of the current scene
            string currentSceneName = SceneManager.GetActiveScene().name;

            // Loop through all loaded asset bundles
            for (int i = 0; i < loadedBundlesList.Length; i++)
            {
                var assetBundle = loadedBundlesList[i];

                // Skip null asset bundles
                if (assetBundle == null)
                    continue;

                // Check if the asset bundle contains the current scene
                bool containsCurrentScene = assetBundle
                    .GetAllScenePaths()
                    .Any(path => path.EndsWith(currentSceneName + ".unity"));

                if (!containsCurrentScene)
                {
                    // Unload the asset bundle if it does not contain the current scene
                    assetBundle.Unload(true);
                    loadedBundlesList[i] = null;
                    Debug.Log($"Unloaded AssetBundle at index {i}");
                }
                else
                {
                    Debug.Log($"Skipped unloading AssetBundle at index {i} because it contains the current scene: {currentSceneName}");
                }
            }
        }

        // Token: 0x040000EF RID: 239
        private GameObject instantiatedPanel;

		// Token: 0x040000F0 RID: 240
		private Transform chapterSelectTransform;

		// Token: 0x040000F1 RID: 241
		public static bool IsCustomLevel;

		// Token: 0x040000F2 RID: 242
		public static GameObject[] allGameObjects;

		// Token: 0x040000F3 RID: 243
		private static Plugin _instance;

		// Token: 0x040000F4 RID: 244
		private AssetBundle loadedAssetBundle;

		// Token: 0x040000F5 RID: 245
		private SpawnableObject wretchSO;

		private EndlessEnemy cgwretch;

		// Token: 0x040000F6 RID: 246
		public AssetBundle[] loadedBundlesList = new AssetBundle[99];

		// Token: 0x040000F7 RID: 247
		public Scene Level1Scene;

		// Token: 0x040000F8 RID: 248
		private bool isMenuStuffInstantiated = false;

		// Token: 0x040000F9 RID: 249
		public static AssetBundle resbundle;

		// Token: 0x040000FA RID: 250
		//public static AssetBundle uibundle;

		// Token: 0x040000FB RID: 251
		public Harmony harmony;

		// Token: 0x02000046 RID: 70
		[HarmonyPatch(typeof(SceneHelper), "RestartScene")]
		public static class SceneHelper_RestartScene_Patch
		{
			// Token: 0x06000169 RID: 361 RVA: 0x0000C688 File Offset: 0x0000A888
			[HarmonyPrefix]
			public static bool Prefix()
			{
				bool isCustomLevel = Plugin.IsCustomLevel;
				bool flag;
				if (isCustomLevel)
				{
					HatefulAssetBundleSceneLoader.ReloadCurrentScene().completed += delegate(AsyncOperation operation)
					{
						SceneHelper.DismissBlockers();
						Plugin.Fixorsmth();
					};
					flag = false;
				}
				else
				{
					flag = true;
				}
				return flag;
			}
		}

		// Token: 0x02000047 RID: 71
		[HarmonyPatch(typeof(LevelNameFinder))]
		[HarmonyPatch("OnEnable")]
		public static class LevelNameFinder_Patch
		{
			// Token: 0x0600016A RID: 362 RVA: 0x0000C6D4 File Offset: 0x0000A8D4
			private static void Postfix(LevelNameFinder __instance)
			{
				bool isCustomLevel = Plugin.IsCustomLevel;
				if (isCustomLevel)
				{
					__instance.txt2.text = Plugin.GetLevelName();
				}
			}
		}

		// Token: 0x02000048 RID: 72
		[HarmonyPatch(typeof(StockMapInfo), "Awake")]
		internal class Patch00
		{
			// Token: 0x0600016B RID: 363 RVA: 0x0000C6FC File Offset: 0x0000A8FC
			private static void Postfix(StockMapInfo __instance)
			{
				string name = SceneManager.GetActiveScene().name;
				bool flag = name == "HateFirst";
				if (flag)
				{
					Plugin.IsCustomLevel = true;
				}
				bool flag2 = name.StartsWith("aa5555");
				if (flag2)
				{
					string text = "hateContent";
					GameObject gameObject = Plugin.resbundle.LoadAsset<GameObject>(text);
					bool flag3 = gameObject != null;
					if (flag3)
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject, gameObject.transform.position, gameObject.transform.rotation);
						Debug.Log(string.Format("{0} prefab instantiated successfully at {1}.", text, gameObject.transform.position));
					}
					else
					{
						Debug.LogError("Failed to load prefab '" + text + "' from AssetBundle.");
					}
				}
			}
		}

		// Token: 0x02000049 RID: 73
		[HarmonyPatch(typeof(HellMap), "Start")]
		internal class Patch01
		{
			// Token: 0x0600016D RID: 365 RVA: 0x0000C7D0 File Offset: 0x0000A9D0
			private static void Postfix(HellMap __instance)
			{
				bool isCustomLevel = Plugin.IsCustomLevel;
				if (isCustomLevel)
				{
					__instance.gameObject.SetActive(false);
				}
			}
		}

		// Token: 0x0200004A RID: 74
		[HarmonyPatch(typeof(GetMissionName), "GetSceneName")]
		internal class Patch02
		{
			// Token: 0x0600016F RID: 367 RVA: 0x0000C800 File Offset: 0x0000AA00
			private static string Postfix(int missionNum)
			{
				string text;
				switch (missionNum)
				{
				case 1:
					text = "Level 0-1";
					break;
				case 2:
					text = "Level 0-2";
					break;
				case 3:
					text = "Level 0-3";
					break;
				case 4:
					text = "Level 0-4";
					break;
				case 5:
					text = "Level 0-5";
					break;
				case 6:
					text = "Level 1-1";
					break;
				case 7:
					text = "Level 1-2";
					break;
				case 8:
					text = "Level 1-3";
					break;
				case 9:
					text = "Level 1-4";
					break;
				case 10:
					text = "Level 2-1";
					break;
				case 11:
					text = "Level 2-2";
					break;
				case 12:
					text = "Level 2-3";
					break;
				case 13:
					text = "Level 2-4";
					break;
				case 14:
					text = "Level 3-1";
					break;
				case 15:
					text = "Level 3-2";
					break;
				case 16:
					text = "Level 4-1";
					break;
				case 17:
					text = "Level 4-2";
					break;
				case 18:
					text = "Level 4-3";
					break;
				case 19:
					text = "Level 4-4";
					break;
				case 20:
					text = "Level 5-1";
					break;
				case 21:
					text = "Level 5-2";
					break;
				case 22:
					text = "Level 5-3";
					break;
				case 23:
					text = "Level 5-4";
					break;
				case 24:
					text = "Level 6-1";
					break;
				case 25:
					text = "Level 6-2";
					break;
				case 26:
					text = "Level 7-1";
					break;
				case 27:
					text = "Level 7-2";
					break;
				case 28:
					text = "Level 7-3";
					break;
				case 29:
					text = "Level 7-4";
					break;
				case 30:
					text = "Level 8-1";
					break;
				case 31:
					text = "Level 8-2";
					break;
				case 32:
					text = "Level 8-3";
					break;
				case 33:
					text = "Level 8-4";
					break;
				case 34:
					text = "Level 9-1";
					break;
				case 35:
					text = "Level 9-2";
					break;
				default:
					switch (missionNum)
					{
					case 500:
						return "Level 10-1";
					case 501:
						return "Level 10-2";
					case 502:
						break;
					case 503:
						return "Level 10-3";
					default:
						switch (missionNum)
						{
						case 666:
							return "Level P-1";
						case 667:
							return "Level P-2";
						case 668:
							return "Level P-3";
						}
						break;
					}
					text = "Main Menu";
					break;
				}
				return text;
			}
		}

		// Token: 0x0200004B RID: 75
		[HarmonyPatch(typeof(GetMissionName), "GetMissionNameOnly")]
		internal class Patch03
		{
			// Token: 0x06000171 RID: 369 RVA: 0x0000CA88 File Offset: 0x0000AC88
			private static string Postfix(int missionNum)
			{
				bool isPlayingCustom = SceneHelper.IsPlayingCustom;
				string text;
				if (isPlayingCustom)
				{
					text = MapInfoBase.InstanceAnyType.levelName;
				}
				else
				{
					switch (missionNum)
					{
					case 0:
						text = "MAIN MENU";
						break;
					case 1:
						text = "INTO THE FIRE";
						break;
					case 2:
						text = "THE MEATGRINDER";
						break;
					case 3:
						text = "DOUBLE DOWN";
						break;
					case 4:
						text = "A ONE-MACHINE ARMY";
						break;
					case 5:
						text = "CERBERUS";
						break;
					case 6:
						text = "HEART OF THE SUNRISE";
						break;
					case 7:
						text = "THE BURNING WORLD";
						break;
					case 8:
						text = "HALLS OF SACRED REMAINS";
						break;
					case 9:
						text = "CLAIR DE LUNE";
						break;
					case 10:
						text = "BRIDGEBURNER";
						break;
					case 11:
						text = "DEATH AT 20,000 VOLTS";
						break;
					case 12:
						text = "SHEER HEART ATTACK";
						break;
					case 13:
						text = "COURT OF THE CORPSE KING";
						break;
					case 14:
						text = "BELLY OF THE BEAST";
						break;
					case 15:
						text = "IN THE FLESH";
						break;
					case 16:
						text = "SLAVES TO POWER";
						break;
					case 17:
						text = "GOD DAMN THE SUN";
						break;
					case 18:
						text = "A SHOT IN THE DARK";
						break;
					case 19:
						text = "CLAIR DE SOLEIL";
						break;
					case 20:
						text = "IN THE WAKE OF POSEIDON";
						break;
					case 21:
						text = "WAVES OF THE STARLESS SEA";
						break;
					case 22:
						text = "SHIP OF FOOLS";
						break;
					case 23:
						text = "LEVIATHAN";
						break;
					case 24:
						text = "CRY FOR THE WEEPER";
						break;
					case 25:
						text = "AESTHETICS OF HATE";
						break;
					case 26:
						text = "GARDEN OF FORKING PATHS";
						break;
					case 27:
						text = "LIGHT UP THE NIGHT";
						break;
					case 28:
						text = "NO SOUND, NO MEMORY";
						break;
					case 29:
						text = "...LIKE ANTENNAS TO HEAVEN";
						break;
					case 30:
						text = "???";
						break;
					case 31:
						text = "???";
						break;
					case 32:
						text = "???";
						break;
					case 33:
						text = "???";
						break;
					case 34:
						text = "???";
						break;
					case 35:
						text = "???";
						break;
					default:
						switch (missionNum)
						{
						case 500:
							text = "A SILENT CACOPHONY";
							break;
						case 501:
							text = "IMPERFECT ELYSIUM";
							break;
						case 502:
							text = "???";
							break;
						default:
							switch (missionNum)
							{
							case 666:
								text = "SOUL SURVIVOR";
								break;
							case 667:
								text = "WAIT OF THE WORLD";
								break;
							case 668:
								text = "???";
								break;
							default:
								text = "MISSION NAME NOT FOUND";
								break;
							}
							break;
						}
						break;
					}
				}
				return text;
			}
		}

		// Token: 0x0200004C RID: 76
		[HarmonyPatch(typeof(GetMissionName), "GetMissionNumberOnly")]
		internal class Patch04
		{
			// Token: 0x06000173 RID: 371 RVA: 0x0000CD34 File Offset: 0x0000AF34
			private static string Postfix(int missionNum)
			{
				bool isPlayingCustom = SceneHelper.IsPlayingCustom;
				string text;
				if (isPlayingCustom)
				{
					text = MapInfoBase.InstanceAnyType.levelName;
				}
				else
				{
					bool isPlayingCustom2 = SceneHelper.IsPlayingCustom;
					if (isPlayingCustom2)
					{
						text = "";
					}
					else
					{
						switch (missionNum)
						{
						case 1:
							text = "0-1";
							break;
						case 2:
							text = "0-2";
							break;
						case 3:
							text = "0-3";
							break;
						case 4:
							text = "0-4";
							break;
						case 5:
							text = "0-5";
							break;
						case 6:
							text = "1-1";
							break;
						case 7:
							text = "1-2";
							break;
						case 8:
							text = "1-3";
							break;
						case 9:
							text = "1-4";
							break;
						case 10:
							text = "2-1";
							break;
						case 11:
							text = "2-2";
							break;
						case 12:
							text = "2-3";
							break;
						case 13:
							text = "2-4";
							break;
						case 14:
							text = "3-1";
							break;
						case 15:
							text = "3-2";
							break;
						case 16:
							text = "4-1";
							break;
						case 17:
							text = "4-2";
							break;
						case 18:
							text = "4-3";
							break;
						case 19:
							text = "4-4";
							break;
						case 20:
							text = "5-1";
							break;
						case 21:
							text = "5-2";
							break;
						case 22:
							text = "5-3";
							break;
						case 23:
							text = "5-4";
							break;
						case 24:
							text = "6-1";
							break;
						case 25:
							text = "6-2";
							break;
						case 26:
							text = "7-1";
							break;
						case 27:
							text = "7-2";
							break;
						case 28:
							text = "7-3";
							break;
						case 29:
							text = "7-4";
							break;
						case 30:
							text = "8-1";
							break;
						case 31:
							text = "8-2";
							break;
						case 32:
							text = "8-3";
							break;
						case 33:
							text = "8-4";
							break;
						case 34:
							text = "9-1";
							break;
						case 35:
							text = "9-2";
							break;
						default:
							switch (missionNum)
							{
							case 500:
								text = "10-1";
								break;
							case 501:
								text = "10-2";
								break;
							case 502:
								text = "10-3";
								break;
							default:
								switch (missionNum)
								{
								case 666:
									text = "P-1";
									break;
								case 667:
									text = "P-2";
									break;
								case 668:
									text = "P-3";
									break;
								default:
									text = "";
									break;
								}
								break;
							}
							break;
						}
					}
				}
				return text;
			}
		}

		public static class MusicMan_Patch
		{
			public static void startFix(MusicManager __instance)
			{
				__instance.StartCoroutine(Plugin.MusicMan_Patch.waitForCustom(__instance));
			}

            private static IEnumerator waitForCustom(MusicManager __instance)
            {
                yield return new WaitForSeconds(0.35f);
                try
                {
                    __instance.battleTheme.outputAudioMixerGroup = MonoSingleton<AudioMixerController>.instance.musicGroup;
                    __instance.bossTheme.outputAudioMixerGroup = MonoSingleton<AudioMixerController>.instance.musicGroup;
                    __instance.cleanTheme.outputAudioMixerGroup = MonoSingleton<AudioMixerController>.instance.musicGroup;
                    __instance.targetTheme.outputAudioMixerGroup = MonoSingleton<AudioMixerController>.instance.musicGroup;
                }
                catch
                {
                }

                foreach (AudioSource audio in Resources.FindObjectsOfTypeAll(typeof(AudioSource)))
                {
                    try
                    {
                        bool flag2 = audio.outputAudioMixerGroup.audioMixer.name == "MusicAudio" || audio.outputAudioMixerGroup.audioMixer.name == "MusicAudio_0";
                        if (flag2)
                        {
                            audio.outputAudioMixerGroup = MonoSingleton<AudioMixerController>.instance.musicGroup;
                        }
                    }
                    catch
                    {
                    }
                }

                UnityEngine.Object[] array = null;
                yield break;
            }
        }
    }
}
