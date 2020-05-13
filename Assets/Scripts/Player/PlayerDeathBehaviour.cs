using UnityEngine;

[RequireComponent(typeof(Alive))]
public class PlayerDeathBehaviour : MonoBehaviour
{
    
        #region Variables

    private Alive myAlive;

        #endregion


    void Start() {

        this.myAlive = this.GetComponent<Alive>();

        myAlive.onDie += Death;

    }

    void Death() {

        

    }

}
