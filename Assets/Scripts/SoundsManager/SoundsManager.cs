using UnityEngine;
using UnityEngine.Audio;

namespace Undefined.Sound {
    public class SoundsManager : MonoBehaviour
    {

            #region Variables

        [SerializeField] private AudioMixer mixer;

        public static SoundsManager instance;

        // Script side
        private AudioSource uiSource;

            #endregion

        void Awake() {

            DontDestroyOnLoad(this.gameObject);

        }

        void Start() {

            // Static instace
            if(instance != null) Destroy(this.gameObject);
            
            instance = this;

            // Gets the UI AudioSource
            uiSource = CameraController.main.GetComponent<AudioSource>();

        }

        ///<summar>Plays a UI SFX</summary>
        ///<param name="sfx">The AudioClip to play</param>
        ///<param name="vol">The SFX volume</param>
        public static void PlayUISFX(AudioClip sfx, float vol = 1.0f) {

            if(sfx == null) return;

            instance.uiSource.PlayOneShot(sfx, vol);

        }

    }

}
