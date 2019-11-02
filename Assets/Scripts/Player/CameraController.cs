using UnityEngine;
using Untitled.Motor;

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

    private Vector3 newPos;
    private Vector2 actPos;

    private Vector2 lookAheadValue;
    private float timeMovingCamera;

        #endregion


    void Update() {

        if(isPlayer)
            PlayerFollow();

        transform.position = newPos;

    }

    void PlayerFollow() {
        
        LookAhead();
        CameraGoto();

        timeMovingCamera = isWalking ? (timeMovingCamera + Time.deltaTime) : 0;

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

}
