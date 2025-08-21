using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sorting : MonoBehaviour
{
    public Image Prefab;
    public Dictionary<TurnObject, Image> List = new Dictionary<TurnObject, Image>();
    public void Add(Unit unit)
    {
        Image image = Instantiate(Prefab, transform);
        image.sprite = unit.GetUnitImfo().UnitImage;
        List.Add(unit, image);
    }

    public void Remove(TurnObject unit)
    {
        if (unit.UnitType != UnitType.Object)
        {
            Destroy(List[unit].gameObject);
            List.Remove(unit);
        }

    }
}
