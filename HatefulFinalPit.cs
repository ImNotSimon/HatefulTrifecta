using System.Diagnostics;
using System.IO;
using UnityEngine;

public class HatefulFinalPit : MonoBehaviour
{
    private NewMovement nmov;
    private StatsManager sm;
    private Rigidbody rb;
    private bool rotationReady;
    private GameObject player;
    private bool infoSent;

    public bool rankless;
    public bool secondPit;
    public string targetLevelName;

    private int levelNumber;

    public bool musicFadeOut;

    private Quaternion targetRotation;

    // AssetBundle and Scene properties
    public string bundlename;
    public string lvlname;

    // Rank-related instance
    public HatefulFinalRank hatefulFinalRank;

    public void LoadLevel(string bundlename, string lvlname)
    {
        // Create a new GameObject and add HABundle as a component
        GameObject habundleObject = new GameObject("HABundleObject");
        HABundle habundle = habundleObject.AddComponent<HABundle>();

        // Log what you're attempting to load for debugging purposes
        UnityEngine.Debug.Log($"Loading via HABundle: {bundlename}, Scene: {lvlname}");

        // Start the coroutine on this new HABundle instance
        StartCoroutine(habundle.LoadAssetBundleAndScene(bundlename, lvlname));
    }
    private void Start()
    {
        // Initialize managers and player references
        sm = MonoSingleton<StatsManager>.Instance;
        player = MonoSingleton<NewMovement>.Instance.gameObject;

        // Slightly adjust rotation to ensure proper handling
        targetRotation = Quaternion.Euler(base.transform.rotation.eulerAngles + Vector3.up * 0.01f);

        // Create the rank handler GameObject
        hatefulFinalRank = new GameObject("HatefulFinalRank").AddComponent<HatefulFinalRank>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && MonoSingleton<NewMovement>.Instance?.hp > 0)
        {
            // Set AssetBundle and Scene names for the loader
            HatefulAssetBundleSceneLoader.publicModPath = Path.Combine(ShaderManager.ModPath(), bundlename);
            HatefulAssetBundleSceneLoader.publicSceneName = Path.Combine(ShaderManager.ModPath(), lvlname);

            // Optionally fade out music
            if (musicFadeOut)
            {
                MonoSingleton<MusicManager>.Instance.off = true;
            }

            // Manage game state
            GameStateManager.Instance.RegisterState(new GameState("pit-falling", base.gameObject)
            {
                cursorLock = LockMode.Lock,
                cameraInputLock = LockMode.Lock
            });

            // Prepare player for transition
            nmov = MonoSingleton<NewMovement>.Instance;
            nmov.gameObject.layer = 15; // Adjust layer
            rb = nmov.rb;
            nmov.activated = false;
            nmov.cc.enabled = false;
            nmov.levelOver = true;

            // Stop gameplay-related systems
            sm.HideShit();
            sm.StopTimer();

            if (nmov.sliding)
            {
                nmov.StopSlide();
            }

            // Reset player's velocity
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);

            // Reset power-ups
            if (MonoSingleton<PowerUpMeter>.Instance)
            {
                MonoSingleton<PowerUpMeter>.Instance.juice = 0f;
            }

            // Save progress and manage in-game objects
            MonoSingleton<CrateCounter>.Instance?.SaveStuff();
            MonoSingleton<CrateCounter>.Instance?.CoinsToPoints();

            // Disable certain objects to avoid conflicts
            foreach (OutOfBounds obj in Object.FindObjectsOfType<OutOfBounds>())
            {
                obj.gameObject.SetActive(false);
            }

            foreach (DeathZone obj in Object.FindObjectsOfType<DeathZone>())
            {
                obj.gameObject.SetActive(false);
            }

            // Start the information sequence
            Invoke("SendInfo", 5f);
        }
        else if (other.gameObject.CompareTag("Player") && MonoSingleton<PlatformerMovement>.Instance?.dead == false)
        {
            MonoSingleton<PlayerTracker>.Instance.ChangeToFPS();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != player || MonoSingleton<NewMovement>.Instance?.hp <= 0)
        {
            return;
        }

        if (nmov == null)
        {
            nmov = other.gameObject.GetComponent<NewMovement>();
            rb = nmov.rb;
        }

        // Correct player's position
        if (other.transform.position.x != base.transform.position.x || other.transform.position.z != base.transform.position.z)
        {
            Vector3 targetPosition = new Vector3(base.transform.position.x, other.transform.position.y, base.transform.position.z);
            float distance = Vector3.Distance(other.transform.position, targetPosition);
            other.transform.position = Vector3.MoveTowards(other.transform.position, targetPosition, 1f + distance * Time.deltaTime);
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        // Handle rotation
        if (!rotationReady)
        {
            nmov.cc.transform.rotation = Quaternion.RotateTowards(
                nmov.cc.transform.rotation,
                targetRotation,
                Time.fixedDeltaTime * 10f * (Quaternion.Angle(nmov.cc.transform.rotation, targetRotation) + 1f));

            if (Quaternion.Angle(nmov.cc.transform.rotation, targetRotation) < 0.01f)
            {
                nmov.cc.transform.rotation = targetRotation;
                rotationReady = true;
            }
        }

        // Send info if ready
        if (rotationReady && !infoSent)
        {
            SendInfo();
        }
    }

    private void SendInfo()
    {
        CancelInvoke();

        if (infoSent)
        {
            return;
        }

        infoSent = true;

        if (!rankless)
        {
            FinalRank fr = sm.fr;

            if (!sm.infoSent)
            {
                levelNumber = MonoSingleton<StatsManager>.Instance.levelNumber;

                // Save level progress
                if (SceneHelper.IsPlayingCustom)
                {
                    GameProgressSaver.SaveProgress(SceneHelper.CurrentLevelNumber);
                }
                else if (levelNumber >= 420)
                {
                    GameProgressSaver.SaveProgress(0);
                }
                else
                {
                    GameProgressSaver.SaveProgress(levelNumber + 1);
                }

                hatefulFinalRank.targetLevelName = targetLevelName;
                
                LoadLevel(bundlename, lvlname);
            }

            if (secondPit)
            {
                fr.finalPitPos = transform.position;
                fr.reachedSecondPit = true;

                hatefulFinalRank.finalPitPos = transform.position;
                hatefulFinalRank.reachedSecondPit = true;

                LoadLevel(bundlename, lvlname);
            }

            if (!sm.infoSent)
            {
                sm.SendInfo();
            }
        }
        else if (secondPit)
        {
            GameProgressSaver.SetTutorial(true);
            LoadLevel(bundlename, lvlname);
        }
    }
}
