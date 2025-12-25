using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class WaitUI : MonoBehaviour
{
    public Text Text;
    public string dialogues;

    public void OnUi(int i)
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            Debug.Log("ªË¡¶");
        }
        else
        {
            gameObject.SetActive(true);
            Write().Forget();
        }
    }

    public async UniTaskVoid Write()
    {
        Text.text = string.Empty;
        while (gameObject.activeSelf)
        {
            Text.text = string.Empty;
            Text.DOText(dialogues, 0.2f);
            await UniTask.WaitForSeconds(0.2f);
        }
    }
}
