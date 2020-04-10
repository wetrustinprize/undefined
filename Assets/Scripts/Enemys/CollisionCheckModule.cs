using UnityEngine;

[RequireComponent(typeof(Motor))]
[RequireComponent(typeof(BaseEnemy))]
public class CollisionCheckModule : MonoBehaviour
{
    
        #region Variables

    private Motor myMotor;
    private Collider2D myCollider;
    private BaseEnemy myBaseEnemy;

        #endregion

    void Start() {

        myMotor = GetComponent<Motor>();
        myCollider = GetComponent<Collider2D>();
        myBaseEnemy = GetComponent<BaseEnemy>();


    }

    public bool WillFall() {

        Vector2 pos = myMotor.GroundColliderPosition + (Vector2)transform.position;
        pos.x += myMotor.GroundCellingColliderSize.x * myBaseEnemy._facingDir.x;

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
