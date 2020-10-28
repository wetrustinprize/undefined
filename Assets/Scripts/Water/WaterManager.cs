using UnityEngine;

public class WaterManager : MonoBehaviour
{

        #region Variables

    [Header("Reflection Camera")]
    [SerializeField] private Camera waterReflectionCamera = null;

    [Header("Reflection Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer = null;

    public static RenderTexture waterReflection;
    public static WaterManager instance;


        #endregion


    void LateUpdate() {

        if(waterReflectionCamera == null) return;
        if(GameManager.Camera == null) return;
 
        waterReflectionCamera.backgroundColor = GameManager.Camera.Camera.backgroundColor;
        waterReflectionCamera.orthographicSize = GameManager.Camera.Camera.orthographicSize;

        Vector3 Distance = this.transform.position - GameManager.Camera.transform.position;

        Vector3 ThisPos = this.transform.position;
        float MainCamZ = GameManager.Camera.transform.position.z;
        Vector3 NewCamPos = new Vector3(ThisPos.x - Distance.x, ThisPos.y + Distance.y, MainCamZ);
        waterReflectionCamera.transform.position = NewCamPos;

    }

    void Start() {

        if(instance == null)
        {
            instance = this;

            waterReflection = new RenderTexture(Screen.width, Screen.height, 0);
            waterReflection.filterMode = FilterMode.Point;

            waterReflectionCamera.targetTexture = waterReflection;
            spriteRenderer.material.SetTexture("_Reflection", waterReflection);
        }
        else
        {
            spriteRenderer.material.SetTexture("_Reflection", waterReflection);
            Destroy(waterReflectionCamera.gameObject);
        }


    }

}
