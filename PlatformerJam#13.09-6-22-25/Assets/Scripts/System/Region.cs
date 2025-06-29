using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    public bool isPassed;
    public GameObject region;
    public Doors leftDoor;
    public Doors rightDoor;

    private GameObject localRegion;
    public void LoadRegion()
    {
        localRegion = Instantiate(region, transform);
    }

    public void DeleteRegion()
    {
        Destroy(localRegion);
    }

}
