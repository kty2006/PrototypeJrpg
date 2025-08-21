using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StageSelectUi : MonoBehaviour
{
    public Image StageImage;
    public Sprite[] StageSprites;
    private int index;

    public void ImageChange(int i)
    {
        index = Mathf.Clamp(index + i, 0, StageSprites.Length - 1);
        StageImage.sprite = StageSprites[index];
    }

    public void Home()
    {
        SceneManager.LoadScene("Loading");
        SceneNumber.Number = 0;
    }

    public void SelectStage()
    {
        SceneManager.LoadScene("Loading");
        SceneNumber.Number = index + 2;
    }

    public void Exit()
    {
        Application.Quit();
    }
}