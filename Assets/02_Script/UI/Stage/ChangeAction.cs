using DG.Tweening;
using UnityEngine;

public class ChangeAction : MonoBehaviour
{
    public Canvas FrontPanel;
    public Canvas BackPanel;
    public void Invoke()
    {
        int order = FrontPanel.sortingOrder;
        FrontPanel.sortingOrder = BackPanel.sortingOrder;
        BackPanel.sortingOrder = order;
        FrontPanel.transform.DOMove(new Vector3(25, 25, 0), 1).SetRelative();
        BackPanel.transform.DOMove(new Vector3(-20, -20, 0), 1).SetRelative();
        Canvas canvas = FrontPanel;
        FrontPanel = BackPanel;
        BackPanel = canvas;
    }
}
