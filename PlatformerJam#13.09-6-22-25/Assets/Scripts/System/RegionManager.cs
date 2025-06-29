using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionManager : MonoBehaviour
{
    public List<Region> regions;
    public int CurrentRegionID;


    void LoadStartRegions()
    {
        if (regions[CurrentRegionID] != null)
        {
            regions[CurrentRegionID].LoadRegion();
        }
        if(regions[CurrentRegionID+1]  != null) regions[CurrentRegionID+1].LoadRegion();
    }

    public void NextRegion()
    {
        regions[CurrentRegionID].DeleteRegion();

        if (CurrentRegionID + 1 < regions.Count) CurrentRegionID += 1;
        if (regions[CurrentRegionID + 1] != null) regions[CurrentRegionID + 1].LoadRegion();
        regions[CurrentRegionID].rightDoor.Open();
        regions[CurrentRegionID].leftDoor.Close();
    }
    
}
