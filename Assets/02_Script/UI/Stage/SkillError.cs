using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SkillError : MonoBehaviour
{
    public Image Panel;
    public async UniTaskVoid OnPanel(int time)
    {
        Panel.gameObject.SetActive(true);
        await UniTask.WaitForSeconds(time);
        Panel.gameObject.SetActive(false);
    }
}
