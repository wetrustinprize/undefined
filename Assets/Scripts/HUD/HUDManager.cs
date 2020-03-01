using UnityEngine;
using UnityEngine.UI;

public enum HUDType {

    Inventory,
    Shop,


}

public class HUDManager : MonoBehaviour
{
    
        #region Variables

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup hudInventory;
    [SerializeField] private CanvasGroup hudShop;

    // Instance
    public static HUDManager instance;

    // Script side variables
    private PlayerInput inputs;

    private HUDInventory inventory;
    private PlayerController player;

        #endregion

    void Awake() {

        DontDestroyOnLoad(this.gameObject);
        inputs = new PlayerInput();

    }

    void OnEnable() {

        inputs.Enable();

    }

    void OnDisable() {

        inputs.Disable();

    }

    void Start() {

        // Instance
        if(instance != null) Destroy(this.gameObject);
        instance = this;

        // Other components
        inventory = hudInventory.GetComponent<HUDInventory>();

        // Player
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Inputs
        inputs.UI.InventoryButton.performed += cb => {
            ToggleHUD(HUDType.Inventory);
        };

        // Hide initial huds
        UpdateHUD(HUDType.Shop, true);
        UpdateHUD(HUDType.Inventory, true);

    }

    ///<summary>Updates a specific HUD</summary>
    ///<param name="hide">Should the HUD be hidden or shown?</param>
    public void UpdateHUD(HUDType type, bool hide) {

        switch(type) {

            case HUDType.Inventory:

                hudInventory.interactable = !hide;
                hudInventory.blocksRaycasts = !hide;
                hudInventory.alpha = hide ? 0 : 1;

                player.receiveInput = hide;

                if(hide) inventory.ShowSelectedItem(null, -1);

                break;
            
            case HUDType.Shop:

                hudShop.interactable = !hide;
                hudShop.blocksRaycasts = !hide;
                hudShop.alpha = hide ? 0 : 1;

                break;
                    

        }

    }

    ///<summary>Toggles a specific HUD</summary>
    public void ToggleHUD(HUDType type) {

        switch(type) {
            case HUDType.Inventory:
                UpdateHUD(type, hudInventory.interactable);
                break;
        }

    }

}
