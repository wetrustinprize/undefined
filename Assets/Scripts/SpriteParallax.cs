using UnityEngine;

public class SpriteParallax : MonoBehaviour
{
    
        #region Variables

    [Header("Parallax config")]
    [SerializeField] private Vector2 effect;
    [SerializeField] private Transform cam;

    private Vector2 startPos;

        #endregion

    void Start() {
        startPos = (Vector2)transform.position;
    }

    void LateUpdate() {

        Vector2 pos = cam.transform.position * -effect;

        transform.position = new Vector2(
            startPos.x + pos.x,
            startPos.y + pos.y
        );

    }

}
