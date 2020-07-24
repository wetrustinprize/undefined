using UnityEngine;
using UnityEngine.SceneManagement;

using Undefined.Checkpoints;

public class CheckPointManager : MonoBehaviour {

        #region Variables

    [Header("Last Save Information")]
    [SerializeField] private Save save;

        #endregion

    public void Save() {
        GameObject player = GameManager.Player.gameObject;
        PlayerController controller = player.GetComponent<PlayerController>();
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        Alive alive = player.GetComponent<Alive>();

        save = new Save(player.transform.position, controller, inventory, alive);
    }

    public void Load() {
        GameManager.Entity.ClearAllEntities();
        GameManager.manager.RespawnPlayer(save.PlayerPosition);
        save.Load();
    }

}