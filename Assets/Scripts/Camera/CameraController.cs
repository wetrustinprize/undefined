using UnityEngine;
using Undefined.Rooms;

public class CameraController : MonoBehaviour
{
    
        #region Variables

    public Room room1;


    [Header("Follow settings")]
    [SerializeField] private GameObject lookAt = null;
    [SerializeField] private float size = 30;

    [Header("Look ahead settings:")]
    [SerializeField] private float lookAheadDistance = 7;
    [SerializeField] private float lookAheadStartTimeSpeed = 3;
    [SerializeField] private float lookAheadEndTimeSpeed = 1;
    [SerializeField] private float lookAheadTime = 3;

    [Header("Vertical look ahead settings:")]
    [SerializeField] private float gravityScale = 50;
    [SerializeField] private AnimationCurve gravityScaleCurve = null;

    // Script-side
    private bool isPlayer { get { return lookAt.GetComponent<PlayerController>(); } }
    private Motor playerMotor { get { return lookAt.GetComponent<Motor>(); } }
    private bool isWalking { get { return playerMotor.GetForce("input").ActualForce.x != 0; } }
    private float dir { get { return playerMotor.LastFacingDir; } }

    private Vector3 newPos = Vector2.zero;

    private Camera thisCam;
    public Camera Camera { get { return thisCam; } }

    private Vector2 lookAheadValue;
    private float timeMovingCamera;

        #region Lookat

    private float look_Smooth;

        #endregion

        #region Camera Size

    private float currentSize;
    private float resizeTime;

        #endregion

        #region Camera Shake

    private float shake_Strength;
    private float shake_Velocity;
    private float shake_MaxTimer;
    private float shake_Timer;
    private float shake_Timer_Max;
    private Vector2 shake_Axis;
    private Vector2 shake_Final;

        #endregion

        #region Camera Push

    private Vector2 push_actDistance;
    private Vector2 push_GoToDistance;
    private float push_GoTimer;
    private float push_GoMaxTimer;
    private float push_ReturnTimer;
    private float push_ReturnMaxTimer;

        #endregion

        #region Boundaries

    private float boundariesMinX;
    private float boundariesMaxX;
    private float boundariesMinY;
    private float boundariesMaxY;

    private float horzExtent;
    private float vertExtent;

        #endregion

        #region Static

    public static CameraController main { get { return GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();} }

        #endregion

        #endregion

    void Awake() {
        // Get the camera component
        thisCam = GetComponent<Camera>();
    }

    void Start() {

        // Bounds setup
        vertExtent = thisCam.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;

        // Initial size
        currentSize = size;
        resizeTime = 1;

        SetRoom(room1);

    }

    void Update() {

        if(isPlayer)
            PlayerFollow();
        else if(lookAt != null)
            OtherFollow();

        CameraShake();
        CameraPush();
        SizeLerp();
        Boundaries();

        
        thisCam.orthographicSize = currentSize;

        newPos.z = -1;
        transform.position = newPos;
        

    }

    public void SetRoom(Room room)
    {
        float originX = room.origin.x;
        float originY = room.origin.y;
        float sizeX = room.tilesetSize.x + originX;
        float sizeY = room.tilesetSize.y + originY;

        boundariesMinX = horzExtent + originX / 2.0f;
        boundariesMaxX = sizeX / 2.0f + horzExtent;
        boundariesMinY = vertExtent + originY / 2.0f;
        boundariesMaxY = sizeY / 2.0f + vertExtent;

    }

        #region Calculatios

