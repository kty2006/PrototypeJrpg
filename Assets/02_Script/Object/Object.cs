using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Object : Unit
{

    public void Boomb()
    {
        SelectFunc(UnitStates.Attack);
        EventHandlers.typeEventHandler.Invoke<GridMap, TurnObject>(typeof(ActionRangeSystem), GridMap, this);
        List<Vector3> gridMaps = EventHandlers.typeEventHandler.Invoke<List<Vector3>>(typeof(ActionRangeSystem));
        List<Unit> units = EventHandlers.typeEventHandler.Invoke<List<Vector3>, List<Unit>>(typeof(UnitRegistry), gridMaps);
        foreach (var unit in units)
        {
            if (unit.LiqStates.Hp > 0)
            {
                unit.DecHp(States.StNormalAttack);
                unit.Die().Forget();
            }
        }
        EventHandlers.typeEventHandler.Invoke<Unit>(typeof(PlayerInformation), (Unit)EventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(TurnSystem)));
        Wait(0.5f).Forget();
    }

    public async UniTaskVoid Wait(float i)
    {
        await UniTask.WaitForSeconds(i);
        EventHandlers.typeEventHandler.Invoke<TurnObject>(typeof(GridSystem), this);
    }
}
