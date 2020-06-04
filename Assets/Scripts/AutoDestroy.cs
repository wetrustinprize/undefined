using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    
    [SerializeField]
    private float time = 1f;

    void Start()
    {
        
        GameObject.Destroy(this.gameObject, time);

    }

}
