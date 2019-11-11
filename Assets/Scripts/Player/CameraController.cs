using UnityEngine;
using Undefined.Motor;

public class CameraController : MonoBehaviour
{
    
        #region Variables


    [Header("Follow settings")]
    public GameObject lookAt;
    public float distance;

    [Header("Look ahead settings:")]
    public float lookAheadDistance;
    public float lookAheadStartTimeSpeed;
    public float lookAheadEndTimeSpeed;
    public float lookAheadTime;

    [Header("Vertical look ahead settings:")]
    public float gravityScale;
    public AnimationCurve gravityScaleCurve;

    // Script-side
    private bool isPlayer { get { return lookAt.GetComponent<PlayerController>(); } }
    private Motor playerMotor { get { return lookAt.GetComponent<Motor>(); } }
    private bool isWalking { get { return playerMotor.GetForce("input").ActualForce.x != 0; } }
    private float dir { get { return playerMotor.lastFaceDir; } }

    private Vector3 newPos = Vector2.zero;
    private Vector2 actPos;

    private Vector2 lookAheadValue;
    private float timeMovingCamera;

        #region Camera Shake

    private float shake_Strength;
    private float shake_Velocity;
    private float shake_MaxTimer;
    private float shake_Timer;
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

        #endregion


    void Update() {

        if(isPlayer)
            PlayerFollow();

        CameraShake();
        CameraPush();

        transform.position = newPos;

    }

        #region Calculator

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

        float x = Mathf.Sin(Time.time * shake_Velocity);
        float y = Mathf.Cos(Time.time * 2 * shake_Velocity);

        float timePercentage = shake_Timer / shake_MaxTimer;
        float strength = shake_Strength * timePercentage;

        Vector2 shake = new Vector2(x * strength, y * strength);

        shake_Final = Vector2.Lerp(shake_Final, shake_Axis * shake, Time.deltaTime / 0.2f);
        
        shake_Timer -= Time.deltaTime;

        newPos += (Vector3)shake_Final;

    }

    void LookAhead() {

        float horizontalLookAhead = 0;
        float verticalLookAhead = 0;
        float time = isWalking ? lookAheadStartTimeSpeed : lookAheadEndTimeSpeed;

        float gravity = playerMotor.GetForce("grav", true).ActualForce.y;

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

        actPos = (Vector2)lookAt.transform.position;

        newPos = actPos + lookAheadValue;
        newPos.z = distance;

    }

        #endregion

    ///<summary>Makes tha camera shake</summary>
    public void Shake(float velocity, float strength, float time, Vector2 axis) {

        shake_Strength = strength;
        shake_Velocity = velocity;
        shake_Timer = time;
        shake_MaxTimer = time;
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

}
