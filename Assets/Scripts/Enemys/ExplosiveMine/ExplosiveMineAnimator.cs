using UnityEngine;

public class ExplosiveMineAnimator : MonoBehaviour
{
    
        #region Variables

    [Header("Sound")]
    [SerializeField] private AudioClip beepSFX;

    // Script-side
    private AudioSource source;

        #endregion

    void Start() {

        source = GetComponent<AudioSource>();

    }

    public void BeepSFX() {
        source.PlayOneShot(beepSFX);
    }

}