    void OnDrawGizmos() {

        Vector3 center = new Vector3(
            boundariesMaxX - (boundariesMaxX - boundariesMinX) / 2,
            boundariesMaxY - (boundariesMaxY - boundariesMinY) / 2,
            0
        );

        Vector3 size = new Vector3(
            (boundariesMaxX - boundariesMinX),
            (boundariesMaxY - boundariesMinY),
            0
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

    }

    void Boundaries() {

        newPos.x = Mathf.Clamp(newPos.x, boundariesMinX, boundariesMaxX);
        newPos.y = Mathf.Clamp(newPos.y, boundariesMinY, boundariesMaxY);

    }

    void OtherFollow() {
        
        if(look_Smooth <= 0)
        {
            newPos = lookAt.transform.position;
            return;
        }
        else
        {
            newPos = Vector3.Lerp(this.transform.position, lookAt.transform.position, Time.deltaTime / look_Smooth);
        }

    }

    void PlayerFollow() {
        
        LookAhead();
        CameraGoto();

        timeMovingCamera = isWalking ? (timeMovingCamera + Time.deltaTime) : 0;

    }

    void CameraPush() {

        if(push_GoTimer < push_GoMaxTimer)
        {
            push_actDistance = Vector2.Lerp(push_actDistance, push_GoToDistance, Time.deltaTime / push_GoMaxTimer);
            push_GoTimer += Time.deltaTime;
        }
        else if(push_ReturnTimer < push_ReturnMaxTimer)
        {
            push_actDistance = Vector2.Lerp(push_actDistance, Vector2.zero, Time.deltaTime / push_ReturnMaxTimer);
            push_ReturnTimer += Time.deltaTime;
        }

        newPos += (Vector3)push_actDistance;

    }

    void CameraShake() {

        if(shake_Timer <= 0) return;

        float x = Mathf.Sin(Time.time * Random.value * shake_Velocity);
        float y = Mathf.Cos(Time.time * Random.value * 2 * shake_Velocity);

        float timePercentage = shake_Timer / shake_MaxTimer;
        float strength = shake_Strength * timePercentage;

        Vector2 shake = new Vector2(x * strength, y * strength);

        shake_Final = Vector2.Lerp(shake_Final, shake_Axis * shake, Time.deltaTime / 0.2f) * (shake_Timer / shake_Timer_Max);
        
        shake_Timer -= Time.deltaTime;

        newPos += (Vector3)shake_Final;

    }

    void SizeLerp() {

        currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime / resizeTime);

    }

    void LookAhead() {

        float horizontalLookAhead = 0;
        float verticalLookAhead = 0;
        float time = isWalking ? lookAheadStartTimeSpeed : lookAheadEndTimeSpeed;

        float gravity = playerMotor.finalSpeed.y;

        Vector2 newLookAhead = Vector2.zero;

        if(timeMovingCamera >= lookAheadTime)
        {
            horizontalLookAhead = dir * lookAheadDistance;
        }

        verticalLookAhead -= gravityScaleCurve.Evaluate(-gravity / gravityScale) * lookAheadDistance;

        newLookAhead.x = horizontalLookAhead;
        newLookAhead.y = verticalLookAhead;

        lookAheadValue = Vector2.Lerp(lookAheadValue, newLookAhead, Time.deltaTime / time);

    }

    void CameraGoto() {

        newPos = (Vector2)lookAt.transform.position + lookAheadValue;

    }

        #endregion

        #region Public Functions

    ///<summary>Changes the transform to the camera look at</summary>
    public void LookAt(Transform newLookAt, float smoothTime = 0f) {
        look_Smooth = smoothTime;
        lookAt = newLookAt.gameObject;
    }

    ///<summary>Focus the camera at the Player</summary>
    public void LookAtPlayer()
    {
        lookAt = GameManager.Player.gameObject;
        look_Smooth = 0f;
    }

    ///<summary>Changes the size of the camera</summary>
    public void Resize(float newSize, float time = -1) {
        size = newSize;
        resizeTime = time < 0 ? resizeTime : time;
        
    }

    ///<summary>Makes tha camera shake</summary>
    public void Shake(float velocity, float strength, float time, Vector2 axis) {

        shake_Strength = strength;
        shake_Velocity = velocity;
        shake_MaxTimer = time;
        shake_Timer = time;
        shake_Timer_Max = time;
        shake_Axis = axis;

    }

    ///<summary>Push the camera at a direction</summary>
    public void Push(Vector2 direction, float gotoTime, float returnTime) {

        push_GoToDistance = push_actDistance + direction;
        push_GoMaxTimer = gotoTime;
        push_ReturnMaxTimer = returnTime;

        if(gotoTime == 0)
            push_actDistance += direction;

        push_ReturnTimer = 0;
        push_GoTimer = 0;

    }

    ///<summary>Gets mouse position in 3D space</summary>
    ///<param name="screenPosition">Mouse position</param>
    ///<param name="z">3D space Z</param>
    public Vector3 GetWorldPositionOnPlane(Vector3 screenPosition, float z) {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane xy = new Plane(Vector3.forward, new Vector3(0, 0, z));
        float distance;
        xy.Raycast(ray, out distance);
        return ray.GetPoint(distance);
    }

        #endregion

}
