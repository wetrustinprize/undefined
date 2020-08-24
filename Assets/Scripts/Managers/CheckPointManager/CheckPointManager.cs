using UnityEngine;
using System;
using Undefined.Checkpoints;

public class CheckPointManager : MonoBehaviour {

        #region Variables

    [Header("Last Save Information")]
    [SerializeField] private Save save;

    // Action
    public Action onSave;
    public Action onLoad;

        #endregion

    public void Save() {
        save = new Save(GameManager.Player.transform.position);
        onSave?.Invoke();
    }

    public void Load() {
        GameManager.Entity.ClearAllEntities();
        GameManager.manager.RespawnPlayer(save.PlayerPosition);
        save.Load();
        onLoad?.Invoke();
    }

}