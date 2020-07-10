using UnityEngine;
using Undefined.Force;

public class GoldDropModule : MonoBehaviour {

        #region Variables

    [Header("Prefabs")]
    [SerializeField] private GameObject coinPrefab = null;

    [Header("Coin Force")]
    [SerializeField] private ForceTemplate spawnForce = null;

    [Header("Value")]
    public int coinValue;

        #endregion

    public void Spawn() {

        GameObject coin = Instantiate(coinPrefab, transform.position, transform.rotation);
        coin.GetComponent<CoinBehaviour>().value = coinValue;
        coin.GetComponent<Motor>().AddForce((Force)spawnForce);

    }

}