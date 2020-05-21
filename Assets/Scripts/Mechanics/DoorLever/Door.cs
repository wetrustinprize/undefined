using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    
        #region Variables

    [Header("Information")]
    [SerializeField] private bool activated;
    [SerializeField] private BoxCollider2D doorCollider;
    [SerializeField] private SpriteRenderer doorSprite;

    [Header("Animation")]
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private float animationDuration = 1.0f;

    [Header("Positions")]
    [SerializeField] private Transform deactivatedPos;
    [SerializeField] private Transform activatedPos;

    // script side
    private List<Color> doorColor = new List<Color> { Color.gray };

    private float travelPercentage = 0.0f;


        #endregion
    
    void Awake() {

        doorColor = new List<Color>();

        travelPercentage = 1f;

        doorCollider.transform.position = activated ? activatedPos.position : deactivatedPos.position;
        doorCollider.transform.rotation = activated ? activatedPos.rotation : deactivatedPos.rotation;

    }

    void FixedUpdate() {

        if(activated && travelPercentage < 1f || !activated && travelPercentage > 0f)
        {
            float deltaTime = (Time.fixedDeltaTime / animationDuration) * (activated ? 1 : -1);
            
            travelPercentage = Mathf.Clamp(travelPercentage + deltaTime, 0f, 1f);

            doorCollider.transform.position = Vector3.Lerp(deactivatedPos.position, activatedPos.position, animationCurve.Evaluate(travelPercentage));
            doorCollider.transform.rotation = Quaternion.Lerp(deactivatedPos.rotation, activatedPos.rotation, animationCurve.Evaluate(travelPercentage));
        }

    }

    public void Toggle() {

        activated = !activated;

    }

    // Adds a new color to the door
    public void AddDoorColor(Color newDoorColor)
    {

        // Adds the new color to the list
        doorColor.Add(newDoorColor);

        // Updates the door sprite color
        doorSprite.color = doorColor[0];

    }

    void OnDrawGizmos() {

        if(doorCollider == null) return;
        if(deactivatedPos == null) return;
        if(activatedPos == null) return;

        Gizmos.color = Color.gray;

        Matrix4x4 old = Gizmos.matrix;

        Gizmos.matrix = activatedPos.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, doorCollider.size);

        Gizmos.matrix = deactivatedPos.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, doorCollider.size);

        Gizmos.matrix = old;
        Gizmos.DrawLine(deactivatedPos.position, activatedPos.position);

    }

}
