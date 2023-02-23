using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    private Door[] doors;

    public void setLocked(bool locked)
    {
        Debug.Log("setting " + doors.Length + " to" + locked);
        foreach (Door door in doors)
        {
            door.isLocked = locked;
        }
    }

    public void setClosed()
    {
        foreach (Door door in doors)
        {
            door.set(false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        doors = GetComponentsInChildren<Door>();
    }


}
