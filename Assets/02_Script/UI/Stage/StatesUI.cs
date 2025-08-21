using UnityEngine;
using UnityEngine.UI;

public class StatesUI : MonoBehaviour
{
    public Image Icon;
    public Text Name;
    public Text Hp;
    public Text Speed;
    public Text MP;

    public void Setting(Unit unit)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            Icon.sprite = unit.States.UnitImage;
            Name.text = $"{unit.States.UnitType.ToString()}";
            Hp.text = $"HP : {unit.LiqStates.Hp} / {unit.States.StHp}";
            Speed.text = $"SPEED : {unit.LiqStates.Speed}";
            MP.text = $"MP : {unit.LiqStates.Mp} / {unit.States.StMp}";
        }
    }
}
