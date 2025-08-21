using UnityEngine;

[CreateAssetMenu(fileName = "UnitsData", menuName = "Scriptable Objects/UnitsData")]
public class UnitsData : ScriptableObject
{
    public UnitImfo[] UnitImfos;

    public TextImfo GetTextImfo(Job type)
    {

        if (UnitImfos.Length > 0)
        {
            for (int i = 0; i < UnitImfos.Length; i++)
            {
                if (UnitImfos[i].UnitType == type)
                {
                    return UnitImfos[i].TextImfo;
                }
            }
        }
        throw new System.ArgumentException($"UnitType {type}에 해당하는 TextImfo가 없습니다.");
    }
}
