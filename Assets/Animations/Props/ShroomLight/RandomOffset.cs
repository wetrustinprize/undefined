using UnityEngine;

public class RandomOffset : MonoBehaviour
{

    void Start()
    {
        
        Animator anim = GetComponent<Animator>();
        anim.SetFloat("Offset", Random.Range(0f, 1f));

    }


}
