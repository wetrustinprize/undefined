using UnityEngine;
using UnityEngine.UI;

public class HUDGameOver : MonoBehaviour
{

        #region Variables

    [Header("Buttons")]
    [SerializeField] private Button btnRetry;

        #endregion

    void Start() {

        btnRetry.onClick.AddListener(() => {GameManager.Checkpoint.Load();});

    }

}