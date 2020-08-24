using UnityEngine;
using UnityEngine.UI;

public class HUDDashCooldown : MonoBehaviour
{

        #region Variable

    [Header("Components")]
    [SerializeField] private Image coolDownDenied = null;

    // Script side
    private float timer = 0;
    private float timeToStop = 0;
    private CanvasGroup myGroup = null;

        #endregion

    void Start() {

        myGroup = GetComponent<CanvasGroup>();
        myGroup.alpha = 0;

        GameManager.Player.dash.onDash += Timer;

    }

    public void Timer(float time) 
    {
        timer = 0;
        timeToStop = time;
        myGroup.alpha = 1;
    }

    void Update() {

        if(timer <= timeToStop)
        {
            coolDownDenied.fillAmount = 1 - timer / timeToStop;
            timer += Time.deltaTime;

            if(timer >= timeToStop)
            {
                myGroup.alpha = 0;
            }
        }

    }

}
