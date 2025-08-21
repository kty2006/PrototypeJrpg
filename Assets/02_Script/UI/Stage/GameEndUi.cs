using UnityEngine;
using UnityEngine.UI;

public class GameEndUi : MonoBehaviour
{
    public Text EndType;

    public void Set(bool type)
    {
        EndType.text = (type) ? "Win" : "Lose";
    }
}
