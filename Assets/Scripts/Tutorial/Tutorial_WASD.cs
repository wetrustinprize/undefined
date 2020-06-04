using UnityEngine;

public class Tutorial_WASD : MonoBehaviour
{
    
        #region Variables

    [Header("Sprites Gameobjects")]
    [SerializeField] private SpriteRenderer w_sprite = null;
    [SerializeField] private SpriteRenderer a_sprite = null;
    [SerializeField] private SpriteRenderer s_sprite = null;
    [SerializeField] private SpriteRenderer d_sprite = null;

    [Header("Sprites")]
    [SerializeField] private Sprite[] w_sprites = new Sprite[2];
    [SerializeField] private Sprite[] a_sprites = new Sprite[2];
    [SerializeField] private Sprite[] s_sprites = new Sprite[2];
    [SerializeField] private Sprite[] d_sprites = new Sprite[2];

    // Script side variables
    private PlayerInput inputs;

        #endregion
    void Awake() {

        this.inputs = new PlayerInput();

    }

    void OnEnable() {
        this.inputs.Enable();
    }

    void OnDisable() {
        this.inputs.Disable();
    }
    
    void Start() {
        this.inputs.Player.Move.performed += cb => { UpdateSprites(cb.ReadValue<Vector2>()); };
        this.inputs.Player.Move.canceled += cb => { UpdateSprites(Vector2.zero); };
    }

    void UpdateSprites(Vector2 dir) {
        this.w_sprite.sprite = this.w_sprites[dir.y > 0 ? 1 : 0];
        this.s_sprite.sprite = this.s_sprites[dir.y < 0 ? 1 : 0];

        this.a_sprite.sprite = this.a_sprites[dir.x < 0 ? 1 : 0];
        this.d_sprite.sprite = this.d_sprites[dir.x > 0 ? 1 : 0];
    }

}
