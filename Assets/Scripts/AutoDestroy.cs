using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    
    [SerializeField]
    private float time;

    void Start()
    {
        
        GameObject.Destroy(this.gameObject, time);

    }

}
