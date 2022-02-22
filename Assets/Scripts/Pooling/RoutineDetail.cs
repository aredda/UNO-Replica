using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineDetail
{
    public string routineName;
    public GameObject gameObject;
    public IEnumerator enumerator;

    public bool Concerns(GameObject gameObject)
    {
        return this.gameObject.Equals(gameObject);
    }

    public bool Matches(string name)
    {
        return this.routineName.Equals(name);
    }
}
