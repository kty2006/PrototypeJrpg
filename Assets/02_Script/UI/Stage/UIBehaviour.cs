using UnityEngine;
using UnityEngine.Events;

public class UIBehaviour : MonoBehaviour
{
    /// <summary>
    /// UI 활성화 할때 호출
    /// </summary>
    /// <param name="onShow">UI활성화 할때 실행하고 싶은 액션</param>
    public void ShowUI(UnityEvent onShow = null)
    {
        gameObject.SetActive(true);
        onShow?.Invoke();
    }

    /// <summary>
    /// UI 비활성화 할때 호출
    /// </summary>
    /// <param name="onHide">UI비활성화 할때 실행하고 싶은 액션</param>
    public void HideUI(UnityEvent onHide = null)
    {
        gameObject.SetActive(false);
        onHide?.Invoke();
    }
}
