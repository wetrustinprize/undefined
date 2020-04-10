using System;
using UnityEngine;

/*

    Author: Prize
    Github: @prize34

*/

[RequireComponent(typeof(BaseEnemy))]
public class VisionModule : MonoBehaviour
{
    
        #region Variables

    [Header("Vision:")]
    [SerializeField] private LayerMask raycastLayers;        // Layers to raycast
    [Range(1f, 180f)]
    [SerializeField] private double maxAngle = 45f;          // Maximun angle in degrees

    // Private variables
    private BaseEnemy baseEnemy;                        // This gameobject baseEnemy

    // Events
    public Action OnViewPlayer;                 // Event called once this gameobject have vision of the player
    public Action OnLostViewPlayer;             // Event called once this gameobject lost vision of the player
    private bool hasViewPlayer = false;         // Checks if has already seen the player

        #endregion

    // Called once
    void Start() {

        baseEnemy = GetComponent<BaseEnemy>();                              // Gets the baseEnemy

    }

    public bool CheckIsInVision(GameObject gameObjectToFind = null) {

        if(gameObjectToFind == null) gameObjectToFind = GameObject.FindWithTag("Player");
        if(gameObjectToFind == null) return false;

        Vector2 dir = (gameObjectToFind.transform.position - transform.position).normalized;                  // Gets the direction
        Vector2 fDir = baseEnemy._facingDir;                                                                  // Gets the facing dir

        dir = gameObjectToFind.transform.InverseTransformDirection(dir);                                      // Fixes the direction
        
        float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);                                  // Gets the angle
        float extraAngle = Mathf.Atan2(fDir.y, fDir.x) * Mathf.Rad2Deg;                             // Gets the facing dir angle

        float finalAngle = Mathf.Abs(angle - extraAngle);                                           // Calculates the final angle

        return finalAngle < maxAngle / 2;

    }

    // Draws the vision gizmos
    void OnDrawGizmos() {

        if(baseEnemy == null) return;

        Vector2 fDir = baseEnemy._facingDir;                                                                // Gets the facing dir

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
