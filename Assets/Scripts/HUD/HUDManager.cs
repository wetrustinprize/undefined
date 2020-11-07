using UnityEngine;
using Undefined.Items.Shop;

public enum HUDType {

    Inventory,
    Shop,
    Health,
    Items,
    GameOver,


}

public class HUDManager : MonoBehaviour
{
    
        #region Variables

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup hudInventory = null;
    [SerializeField] private CanvasGroup hudShop = null;
    [SerializeField] private CanvasGroup hudHealth = null;
    [SerializeField] private CanvasGroup hudItems = null;
    [SerializeField] private CanvasGroup hudGameOver = null;

    [Header("Able")]
    public bool canOpenInventory = true;

    // Script side variables
    private PlayerInput inputs;
    private PlayerController player;

    // References
    public HUDInventory Inventory { get; private set; }
    public HUDShop Shop { get; private set; }

    public bool InventoryOpen { get { return hudInventory.interactable; } }
    public bool ShopOpen { get { return hudShop.interactable; } }

        #endregion

    void Awake() {

        inputs = new PlayerInput();

    }

    void OnEnable() {

        inputs.Enable();

    }

    void OnDisable() {

        inputs.Disable();

    }

    void Start() {

        // Other components
        Inventory = hudInventory.GetComponent<HUDInventory>();
        Shop = hudShop.GetComponent<HUDShop>();

        // Player
        player = GameManager.Player.GetComponent<PlayerController>();

        // Inputs
        inputs.UI.InventoryButton.performed += cb => {
            if(ShopOpen) return;

            if(canOpenInventory) {
                ToggleHUD(HUDType.Inventory);
            }
        };

        inputs.UI.Cancel.performed += cb => {
            CancelBehaviour();
        };

        // Hide initial huds
        UpdateHUD(HUDType.Shop, true);
        UpdateHUD(HUDType.Inventory, true);
        UpdateHUD(HUDType.GameOver, true);

        // Show initial huds
        UpdateHUD(HUDType.Health, false);
        UpdateHUD(HUDType.Items, false);

    }

    void CancelBehaviour() {

        if(InventoryOpen) { UpdateHUD(HUDType.Inventory, true); return; }
        if(ShopOpen) { HideShop(); return; }

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

                if(!hide) Inventory.RefreshBackpack();
                if(hide) Inventory.ShowSelectedItem(null, -1);

                break;
            
            case HUDType.Shop:

                hudShop.interactable = !hide;
                hudShop.blocksRaycasts = !hide;
                hudShop.alpha = hide ? 0 : 1;

                player.receiveInput = hide;

                break;
            
            case HUDType.Health:
                hudHealth.alpha = hide ? 0 : 1;
                break;

            case HUDType.Items:
                hudItems.alpha = hide ? 0 : 1;
                break;

            case HUDType.GameOver:

                hudGameOver.interactable = !hide;
                if(!hide) hudGameOver.GetComponent<Animator>().SetTrigger("Fade");
                
                break;
                    

        }

    }

    public void UpdateAllHUD(bool u) {
        UpdateHUD(HUDType.Inventory, u);
        UpdateHUD(HUDType.Shop, u);
        UpdateHUD(HUDType.Items, u);
        UpdateHUD(HUDType.Health, u);
    }

    public void ShowAllHUD() {
        UpdateAllHUD(false);
    }

    public void HideAllHUD() {
        UpdateAllHUD(true);
    }   

    public void ToggleInventory() {
        ToggleHUD(HUDType.Inventory);
    }

    public void ShowShop(ShopDetails shopDetails) {
        if(InventoryOpen) return;

        Shop.Setup(shopDetails);
        UpdateHUD(HUDType.Shop, false);
    }

    public void HideShop() {
        UpdateHUD(HUDType.Shop, true);
    }
    
    void ToggleHUD(HUDType type) {

        switch(type) {
            case HUDType.Inventory:
                UpdateHUD(type, hudInventory.interactable);
                break;
        }

    }

}
