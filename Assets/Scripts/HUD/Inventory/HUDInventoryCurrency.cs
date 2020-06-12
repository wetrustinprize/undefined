using UnityEngine;

using TMPro;

public class HUDInventoryCurrency : MonoBehaviour
{

        #region Variables

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI myGold = null;
    [SerializeField] private TextMeshProUGUI mySecret = null;

        #endregion

    public void Setup(int gold, int secret)
    {
        myGold.text = gold.ToString();
        mySecret.text = secret.ToString();
    }

}
