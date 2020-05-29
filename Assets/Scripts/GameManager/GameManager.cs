using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    
        #region Variables

    [Header("Prefabs")]
    [SerializeField] private GameObject PlayerPrefab;
    [SerializeField] private GameObject BirdPrebab;
    [SerializeField] private GameObject HUDPrefab;
    [SerializeField] private GameObject CameraPrefab;

    // Script side variables
    private static GameObject playerInstance = null;
    private static GameObject birdInstance = null;
    private static GameObject hudInstance = null;
    private static GameObject cameraInstance = null;

    // Public acess variables
    public static PlayerController Player { get { return playerInstance.GetComponent<PlayerController>(); } }
    public static CreatureController Creature { get { return birdInstance.GetComponent<CreatureController>(); } }
    public static HUDManager HUD { get { return hudInstance.GetComponent<HUDManager>(); } }
    public static CameraController Camera { get { return cameraInstance.GetComponent<CameraController>(); } }
    public static SoundsManager Sound { get { return manager.GetComponent<SoundsManager>();} }

    public bool AllInstancesExists { get { return playerInstance != null && birdInstance != null && hudInstance != null && cameraInstance; } }

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

        Transform spawn = GameObject.FindWithTag("PlayerSpawn").transform;

        playerInstance.transform.position = spawn.position;
        birdInstance.transform.position = spawn.position;

    }

    void CheckInstances() {

        if(playerInstance == null)
            playerInstance = Instantiate(PlayerPrefab, Vector3.zero, Quaternion.identity);
        
        if(birdInstance == null)
            birdInstance = Instantiate(BirdPrebab);
        
        if(hudInstance == null)
            hudInstance = Instantiate(HUDPrefab);
        
        if(cameraInstance == null)
        {
            cameraInstance = Instantiate(CameraPrefab);
            Camera.LookAt(playerInstance.transform);
        }

    }

}
