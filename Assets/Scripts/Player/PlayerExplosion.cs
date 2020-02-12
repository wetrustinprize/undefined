using UnityEngine;

public class PlayerExplosion : MonoBehaviour {

        #region Variables

    [SerializeField] private GameObject explosion_Prefab;

    // Local variables
    private CreatureController creature;

        #endregion

    void Start() {
    
        // Gets the creature
        creature = GameObject.FindWithTag("Creature").GetComponent<CreatureController>();

    }

    public void Execute() {

        if(creature.State != CreatureState.Following) return;

        Instantiate(explosion_Prefab, creature.transform.position, creature.transform.rotation);
        CameraController.main.Shake(20, 15, 0.3f, Vector2.one);

    }

}