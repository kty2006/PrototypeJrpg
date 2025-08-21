using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainUi : MonoBehaviour
{
    public Text Title;
    public Text Explan;

    [Header("쉐이크 설정")]
    public float shakeDuration = 1f;
    public float shakeStrength = 0.5f;
    public int vibrato = 10;

    void Start()
    {
        ShakeUntilCondition().Forget();
        Application.targetFrameRate = 120;
    }

    private async UniTask ShakeUntilCondition()
    {
        Tween shakeTween = null;
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        shakeTween = Camera.main.transform.DOShakeRotation(shakeDuration, new Vector3(0, 0, shakeStrength), vibrato)
                              .SetLoops(-1, LoopType.Restart);

        // 2. isShakeFinished가 true가 될 때까지 효율적으로 대기합니다.
        //    오브젝트가 파괴되면 CancellationToken에 의해 예외가 발생하며 대기가 중단됩니다

        await UniTask.WaitUntil(() => Input.anyKey, cancellationToken: cancellationToken);
        if (shakeTween != null && shakeTween.IsActive())
        {
            Camera.main.transform.DOKill(false);
        }

        Title.gameObject.SetActive(false);
        Explan.gameObject.SetActive(false);

        while (Camera.main.fieldOfView > 10)
        {
            Camera.main.fieldOfView -= 0.5f;
            await UniTask.Yield();
        }

        SceneManager.LoadScene("Loading");
        SceneNumber.Number = 1;

    }
}
