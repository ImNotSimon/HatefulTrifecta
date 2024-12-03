using HatefulScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HABundle : MonoBehaviour
{
    public string publicModPath;
    public string publicSceneName;

    // Reference to the Plugin instance (BepInEx managed)
    private Plugin pluginInstance;

    public string ModPath()
    {
        return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }

    private void Awake()
    {
        // Get the Plugin instance managed by BepInEx (assuming Plugin.Instance is properly set)
        pluginInstance = Plugin.Instance;

        if (pluginInstance == null)
        {
            Debug.LogError("Plugin instance is null. Ensure the Plugin is properly loaded by BepInEx.");
        }
    }

    private void OnEnable()
    {
        // Subscribe to the scene unload event
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid potential memory leaks
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    // Called when a scene is unloaded, cleans up all loaded AssetBundles
    private void OnSceneUnloaded(Scene scene)
    {
        UnloadAllBundles();
    }

    public IEnumerator LoadAssetBundleAndScene(string resourceName, string sceneNameToLoad)
    {
        if (string.IsNullOrEmpty(resourceName) || string.IsNullOrEmpty(sceneNameToLoad))
        {
            Debug.LogError("Invalid resource or scene name provided.");
            yield break;
        }

        string bundlePath = Path.Combine(ModPath(), resourceName);
        bool bundleAlreadyLoaded = pluginInstance.loadedBundlesList.Any(bundle => bundle != null && bundle.name == resourceName);

        if (bundleAlreadyLoaded)
        {
            Debug.LogError($"AssetBundle '{resourceName}' is already loaded.");
            yield break;
        }

        Debug.Log($"Attempting to load AssetBundle: {bundlePath}");

        AssetBundleCreateRequest bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return bundleRequest;

        AssetBundle loadedBundle = bundleRequest.assetBundle;
        if (loadedBundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle from path: {bundlePath}");
            yield break;
        }

        Debug.Log("AssetBundle loaded successfully.");

        // Add bundle to the global loadedBundlesList for dynamic unloading
        AddBundleToGlobalList(loadedBundle);

        bool isStreamedSceneAssetBundle = loadedBundle.isStreamedSceneAssetBundle;
        if (!isStreamedSceneAssetBundle)
        {
            Debug.LogError("The AssetBundle does not contain any scenes.");
            yield break;
        }

        // Check if the scene exists in the bundle
        string[] scenePaths = loadedBundle.GetAllScenePaths();
        string scenePathToLoad = Array.Find(scenePaths, path => path.EndsWith($"{sceneNameToLoad}.unity", StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrEmpty(scenePathToLoad))
        {
            Debug.LogError($"Scene '{sceneNameToLoad}' not found in AssetBundle.");
            yield break;
        }

        // Unload the current scene if it's already loaded
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (scenePaths.Any(path => path.EndsWith(currentSceneName + ".unity")))
        {
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(currentSceneName);
            yield return unloadOperation;
            Debug.Log($"Unloaded current scene: {currentSceneName}");
        }

        // Load the new scene from the AssetBundle
        AsyncOperation sceneLoadRequest = SceneManager.LoadSceneAsync(scenePathToLoad, LoadSceneMode.Single);
        yield return sceneLoadRequest;

        if (sceneLoadRequest.isDone)
        {
            Debug.Log($"Scene '{sceneNameToLoad}' loaded successfully.");

            // Now reload the new scene
            ReloadCurrentScene();
        }
        else
        {
            Debug.LogError($"Failed to load scene '{sceneNameToLoad}' from AssetBundle.");
        }
    }

    // Add the AssetBundle to the global loadedBundlesList in Plugin.cs
    private void AddBundleToGlobalList(AssetBundle bundle)
    {
        if (pluginInstance != null)
        {
            // Try to find an empty slot in the global list
            for (int i = 0; i < pluginInstance.loadedBundlesList.Length; i++)
            {
                if (pluginInstance.loadedBundlesList[i] == null)
                {
                    pluginInstance.loadedBundlesList[i] = bundle;
                    Debug.Log($"Added AssetBundle '{bundle.name}' to global list at index {i}.");
                    return;
                }
            }
            Debug.LogWarning("Global loaded bundles list is full; unable to add the AssetBundle.");
        }
        else
        {
            Debug.LogError("Plugin instance not found.");
        }
    }

    // Unload all loaded AssetBundles from the global list
    public void UnloadAllBundles()
    {
        if (pluginInstance != null)
        {
            for (int i = 0; i < pluginInstance.loadedBundlesList.Length; i++)
            {
                var bundle = pluginInstance.loadedBundlesList[i];
                if (bundle != null)
                {
                    bundle.Unload(true);
                    pluginInstance.loadedBundlesList[i] = null;  // Clear the slot in the global list
                    Debug.Log($"Unloaded AssetBundle at index {i}: {bundle.name}");
                }
            }
            Debug.Log("All AssetBundles unloaded.");
        }
    }

    public AsyncOperation ReloadCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Reloading current scene: {currentSceneName}");

        AsyncOperation reloadOperation = SceneManager.LoadSceneAsync(currentSceneName);
        reloadOperation.completed += operation =>
        {
            SceneHelper.DismissBlockers();
            Plugin.Fixorsmth();
        };

        return reloadOperation;
    }
}
