using UnityEngine;
using Undefined.Force;

public class JumpPad : MonoBehaviour
{
    
        #region

    [SerializeField] private Vector2 forceToApply = Vector2.zero;
    [SerializeField] private float duration = 1f;

        #endregion


    void ApplyForce(Collider2D col) {

        if(col.gameObject.TryGetComponent<Motor>(out Motor motor)) {

            Force force = new Force($"jumppad_{this.gameObject.name}",
                                    forceToApply,
                                    duration,
                                    CollisionStopBehaviour.Ground | CollisionStopBehaviour.Celling);
            
            motor.RemoveForce("jump");
            motor.AddForce(force, false, true, true, true);

        }

    }

    void OnTriggerStay2D(Collider2D col) {

        ApplyForce(col);

    }

}
