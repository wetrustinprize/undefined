using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FootstepsManager : MonoBehaviour {

        #region Variables

    [Header("Sounds")]
    [SerializeField] private AudioClip[] footsteps = null;

    // Script side
    private AudioSource source;

        #endregion

    void Awake() {
        source = GetComponent<AudioSource>();
    }

    public void Footstep() {

        source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);

    }

}