using DG.Tweening;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    public Vector3 AttackerPos;
    public Vector3 DefenderPos;
    public Transform AttackerPosition;
    public Transform DefenderPosition;
    public Camera SceneCamera;
    public Transform CameraSt;
    public Transform CameraEnd;
    public float CameraMoveTime = 1;
    public EventHandlers EventHandlers;

    public void Initialize(EventHandlers eventHandlers)
    {
        this.EventHandlers = eventHandlers;
        EventHandlers.typeEventHandler.Resgister<Unit>(typeof(BattleScene), Invoke);
    }

    public void Invoke(Unit Attacker)
    {
        Sequence mySequence = DOTween.Sequence();

        this.AttackerPos = Attacker.transform.position;
        this.DefenderPos = Attacker.Target.transform.position;
        Attacker.transform.SetPositionAndRotation(AttackerPosition.position, AttackerPosition.rotation);
        Attacker.Target.transform.SetPositionAndRotation(DefenderPosition.position, DefenderPosition.rotation); ;

        SceneCamera.gameObject.SetActive(true);
        SceneCamera.transform.DOMove(CameraEnd.transform.position, CameraMoveTime);
        SceneCamera.transform.DORotate(CameraEnd.transform.rotation.eulerAngles, CameraMoveTime);

        Attacker.Action.StartTime = CameraMoveTime;

        EventHandlers.typeEventHandler.Invoke<Unit>(typeof(BattleSceneUi), Attacker);

        Attacker.Action.FuncEnd += () => SceneCamera.gameObject.SetActive(false);
        Attacker.Action.FuncEnd += () => EventHandlers.typeEventHandler.Invoke<Unit>(typeof(BattleSceneUi), Attacker);
        Attacker.Action.FuncEnd += () =>
        {
            Attacker.transform.SetLocalPositionAndRotation(this.AttackerPos, Quaternion.identity);
            Attacker.Target.transform.SetLocalPositionAndRotation(this.DefenderPos, Quaternion.identity);
        };

        Debug.Log("dd");
        SceneCamera.transform.position = CameraSt.transform.position;
        SceneCamera.transform.rotation = CameraSt.transform.rotation;
    }
}
