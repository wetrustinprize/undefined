using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
        #region Variables

    [Header("Prefabs")]
    [SerializeField] private GameObject PlayerPrefab = null;
    [SerializeField] private GameObject BirdPrebab = null;
    [SerializeField] private GameObject HUDPrefab = null;
    [SerializeField] private GameObject CameraPrefab = null;

    // Script side variables
    private static PlayerController playerInstance = null;
    private static CreatureController birdInstance = null;
    private static HUDManager hudInstance = null;
    private static CameraController cameraInstance = null;
    private static SoundsManager soundInstance = null;
    private static EntityManager entityInstance = null;
    private static CheckPointManager checkpointManager = null;

    // Public acess variables
    public static PlayerController Player { get { return playerInstance; } }
    public static CreatureController Creature { get { return birdInstance; } }
    public static HUDManager HUD { get { return hudInstance; } }
    public static CameraController Camera { get { return cameraInstance; } }
    public static SoundsManager Sound { get { return soundInstance; } }
    public static EntityManager Entity { get { return entityInstance; } }
    public static CheckPointManager Checkpoint { get { return checkpointManager; } }

    // Instance
    public static GameManager manager;

        #endregion

    void Awake() {

        DontDestroyOnLoad(this.gameObject);

    }

    void OnEnable() {

        if(manager == null) manager = this;
        else Destroy(this.gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {

        CheckInstances();

        Transform spawn = GameObject.Find("PlayerSpawn").transform;

        playerInstance.transform.position = spawn.position;
        birdInstance.transform.position = spawn.position;

        Checkpoint.Save();

    }

    public void RespawnPlayer(Vector2 pos) {
        Destroy(playerInstance.gameObject);
        Destroy(birdInstance.gameObject);
        Destroy(hudInstance.gameObject);
        Destroy(cameraInstance.gameObject);

        playerInstance = null;
        birdInstance = null;
        hudInstance = null;
        cameraInstance = null;

        CheckInstances();

    }

    void CheckInstances() {

        GameObject MapInfo = GameObject.Find("_MapInfo");

        if(playerInstance == null)
            playerInstance = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerController>();
        
        if(birdInstance == null)
            birdInstance = Instantiate(BirdPrebab).GetComponent<CreatureController>();
        
        if(hudInstance == null)
            hudInstance = Instantiate(HUDPrefab).GetComponent<HUDManager>();
        
        if(soundInstance == null)
            soundInstance = this.GetComponent<SoundsManager>();

        if(checkpointManager == null)
            checkpointManager = this.GetComponent<CheckPointManager>();
        
        if(cameraInstance == null)
        {
            cameraInstance = Instantiate(CameraPrefab).GetComponent<CameraController>();
            Camera.LookAt(playerInstance.transform);
        }
        if(entityInstance == null)
        {
            if(MapInfo.TryGetComponent<EntityManager>(out EntityManager em))
            {
                entityInstance = em;
            }
        }

    }

}
