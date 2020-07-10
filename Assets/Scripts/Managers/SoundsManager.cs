using UnityEngine;
using UnityEngine.Audio;

public class SoundsManager : MonoBehaviour
{

        #region Variables

    [SerializeField] private AudioMixer mixer;

    // Script side
    private AudioSource uiSource;

        #endregion

    void Awake() {

        DontDestroyOnLoad(this.gameObject);

    }

    void Start() {

        // Gets the UI AudioSource
        uiSource = CameraController.main.GetComponent<AudioSource>();

    }

    ///<summar>Plays a UI SFX</summary>
    ///<param name="sfx">The AudioClip to play</param>
    ///<param name="vol">The SFX volume</param>
    public void PlayUISFX(AudioClip sfx, float vol = 1.0f) {

        if(sfx == null) return;

        this.uiSource.PlayOneShot(sfx, vol);

    }

}
