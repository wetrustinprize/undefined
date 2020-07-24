using UnityEngine;

public class SkillEnabler : MonoBehaviour
{

    public enum SkillToEnable {
        Explosion,
        Dash,
        Teleport,
        WallJump
    }

        #region Variables

    [Header("Skill to enable")]
    [SerializeField] private SkillToEnable skill = SkillToEnable.Explosion;

    // Script side
    private bool hasEnabled = false;

        #endregion

    void OnTriggerEnter2D(Collider2D col) {

        if(col.tag == "Player")
        {
            hasEnabled = true;
            PlayerController controller = GameManager.Player;

            switch(skill)
            {
                case SkillToEnable.Dash:
                    controller.canDash = true;
                    break;
                
                case SkillToEnable.Explosion:
                    controller.canExplode = true;
                    break;
                
                case SkillToEnable.Teleport:
                    controller.canTeleport = true;
                    break;
                
                case SkillToEnable.WallJump:
                    controller.canWallJump = true;
                    break;
            }

            GameManager.Checkpoint.Save();
        }

    }

}
