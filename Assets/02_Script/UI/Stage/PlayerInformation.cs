using UnityEngine;
using UnityEngine.UI;

public class PlayerInformation : MonoBehaviour
{
    public Image playerImage;
    public Image playerHpBar;
    public Image playerMpBar;

    public void SetUi(Unit unit)
    {
        playerImage.sprite = unit.States.UnitImage;
        playerHpBar.fillAmount = unit.LiqStates.Hp / unit.States.StHp;
        playerMpBar.fillAmount = unit.LiqStates.Mp / unit.States.StMp;
    }
}
