using UnityEngine;

[RequireComponent(typeof(Motor))]
public abstract class BaseEnemy : MonoBehaviour {

    public Vector2 _facingDir = new Vector2(1, 0);

}