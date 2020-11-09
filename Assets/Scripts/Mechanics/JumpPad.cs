using UnityEngine;
using Undefined.Force;

public class JumpPad : MonoBehaviour
{
    
        #region

    [SerializeField] private ForceTemplate forceToApply = null;
    [SerializeField] private bool useAutoTime = true;

    [Space()]
    [SerializeField] private Animator jumppad_animator = null;

        #endregion

    void ApplyForce(Collider2D col) {

        if(col.gameObject.TryGetComponent<Motor>(out Motor motor)) {
            
            if(useAutoTime)
                forceToApply.TimeToStop = 0.3f * (forceToApply.ForceToApply.y / 15f);

            motor.RemoveForce("jump");
            motor.AddForce((Force)forceToApply, false, true, true, true);
            jumppad_animator.SetTrigger("Push");

        }

    }

    void OnTriggerStay2D(Collider2D col) {

        ApplyForce(col);

    }

    void OnDrawGizmos() {

        if(forceToApply == null) return;

        Gizmos.DrawLine(this.transform.position, this.transform.position + (Vector3)forceToApply.ForceToApply);

    }

}
