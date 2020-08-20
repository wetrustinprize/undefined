using UnityEngine;
using UnityEngine.UI;

public class HUDHP : MonoBehaviour {

    public Sprite[] sheet;
    public Image hpImage;

    public int vidaAtual;
    public int vidaMax;

    public void SetHealth(int newHealth)
    {
        vidaAtual = newHealth;
        Atualizar();
    }

    public void SetMaxHealth(int newMaxHealth)
    {
        vidaMax = newMaxHealth;
        Atualizar();
    }

    void Atualizar() {

        float porcentagem = 1 - ((float)vidaAtual / (float)vidaMax);
        int novoSpriteIndex = (int)(sheet.Length * porcentagem) - 1;

        novoSpriteIndex = Mathf.Clamp(novoSpriteIndex, 0, sheet.Length);

        Sprite novoSprite = sheet[novoSpriteIndex];

        hpImage.sprite = novoSprite;

    }

}