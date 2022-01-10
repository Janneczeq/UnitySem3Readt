using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFuel : MonoBehaviour
{
    public DrivingScript ds;
    public bool Add()
    {
        if (ds.enebled)
        {
            ds.nitroFuel += 1;
            ds.nitroFuel = Mathf.Clamps(ds.nitroFuel, 0, 5);
            ds.ChangeFuelText();
        }
    }
   
}
