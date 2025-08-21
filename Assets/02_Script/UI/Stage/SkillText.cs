using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillText : UIBehaviour
{
    public UnitsData UnitsData;
    public int SkillIndex = 0;
    public Text SkillName;
    public Text SkillDescription;
    public Text ManaCost;
    private EventHandlers eventHandlers;
    public void Initialize(EventHandlers eventHandlers)
    {
        this.eventHandlers = eventHandlers;
        eventHandlers.typeEventHandler.Resgister<Job>(typeof(SkillText), Set);

    }

    public void Awake()
    {
    }
    public void Set(Job unitType)
    {
        SkillName.text = UnitsData.GetTextImfo(unitType).SkillTextImfos[SkillIndex].SkillName;
        SkillDescription.text = UnitsData.GetTextImfo(unitType).SkillTextImfos[SkillIndex].SkillDescription;
        ManaCost.text = $"{UnitsData.GetTextImfo(unitType).SkillTextImfos[SkillIndex].ManaCost}";
    }
}
