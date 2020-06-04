using UnityEngine;

public class PlayerExplosion : MonoBehaviour {

        #region Variables

    [Header("Informatio")]
    [SerializeField] private float maxExplosionDistance = 20f;

    [Header("Visuals")]
    [SerializeField] private GameObject explosionRadiusVisual = null;
    [SerializeField] private SpriteRenderer[] explosionRadiusSprites = null;

    [Header("Prefabs")]
    public GameObject explosion_Prefab = null;

    // Local variables
    private CreatureController creature;
    private bool changedSpriteColors;
    private Vector2 explosionPosition;

    // Public acess
    public bool IsShowingVisual { get; private set; } = false;
    public bool CanExplode { get; private set; } = false;

        #endregion

    void Start() {
    
        // Gets the creature
        creature = GameManager.Creature;
        StopExplosionVisual();

    }

    void LateUpdate() {

        if(IsShowingVisual)
        {
            explosionPosition = explosionRadiusVisual.transform.position = GameManager.Camera.GetWorldPositionOnPlane(GameManager.Player.lastMousePosition, 0);

            if(changedSpriteColors != CanExplode)
            {
                changedSpriteColors = CanExplode;
                foreach(SpriteRenderer sr in explosionRadiusSprites)
                {
                    sr.color = CanExplode ? new Color(1,1,1,0.25f) : Color.red;
                }
            }
        }

    }

    void FixedUpdate() {

        float distance = Vector2.Distance(this.transform.position, explosionRadiusVisual.transform.position);
        
        CanExplode = distance <= maxExplosionDistance;

    }

    public void Execute() {

        if(!CanExplode) return;

        creature.Explode(explosionPosition);

    }

    public void StartExplosionVisual() {
        if(creature.IsBusy) return;

        explosionRadiusVisual.SetActive(true);
        IsShowingVisual = true;
    }

    public void StopExplosionVisual() {
        CanExplode = false;
        explosionRadiusVisual.SetActive(false);
        IsShowingVisual = false;
    }

}