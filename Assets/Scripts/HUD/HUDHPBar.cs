using UnityEngine;
using UnityEngine.UI;

public class HUDHPBar : MonoBehaviour
{
        #region Variables

    [Header("Bars")]
    [SerializeField] private Image[] hpBars = null;   // ALl the HP bars
    [SerializeField] private Animator hpAnimator = null;

    [Header("Bars Texture")]
    [SerializeField] private Sprite hpFull = null;
    [SerializeField] private Sprite hpHalfBroken = null;
    [SerializeField] private Sprite hpBroken = null;
    [SerializeField] private float thresholdToBroken = 0.25f;

    [Header("Particles")]
    [SerializeField] private GameObject hpBreakParticle = null;

    // Local variables
    private Alive playerAliveComponent;             // The player Alive component
    private int lastExplodedBar;

        #endregion
    void Start()
    {
        lastExplodedBar = 999;

        playerAliveComponent = GameManager.Player.alive;

        // Event to run when the player gets damaged.
        playerAliveComponent.onDamage += (dmg, dealer) => {OnDamage();};

        playerAliveComponent.onHeal += (dmg, healer) => {OnHeal();};

    }

    void UpdateSprites() {
        int curHealth = playerAliveComponent.Health;
        int maxHealth = playerAliveComponent.MaxHealth;

        float percentage = Mathf.Clamp((float)curHealth / (float)maxHealth, 0, 1);

        for(int i = 0; i < hpBars.Length; i++) {

            float myPercentage = (1/(float)hpBars.Length) * (i + 1);


            if(myPercentage > percentage + thresholdToBroken) {
                if(i < lastExplodedBar)
                    lastExplodedBar = i;
                
                hpBars[i].sprite = hpBroken;
            }
            else if(myPercentage > percentage + thresholdToBroken/2) {
                hpBars[i].sprite = hpHalfBroken;
            }
            else
            {
                hpBars[i].sprite = hpFull;
            }

        }
    }

    void OnHeal() {

        UpdateSprites();
        lastExplodedBar = 999;

    }

    void OnDamage() {

        int curHealth = playerAliveComponent.Health;
        int maxHealth = playerAliveComponent.MaxHealth;

        int lastExplodedBarReminder = lastExplodedBar;

        float percentage = Mathf.Clamp((float)curHealth / (float)maxHealth, 0, 1);

        UpdateSprites();

        if(lastExplodedBarReminder != lastExplodedBar)
        {
            Instantiate(hpBreakParticle, hpBars[lastExplodedBar].transform);
            hpAnimator.SetTrigger("DamagedBreak");
        }
        else if(percentage < 0.5f)
        {
            hpAnimator.SetTrigger("DamagedMedium");
        }
        else
        {
            hpAnimator.SetTrigger("Damaged");
        }

        hpAnimator.SetFloat("DamagedPercentage", 1 - percentage);

    }
}
