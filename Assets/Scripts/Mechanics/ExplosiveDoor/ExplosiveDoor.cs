using UnityEngine;

[RequireComponent(typeof(Alive))]
public class ExplosiveDoor : MonoBehaviour
{
    
        #region Variables

    // Script side variables
    private Alive aliveModule;

        #endregion

    void Awake() {

        aliveModule = GetComponent<Alive>();

    }

    void Start() {

        aliveModule.onDamage += CheckIsExplosion;

    }

    void CheckIsExplosion(int damage, GameObject dealer) {
        if(dealer.tag == "PlayerExplosion")
        {
            Destroy(this.gameObject);
        }

    }

}
