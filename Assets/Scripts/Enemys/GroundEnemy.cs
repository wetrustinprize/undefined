using UnityEngine;
using Undefined.AI;

[RequireComponent(typeof(Motor))]
public class GroundEnemy : MonoBehaviour
{

    Motor motor { get { return GetComponent<Motor>(); } }

    public GameObject test;

    Vector2 direction;

    void Start() {

        direction = new Vector2(1, 0);

    }

    void Update() {

        bool flip = AIDetection.Edge(direction, motor, this.gameObject);

        if(flip)
            direction.x *= -1;
        else
            return;
        
        motor.ReceiveInput(direction);


    }

    void OnDrawGizmos() {

        AIDetection.Debug.Edge(direction, motor, this.gameObject);
        AIDetection.Debug.Vision(this.gameObject, Mathf.Infinity, test, true, 0.5f);

    }

}
