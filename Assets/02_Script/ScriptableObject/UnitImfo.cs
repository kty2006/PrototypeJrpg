using System;
using UnityEngine;



[Serializable]
public struct GridMap
{
    public int Up;
    public int Down;
    public int Ysize; /*{ get => Ysize; set { if (value >= 3) { value -= 1; } } }*/
    public int Left;
    public int Right;
    public int Xsize;/*{ get => Xsize; set { if (value >= 3) { value -= 1; } } }*/
}

[Serializable]
public struct LiqStates
{
    public float Speed;
    public float Hp;
    public float Mp;
    public float NormalAttack;
}

[CreateAssetMenu(fileName = "States", menuName = "Scriptable Objects/States")]
public class UnitImfo : ScriptableObject
{
    public Job UnitType;
    public Sprite UnitImage;
    public float StHp;
    public float StSpeed;
    public float StMp;
    public float StNormalAttack;
    public TextImfo TextImfo;
    public SkillDatas[] SkillDatas;


    public GridMap GetGridMap(UnitStates unitStates)
    {
        int count = 0;
        GridMap gridMap = new GridMap();
        while (count < SkillDatas.Length)
        {
            if (SkillDatas[count].UnitStates == unitStates)
            {
                gridMap = SkillDatas[count].GridMap;
                break;
            }
            count++;
        }
        return gridMap;
    }

    public int GetScale(UnitStates unitStates)
    {
        int count = 0;
        int scale = 0;
        while (count < SkillDatas.Length)
        {
            if (SkillDatas[count].UnitStates == unitStates)
            {
                scale = SkillDatas[count].Scale;
                break;
            }
            count++;
        }
        return scale;
    }

    public float GetMpCost(UnitStates unitStates)
    {
        int count = 0;
        float mpCost = 0;
        while (count < SkillDatas.Length)
        {
            if (SkillDatas[count].UnitStates == unitStates)
            {
                mpCost = SkillDatas[count].MpCost;
                break;
            }
            count++;
        }
        return mpCost;
    }
}

[Serializable]
public class SkillDatas
{
    public UnitStates UnitStates;
    public int Scale;
    public float MpCost;
    public GridMap GridMap;
}

[Serializable]
public class TextImfo
{
    public SkillTextImfo[] SkillTextImfos;
}

[Serializable]
public class SkillTextImfo
{
    public string SkillName;
    public string SkillDescription;
    public float ManaCost;
}
