using UnityEngine;
using System;
using System.Collections;

public static class Global
{
    public static EventHandler<Type> EventHandler = new();
    public static EventHandler<int> ObjectEventHandler = new();
    public static TurnSystem TurnSystem = new TurnSystem();
}
