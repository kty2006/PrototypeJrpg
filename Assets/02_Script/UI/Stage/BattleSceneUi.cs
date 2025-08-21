using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneUi : MonoBehaviour
{
    public Image playerHpBar;
    public Image EnemyHpBar;
    public Vector3 UiPostion;
    public Camera SceneCamera;

    public async UniTaskVoid SetUi(Unit unit)
    {
        while (gameObject.activeSelf)
        {
            playerHpBar.fillAmount = unit.LiqStates.Hp / unit.States.StHp;
            EnemyHpBar.fillAmount = unit.Target.LiqStates.Hp / unit.Target.States.StHp;
            await UniTask.Yield();
        }
    }
}
