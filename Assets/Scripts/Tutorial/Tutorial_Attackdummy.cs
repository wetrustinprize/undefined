using UnityEngine;

[RequireComponent(typeof(Alive))]
public class Tutorial_Attackdummy : MonoBehaviour
{
    
        #region Variables

    [Header("Gameobjects")]
    [SerializeField] private Transform dummyBody;

    // Script side
    private Alive myAlive;
    private Transform playerTransform;

        #endregion

    void Start() {
        this.myAlive = this.GetComponent<Alive>();
        this.playerTransform = GameManager.Player.gameObject.transform;

        this.myAlive.onDamage += cb => { Attacked(); };
    }

    void Attacked() {

        this.myAlive.Heal(1);

        Vector2 dir = (this.transform.position - this.playerTransform.position).normalized;
        Vector3 punch = new Vector3(0, 0, -dir.x * Random.Range(45, 90));

        this.dummyBody.Rotate(punch, Space.Self);

    }

    void Update() {

        this.dummyBody.rotation = Quaternion.Lerp(this.dummyBody.rotation, Quaternion.identity, Time.deltaTime / 0.05f);

    }

}
