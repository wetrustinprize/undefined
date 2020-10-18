using UnityEngine;
using Undefined.Rooms;

public class CameraController : MonoBehaviour
{
    
        #region Variables
    
    [Header("Camera settings")]
    [SerializeField] private float originalSize = 30;

    [Header("Follow settings")]
    [SerializeField] private GameObject lookAt = null;
    [SerializeField] private float lookVelocity = 1;

    [Header("Boundaries settings:")]
    [SerializeField] private float boundariesTransitionTime = 1f;
    [SerializeField] private AnimationCurve boundariesTransitionCurve;


    // Script-side
    private bool isPlayer { get { return lookAt.GetComponent<PlayerController>(); } }
    private Motor playerMotor { get { return lookAt.GetComponent<Motor>(); } }
    private bool isWalking { get { return playerMotor.GetForce("input").ActualForce.x != 0; } }
    private float dir { get { return playerMotor.LastFacingDir; } }

    private Vector3 newPos = Vector2.zero;

    private Camera thisCam;
    public Camera Camera { get { return thisCam; } }

        #region Lookat

    private float look_Smooth;

        #endregion

        #region Camera Size

    private float size;
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

    private Vector2 oldMinBoundaries;
    private Vector2 oldMaxBoundaries;
    
    private Vector2 curMinBoundaries;
    private Vector2 curMaxBoundaries;

    private Vector2 newMinBoundaries;
    private Vector2 newMaxBoundaries;


    private float boundariesTimer;

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

        // Initial size
        currentSize = originalSize;
        size = currentSize;
        resizeTime = 1;

    }

        #region FixedUpdate

    void FixedUpdate() {

        CameraGoto();
        CameraEffects();
        Boundaries();

        newPos.z = -1;
        transform.position = newPos;
    }

    void CameraEffects() {

        newPos += (Vector3)(push_actDistance + shake_Final);

    }

    void CameraGoto() {

        if(boundariesTimer <= 0)
            newPos = Vector2.Lerp(newPos, (Vector2)lookAt.transform.position, Time.fixedDeltaTime * lookVelocity);

    }

    void Boundaries() {

        newPos.x = Mathf.Clamp(newPos.x, curMinBoundaries.x, curMaxBoundaries.x);
        newPos.y = Mathf.Clamp(newPos.y, curMinBoundaries.y, curMaxBoundaries.y);

    }

        #endregion

    void Update() {

        CameraShakeAnimation();
        CameraPush();
        SizeLerpAnimation();
        BoundariesAnimation();

        thisCam.orthographicSize = currentSize;

    }

    public void SetRoom(Room room)
    {
        // Bounds setup
        vertExtent = thisCam.orthographicSize;
        horzExtent = vertExtent * Screen.width / Screen.height;

        float originX = room.origin.x;
        float originY = room.origin.y;
        float sizeX = room.tilesetSize.x;
        float sizeY = room.tilesetSize.y;

        boundariesTimer = boundariesTransitionTime;

        oldMinBoundaries = curMinBoundaries;
        oldMaxBoundaries = curMaxBoundaries;

        newMinBoundaries.x = originX + horzExtent;
        newMinBoundaries.y = originY + vertExtent;

        newMaxBoundaries.x = originX + sizeX - horzExtent;
        newMaxBoundaries.y = originY + sizeY - vertExtent;

    }

        #region Calculatios

    void OnDrawGizmos() {

        Vector3 center = new Vector3(
            curMaxBoundaries.x - (curMaxBoundaries.x - curMinBoundaries.x) / 2,
            curMaxBoundaries.y - (curMaxBoundaries.y - curMinBoundaries.y) / 2,
            0
        );

        Vector3 size = new Vector3(
            (curMaxBoundaries.x - curMinBoundaries.x),
            (curMaxBoundaries.y - curMinBoundaries.y),
            0
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);

    }

    void BoundariesAnimation() {

        if(boundariesTimer > 0)
        {
            boundariesTimer -= Time.deltaTime;
            
            float boundariesCurveValue = boundariesTransitionCurve.Evaluate(1-boundariesTimer/boundariesTransitionTime);

            curMaxBoundaries = Vector2.Lerp(oldMaxBoundaries, newMaxBoundaries, boundariesCurveValue);
            curMinBoundaries = Vector2.Lerp(oldMinBoundaries, newMinBoundaries, boundariesCurveValue);

        }

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

    }

    void CameraShakeAnimation() {

        if(shake_Timer <= 0) return;

        float x = Mathf.Sin(Time.time * Random.value * shake_Velocity);
        float y = Mathf.Cos(Time.time * Random.value * 2 * shake_Velocity);

        float timePercentage = shake_Timer / shake_MaxTimer;
        float strength = shake_Strength * timePercentage;

        Vector2 shake = new Vector2(x * strength, y * strength);

        shake_Final = Vector2.Lerp(shake_Final, shake_Axis * shake, Time.deltaTime / 0.2f) * (shake_Timer / shake_Timer_Max);
        
        shake_Timer -= Time.deltaTime;

    }

    void SizeLerpAnimation() {

        currentSize = Mathf.Lerp(currentSize, size, Time.deltaTime / resizeTime);

    }

        #endregion

        #region Public Functions

    ///<summary>Changes the transform to the camera look at</summary>
    public void LookAt(Transform newLookAt, float smoothTime = 0f) {
        lookAt = newLookAt.gameObject;
    }

    ///<summary>Focus the camera at the Player</summary>
    public void LookAtPlayer()
    {
        lookAt = GameManager.Player.gameObject;
    }

    ///<summary>Resets the camera size</summary>
    public void ResetSize(float time = -1) {
        Resize(originalSize, time);
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
