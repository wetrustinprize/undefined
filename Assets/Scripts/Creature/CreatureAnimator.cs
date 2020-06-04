using UnityEngine;

public class CreatureAnimator : MonoBehaviour
{

        #region Variables

    [SerializeField] private CreatureController creatureController = null;

    // Script side
    private SpriteRenderer spriteRenderer;
    private Animator animator;

        #endregion



        #region Animation Voids

    public void CreateExplosion() {

        creatureController.CreateExplosion();

    }

        #endregion

    void Start() {

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        creatureController.OnExplode += () => { animator.SetTrigger("Explosion"); };

    }

    void Update()
    {
        
        spriteRenderer.flipX = creatureController.LastFacingDir != 1;
        animator.SetBool("Flying", !creatureController.IsNearToThePlayer);

    }
}
