using UnityEngine;

[RequireComponent(typeof(Motor))]

public class PlayerDash : MonoBehaviour
{
    
    Motor m {get {return GetComponent<Motor>();}}

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q)) Dash();
    }

    void Dash() {
        Vector2 vector = new Vector2(12, 0);
        m.AddForce(new Force("dash", vector, 2f, true), false, true, true);
    }
}
