using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Motor))]
public abstract class BaseAgent : MonoBehaviour {

        #region Variables
    

    #region Pathfinding Variables

    [Header("Base Agent Settings")]
    public float waypointMinDistance = 0.3f;

    protected Seeker mySeeker;
    protected Path myPath;
    protected int curWaypoint;
    protected Motor myMotor;

    public Vector2 DirToPath { 
        get
        {
            if(myPath == null || curWaypoint >= myPath.vectorPath.Count)
                return Vector2.zero;

            return (PathPos - (Vector2)this.transform.position).normalized; 
        } 
    }

    public bool ReachedPathEnd {
        get
        {
            if(myPath == null) return true;

            return curWaypoint >= myPath.vectorPath.Count;
        }
    }

    public Vector2 PathPos {
        get
        {
            if(myPath == null || curWaypoint >= myPath.vectorPath.Count)
                return Vector2.zero;

            return (Vector2)myPath.vectorPath[curWaypoint];
        }
    }

    #endregion

        #endregion

    protected virtual void Awake() {

        this.mySeeker = this.GetComponent<Seeker>();
        this.myMotor = this.GetComponent<Motor>();

    }

        #region Pathfinding Methods

    protected void CalculatePath(Vector3 pos) {

        if(!this.mySeeker.IsDone()) return;

        mySeeker.StartPath(this.transform.position, pos, cb =>{
            if(!cb.error)
            {
                curWaypoint = 1;
                myPath = cb;
            }
        });

    }

    protected void CheckWaypointDistance() {

        if(myPath == null) return;
        if(curWaypoint >= myPath.vectorPath.Count) return;

        float distance = Vector2.Distance(this.transform.position, myPath.vectorPath[curWaypoint]);

        if(distance < waypointMinDistance) curWaypoint++;

    }

        #endregion

}