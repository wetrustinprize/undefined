using UnityEngine;

[RequireComponent(typeof(Motor))]
public class CollisionCheckModule : MonoBehaviour
{
    
        #region Variables

    private Motor myMotor;
    private Collider2D myCollider;

        #endregion

    void Start() {

        myMotor = GetComponent<Motor>();
        myCollider = GetComponent<Collider2D>();


    }

    public bool WillFall(float side) {

        Vector2 pos = myMotor.GroundColliderPosition + (Vector2)transform.position;
        pos.x += myMotor.GroundCellingColliderSize.x * side;

        foreach(Collider2D c in Physics2D.OverlapBoxAll(pos, myMotor.GroundCellingColliderSize, 0, myMotor.CollisionLayer))
        {
            if(c == myCollider) continue;
            else
            {
                return false;
            }
        }

        return true;

    }

}
