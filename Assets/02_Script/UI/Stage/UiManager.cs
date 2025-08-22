using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public EventHandlers EventHandlers;
    public PlayerInformation PlayerInformation;
    public Sorting Sorting;
    public BattleSceneUi BattleSceneCanvas;
    public Canvas InGameCanvas;
    public WaitUI WaitUI;
    public StatesUI StatesUI;
    public SkillError SkillError;
    public GameEndUi EndUi;
    public void Initialize(EventHandlers eventHandlers)
    {
        EventHandlers = eventHandlers;
        EventHandlers.typeEventHandler.Resgister<Unit>(typeof(PlayerInformation), PlayerInformation.SetUi);
        EventHandlers.typeEventHandler.Resgister<Unit>(typeof(Sorting), Sorting.Add);
        EventHandlers.typeEventHandler.Resgister<TurnObject>(typeof(Sorting), Sorting.Remove);
        EventHandlers.typeEventHandler.Resgister<Unit>(typeof(BattleSceneUi), UiChange);
        EventHandlers.typeEventHandler.Resgister<int>(typeof(WaitUI), WaitUI.OnUi);
        EventHandlers.typeEventHandler.Resgister<Unit,bool>(typeof(StatesUI), StatesUI.Setting);
        EventHandlers.typeEventHandler.Resgister<int>(typeof(SkillError), (time) => SkillError.OnPanel(time).Forget());
        EventHandlers.typeEventHandler.Resgister<bool>(typeof(GameEndUi), End);
    }

    public void UiChange(Unit unit)
    {
        if (InGameCanvas.gameObject.activeSelf)
            InGameCanvas.gameObject.SetActive(false);
        else
            InGameCanvas.gameObject.SetActive(true);

        if (BattleSceneCanvas.gameObject.activeSelf)
            BattleSceneCanvas.gameObject.SetActive(false);
        else
        {
            BattleSceneCanvas.gameObject.SetActive(true);
            BattleSceneCanvas.SetUi(unit).Forget();
        }

    }

    public void Home()
    {
        SceneManager.LoadScene("Loading");
        SceneNumber.Number = 1;
    }

    public void ReTry()
    {
        SceneManager.LoadScene("Loading");
        SceneNumber.Number = SceneManager.GetActiveScene().buildIndex;
    }

    public void End(bool type)
    {
        //BattleSceneCanvas.gameObject.SetActive(false);
        //InGameCanvas.gameObject.SetActive(false);
        EndUi.Set(type);
        EndUi.gameObject.SetActive(true);
    }
}
