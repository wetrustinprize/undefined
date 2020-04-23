using System;
using UnityEngine;

public class VisionModule : MonoBehaviour
{
    
        #region Variables

    [Header("Vision:")]
    [SerializeField] private Vector2 facingDir = new Vector2(1,0);
    [SerializeField] private LayerMask raycastLayers;        // Layers to raycast
    
    [Range(1f, 180f)]
    [SerializeField] private double maxAngle = 45f;          // Maximun angle in degrees

    public Vector2 FacingDir { get { return facingDir; } }

        #endregion


    public void SetFacingDir(Vector2 newFacingDir) {

        newFacingDir = newFacingDir.normalized;

        if(newFacingDir == Vector2.zero) return;

        facingDir = newFacingDir;

    }

    public bool CheckIsInVision(GameObject gameObjectToFind = null) {

        if(gameObjectToFind == null) gameObjectToFind = GameObject.FindWithTag("Player");
        if(gameObjectToFind == null) return false;

        Vector2 dir = (gameObjectToFind.transform.position - transform.position).normalized;                  // Gets the direction
        Vector2 fDir = facingDir;                                                                  // Gets the facing dir

        dir = gameObjectToFind.transform.InverseTransformDirection(dir);                                      // Fixes the direction
        
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);                                  // Gets the angle
        float extraAngle = Mathf.Atan2(fDir.y, fDir.x) * Mathf.Rad2Deg;                             // Gets the facing dir angle

        float finalAngle = Mathf.Abs(angle - extraAngle);                                           // Calculates the final angle

        return finalAngle < maxAngle / 2;

    }

    public bool RaycastVision(bool checkVision, GameObject gameObjectToFind = null, float maxDist = Mathf.Infinity) {
        if(gameObjectToFind == null) gameObjectToFind = GameObject.FindWithTag("Player");

        if(gameObjectToFind == null) return false;
        if(!CheckIsInVision(gameObjectToFind) && checkVision) return false;

        Vector2 dir = (gameObjectToFind.transform.position - transform.position).normalized;                          // Calculates the direction

        RaycastHit2D[] hits2D = Physics2D.RaycastAll(transform.position, dir, maxDist, raycastLayers);     //  Raycast

        foreach(RaycastHit2D hit in hits2D) {
            if(hit.collider.gameObject == this.gameObject) continue;
            return hit.collider.gameObject == gameObjectToFind;
        }

        return false;

    }

    // Draws the vision gizmos
    void OnDrawGizmos() {

        Vector2 fDir = facingDir;                                                        // Gets the facing dir

        float angleDraw = ((float)maxAngle / 2) * Mathf.Deg2Rad;                                            // Calculates the angle
        float extraAngleDraw = Mathf.Atan2(fDir.y, fDir.x);                                                 // Calculates the facing dir angle

        float finalAngleDrawUp = angleDraw - extraAngleDraw;                                                // Calculates the lines
        float finalAngleDrawDown = angleDraw + extraAngleDraw;
        
        Vector3 raydirUp = new Vector3(Mathf.Cos(finalAngleDrawUp), -Mathf.Sin(finalAngleDrawUp));          // Creates the vectors
        Vector3 raydirDown = new Vector3(Mathf.Cos(finalAngleDrawDown), Mathf.Sin(finalAngleDrawDown));

        Gizmos.DrawRay(transform.position, raydirUp * 3);                                                   // Draw rays
        Gizmos.DrawRay(transform.position, raydirDown * 3);

    }

}
