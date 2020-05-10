using UnityEngine;
using UnityEngine.UI;

public class HUDHPBar : MonoBehaviour
{
        #region Variables

    [Header("Bars")]
    [SerializeField] private Image[] hpBars;   // ALl the HP bars
    [SerializeField] private Animator hpAnimator;

    [Header("Bars Texture")]
    [SerializeField] private Sprite hpFull;
    [SerializeField] private Sprite hpHalfBroken;
    [SerializeField] private Sprite hpBroken;
    [SerializeField] private float thresholdToBroken;

    [Header("Particles")]
    [SerializeField] private GameObject hpBreakParticle;

    // Local variables
    private Alive playerAliveComponent;             // The player Alive component
    private int lastExplodedBar;

        #endregion
    void Start()
    {
        lastExplodedBar = 999;

        playerAliveComponent = GameObject.FindWithTag("Player").GetComponent<Alive>();

        // Event to run when the player gets damaged.
        playerAliveComponent.onDamage += cb => {OnDamage();};

        playerAliveComponent.onHeal += cb => {OnHeal();};

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
