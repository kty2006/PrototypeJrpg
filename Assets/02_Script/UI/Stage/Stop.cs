using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Stop : MonoBehaviour
{
    public Image Panel;
    public void StopTime()
    {
        Time.timeScale = 0;
        Panel.gameObject.SetActive(true);
        Play().Forget();
    }

    public async UniTaskVoid Play()
    {
        await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        Time.timeScale = 1;
        Panel.gameObject.SetActive(false);
    }
}
