using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedBehaviour 
    : MonoBehaviour
{
    public ManagerDirector Director
    {
        get { return ManagerDirector.director; }
    }

    public GameMaster Master
    {
        get { return Director.gameMaster; }
    }
}
