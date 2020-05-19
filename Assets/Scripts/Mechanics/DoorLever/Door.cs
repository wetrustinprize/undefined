using UnityEngine;


public class Door : MonoBehaviour
{
    
        #region Variables

    [Header("Information")]
    [SerializeField] private bool activated;
    [SerializeField] private BoxCollider2D doorCollider;

    [Header("Positions")]
    [SerializeField] private Transform disabledPos;
    [SerializeField] private Transform enabledPos;

    // script side
    private Color doorColor = Color.gray;


        #endregion
    
    void OnDrawGizmos() {

        if(doorCollider == null) return;
        if(disabledPos == null) return;
        if(enabledPos == null) return;

        Transform nowPos = activated ? enabledPos : disabledPos;
        Transform goPos = activated ? disabledPos : enabledPos;

        Gizmos.color = doorColor;

        Matrix4x4 old = Gizmos.matrix;

        Gizmos.matrix = goPos.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, doorCollider.size);

        Gizmos.matrix = old;
        Gizmos.DrawLine(nowPos.position, goPos.position);

    }

}
