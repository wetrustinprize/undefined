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
        Vector2 vector = new Vector2(m.lastFaceDir * 75, 0);
        m.ApplyForce(new Force("dash", vector, 0.1f, false));
    }
}
